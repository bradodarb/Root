
using System;
using System.ComponentModel.DataAnnotations;

namespace Util.Root.Attributes
{

    public class NotEqualToAttribute : ValidationAttribute
    {

        private string _CompareValue = string.Empty;
        public NotEqualToAttribute(string value)
        {
            _CompareValue = value;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            return (!value.Equals(_CompareValue));
        }

    }
}
