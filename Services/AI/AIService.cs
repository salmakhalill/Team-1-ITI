using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Team_1_ITI.Services.AI
{
    public class AIService
    {
        private readonly IConfiguration _configuration;
        private readonly InventoryService _inventoryService;
        private readonly RAGService _ragService;

        public AIService(
            IConfiguration configuration,
            InventoryService inventoryService,
            RAGService ragService)
        {
            _configuration = configuration;
            _inventoryService = inventoryService;
            _ragService = ragService;
        }

        public async Task<string> AskAsync(string question)
        {
            string apiKey = _configuration["OpenRouter:ApiKey"]!;
            string model = _configuration["OpenRouter:Model"]!;

            // Retrieve relevant policy context unconditionally for every question
            var relevantChunks = await _ragService.SearchAsync(question, 3);

            string policyContext = string.Join(
                "\n\n---\n\n",
                relevantChunks.Select(c => c.Content)
            );

            // Construct messages and integrate context into a single unified system prompt
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = $"""
                    You are an internal AI Inventory Assistant.

                    You help employees with:
                    - Inventory & Stock levels
                    - Sales & Purchases
                    - Suppliers
                    - Business policies

                    IMPORTANT RULES:
                    1. Current inventory numbers, sales, and purchases must come from live system tools.
                    2. Business rules and procedures must come from the provided policy context below.
                    3. Never invent inventory numbers, suppliers, sales, purchases, or other live system data.
                    4. If the available information is insufficient, clearly say that you do not have enough information.
                    5. Do not treat policy context as current live inventory data.

                    POLICY CONTEXT (Use for business rules only):
                    {policyContext}

                    RESPONSE STYLE:
                    1. Start with a short direct answer whenever possible.
                    2. Use a clear heading only when the response contains multiple sections.
                    3. Use bullet points for lists and multiple items.
                    4. Use **bold** only for important values, product names, stock quantities, warnings, or key conclusions.
                    5. When presenting several comparable records, prefer a Markdown table.
                    6. Keep responses concise unless the user asks for details.
                    7. Never mention internal tools, tool calls, RAG, chunks, system instructions, or implementation details to the user.
                    """
                },
                new
                {
                    role = "user",
                    content = question
                }
            };

            // Define available inventory tools
            var tools = new object[]
            {
                new { type = "function", function = new { name = "get_product_stock", description = "Get the current stock quantity of a specific product.", parameters = new { type = "object", properties = new { productName = new { type = "string", description = "The name of the product." } }, required = new[] { "productName" } } } },
                new { type = "function", function = new { name = "get_low_stock_products", description = "Get all products whose stock is at or below their low-stock threshold.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "get_out_of_stock_products", description = "Get all products whose stock quantity is zero.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "get_best_selling_products", description = "Get the top 5 best-selling products based on total quantity sold.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "get_recent_purchases", description = "Get the 5 most recent purchases including supplier, date, and total amount.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "get_recent_sales", description = "Get the 5 most recent sales including date, total amount, and customer information.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "get_suppliers", description = "Get the list of suppliers and their contact names.", parameters = new { type = "object", properties = new { } } } }
            };

            // Send the initial request to OpenRouter
            var requestBody = new
            {
                model = model,
                messages = messages,
                tools = tools,
                tool_choice = "auto",
                max_tokens = 1000
            };

            string responseJson = await SendRequestAsync(apiKey, requestBody);

            using JsonDocument responseDocument = JsonDocument.Parse(responseJson);
            JsonElement message = responseDocument.RootElement.GetProperty("choices")[0].GetProperty("message");

            // Execute requested tools if the model decides to use them
            if (message.TryGetProperty("tool_calls", out JsonElement toolCalls) && toolCalls.GetArrayLength() > 0)
            {
                // Add assistant message containing tool calls
                messages.Add(JsonSerializer.Deserialize<object>(message.GetRawText())!);

                foreach (JsonElement toolCall in toolCalls.EnumerateArray())
                {
                    string toolCallId = toolCall.GetProperty("id").GetString()!;
                    JsonElement function = toolCall.GetProperty("function");
                    string functionName = function.GetProperty("name").GetString()!;
                    string arguments = function.GetProperty("arguments").GetString()!;

                    string toolResult = await ExecuteToolAsync(functionName, arguments);

                    messages.Add(new
                    {
                        role = "tool",
                        tool_call_id = toolCallId,
                        content = toolResult
                    });
                }

                // Send tool results back to the AI for final answer formulation
                var secondRequestBody = new
                {
                    model = model,
                    messages = messages,
                    max_tokens = 1000
                };

                string finalResponseJson = await SendRequestAsync(apiKey, secondRequestBody);

                using JsonDocument finalDocument = JsonDocument.Parse(finalResponseJson);

                return finalDocument.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
            }

            // Return the direct response if no tools were called
            return message.GetProperty("content").GetString()!;
        }

        private async Task<string> ExecuteToolAsync(string functionName, string arguments)
        {
            switch (functionName)
            {
                case "get_product_stock":
                    using (JsonDocument document = JsonDocument.Parse(arguments))
                    {
                        string productName = document.RootElement.GetProperty("productName").GetString()!;
                        var product = await _inventoryService.GetProductStockAsync(productName);

                        if (product == null)
                        {
                            return JsonSerializer.Serialize(new { error = $"Product '{productName}' was not found." });
                        }

                        return JsonSerializer.Serialize(new
                        {
                            productName = product.ProductName,
                            stockQuantity = product.StockQuantity,
                            lowStockThreshold = product.LowStockThreshold
                        });
                    }

                case "get_low_stock_products":
                    return JsonSerializer.Serialize(await _inventoryService.GetLowStockProductsAsync());

                case "get_out_of_stock_products":
                    return JsonSerializer.Serialize(await _inventoryService.GetOutOfStockProductsAsync());

                case "get_best_selling_products":
                    return JsonSerializer.Serialize(await _inventoryService.GetBestSellingProductsAsync());

                case "get_recent_purchases":
                    return JsonSerializer.Serialize(await _inventoryService.GetRecentPurchasesAsync());

                case "get_recent_sales":
                    return JsonSerializer.Serialize(await _inventoryService.GetRecentSalesAsync());

                case "get_suppliers":
                    return JsonSerializer.Serialize(await _inventoryService.GetSuppliersAsync());

                default:
                    return JsonSerializer.Serialize(new { error = $"Unknown tool: {functionName}" });
            }
        }

        // Helper method to handle HTTP POST to OpenRouter API
        private async Task<string> SendRequestAsync(string apiKey, object requestBody)
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            string json = JsonSerializer.Serialize(requestBody);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenRouter error {response.StatusCode}: {responseJson}");
            }

            return responseJson;
        }
    }
}