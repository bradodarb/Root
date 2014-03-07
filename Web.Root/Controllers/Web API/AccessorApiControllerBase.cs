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
using Newtonsoft.Json.Serialization;

namespace Web.Root.Controllers
{
    public abstract class AccessorApiControllerBase<T> : ApiController where T : DtoBase
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static JsonSerializerSettings _JSONsettings;
        public static JsonSerializerSettings JSONsettings
        {
            get
            {
                if (_JSONsettings == null)
                {
                    _JSONsettings = JsonSettings();
                }
                return _JSONsettings;
            }
        }

        [HttpGet]
        public virtual T Get(int param)
        {
            var admin = GetAccessor();
            var model = admin.Get(param);

            return model;
        }


        [HttpGet]
        public virtual List<T> Many(string param)
        {
            var items = new List<T>();
            var admin = GetAccessor();
            try
            {
                var ids = param.Split(',');
                int target = -1;
                if (ids != null)
                {
                    foreach (var i in ids)
                    {
                        target = -1;
                        if (Int32.TryParse(i, out target) && target > 0)
                        {
                            var area = admin.Get(target);
                            if (area == null)
                            {
                                throw new Exception(String.Format("Get Item {0} failed!", target));
                            }
                            else
                            {
                                items.Add(area);
                            }
                        }
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = String.Format("Could not get 'many' model(s)... request = {0}", param),
                            message = ex.Message
                        })),
                    ReasonPhrase = "Update Item Failed"
                });
            }
        }


        [HttpGet]
        public virtual dynamic Page(int count, int page, string field, bool? desc, string filter)
        {
            var descending = desc.HasValue ? desc.Value : false;

            var admin = GetAccessor();

            var list = new List<T>();

            bool hasprev = false;
            bool hasnext = false;
            int total = 0;
            string position = "";



            list = admin.GetPage(count, page, field, descending, filter, out total, out hasprev, out hasnext, out position, "");

            var result = new
            {
                prev = hasprev,
                next = hasnext,
                count = total,
                caption = position,
                items = list

            };

            return result;
        }


        [HttpGet]
        public virtual dynamic AutoComplete(string query)
        {
            var admin = GetAccessor() as IAutoCompleteProvider;

            if (admin != null)
            {
                var source = admin.GetMatches(query);
                return new { options = source };
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = "Auto-complete not supported",
                            message = "This admin does not support an auto-complete method and does not implement the IAutoCompleteProvider interface"
                        })),
                    ReasonPhrase = "Auto Complete"
                });
            }
        }


        protected abstract IAccessorService<T> GetAccessor();


        protected virtual HttpResponseMessage ExecuteNonQuery(Action method, params object[] args)
        {
            try
            {
                method.DynamicInvoke(args);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            message = "ok"
                        }))
                };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            message = ex.Message
                        }))
                });
            }
        }
        protected virtual HttpResponseMessage ExecuteQuery<T>(Func<T> method, params object[] args)
        {
            try
            {
                var result = method.DynamicInvoke(args);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {

                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            data = result
                        }, JSONsettings))
                };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            message = ex.Message
                        }))
                });
            }
        }

        protected static JsonSerializerSettings JsonSettings()
        {
            var result = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateParseHandling = DateParseHandling.DateTime,

            };
            result.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return result;
        }
    }
}