//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Routing;

using System.Web;
using System.Web.Routing;
namespace Web.Root.Embedded
{
    public class EmbeddedResourceRouteHandler : IRouteHandler
    {
        public EmbeddedResourceHandlerConfig Config { get; private set; }
        public EmbeddedResourceRouteHandler(EmbeddedResourceHandlerConfig config)
        {
            Config = config;
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(requestContext, Config);
        }
    }
}
