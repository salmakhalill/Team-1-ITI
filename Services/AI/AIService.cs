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
            string apiKey =
                _configuration["OpenRouter:ApiKey"]!;

            string model =
                _configuration["OpenRouter:Model"]!;

            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = """
                    You are an internal AI Inventory Assistant.

                    You help employees with:
                    - Inventory
                    - Products
                    - Stock levels
                    - Sales
                    - Purchases
                    - Suppliers
                    - Business policies

                    IMPORTANT RULES:

                    1. Current inventory numbers must come from live system tools.

                    2. Current sales and purchases must come from live system tools.

                    3. Business rules and procedures must come from the
                       provided policy context.

                    4. Never invent inventory numbers, suppliers, sales,
                       purchases, or other live system data.

                    5. If the available information is insufficient,
                       clearly say that you do not have enough information.

                    6. You can combine live system data with policy information.

                    7. When policy context is provided, use it for business
                       rules and procedures only.

                    8. Do not treat policy context as current inventory data.

                    RESPONSE STYLE:

                    Write responses in a clean, concise, and easy-to-scan format.

                    Follow these guidelines:

                    1. Start with a short direct answer whenever possible.

                    2. Use a clear heading only when the response contains multiple sections.
                       Do not use headings for very short answers.

                    3. Use bullet points for lists and multiple items.

                    4. Use **bold** only for important values, product names,
                       stock quantities, totals, warnings, or key conclusions.
                       Do not overuse bold text.

                    5. When presenting several comparable records, prefer a Markdown table.

                    6. Keep tables simple and readable.
                       Use only the columns that are useful for answering the question.

                    7. Separate different sections with a blank line.

                    8. Avoid unnecessary repetition and long introductions.

                    9. Keep responses concise unless the user asks for details.

                    10. For warnings or important conditions, make them visually clear
                        using a short bold label such as **Warning:** or **Important:**.

                    11. Do not use Markdown headings for every sentence.

                    12. Do not wrap the entire response in a code block.

                    13. Never mention internal tools, tool calls, RAG, system instructions,
                        or implementation details to the user.
                    """
                }
            };

            // Get relevant policy context

            if (IsPolicyQuestion(question))
            {
                var relevantChunks =
                    await _ragService.SearchAsync(question, 3);

                string policyContext =
                    string.Join(
                        "\n\n---\n\n",
                        relevantChunks.Select(c => c.Content)
                    );

                messages.Add(new
                {
                    role = "system",
                    content = $"""
                    POLICY CONTEXT:

                    The following information was retrieved from the official
                    Inventory Operations & Replenishment Policy.

                    Use this information only for business rules,
                    procedures, and policies.

                    Do NOT treat this information as current inventory,
                    sales, purchases, or supplier data.

                    POLICY INFORMATION:

                    {policyContext}
                    """
                });
            }

            // Add the user's question

            messages.Add(new
            {
                role = "user",
                content = question
            });

            // Define available inventory tools

            var tools = new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_product_stock",
                        description =
                            "Get the current stock quantity of a specific product.",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                productName = new
                                {
                                    type = "string",
                                    description =
                                        "The name of the product."
                                }
                            },
                            required = new[]
                            {
                                "productName"
                            }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_low_stock_products",
                        description =
                            "Get all products whose stock is at or below their low-stock threshold.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_out_of_stock_products",
                        description =
                            "Get all products whose stock quantity is zero.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_best_selling_products",
                        description =
                            "Get the top 5 best-selling products based on total quantity sold.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_recent_purchases",
                        description =
                            "Get the 5 most recent purchases including supplier, date, and total amount.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_recent_sales",
                        description =
                            "Get the 5 most recent sales including date, total amount, and customer information.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                },

                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_suppliers",
                        description =
                            "Get the list of suppliers and their contact names.",
                        parameters = new
                        {
                            type = "object",
                            properties = new { }
                        }
                    }
                }
            };

            // Send the question to OpenRouter

            var requestBody = new
            {
                model = model,
                messages = messages,
                tools = tools,
                tool_choice = "auto",
                max_tokens = 1000
            };

            string responseJson =
                await SendRequestAsync(
                    apiKey,
                    requestBody
                );

            using JsonDocument responseDocument =
                JsonDocument.Parse(responseJson);

            JsonElement message =
                responseDocument
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message");

            // Execute requested tools

            if (
                message.TryGetProperty(
                    "tool_calls",
                    out JsonElement toolCalls)
                &&
                toolCalls.GetArrayLength() > 0
            )
            {
                // Add assistant message containing tool calls
                messages.Add(
                    JsonSerializer.Deserialize<object>(
                        message.GetRawText()
                    )!
                );

                foreach (JsonElement toolCall in toolCalls.EnumerateArray())
                {
                    string toolCallId =
                        toolCall
                            .GetProperty("id")
                            .GetString()!;

                    JsonElement function =
                        toolCall.GetProperty("function");

                    string functionName =
                        function
                            .GetProperty("name")
                            .GetString()!;

                    string arguments =
                        function
                            .GetProperty("arguments")
                            .GetString()!;

                    string toolResult =
                        await ExecuteToolAsync(
                            functionName,
                            arguments
                        );

                    messages.Add(new
                    {
                        role = "tool",
                        tool_call_id = toolCallId,
                        content = toolResult
                    });
                }

                // Send tool results back to the AI

                var secondRequestBody = new
                {
                    model = model,
                    messages = messages,
                    max_tokens = 1000
                };

                string finalResponseJson =
                    await SendRequestAsync(
                        apiKey,
                        secondRequestBody
                    );

                using JsonDocument finalDocument =
                    JsonDocument.Parse(finalResponseJson);

                string finalAnswer =
                    finalDocument
                        .RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;

                return finalAnswer;
            }

            // Return the direct response

            string answer =
                message
                    .GetProperty("content")
                    .GetString()!;

            return answer;
        }


        private async Task<string> ExecuteToolAsync(
            string functionName,
            string arguments)
        {
            switch (functionName)
            {
                case "get_product_stock":
                    {
                        using JsonDocument document =
                            JsonDocument.Parse(arguments);

                        string productName =
                            document.RootElement
                                .GetProperty("productName")
                                .GetString()!;

                        var product =
                            await _inventoryService
                                .GetProductStockAsync(productName);

                        if (product == null)
                        {
                            return JsonSerializer.Serialize(
                                new
                                {
                                    error =
                                        $"Product '{productName}' was not found."
                                }
                            );
                        }

                        return JsonSerializer.Serialize(
                            new
                            {
                                productName = product.ProductName,
                                stockQuantity = product.StockQuantity,
                                lowStockThreshold =
                                    product.LowStockThreshold
                            }
                        );
                    }

                case "get_low_stock_products":
                    {
                        var products =
                            await _inventoryService
                                .GetLowStockProductsAsync();

                        return JsonSerializer.Serialize(products);
                    }

                case "get_out_of_stock_products":
                    {
                        var products =
                            await _inventoryService
                                .GetOutOfStockProductsAsync();

                        return JsonSerializer.Serialize(products);
                    }

                case "get_best_selling_products":
                    {
                        var products =
                            await _inventoryService
                                .GetBestSellingProductsAsync();

                        return JsonSerializer.Serialize(products);
                    }

                case "get_recent_purchases":
                    {
                        var purchases =
                            await _inventoryService
                                .GetRecentPurchasesAsync();

                        return JsonSerializer.Serialize(purchases);
                    }

                case "get_recent_sales":
                    {
                        var sales =
                            await _inventoryService
                                .GetRecentSalesAsync();

                        return JsonSerializer.Serialize(sales);
                    }

                case "get_suppliers":
                    {
                        var suppliers =
                            await _inventoryService
                                .GetSuppliersAsync();

                        return JsonSerializer.Serialize(suppliers);
                    }

                default:
                    return JsonSerializer.Serialize(
                        new
                        {
                            error =
                                $"Unknown tool: {functionName}"
                        }
                    );
            }
        }

        // Send request to OpenRouter

        private async Task<string> SendRequestAsync(
            string apiKey,
            object requestBody)
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey
                );

            string json =
                JsonSerializer.Serialize(requestBody);

            using var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

            var response =
                await client.PostAsync(
                    "https://openrouter.ai/api/v1/chat/completions",
                    content
                );

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OpenRouter error {response.StatusCode}: {responseJson}"
                );
            }

            return responseJson;
        }

        // Check if the question needs policy context

        private bool IsPolicyQuestion(string question)
        {
            string[] policyKeywords =
            {
                "policy",
                "rule",
                "rules",
                "procedure",
                "procedures",
                "should",
                "how should",
                "replenish",
                "replenishment",
                "priority",
                "threshold",
                "allowed",
                "requirement",
                "requirements",
                "validation",
                "deduct stock",
                "add stock"
            };

            return policyKeywords.Any(keyword =>
                question.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }
    }
}