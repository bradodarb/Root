using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Web.Root
{
	public static class RecaptchaValidatorHelper
	{
		public static bool ValidateCaptcha(string privateKey, string challenge, string response)
		{
			if (!string.IsNullOrEmpty(challenge) && !string.IsNullOrEmpty(response))
			{
				string remoteIp = HttpContext.Current.Request.UserHostAddress;
				string body = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
					privateKey,
					remoteIp,
					challenge,
					response);

				WebRequest request = WebRequest.Create("http://www.google.com/recaptcha/api/verify");
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";

				byte[] bytes = System.Text.Encoding.ASCII.GetBytes(body);
				request.ContentLength = bytes.Length;
				Stream output = request.GetRequestStream();
				output.Write(bytes, 0, bytes.Length);
				output.Close();

				WebResponse webResp = request.GetResponse();
				StreamReader reader = new StreamReader(webResp.GetResponseStream());
				string responseValue = reader.ReadLine();

				bool success = Convert.ToBoolean(responseValue);

				return success;
			}
			else
			{
				return false;
			}
		}
	}
}
