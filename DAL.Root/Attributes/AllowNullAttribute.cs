﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowNull : System.Attribute
    {
        public AllowNull()
        {
            
        }
    }
}
