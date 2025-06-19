using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Entities;
using System.Collections.Generic;
using Desktop.Repository;

public class ApiClient
{
    private static string token;
    private readonly HttpClient client;

    public ApiClient(string baseAddress)
    {
        client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Load token from local storage
        token = LocalStorage.LoadToken();
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new { username, password }), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/auth/login", content);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
        token = tokenResponse.Token;

        // Save token to local storage
        LocalStorage.SaveToken(token);

        return token;
    }

    public async Task<T> GetAsync<T>(string requestUri)
    {
        using (var client = new HttpClient { BaseAddress = new Uri("http://45.144.64.179") })
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, object data)
    {
        using (var client = new HttpClient { BaseAddress = new Uri("http://45.144.64.179") })
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await client.PostAsync(requestUri, content);
        }
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, object data)
    {
        using (var client = new HttpClient { BaseAddress = new Uri("http://45.144.64.179") })
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await client.PutAsync(requestUri, content);
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        using (var client = new HttpClient { BaseAddress = new Uri("http://45.144.64.179") })
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await client.DeleteAsync(requestUri);
        }
    }
}

public class TokenResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}
