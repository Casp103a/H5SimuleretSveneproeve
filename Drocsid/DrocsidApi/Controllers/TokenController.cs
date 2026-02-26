using DrocsidApi.Managers;
using DrocsidApi.Models;
using DrocsidApi.Services;
using LogicLibrary.Models;
using LogicLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DrocsidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IUserManager userManager;
        private readonly ITokenService tokenService;
        private readonly IRefreshTokenManager refreshTokenManager;
        public TokenController(IUserManager userManager, ITokenService tokenService, IRefreshTokenManager refreshTokenManager)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.refreshTokenManager = refreshTokenManager;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(LoginTokens loginTokens)
        {
            UserModel? user;
            RefreshTokens? userRefreshToken;
            RefreshTokens? loggedinRefreshToken;

            try
            {
                ClaimsPrincipal principal = tokenService.ValidateToken(loginTokens.Token);

                if (principal?.Identity?.Name == null)
                {
                    return Unauthorized();
                }

                user = await userManager.GetUser(principal.Identity.Name);
            }
            catch (SecurityTokenExpiredException e)
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(loginTokens.Token);

                user = await userManager.GetUser(jwtToken.Claims.FirstOrDefault(x => x.Type == "given_name").Value);
            }

            if (user == null)
            {
                return Unauthorized();
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(loginTokens.Token);

            string? refreshTokenIdFromToken = token.Claims.FirstOrDefault(x => x.Type.Contains("sid"))?.Value;

            Guid.TryParse(user.RefreshTokenId, out Guid refreshTokenId);
            Guid.TryParse(refreshTokenIdFromToken, out Guid loginrefreshTokenId);

            userRefreshToken = await refreshTokenManager.GetRefreshToken(refreshTokenId, user);
            loggedinRefreshToken = await refreshTokenManager.GetRefreshToken(loginrefreshTokenId, user);

            if (userRefreshToken == null || loggedinRefreshToken == null)
            {
                return Unauthorized();
            }

            if (user == null || userRefreshToken.RefreshToken != loggedinRefreshToken.RefreshToken || userRefreshToken.RefreshTokenExpiry < DateTime.Now || userRefreshToken.RefreshTokenIsUsed)
            {
                return Unauthorized();
            }

            return await SaveResfreshToken(user, userRefreshToken);
        }

        [HttpPost("ValidateToken")]
        public async Task<IActionResult> ValidateToken(LoginTokens tokens)
        {
            UserModel? user;
            RefreshTokens? userRefreshToken;
            RefreshTokens? loggedinRefreshToken;

            try
            {
                ClaimsPrincipal principal = tokenService.ValidateToken(tokens.Token);

                if (principal?.Identity?.Name == null)
                {
                    return Unauthorized();
                }

                user = await userManager.GetUser(principal.Identity.Name);
            }
            catch (SecurityTokenExpiredException e)
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens.Token);

                user = await userManager.GetUser(jwtToken.Claims.FirstOrDefault(x => x.Type == "given_name").Value);
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokens.Token);

            string? refreshTokenIdFromToken = token.Claims.FirstOrDefault(x => x.Type.Contains("sid"))?.Value;

            Guid.TryParse(user.RefreshTokenId, out Guid refreshTokenId);
            Guid.TryParse(refreshTokenIdFromToken, out Guid loginrefreshTokenId);

            userRefreshToken = await refreshTokenManager.GetRefreshToken(refreshTokenId, user);
            loggedinRefreshToken = await refreshTokenManager.GetRefreshToken(loginrefreshTokenId, user);

            if (userRefreshToken == null || loggedinRefreshToken == null)
            {
                return Unauthorized();
            }

            if (user == null || userRefreshToken.RefreshToken != loggedinRefreshToken.RefreshToken || userRefreshToken.RefreshTokenExpiry < DateTime.Now || userRefreshToken.RefreshTokenIsUsed)
            {
                return Unauthorized();
            }

            return Ok();
        }

        private async Task<IActionResult> SaveResfreshToken(UserModel user, RefreshTokens userRefreshToken)
        {
            var token = tokenService.CreateToken(user, out Guid accessTokenID);


            var refreshToken = new RefreshTokens()
            {
                ID = accessTokenID,
                RefreshToken = tokenService.CreateRefreshToken(),
                RefreshTokenExpiry = DateTime.Now.AddHours(2),
                RefreshTokenIsUsed = false,
                UserId = user.Id,
            };

            bool didTheRefreshTokenSaved = await refreshTokenManager.Add(refreshToken);
            if (didTheRefreshTokenSaved) didTheRefreshTokenSaved = await refreshTokenManager.Update(userRefreshToken.ID, true);

            if (!didTheRefreshTokenSaved)
            {
                return StatusCode(500, "Failed while saving");
            }

            user.RefreshTokenId = refreshToken.ID.ToString();
            var savedUser = await userManager.UpdateAsync(user);

            if (savedUser == null)
            {
                return StatusCode(500, "Failed while saving");
            }

            return Ok(new LoginTokens
            {
                Token = token
            });
        }
    }
}
