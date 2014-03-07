
using System;
using System.ComponentModel.DataAnnotations;

namespace Util.Root.Attributes
{

    public class MustBeTrueAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            if (value.GetType() != typeof(bool))
                throw new InvalidOperationException("Can only be used on boolean properties.");

            return (bool)value == true;
        }

    }
}
