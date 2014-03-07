using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root.Common
{
    public interface IAutoCompleteProvider
    {

        object[] GetMatches(string target);

    }
}
