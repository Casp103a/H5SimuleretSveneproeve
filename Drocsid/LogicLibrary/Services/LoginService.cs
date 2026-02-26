using LogicLibrary.Dtos;
using LogicLibrary.Interface;
using LogicLibrary.Models;
using System.Text;
using System.Text.Json;

namespace LogicLibrary.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDto> Login(LoginRegisterUserDto login);
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

        public async Task<LoginResponseDto> Login(LoginRegisterUserDto userDto)
        {
            HttpClient client = new HttpClient();
            if (httpClient != null)
            {
                client = httpClient;
            }

            var jsoncontent = JsonSerializer.Serialize<LoginRegisterUserDto>(userDto);
            StringContent stringContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            LoginResponseDto dto;

            try
            {
                var response = await client.PostAsync(applicationSpecific.BaseUrl + "/api/user/login", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    LoginTokens? tokens = JsonSerializer.Deserialize<LoginTokens>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                    if (tokens == null)
                    {
                        dto = new()
                        {
                            Success = false,
                            Error = "Failed on sign in",
                        };
                        return dto;
                    }

                    dto = new()
                    {
                        Success = true,
                        Token = tokens.Token,
                        User = tokens.CurrentUser
                    };

                    return dto;
                }
                else
                {
                    dto = new()
                    {
                        Success = false,
                        Error = await response.Content.ReadAsStringAsync()
                    };
                    return dto;
                }
            }
            catch (Exception ex)
            {
                dto = new()
                {
                    Success = false,
                    Error = "Failed on sign in",
                };

                return dto;
            }
        }
    }
}
