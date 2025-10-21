using data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using backend.models;

namespace backend.services
{
    public class AccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
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

            };

            _context.users.Add(to_add);
            await _context.SaveChangesAsync();

            return true;
        }
        


        private async Task<bool> IsEmailValid(string new_email)
        {
        
            var has_email = await _context.users.FirstOrDefaultAsync(u => u.email == new_email);


            if (has_email != null) {
                return false;
            }

            return true;


        }

    }


    

}
