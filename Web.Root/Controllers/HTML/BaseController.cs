using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Util.Root.Log4Net; 

namespace Web.Root.Controllers
{
    public class BaseController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			log.InfoFormat("Executing Request: {0}\t Controller: {1}, Action: {2}", filterContext.RequestContext.HttpContext.Request.Url, filterContext.RouteData.Values["controller"], filterContext.RouteData.Values["action"]);
			base.OnActionExecuting(filterContext);
		}

        protected override void OnException(ExceptionContext filterContext)
        {
            log.Error(LogProvider.BuildExceptionMessage(filterContext.Exception));
            base.OnException(filterContext);
        }
    }
}
