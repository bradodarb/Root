
using System.ComponentModel.DataAnnotations;

namespace Util.Root.Attributes
{

    public class EmailAddressAttribute : RegularExpressionAttribute
    {

        public EmailAddressAttribute() : base(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$") { }

    }

}
