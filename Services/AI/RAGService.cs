using Team_1_ITI.Models;
using Team_1_ITI.Services.AI;
using UglyToad.PdfPig;

namespace Team_1_ITI.Services
{
    public class RAGService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly EmbeddingService _embeddingService;

        public RAGService(
            IWebHostEnvironment environment,
            EmbeddingService embeddingService)
        {
            _environment = environment;
            _embeddingService = embeddingService;
        }

        public async Task<List<DocumentChunk>> GetChunksAsync()
        {
            string path = Path.Combine(
                _environment.WebRootPath,
                "documents",
                "Inventory_Operations_Replenishment_Policy.pdf"
            );

            using PdfDocument document =
                PdfDocument.Open(path);

            string text = string.Join(
                "\n",
                document.GetPages().Select(page => page.Text)
            );

            var chunks = SplitIntoChunks(text);

            foreach (var chunk in chunks)
            {
                chunk.Embedding =
                    await _embeddingService
                        .CreateEmbeddingAsync(chunk.Content);
            }

            return chunks;
        }

        public async Task<List<DocumentChunk>> SearchAsync(
            string question,
            int topK = 3)
        {
            // Create an embedding for the question
            float[] questionEmbedding =
                await _embeddingService
                    .CreateEmbeddingAsync(question);

            // Find the most relevant document chunks
            List<DocumentChunk> chunks =
                await GetChunksAsync();

            var results = chunks
                .Select(chunk => new
                {
                    Chunk = chunk,

                    Score = CosineSimilarity(
                        questionEmbedding,
                        chunk.Embedding
                    )
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Chunk)
                .ToList();

            return results;
        }

        private double CosineSimilarity(
            float[] vectorA,
            float[] vectorB)
        {
            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0;
                 i < vectorA.Length;
                 i++)
            {
                dotProduct +=
                    vectorA[i] * vectorB[i];

                magnitudeA +=
                    vectorA[i] * vectorA[i];

                magnitudeB +=
                    vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 ||
                magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct /
                (Math.Sqrt(magnitudeA) *
                 Math.Sqrt(magnitudeB));
        }

        private List<DocumentChunk> SplitIntoChunks(
            string text)
        {
            const int chunkSize = 1000;

            var chunks =
                new List<DocumentChunk>();

            for (int i = 0;
                 i < text.Length;
                 i += chunkSize)
            {
                string content =
                    text.Substring(
                        i,
                        Math.Min(
                            chunkSize,
                            text.Length - i
                        )
                    );

                chunks.Add(new DocumentChunk
                {
                    Id = chunks.Count + 1,
                    Content = content
                });
            }

            return chunks;
        }
    }
}
