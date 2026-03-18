namespace BudgetingApp.Services
{
    public class MLService : IMLService
    {
        private readonly HttpClient _client;

        public MLService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> PredictCategoryAsync(string merchant)
        {
            var request = new
            {
                merchant
            };

            var response = await _client.PostAsJsonAsync("/predict", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
            if (result == null)
            {
                return "Uncategorized";
            }

            return result.Category;
        }

        public async Task<List<string>> PredictCategoryBatchAsync(List<string> merchants)
        {
            var request = new
            {
                merchants
            };
            var response = await _client.PostAsJsonAsync("/predict/batch", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BatchPredictionResponse>();
            if (result == null)
            {
                return new List<string>();
            }

            return result.Categories;
        }

        public async Task InvalidateCategoryCacheAsync()
        { 
            var response = await _client.PostAsync("/api/v1/cache/invalidate", null);
        }
    }

    public class PredictionResponse
    {
        public required string Category { get; set; }
    }

    public class BatchPredictionResponse
    {
        public required List<string> Categories { get; set; }
    }
}
