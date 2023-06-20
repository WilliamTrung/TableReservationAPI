#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace Validator
{
    public class DesiredDateValidator : ValidationAttribute
    {

        protected override ValidationResult IsValid(object objValue, ValidationContext validationContext)
        {

            PropertyInfo property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            DisplayAttribute displayNameAttribute = property.GetCustomAttribute<DisplayAttribute>();
            if (objValue == null)
            {
                return new ValidationResult("Wrong format");
            }

            var dateValue = objValue as DateOnly? ?? new DateOnly();

            //alter this as needed. I am doing the date comparison if the value is not null
            var today = DateOnly.FromDateTime(DateTime.Today);
            var deadline = today.AddDays(14);

            //in the future
            if (!(dateValue >= today && dateValue <= deadline))
            {
                return new ValidationResult("Invalid date");
            }
            return ValidationResult.Success;
        }
    }
}