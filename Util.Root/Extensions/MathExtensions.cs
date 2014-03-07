using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root
{
    public static class MathExtensions
    {

        public static int FloorToNearest(this int num, int nearest) 
        {
            return ((int) Math.Floor((decimal)num/nearest))*nearest;
        }

        public static int CeilToNearest(this int num, int nearest)
        {
            return ((int)Math.Ceiling((decimal)num / nearest)) * nearest;
        }
    }
}
