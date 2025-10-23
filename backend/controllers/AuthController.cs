using Microsoft.AspNetCore.Mvc;
using FalveyInsuranceGroup.Backend.Services;


namespace FalveyInsuranceGroup.Backend.Controllers
{

    /// <summary>
    /// handles login, token management, and more
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth_service;

        public AuthController(AuthService auth_service)
        {
            _auth_service = auth_service;
        }

        /// <summary>
        /// validates the provided email and password and logs them in
        /// </summary>
        /// <param name="email">The email string to be validated</param>
        /// <param name="password">The password string to be validated</param>
        /// <returns>An HTTP response that </returns>
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(string email, string password)
        {
            var is_user = await _auth_service.validateUser(email, password);

            if (!is_user)
            {
                return BadRequest("Invalid email or password.");
            }

            return Ok("Login successful");

        }

        /* 
        Implement this another time
        [HttpPost("logoff")]
        public async Task<ActionResult> LogOffUser()
        */

        
    }




}

