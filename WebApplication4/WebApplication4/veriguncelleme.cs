using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WebApplication4.veriguncelleme
{
    public class veriguncelleme
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        
        

        public veriguncelleme(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            
        }
        public async Task veriguncellemee()
        {
            var apiKey = configuration["TMDb:d4335a9f8b701eff9ea153d1265b8a44"];
            var baseUrl = "https://api.themoviedb.org/3";
            var client = httpClientFactory.CreateClient(baseUrl);
            var response = await client.GetStringAsync($"{baseUrl}/movie/popular?api_key={apiKey}");
            var movieData = JObject.Parse(response);


        }
    }
}

