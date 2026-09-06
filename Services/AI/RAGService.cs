using Team_1_ITI.Models;
using Team_1_ITI.Services.AI;
using UglyToad.PdfPig;

namespace Team_1_ITI.Services
{
    public class RAGService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly EmbeddingService _embeddingService;

        // Static cache to store chunks in memory and avoid redundant file reading and API calls
        private static List<DocumentChunk>? _cachedChunks;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public RAGService(
            IWebHostEnvironment environment,
            EmbeddingService embeddingService)
        {
            _environment = environment;
            _embeddingService = embeddingService;
        }

        public async Task<List<DocumentChunk>> GetChunksAsync()
        {
            // Return cached chunks immediately if available
            if (_cachedChunks != null)
            {
                return _cachedChunks;
            }

            // Ensure thread safety when multiple requests try to load the file simultaneously
            await _semaphore.WaitAsync();
            try
            {
                // Double-check locking pattern
                if (_cachedChunks != null)
                {
                    return _cachedChunks;
                }

                string path = Path.Combine(
                    _environment.WebRootPath,
                    "documents",
                    "Inventory_Operations_Replenishment_Policy.pdf"
                );

                using PdfDocument document = PdfDocument.Open(path);

                string text = string.Join(
                    "\n",
                    document.GetPages().Select(page => page.Text)
                );

                var chunks = SplitIntoChunks(text);

                // Generate embeddings for all chunks only once
                foreach (var chunk in chunks)
                {
                    chunk.Embedding = await _embeddingService.CreateEmbeddingAsync(chunk.Content);
                }

                // Store in memory cache
                _cachedChunks = chunks;
                return _cachedChunks;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<DocumentChunk>> SearchAsync(string question, int topK = 3)
        {
            // Create an embedding for the user's question
            float[] questionEmbedding = await _embeddingService.CreateEmbeddingAsync(question);

            // Fetch the cached document chunks
            List<DocumentChunk> chunks = await GetChunksAsync();

            // Find the most relevant document chunks based on Cosine Similarity
            var results = chunks
                .Select(chunk => new
                {
                    Chunk = chunk,
                    Score = CosineSimilarity(questionEmbedding, chunk.Embedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Chunk)
                .ToList();

            return results;
        }

        private double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }

        private List<DocumentChunk> SplitIntoChunks(string text)
        {
            var chunks = new List<DocumentChunk>();
            const int maxChunkSize = 1000;
            int start = 0;

            while (start < text.Length)
            {
                int length = Math.Min(maxChunkSize, text.Length - start);

                // Semantic chunking: avoid splitting words in the middle
                if (start + length < text.Length)
                {
                    int lastSpace = text.LastIndexOfAny(new[] { ' ', '\n', '\r', '.' }, start + length - 1, length);
                    if (lastSpace > start)
                    {
                        length = lastSpace - start + 1;
                    }
                }

                string content = text.Substring(start, length).Trim();

                if (!string.IsNullOrWhiteSpace(content))
                {
                    chunks.Add(new DocumentChunk
                    {
                        Id = chunks.Count + 1,
                        Content = content
                    });
                }

                start += length;
            }

            return chunks;
        }
    }
}