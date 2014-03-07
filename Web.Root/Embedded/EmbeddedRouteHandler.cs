using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Web.Root.Embedded
{
    public class EmbeddedResourceRouteHandler : IRouteHandler
    {
        private Type _SourceType;

        public EmbeddedResourceRouteHandler(Type sourceType)
        {
            _SourceType = sourceType;
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(_SourceType, requestContext.RouteData);
        }
    }

    public class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private List<string> _AllowedExtensions = new List<string> { ".css", ".js", ".json", ".html", ".xml", ".jpg", ".bmp", "png", ".gif", ".svg" };
        private Type _SourceType;
        private RouteData _RouteData;

        public EmbeddedResourceHttpHandler(Type sourceType, RouteData routeData)
        {
            _SourceType = sourceType;
            _RouteData = routeData;
        }
        
        public bool IsReusable
        {
            get { return false; }
        }
        
        public void ProcessRequest(HttpContext context)
        {
            var routeDataValues = _RouteData.Values;
            var key = routeDataValues["resource"];
            if (key != null)
            {

                string file = key.ToString();
                if (file.Contains('.'))
                {
                    string filename = file.Substring(file.LastIndexOf('/') + 1);

                    if (filename.Contains('.'))
                    {
                        string ext = filename.Substring(filename.LastIndexOf('.'));

                        if (_AllowedExtensions.Contains(ext))
                        {
                            string nameSpace = _SourceType.Assembly.GetName().Name;

                            var target = _SourceType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.ToLower().EndsWith(filename.ToLower()));

                            if (target != null)
                            {

                                var stream = _SourceType.Assembly.GetManifestResourceStream(target);

                                context.Response.Clear();


                                switch (ext)
                                {
                                    case ".css":
                                        context.Response.ContentType = "text/css";
                                        break;
                                    case ".js":
                                        context.Response.ContentType = "text/javascript";
                                        break;
                                    case ".json":
                                        context.Response.ContentType = "application/json";
                                        break;
                                    case ".html":
                                        context.Response.ContentType = "text/html";
                                        break;
                                    case ".xml":
                                        context.Response.ContentType = "application/xml";
                                        break;
                                    case ".jpg":
                                        context.Response.ContentType = "image/jpg";
                                        break;
                                    case ".bmp":
                                        context.Response.ContentType = "image/bmp";
                                        break;
                                    case ".png":
                                        context.Response.ContentType = "image/png";
                                        break;
                                    case ".gif":
                                        context.Response.ContentType = "image/gif";
                                        break;
                                    case ".svg":
                                        context.Response.ContentType = "text/css";
                                        break;
                                    default:
                                        break;
                                }
                                if (stream != null)
                                {
                                    stream.CopyTo(context.Response.OutputStream);

                                    context.Response.Flush();
                                    //context.Response.Close();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
