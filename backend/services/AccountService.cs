using FalveyInsuranceGroup.Db;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FalveyInsuranceGroup.Backend.Models;

namespace backend.services
{
    public class AccountService
    {
        private readonly FalveyInsuranceGroupContext _context;

        public AccountService(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUser(string acc_email, string password)
        {
            if (!await IsEmailValid(acc_email)) {
                return false;
            }

            var hashed_pass = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            var to_add = new User
            {

                email = acc_email,
                password_hash = hashed_pass,
                role = "Employee",
                is_active = true,

            };

            _context.Users.Add(to_add);
            await _context.SaveChangesAsync();

            return true;
        }
        


        private async Task<bool> IsEmailValid(string new_email)
        {
        
            var has_email = await _context.Users.FirstOrDefaultAsync(u => u.email == new_email);


            if (has_email != null) {
                return false;
            }

            return true;


        }

    }


    

}
