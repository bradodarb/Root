using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root
{

    public class Abbreviation : Attribute
    {
        public string AbbreviatedValue { get; private set; }

        public Abbreviation(string abbreviatevalue)
        {
            this.AbbreviatedValue = abbreviatevalue; 
        }
    }

}
