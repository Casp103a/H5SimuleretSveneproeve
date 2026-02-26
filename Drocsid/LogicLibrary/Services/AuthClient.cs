using LogicLibrary.Interface;
using LogicLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace LogicLibrary.Services
{
    internal class AuthClient : HttpClient
    {
        private HttpClient httpClient;
        private readonly IApplicationSpecific applicationSpecific;
        private string token { get => applicationSpecific.GetToken(); }
        private readonly Action signOut;
        private readonly Action<string> asSignNewToken;

        public AuthClient(IApplicationSpecific applicationSpecific)
        {
            this.applicationSpecific = applicationSpecific;

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            signOut = applicationSpecific.SignUserOut;
            asSignNewToken = applicationSpecific.SaveTokenInSecureStorage;
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IsAuthorized(out bool isAuthorized);
            if (!isAuthorized)
            {
                signOut.Invoke();
                return null;
            }
            if (httpClient != null)
            {
                return httpClient.SendAsync(request, cancellationToken);
            }
            return base.SendAsync(request, cancellationToken);
        }

        private Task IsAuthorized(out bool isAuthurized)
        {
            if (token != null)
            {
                if (IsTokenUsedOrChanged(token))
                {
                    isAuthurized = false;
                    return Task.CompletedTask;
                }

                if (IsTokenExpired(token))
                {
                    GetToken(token, out string newToken);
                    if (newToken == null)
                    {
                        isAuthurized = false;
                        return Task.CompletedTask;
                    }
                    asSignNewToken.Invoke(newToken);
                    DefaultRequestHeaders.Remove("Authorization");
                    DefaultRequestHeaders.Add("Authorization", "Bearer " + newToken);
                    isAuthurized = true;
                    return Task.CompletedTask;
                }
                isAuthurized = true;
                return Task.CompletedTask;
            }

            isAuthurized = false;
            return Task.CompletedTask;
        }

        private bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (jwtToken.ValidTo.ToUniversalTime() < DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        private Task GetToken(string expiredToken, out string? newToken)
        {
            newToken = null;
            var tokenObj = new LoginTokens { Token = expiredToken };
            var jsonObj = JsonSerializer.Serialize(tokenObj);
            StringContent content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;
                    if (httpClient != null)
                    {
                        response = httpClient.PostAsync(applicationSpecific.BaseUrl + "/api/Account/RefreshToken", content).Result;
                    }
                    else
                    {
                        response = client.PostAsync(applicationSpecific.BaseUrl + "/api/Account/RefreshToken", content).Result;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        Stream responseContent = response.Content.ReadAsStream();
                        LoginTokens? tokens = JsonSerializer.Deserialize<LoginTokens>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

                        if (tokens == null)
                        {
                            newToken = null;
                            return Task.CompletedTask;
                        }

                        newToken = tokens.Token;
                    }
                    else
                    {
                        newToken = null;
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                newToken = null;
                return Task.CompletedTask;
            }
        }

        private bool IsTokenUsedOrChanged(string token)
        {
            var tokens = new LoginTokens
            {
                Token = token
            };

            var json = JsonSerializer.Serialize(tokens);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(applicationSpecific.BaseUrl + "/api/Account/ValidateToken", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
