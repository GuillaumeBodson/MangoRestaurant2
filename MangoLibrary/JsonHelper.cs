using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MangoLibrary
{
    public static class JsonHelper
    {
        public static TValue DeserializeIgnoringCase<TValue>(string json)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<TValue>(json, jsonOptions);
        }
        public async static Task<T> GetDeserializeHttpResponseContent<T>(this HttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);
            var content = await response.Content.ReadAsStringAsync();

            return DeserializeIgnoringCase<T>(content);
        }
        public static StringContent ToUTF8EncodedJsonStringContent(this object data)
        {
            if (data == null)
                return null;
            return new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }
    }
}
