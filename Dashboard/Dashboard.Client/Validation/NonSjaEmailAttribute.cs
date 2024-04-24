using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Validation
{
    public class NonSjaEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email && email.EndsWith("@sja.org.uk"))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), 
                    validationContext.MemberName != null ? [validationContext.MemberName] : null);
            }

            return ValidationResult.Success;
        }
    }
}
