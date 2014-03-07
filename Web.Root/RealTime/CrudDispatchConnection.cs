using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL.Root;
using BLL.Root.Dto;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Util.Root;

namespace Web.Root.Realtime
{ 
    /// <summary>
    /// Dispatches CRUD operations to all clients, excluding the caller
    /// </summary>
    public class CrudDispatchConnection : PersistentConnection 
    {

        public static Action OnInit { get; set; }

        public static string ConnectionCookieContainer { get; set; }

        public static string ConnectionIdCookie { get; set; }

        public CrudDispatchConnection()
        {
            ConnectionCookieContainer = "crudConnectionId";
            ConnectionIdCookie = "id";
            if (OnInit != null)
            {
                OnInit();
                OnInit = null;
            }
        }


        public static void Dispatch(string adminName, CRUDDispatchType action, params DtoBase[] entities)
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<CrudDispatchConnection>();
            string payload = String.Empty;
            if (entities != null)
            {
                if (entities.Length > 1)
                {
                    payload = JsonConvert.SerializeObject(new
                    {
                        adminName = adminName,
                        action = Util.Root.EnumHelpers.GetDescription(action),
                        data = entities.ToArray()
                    }, new StringEnumConverter());
                }
                else if (entities.Length > 0)
                {
                    payload = JsonConvert.SerializeObject(new
                    {
                        adminName = adminName,
                        action = Util.Root.EnumHelpers.GetDescription(action),
                        data = entities.FirstOrDefault()
                    }, new StringEnumConverter());
                }
            }
            if (payload != String.Empty)
            {
                var connectionid = ReadConnection();
                if (connectionid != String.Empty)
                {
                    context.Connection.Broadcast(payload, connectionid);
                }
                else
                {
                    context.Connection.Broadcast(payload);
                }
            }
        }


        #region Connection Id Management
        protected override System.Threading.Tasks.Task OnConnected(IRequest request, string connectionId)
        {
            AddConnection(request, connectionId);

            return base.OnConnected(request, connectionId);
        }

        protected override System.Threading.Tasks.Task OnDisconnected(IRequest request, string connectionId)
        {
            RemoveConnection(request, connectionId);

            return base.OnDisconnected(request, connectionId);
        }

        protected override System.Threading.Tasks.Task OnReconnected(IRequest request, string connectionId)
        {
            AddConnection(request, connectionId);

            return base.OnReconnected(request, connectionId);
        }

        protected void AddConnection(IRequest request, string connectionId)
        {
            var response = HttpContext.Current.Response;
            response.Cookies[ConnectionCookieContainer][ConnectionIdCookie] = connectionId;
            response.Cookies[ConnectionCookieContainer].Expires = DateTime.Now.AddDays(1d);

        }
        
        protected void RemoveConnection(IRequest request, string connectionId)
        {
            if (HttpContext.Current != null)
            {
                if (request.Cookies[ConnectionCookieContainer] != null)
                {
                    if (HttpContext.Current.Response != null)
                    {
                        var cookie = request.Cookies[ConnectionCookieContainer].Value;
                        if (cookie != null)
                        {
                            HttpCookie removeCookie = new HttpCookie(ConnectionCookieContainer);
                            removeCookie.Expires = DateTime.Now.AddDays(-1d);
                            HttpContext.Current.Response.Cookies.Add(removeCookie);
                        }
                    }
                }
            }
        }

        protected static string ReadConnection()
        {
            var request = HttpContext.Current.Request;
            if (request.Cookies[ConnectionCookieContainer] != null)
            {

                if (request.Cookies[ConnectionCookieContainer][ConnectionIdCookie] != null)
                {
                    return request.Cookies[ConnectionCookieContainer][ConnectionIdCookie];
                }
            }

            return String.Empty;
        } 
        #endregion
    }
}