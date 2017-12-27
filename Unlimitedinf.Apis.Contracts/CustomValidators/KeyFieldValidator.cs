using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Unlimitedinf.Apis.Contracts.CustomValidators
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class KeyFieldValidator
    {
        // https://stackoverflow.com/a/37749583
        private static readonly Regex DisallowedCharsInTableKeys = new Regex(@"[\\\\#%+/?\u0000-\u001F\u007F-\u009F]", RegexOptions.Compiled);

        public static ValidationResult KeyFieldValidation(string fieldValue, ValidationContext context)
        {
            if (fieldValue == null)
                return new ValidationResult("Value cannot be null.");
            else if (DisallowedCharsInTableKeys.IsMatch(fieldValue))
                return new ValidationResult("Value cannot contain invalid tablestorage key field characters.");
            else
                return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
