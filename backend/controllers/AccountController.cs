using Microsoft.AspNetCore.Mvc;
using backend.services;


namespace backend.controllers
{

    /// <summary>
    /// handles registration, updating user profile, and fetching user profile with their specific hub
    /// e.g. signing in as an admin would fetch their hub with a feature that no other user has
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly AccountService _acc_service;

        public AccountController(AccountService acc_service)
        {
            _acc_service = acc_service;
        }

        /// <summary>
        /// creates a new user with a given email and password
        /// </summary>
        /// <param name="email">The email to sign up with</param>
        /// <param name="password">The password to be hashed and stored</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(string email, string password)
        {
            bool is_created = await _acc_service.CreateUser(email, password);

            if (!is_created)
            {
                return BadRequest("Email is unavailable");
            }

            return Ok("Account successfully created");
        }
    }


}