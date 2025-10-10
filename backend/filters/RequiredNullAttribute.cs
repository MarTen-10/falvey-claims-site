using System.ComponentModel.DataAnnotations;

namespace FalveyInsuranceGroup.backend.filters
{
    public class RequiredNullAttribute : ValidationAttribute
    {
        public RequiredNullAttribute()
        {
            ErrorMessage = "ERROR: Field must not be given";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }


    }

}
