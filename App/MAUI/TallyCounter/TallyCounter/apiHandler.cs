namespace TallyCounter;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.thefetagroup.com/pjs/";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<Counter> GetCounterAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Counter>($"{BaseUrl}{id}");
    }

    public async Task<bool> UpdateCounterAsync(int id, Counter counter)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}{id}", counter);
        return response.IsSuccessStatusCode;
    }
}
