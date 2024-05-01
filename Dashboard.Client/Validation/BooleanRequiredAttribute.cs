using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Validation;

public class BooleanRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not bool b || !b)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName != null ? [validationContext.MemberName] : null);
        }

        return ValidationResult.Success;
    }
}
