using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbStringLength : System.Attribute
    {
        public int Length = 0;
        public DbStringLength(int length)
        {
            Length = length;
        }
    }
}
