using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DAL.Root.Validation
{
    public class IsFutureDate : ValidationAttribute
    {
        private readonly string _StartDatePropertyName;
        private readonly bool _AllowEqualDates;

        public IsFutureDate(string startdatepropertyname, bool allowequaldates = false)
        {
            this._StartDatePropertyName = startdatepropertyname;
            this._AllowEqualDates = allowequaldates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this._StartDatePropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this._StartDatePropertyName));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success;
            }

            if (propertyTestedValue == null || !(propertyTestedValue is DateTime))
            {
                return ValidationResult.Success;
            }

            // Compare values
            if ((DateTime)value >= (DateTime)propertyTestedValue)
            {
                if (this._AllowEqualDates)
                {
                    return ValidationResult.Success;
                }
                if ((DateTime)value > (DateTime)propertyTestedValue)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
