using System.Text.Json.Serialization;

namespace BudgetingApp.Services
{
    public class MLService : IMLService
    {
        private readonly HttpClient _client;

        public MLService(HttpClient client)
        {
            _client = client;
        }

        public async Task<(string category, string cleanName)> PredictCategoryAsync(string merchant)
        {
            var request = new
            {
                merchant
            };

            var response = await _client.PostAsJsonAsync("/api/v1/predict", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
            if (result != null)
            {
                return (result.Category, result.MerchantClean);
            }

            return ("Uncategorized", merchant);
        }

        public async Task<List<(string category, string cleanName)>> PredictCategoryBatchAsync(List<string> merchants)
        {
            var request = new
            {
                merchants
            };
            var response = await _client.PostAsJsonAsync("/api/v1/predict/batch", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BatchPredictionResponse>();
            if (result != null)
            {
                var categoriesWithCleanNames = new List<(string category, string cleanName)>();
                for (int i = 0; i < result.Categories.Count; i++)
                {
                    categoriesWithCleanNames.Add((result.Categories[i], result.Details[i].MerchantClean));
                }
                return categoriesWithCleanNames;
            }
            return new List<(string category, string cleanName)>();
        }

        public async Task InvalidateCategoryCacheAsync()
        { 
            var response = await _client.PostAsync("/api/v1/cache/invalidate", null);
        }
    }

    public class PredictionResponse
    {
        public required string Category { get; set; }
        public required double Confidence { get; set; }
        [JsonPropertyName("merchant_clean")]
        public required string MerchantClean { get; set; }
        [JsonPropertyName("alias_source")]
        public required string AliasSource { get; set; }

    }

    public class BatchPredictionResponse
    {
        public required List<string> Categories { get; set; }

        public required List<PredictionResponse> Details { get; set; }
    }
}
