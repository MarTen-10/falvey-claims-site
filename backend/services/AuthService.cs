using FalveyInsuranceGroup.Db;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace backend.services
{
    public class AuthService
    {

        private readonly FalveyInsuranceGroupContext _context;

        public AuthService(FalveyInsuranceGroupContext context)
        {
            _context = context;

        }

        /// <summary>
        /// Checks to see if the a user with the provided email exists and
        /// checks if the provided password matches the hash stored with the fetched user
        /// </summary>
        /// <param name="user_email">The email to validate</param>
        /// <param name="password">The password to hash and validate</param>
        /// <returns>A bool value</returns>
        public async Task<bool> validateUser(string user_email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == user_email);

            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.password_hash))
            {
                return false;
            }

            return true;


        }

    }





}