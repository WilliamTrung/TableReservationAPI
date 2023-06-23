using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Validator
{

    public class PhoneValidator : ValidationAttribute
    {
        private const string Pattern = @"^(?:\+?84|0)(?:\d{9}|(?:\d{2}-\d{3}-\d{4})|(?:\d{3}-\d{2}-\d{2}-\d{2}))$";

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Handle null values according to your requirements
                return ValidationResult.Success;
            }

            string phoneNumber = value.ToString();

            // Create a Regex object with the pattern
            Regex regex = new Regex(Pattern);

            // Check if the phone number matches the pattern
            if (regex.IsMatch(phoneNumber))
            {
                return ValidationResult.Success;
            }

            // Phone number is not valid, return an error message
            return new ValidationResult(ErrorMessage);
        }
    }

}
