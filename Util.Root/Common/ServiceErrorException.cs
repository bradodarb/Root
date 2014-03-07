using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root.Common
{
    public class ServiceErrorException : Exception
    {
        public ServiceErrorException(string message)
            : base(message)
        { }
    }
}
