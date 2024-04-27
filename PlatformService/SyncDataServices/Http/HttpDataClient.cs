using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpDataClient: ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                    Encoding.UTF8,
                    "application/json");
            var responce = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            Console.WriteLine(responce.IsSuccessStatusCode
                ? "--> Sync POST to CommandService was OK!"
                : "--> Sync POST to CommandService was NOT OK!");
        }
    }
}
