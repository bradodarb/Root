using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using BLL.Root;
using BLL.Root.Dto;
using Util.Root.Common;
using BLL.Root.Validation;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Web.Root.Controllers
{
    public abstract class DtoAdminApiControllerBase<T> : AdminApiControllerBase<T> where T : DtoBase
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
    }
}