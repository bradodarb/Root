using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Web.Client
{
    public class EmbeddedContentHandler : IHttpAsyncHandler
    {
        public bool IsReusable { get { return false; } }

        public EmbeddedContentHandler()
        {
        }
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, Object extraData)
        {
            AsyncResourceResult asynch = new AsyncResourceResult(cb, context, extraData);
            asynch.StartAsyncWork();

            return asynch;
        }

        public void EndProcessRequest(IAsyncResult result)
        {

        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }


    }

  
}