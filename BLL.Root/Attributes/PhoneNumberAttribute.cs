
using System.ComponentModel.DataAnnotations;

namespace Util.Root.Attributes
{

    public class PhoneNumberAttribute : RegularExpressionAttribute
    {

        public PhoneNumberAttribute() : base(@"^(1)?[-\.\s]?\(?([0-9]{3})\)?[-\.\s]?([0-9]{3})[-\.\s]?([0-9]{4})$") { }

    }

}

