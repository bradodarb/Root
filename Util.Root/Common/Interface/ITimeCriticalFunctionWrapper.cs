using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root.Common.Interface
{
    public interface ITimeCriticalFunctionWrapper<T>
    {
        int Timeout { get; }
        bool ThrowExceptionOnTimeout { get; set; }


        T Execute(out bool success, T defaultvalue, string message);
        T Execute(T defaultvalue, string message);
    }
}
