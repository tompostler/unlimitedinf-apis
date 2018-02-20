using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.Frequencies;

namespace Unlimitedinf.Apis.Contracts.CustomValidators
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class FrequenciesSkittlesValidator
    {
        public static ValidationResult SkittlesValidation(Skittles skittles, ValidationContext context)
        {
            if (skittles == null)
                return new ValidationResult("Value cannot be null.");
            switch (skittles.type)
            {
                case Skittles.SkittleType.classic:
                    if (!(skittles.colors is Skittles.SkittleColorClassic))
                        return new ValidationResult("Based on the skittles.type, skittles.colors should be SkittleColorClassic");
                    break;

                default:
                    return new ValidationResult("Not a valid type of skittles");
            }

            return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
