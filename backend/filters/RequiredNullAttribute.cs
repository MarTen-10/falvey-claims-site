using System.ComponentModel.DataAnnotations;

namespace FalveyInsuranceGroup.Backend.Filters
{
    /// <summary>
    /// Field-level attribute ensures a value is null
    /// DO NOT USE if the database does not automatically increment/assign a value to the field
    /// </summary>
    public class RequiredNullAttribute : ValidationAttribute
    {
        public RequiredNullAttribute()
        {
            ErrorMessage = "ERROR: Field must not be given";
        }

        /// <summary>
        /// Checks to see if the value is null
        /// </summary>
        /// <param name="value">The member field being checked</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success; // Success if null
        }


    }

}


