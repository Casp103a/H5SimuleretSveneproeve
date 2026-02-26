using DrocsidApi.Managers;
using DrocsidApi.Models;
using DrocsidApi.Services;
using LogicLibrary.Dtos;
using LogicLibrary.Models;
using LogicLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrocsidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userManager;
        private readonly IDatabaseManager databaseManager;
        private readonly IPasswordService passwordService;
        private readonly ITokenService tokenService;
        private readonly IRefreshTokenManager refreshTokenManager;
        public UserController(IUserManager userManager, IDatabaseManager databaseManager, ITokenService tokenService, IRefreshTokenManager refreshTokenManager, IPasswordService passwordService)
        {
            this.userManager = userManager;
            this.databaseManager = databaseManager;
            this.tokenService = tokenService;
            this.refreshTokenManager = refreshTokenManager;
            this.passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (userDto.Username == null || userDto.Password == null)
                {
                    return BadRequest("All fields are required.");
                }

                if (string.IsNullOrEmpty(userDto.Password))
                {
                    return BadRequest();
                }

                var createdUser = await userManager.CreateAsync(userDto);

                if (createdUser != null)
                {
                    return Ok();
                }

                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = userManager.CheckPasswordSignInAsync(userDto, passwordService.DecodePassword(userDto.Password));
            if (!result.Result) return Unauthorized("Username not found and/or password incorrect");

            var user = await databaseManager.GetUser(userDto.Username);

            var token = tokenService.CreateToken(user, out Guid id);

            var refreshToken = new RefreshTokens()
            {
                ID = id,
                RefreshToken = tokenService.CreateRefreshToken(),
                RefreshTokenExpiry = DateTime.Now.AddHours(2),
                RefreshTokenIsUsed = false,
                UserId = user.Id,
            };

            bool refreshTokenSaved = await refreshTokenManager.Add(refreshToken);

            if (!refreshTokenSaved)
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
