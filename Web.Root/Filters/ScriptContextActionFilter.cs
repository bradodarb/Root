using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Web.Root.Helpers;

namespace Web.Root.Filters
{
    public interface IScriptContextController
    {
        Web.Root.Helpers.ScriptContext scriptContext { get; set; }
    }

    public class ScriptContextActionFilter : ActionFilterAttribute,  IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controller = filterContext.Controller as IScriptContextController;
            //if (controller != null)
            //{
            //    controller.scriptContext = Web.Root.Helpers.ScriptHtmlHelperExtensions.CreateScriptContext(filterContext.HttpContext);
            //}

        }
    }
}
