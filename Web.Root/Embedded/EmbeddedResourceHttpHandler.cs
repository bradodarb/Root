using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Web.Root.Embedded
{
    public class EmbeddedResourceHttpHandler: IHttpHandler
    {
        private RequestContext _RequestContext;
        private HttpContext _HttpContext;

        public bool IsReusable { get { return false; } }
        public RequestContext RequestContext { get; private set; }
        public EmbeddedResourceHandlerConfig Config { get; private set; }

        public EmbeddedResourceHttpHandler(RequestContext requestContext, EmbeddedResourceHandlerConfig config)
        {
            Config = config;

            _RequestContext = requestContext;
        }


        public void ProcessRequest(HttpContext context)
        {
            _HttpContext = context;

            var asset = _RequestContext.RouteData.Values[Config.RouteKey];
            string assetID = asset.ToString();

            if (string.IsNullOrWhiteSpace(assetID))
            {
                throw new ArgumentNullException();
            }

            string path = _HttpContext.Request.MapPath("~/" + Config.Prefix + "/" + assetID, "", false);
            if (File.Exists(path))
            {               
                // hard coded to the image/jpg type (obviously needs to adjust)
                context.Response.AddHeader("Content-Type", "image/jpg");
                context.Response.AddHeader("Content-Length", new FileInfo(path).Length.ToString());
                context.Response.WriteFile(path);
            }
        }
    }
}
