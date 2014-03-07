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
using Web.Root.Controllers;
using Model.Root;
using Newtonsoft.Json.Serialization;

namespace Web.Root.Controllers
{
    public abstract class AdminApiControllerBase<T> : ApiController where T : IPersistenceEntity
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
            var admin = GetAdmin();
            var model = admin.Get(param);

            return model;
        }

        [HttpGet]
        public virtual dynamic All()
        {
            var admin = GetAdmin();
            var models = admin.GetAll();


            var result = new
            {
                items = models
            };

            return result;
        }



        [HttpGet]
        public virtual dynamic Many(string param)
        {
            var models = new List<T>();
            var admin = GetAdmin();
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
                                models.Add(area);
                            }
                        }
                    }
                }
                var result = new
                {
                    items = models
                };

                return result;
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

            var admin = GetAdmin();

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

        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]T param)
        {
            var admin = GetAdmin();
            try
            {
                if (param != null)
                {
                    var result = admin.New(param);
                    return new HttpResponseMessage(HttpStatusCode.Created);
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                title = "Could not create new Item",
                                message = "Item in request was null"
                            })),
                        ReasonPhrase = "Create Item Failed"
                    });
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = "Could not create new model ",
                            message = ex.Message
                        })),
                    ReasonPhrase = "Create Item Failed"
                });
            }
        }

        [HttpPost]
        public virtual T New([FromBody]T param)
        {
            var admin = GetAdmin();
            try
            {

                var result = admin.New(param);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    throw new Exception("Failed to create new Item");
                }

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = "Could not create new model ",
                            message = ex.Message
                        })),
                    ReasonPhrase = "Create Item Failed"
                });
            }
        }

        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]T param)
        {
            var admin = GetAdmin();
            try
            {
                int id = 0;
                List<ValidationInfo> valList = new List<ValidationInfo>();
                if (admin.Validate(param, valList) == true)
                {
                    admin.Update(param, out id);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                id = id
                            }))
                    };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                id = id
                                ,
                                errors = valList
                            }))
                    };
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = String.Format("Could not update the model {0}", param.Id),
                            message = ex.Message
                        })),
                    ReasonPhrase = "Update Item Failed"
                });
            }

        }

        [HttpPatch]
        public virtual HttpResponseMessage Patch([FromUri]int id, [FromBody]string param)
        {
            var admin = GetAdmin();
            try
            {
                dynamic source = JObject.Parse(param);

                if (admin.Patch(source, id))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                     {
                         Content = new StringContent(
                             JsonConvert.SerializeObject(new
                             {
                                 id = id
                             }))
                     };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                id = id
                            }))
                    };
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = String.Format("Could not patch the model {0}", id),
                            message = ex.Message
                        })),
                    ReasonPhrase = "Path Item Failed"
                });
            }
        }

        [HttpDelete]
        public virtual HttpResponseMessage Delete([FromUri]int param)
        {
            var admin = GetAdmin();
            try
            {
                if (!admin.Remove(param))
                {
                    throw new Exception("Remove Item failed!");
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = String.Format("Could not remove the model(s) {0}", param),
                            message = ex.Message
                        })),
                    ReasonPhrase = "Update Item Failed"
                });
            }
        }

        [HttpDelete]
        public virtual HttpResponseMessage DeleteMany([FromUri] string param)
        {
            var admin = GetAdmin();
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
                            if (!admin.Remove(target))
                            {
                                throw new Exception("Remove area failed!");
                            }
                        }
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            title = String.Format("Could not remove the model(s) {0}", param),
                            message = ex.Message
                        })),
                    ReasonPhrase = "Update Area Failed"
                });
            }
        }

        [HttpGet]
        public virtual dynamic AutoComplete(string query)
        {
            var admin = GetAdmin() as IAutoCompleteProvider;

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

        protected abstract ICompositeService<T> GetAdmin();


        protected static JsonSerializerSettings JsonSettings()
        {
            var result = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateParseHandling = DateParseHandling.DateTime
            };
            result.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return result;
        }
    }
}