using Microsoft.AspNetCore.Mvc;
using LogicLibrary.Dtos;
using DrocsidApi.Models;
using DrocsidApi.Managers;

namespace DrocsidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (userDto.Username == null || userDto.Password == null)
                {
                    return BadRequest("All fields are required.");
                }

                var userModel = new UserModel()
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    IsActive = true
                };

                if (string.IsNullOrEmpty(userDto.Password))
                {
                    return BadRequest();
                }

                var createdUser = await _userManager.CreateAsync(userModel, userDto.Password);

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
    }
}
