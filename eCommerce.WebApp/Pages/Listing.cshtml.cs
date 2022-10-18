using eCommerce.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace eCommerce.WebApp.Pages
{
    public partial class ListingModel : PageModel
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<ListingModel> _logger;
        private readonly HttpContext? _httpContext;

        public ListingModel(HttpClient apiClient, ILogger<ListingModel> logger, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7191/");
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public List<Product> Products { get; set; }
        public string CategoryName { get; set; } = "";

        [LoggerMessage(0, LogLevel.Warning, "API failure: {fullPath} Response: {statusCode}, Trace: {traceId}")]
        partial void LogApiFailure(string fullPath, int statusCode, string traceId);

        public async Task OnGetAsync()
        {
            var cat = Request.Query["cat"].ToString();
            if (string.IsNullOrEmpty(cat))
            {
                throw new Exception("failed");
            }

            if (_httpContext != null)
            {
                var accessToken = await _httpContext.GetTokenAsync("access_token");
                _apiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                // for better way to include and manage access tokens for API calls refer to:
                // https://identitymodel.readthedocs.io/en/latest/aspnetcore/web.html
            }

            var response = await _apiClient.GetAsync($"Product?category={cat}");
            if (!response.IsSuccessStatusCode)
            {
                var fullPath = $"{_apiClient.BaseAddress}Product?category={cat}";

                // trace id
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>() ??
                    new ProblemDetails();
                var traceId = details.Extensions["traceId"]?.ToString();

                LogApiFailure(fullPath, (int)response.StatusCode, traceId ?? "");

                //_logger.LogWarning("API Failure: {fullPath} Response: {response}, Trace: {trace}, User: {user}",
                //    fullPath,
                //    (int)response.StatusCode,
                //    traceId,
                //    userName);

                throw new Exception("API call failed!");
            }

            Products = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();
            if (Products.Any())
            {
                CategoryName = Products.First().Category.First().ToString().ToUpper() +
                               Products.First().Category[1..];
            }
        }
    }
}
