using LogicLibrary.Dtos;
using LogicLibrary.Interface;
using LogicLibrary.Models;
using System.Text;
using System.Text.Json;

namespace LogicLibrary.Services
{
    internal interface ILoginService
    {
        Task<(bool, string)> Login(LoginRegisterUserDto login);
    }

    public class LoginService : ILoginService
    {
        private readonly IApplicationSpecific applicationSpecific;
        private readonly HttpClient? httpClient;

        public LoginService(IApplicationSpecific applicationSpecific, HttpClient? httpClient = null)
        {
            this.applicationSpecific = applicationSpecific;
            this.httpClient = httpClient;
        }

        public async Task<(bool, string)> Login(LoginRegisterUserDto userDto)
        {
            HttpClient client = new HttpClient();
            if (httpClient != null)
            {
                client = httpClient;
            }

            var jsoncontent = JsonSerializer.Serialize<LoginRegisterUserDto>(userDto);
            StringContent stringContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(applicationSpecific.BaseUrl + "/api/login", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    LoginTokens? tokens = JsonSerializer.Deserialize<LoginTokens>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                    if (tokens == null)
                    {
                        return (false, "Failed on sign in");
                    }
                    return (true, tokens.Token);
                }
                else
                {
                    return (false, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return (false, "Failed on sign in");
            }
        }
    }
}
