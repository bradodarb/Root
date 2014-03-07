using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root
{
    public static class FormattingFunctions
    {

        public static string BooleanToYesNo(this Boolean inbool)
        {
            if (inbool)
            { return "Yes";}
            else
            {    return "No";}

        }

    }
}
