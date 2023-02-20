using Mango.web.Models;
using Mango.web.Models.Factories;
using Mango.web.services.Iservices;
using MangoLibrary;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mango.web.services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected ResponseDto ResponseModel { get; set; }

        public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            ResponseModel = new ResponseDto();
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage message)
        {
            try
            {
                var client = _httpClient.CreateClient("MangoApi");
                client.DefaultRequestHeaders.Clear();

                await AddTokenToClient(client);

                HttpResponseMessage apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                var res = JsonHelper.DeserializeIgnoringCase<T>(apiContent);
                return res;
            }
            catch (Exception ex)
            {
                var response = ResponseDto.NewErrorResponse(ex);

                var res = JsonSerializer.Serialize(response);
                return JsonSerializer.Deserialize<T>(res);
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private async Task AddTokenToClient(HttpClient client)
        {
            string token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
