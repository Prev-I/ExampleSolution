using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Example.Site.Annotations
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowedValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowedValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = $"Please enter one of the allowable values: {string.Join(", ", (AllowedValues ?? new string[] { "No allowable values found" }))}.";
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
