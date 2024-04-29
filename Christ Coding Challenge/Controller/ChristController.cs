using Christ_Coding_Challenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiUrl = "https://christ-coding-challenge.test.pub.k8s.christ.de/Article/GetArticles";

    public ArticlesController(ArticleDbContext dbContext, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("SyncWithExternalApi")]
    public async Task<IActionResult> SyncWithExternalApi(IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://christ-coding-challenge.test.pub.k8s.christ.de/Article/GetArticles");
        try
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions

            {
                PropertyNameCaseInsensitive = true
            };
            var articles = JsonSerializer.Deserialize<List<Article>>(content, options);

            if (articles != null)
            {
                _dbContext.Articles.AddRange(articles);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("Artikel erfolgreich synchronisiert und gespeichert.");
        }
        catch (HttpRequestException e)
        {
            return StatusCode(500, $"API-Anfrage fehlgeschlagen: {e.Message}");
        }
        catch (JsonException ex)
        {
            return StatusCode(500, $"Fehler bei der Deserialisierung des JSON: {ex.Message}");
        }
    }




    [HttpGet("GetArticles")]
    public async Task<IActionResult> GetArticles()
    {
        var articles = await _dbContext.Articles
            .Include(a => a.Attributes)
            .ToListAsync();
        return Ok(articles);
    }


}
