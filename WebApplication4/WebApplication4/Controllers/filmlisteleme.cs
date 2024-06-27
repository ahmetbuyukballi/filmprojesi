using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication4.Data;
using WebApplication4.Models;
using WebApplication4.recommendsinifi;
using Microsoft.EntityFrameworkCore;
using WebApplication4.EmailService;



namespace WebApplication4.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class filmlisteleme : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
       private readonly VeriTabani.FilmContext filmContext;
        private readonly recommendsinifi.Recommend Recommend;
        private readonly EmailService.EmailService EmailService;

        public filmlisteleme(IConfiguration configuration, IHttpClientFactory httpClientFactory,VeriTabani.FilmContext filmContext,recommendsinifi.Recommend Recommend,EmailService.EmailService EmailService)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
           this.filmContext = filmContext;
            this.Recommend=Recommend;
            this.EmailService = EmailService;
        }
        public async Task<IActionResult> populerfilmlerilistele([FromQuery] int page = 1, int pagesize = 20)
        {
            var apikey = configuration["TMDb:d4335a9f8b701eff9ea153d1265b8a44"];
            var url = "https://api.themoviedb.org/3";
            var client = httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{url}/movies/popular?api_key={apikey}&page={page}");
            var filmdata = JObject.Parse(response);
            var sayfa = filmdata["results"].Take(pagesize);
            return Ok(sayfa);



        }
        [HttpPost("tablo")]
        [Authorize]
        public async Task<IActionResult> filmpuanlandırma([FromBody] Tablo1 tablo)
        {
            
            
            
           
            
            if (tablo.Puanlama < 1 || tablo.Puanlama > 10)
            {
                return BadRequest("1 ile 10 arasında puan giriniz!!");

            }
            tablo.KullaniciId= User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            filmContext.tablo1s.Add(tablo);
            await filmContext.SaveChangesAsync();
            return Ok(tablo);

        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> filmdetaylari(int id)
        {

            var apikey = configuration["TMDb:d4335a9f8b701eff9ea153d1265b8a44"];
            var baseUrl = "https://api.themoviedb.org/3";
            var client = httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{baseUrl}/movie/{id}?api_key={apikey}");
            var moviedata= JObject.Parse(response);
            var userid= User.Claims.FirstOrDefault(c=>c.Type=="sub")?.Value;
            var tablo =  filmContext.tablo1s.FirstOrDefault(r => r.FilmId == id&&r.KullaniciId==userid);
            var ortalamapuan = filmContext.tablo1s.Where(r => r.FilmId == id).AverageAsync(r => r.Puanlama);
            var filmdetaylari = new
            {

                Film = moviedata,
                Kullanicitablosu = tablo,
                Ortalamapuan = ortalamapuan,

            };
            return Ok(filmdetaylari);




        }
        [HttpPost("recommend")]
        [Authorize]
        public async Task<IActionResult> filmtavsiyesi([FromBody] Recommend recommend)
        {
            var apikey = configuration["TMDb:d4335a9f8b701eff9ea153d1265b8a44"];
            var url= "https://api.themoviedb.org/3";
            var client = httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{url}/movie/{recommend.FilmId}?api_key={apikey}");
            var filmdata=JObject.Parse(response);
           

            return Ok();



        }
       

    }
}
