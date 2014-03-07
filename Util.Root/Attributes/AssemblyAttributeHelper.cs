using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Util.Root.Attributes
{
    public class AssemblyAttributeHelper
    {
        public static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {

            if (assembly == null) return null;

            object[] attributes = assembly.GetCustomAttributes(typeof(T), true);

            if (attributes == null) return null;

            if (attributes.Length == 0) return null;

            return (T)attributes[0];

        }
    }
}
