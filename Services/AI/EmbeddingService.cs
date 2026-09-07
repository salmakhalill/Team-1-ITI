using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Team_1_ITI.Services.AI
{
    public class EmbeddingService
    {
        private readonly IConfiguration _configuration;

        public EmbeddingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<float[]> CreateEmbeddingAsync(string text)
        {
            string apiKey =
                _configuration["OpenRouter:ApiKey"]!;

            string model =
                _configuration["OpenRouter:EmbeddingModel"]!;

            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = model,
                input = text
            };

            string json =
                JsonSerializer.Serialize(requestBody);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://openrouter.ai/api/v1/embeddings",
                content
            );

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Embedding error {response.StatusCode}: {responseJson}"
                );
            }

            using JsonDocument document =
                JsonDocument.Parse(responseJson);

            return document.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
        }
    }
}