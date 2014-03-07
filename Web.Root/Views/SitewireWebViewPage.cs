using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Web.Root.Helpers;

namespace Web.Root.Views
{   
    public abstract class SitewireWebViewPage<T> : WebViewPage<T>
    {
        //public ScriptContext scriptContext { get; }  
        //public SitewireWebViewPage<T> {
            
        
        //}
    }

    public abstract class SitewireWebViewPage : SitewireWebViewPage<object> { }
}
