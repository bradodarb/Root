using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Root
{
	/// <summary>
	/// Similiar to <see cref="JsonResult"/>, with
	/// the exception that the <see cref="DataContract"/> attributes are
	/// respected. Requires JSON.NET.
	/// </summary>
	/// <remarks>
	/// Based on the excellent stackoverflow answer:
	/// http://stackoverflow.com/a/263416/1039947
	/// </remarks>
	public class JsonDataContractActionResult : ActionResult
	{
		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="data">Data to parse.</param>
		public JsonDataContractActionResult(Object data, JsonSerializerSettings settings = null)
		{
			Data = data;
			Settings = settings;
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public Object Data { get; private set; }

		public JsonSerializerSettings Settings { get; private set; }

		/// <summary>
		/// Enables processing of the result of an action method by a 
		/// custom type that inherits from the ActionResult class. 
		/// </summary>
		/// <param name="context">The controller context.</param>
		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			string output = JsonConvert.SerializeObject(Data, Formatting.None, Settings);

			if (context.HttpContext.Request.Browser.Browser == "IE" && context.HttpContext.Request.Browser.MajorVersion < 10)
			{
				// IE version 9 and below doesn't support the application/json MIME Type, so use text/plain instead
				context.HttpContext.Response.ContentType = "text/plain"; 
			}
			else
			{
				context.HttpContext.Response.ContentType = "application/json";
			}
			context.HttpContext.Response.Write(output);
		}
	}
}
