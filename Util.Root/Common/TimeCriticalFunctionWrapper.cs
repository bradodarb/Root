using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Util.Root.Common.Interface;

namespace Util.Root.Common
{
    public class TimeCriticalFunctionWrapper<T> : ITimeCriticalFunctionWrapper<T>
    {
        private int _Timeout = 0;
        private Func<T> _Function;


        public bool ThrowExceptionOnTimeout { get; set; }

        public int Timeout
        {
            get
            {
                return _Timeout;
            }
        }

   

        public TimeCriticalFunctionWrapper(int timeout, Func<T> function)
        {
            _Timeout = timeout;
            _Function = function;
            ThrowExceptionOnTimeout = true;
        }
        public TimeCriticalFunctionWrapper(int timeout, Func<T> function, bool throwontimeout)
        {
            _Timeout = timeout;
            _Function = function;
            ThrowExceptionOnTimeout = throwontimeout;
        }
        public T Execute(out bool success, T defaultvalue = default(T), string message = "Function did not complete within the require time.")
        {

			T value = defaultvalue;
			var thread = new Thread(() => value = _Function());
			thread.IsBackground = true;
			thread.Start();
			success = thread.Join(_Timeout);

			if (!success)
			{
				thread.Abort();
				value = defaultvalue;
				if (ThrowExceptionOnTimeout)
				{
					throw new FunctionTimeoutException(message);
				}
			}

            return value;
        }
        public T Execute(T defaultvalue = default(T), string message = "Function did not complete within the require time.")
        {
            bool success = false;
            return Execute(out success, defaultvalue, message);
        }
    }
     
    public class FunctionTimeoutException : Exception
    {
        public FunctionTimeoutException(string message)
            : base(message)
        { }
    }
}
