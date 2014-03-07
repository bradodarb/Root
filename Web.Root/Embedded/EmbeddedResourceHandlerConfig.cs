using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Root.Embedded
{
    public class EmbeddedResourceHandlerConfig
    {
        public Type SourceType { get; private set; }
        public string Prefix { get; private set; }
        public string RouteKey { get; private set; }
    }
}
