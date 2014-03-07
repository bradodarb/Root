using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Web.Root.Cookies
{
    public static class CookieManager
    {

        public static void RemoveCookie(string cookieId, HttpContext context = null)
        {
            if (context == null)
            {
                context = HttpContext.Current;
            }
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    if (request.Cookies[cookieId] != null)
                    {
                        if (context.Response != null)
                        {
                            var cookie = request.Cookies[cookieId].Value;
                            if (cookie != null)
                            {
                                HttpCookie removeCookie = new HttpCookie(cookieId);
                                removeCookie.Expires = DateTime.Now.AddDays(-1d);
                                context.Response.Cookies.Add(removeCookie);
                            }
                        }
                    }
                }
            }
        }

        public static void RemoveCookie(string cookiecontainerId, string cookieId, HttpContext context = null)
        {
            if (context == null)
            {
                context = HttpContext.Current;
            }
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    if (request.Cookies[cookiecontainerId] != null)
                    {
                        if (request.Cookies[cookiecontainerId][cookieId] != null)
                        {
                            if (context.Response != null)
                            {
                                var cookie = request.Cookies[cookiecontainerId][cookieId];
                                if (cookie != null)
                                {
                                    HttpCookie removeCookie = new HttpCookie(cookieId);
                                    removeCookie.Expires = DateTime.Now.AddDays(-1d);
                                    context.Response.Cookies.Add(removeCookie);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddCookie(string cookiecontainerId, string cookieId, object cookievalue, DateTime lifespan, HttpContext context = null)
        {
            if (context == null)
            {
                context = HttpContext.Current;
            }
            if (context != null)
            {
                var response = context.Response;
                if (response != null)
                {

                    response.Cookies[cookiecontainerId][cookieId] = cookievalue.ToString();
                    response.Cookies[cookiecontainerId].Expires = lifespan;

                }
            }
        }

        public static string GetCookie(string cookiecontainerId, string cookieId, HttpContext context = null)
        {
            if (context == null)
            {
                context = HttpContext.Current;
            }
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    if (request.Cookies[cookiecontainerId] != null)
                    {
                        if (request.Cookies[cookiecontainerId][cookieId] != null)
                        {
                            return request.Cookies[cookiecontainerId][cookieId];
                        }
                    }
                }
            }
            return String.Empty;
        }

    }
}
