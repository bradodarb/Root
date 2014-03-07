using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Web.Root.UserTracking
{
    public static class UserTrackingUtils
    {
        public static string GetUserIP(HttpRequest request)
        {
            var result = String.Empty;
            string ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!String.IsNullOrEmpty(ipList))
            {
                result = ipList.Split(',')[0];
            }

            if (String.IsNullOrEmpty(result))
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }

            if (result.Contains("::1"))
            {
                result = MachineIPAddress();
            }
            return result;
        }

        public static string MachineIPAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = (
                       from addr in hostEntry.AddressList
                       where addr.AddressFamily.ToString() == "InterNetwork"
                       select addr.ToString()
                ).FirstOrDefault();
            return ip;
        }
        public static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
