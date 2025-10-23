using Microsoft.EntityFrameworkCore;
using FalveyInsuranceGroup.Db;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;


namespace FalveyInsuranceGroup.Backend.Helpers
{
    public class InputService
    {
        private readonly FalveyInsuranceGroupContext _context;
        public InputService(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks to see if the version number follows semantic versioning
        /// </summary>
        /// <param name="version">The version number</param>
        /// <returns>
        ///     True - Has a valid format
        ///     False - Has an invalid format
        /// </returns>
        public bool hasValidVersion(string version)
        {
            // Format is v[MAJOR].[MINOR].[PATCH]
            var version_regex = "^v\\d+\\.\\d+\\.\\d+$";

            // Defensive: check null/empty before regex
            if (string.IsNullOrEmpty(version))
            {
                return false;
            }
            if (!Regex.IsMatch(version, version_regex))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if a entity object holds a valid policy
        /// </summary>
        /// <param name="policy_id">The id of a policy</param>
        /// <returns>
        ///     True - Has a valid policy
        ///     False - Has an invalid policy
        /// </returns>
        public async Task<bool> hasValidPolicy(int policy_id)
        {
            return await _context.Policies.AnyAsync(p => p.policy_id == policy_id);
        }

        /// <summary>
        /// Checks to see if an entity object holds a valid enum type
        /// </summary>
        /// <param name="type">The enum type of entity</param>
        /// <returns>
        ///     True - Has a valid enum type
        ///     False - Has an invalid enum type
        /// </returns>
        public bool hasValidEnumType(string[] allowed_types, string type)
        {
            return allowed_types.Contains(type);
        }

        /// <summary>
        ///  Checks to see if a claim has a duplicate claim number
        /// </summary>
        /// <param name="claim_number">The claim number to check</param>
        /// <returns>
        ///     True - has a duplicate claim number
        ///     False - Has a unique claim number
        /// </returns>
        public async Task<bool> hasDuplicateClaimNumber(string claim_number)
        {

            return await _context.Claims.AnyAsync(c => c.claim_number == claim_number);
        }

        /// <summary>
        ///  Checks to see if entity model has a duplicate email
        /// </summary>
        /// <param name="new_email">The email to check</param>
        /// <returns>
        ///     True - has a duplicate email
        ///     False - Has either a unique email or no email at all
        /// </returns>
        public  async Task<bool> hasDuplicateEmail<T>(string? new_email)
        where T : class
        {
            if (new_email.IsNullOrEmpty())
            {
                return false;
            }

            // ***Build u => u.email == new_email***

            var parameter = Expression.Parameter(typeof(T), "u"); // Represents parameter u

            // Creates u.email == new_email
            var equality_expression = Expression.Equal(
                Expression.PropertyOrField(parameter, "email"), // builds u.email selector
                Expression.Constant(new_email) // builds an object representing new_email

            );

            // Creates the lambda: u => u.email == new_email
            var email_criteria = Expression.Lambda<Func<T, bool>>(equality_expression, parameter);

            // ***

            return await _context.Set<T>().AnyAsync(email_criteria);
        }

    }

}