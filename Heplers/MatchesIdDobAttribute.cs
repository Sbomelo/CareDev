using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Heplers
{
    public class MatchesIdDobAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _idPropertyName;

        // pass the name of the id property to compare against
        public MatchesIdDobAttribute(string idPropertyName)
        {
            _idPropertyName = idPropertyName;
            ErrorMessage = "ID number does not match date of birth.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // value is the DOB
            var dob = value as DateTime?;
            var idProp = validationContext.ObjectType.GetProperty(_idPropertyName);
            if (idProp == null) return new ValidationResult($"Unknown property: {_idPropertyName}");

            var idValue = idProp.GetValue(validationContext.ObjectInstance) as string;
            if (string.IsNullOrWhiteSpace(idValue) || !dob.HasValue)
            {
                return ValidationResult.Success; // Let other validators handle required checks
            }

            var parsed = IdNumberHelper.ParseSouthAfricanIdDob(idValue);
            if (!parsed.HasValue) return new ValidationResult("Invalid ID number format.");

            // Compare dates (only date portion)
            if (parsed.Value.Date != dob.Value.Date)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        // Client-side validation adapter: adds data-val attributes
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-matchesiddob", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-matchesiddob-idprop", _idPropertyName);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return false;
            attributes.Add(key, value);
            return true;
        }
    }
}
