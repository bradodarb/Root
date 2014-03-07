using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Web.Root.Embedded;

namespace Web.Client
{
    public class AsyncResourceResult : IAsyncResult
    {
        const string _EmbeddedPrefix = "SW/Resource";

        private HttpContext _Context;
        private Object _State;
        private AsyncCallback _Callback;
        private bool _Completed;

         bool IAsyncResult.IsCompleted { get { return _Completed; } }
         WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
         Object IAsyncResult.AsyncState { get { return _State; } }
         bool IAsyncResult.CompletedSynchronously { get { return false; } }

        public AsyncResourceResult(AsyncCallback callback, HttpContext context, Object state)
        {
            _Callback = callback;
            _Context = context;
            _State = state;
            _Completed = false;
        }

        public void StartAsyncWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartAsyncTask), null);
        }

        private void StartAsyncTask(Object workItemState)
        {

            var path = _Context.Request.Url.AbsolutePath;
            var file = GetFile(path);
            if (file != null)
            {
                _Context.Response.BinaryWrite(file);
            }
            _Completed = true;
            _Callback(this);
        }

        private byte[] GetFile(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                var assembly = typeof(AsyncResourceResult).Assembly;
                string fileNameWithExtension = virtualPath.ToLower().Replace(_EmbeddedPrefix.ToLower() + "/", "");
                string nameSpace = assembly.GetName().Name;
                byte[] result = null;
                string manifestResourceName = string.Format("{0}{1}", nameSpace,
                    fileNameWithExtension.Replace("/", ".")).Replace("-", "_");

                var target = assembly.GetManifestResourceNames().FirstOrDefault(x => x.ToLower() == manifestResourceName.ToLower());
                if (target != null)
                {
                    var stream = assembly.GetManifestResourceStream(target);
                    SetReponseType(target);
                    result = new byte[stream.Length];

                    stream.Read(result, 0, result.Length);
                }
                return result;
            }
            else
                return null;
        }
        private bool IsEmbeddedPath(string path)
        {
            return path.ToLower().Contains(_EmbeddedPrefix.ToLower());
        }


        private void SetReponseType(string path)
        {
            string ext = path.Substring(path.LastIndexOf('.'));
            if (!String.IsNullOrEmpty(ext))
            {
                switch (ext)
                {
                    case ".css":
                        _Context.Response.ContentType = "text/css";
                        break;
                    case ".js":
                        _Context.Response.ContentType = "text/javascript";
                        break;
                    case ".json":
                        _Context.Response.ContentType = "application/json";
                        break;
                    case ".html":
                        _Context.Response.ContentType = "text/html";
                        break;
                    case ".xml":
                        _Context.Response.ContentType = "application/xml";
                        break;
                    case ".jpg":
                        _Context.Response.ContentType = "image/jpg";
                        break;
                    case ".bmp":
                        _Context.Response.ContentType = "image/bmp";
                        break;
                    case ".png":
                        _Context.Response.ContentType = "image/png";
                        break;
                    case ".gif":
                        _Context.Response.ContentType = "image/gif";
                        break;
                    case ".svg":
                        _Context.Response.ContentType = "text/css";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
