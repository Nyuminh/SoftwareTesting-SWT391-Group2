using HIVTreatment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IConfiguration _config;

    public ChatController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var apiKey = _config["GoogleAI:ApiKey"];
        var model = _config["GoogleAI:Model"] ?? "gemini-2.0-flash";

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = request.Message }
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(body);
        using var client = new HttpClient();
        var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Lỗi từ Google AI API: {err}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseBody);
        string reply = result?.candidates?[0]?.content?.parts?[0]?.text ?? "Không có phản hồi";

        return Ok(new { reply });
    }
}