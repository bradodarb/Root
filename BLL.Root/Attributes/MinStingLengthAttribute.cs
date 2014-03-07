
using System;
using System.ComponentModel.DataAnnotations;

namespace Util.Root.Attributes
{

    public class MinStringLength : ValidationAttribute
    {
        public int MinimumCharacterCount { get; set; }

        public MinStringLength(int minlength)
        {
            MinimumCharacterCount = minlength;
        }
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("Can only be used on System.String properties.");

            return value.ToString().Length > MinimumCharacterCount;
        }

    }
}
