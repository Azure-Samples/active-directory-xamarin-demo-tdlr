using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace tdlr
{
	public class GraphHelper
	{
		public static bool isAADDomain(string domain)
		{
			string url_template = "https://login.microsoftonline.com/{0}/.well-known/openid-configuration";
			string url = String.Format (url_template, domain);
			var req = WebRequest.CreateHttp (url);

			try {
				var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
					req.BeginGetResponse, req.EndGetResponse, null).Result;

				HttpWebResponse resp = res as HttpWebResponse;
				if (resp.StatusCode != HttpStatusCode.OK)
					return false;
				
			} catch (Exception ex) {
				return false;
			}

			return true;
		}

		public static async System.Threading.Tasks.Task<List<AADObject>> SearchUsers(string inputValue)
		{

			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.graphApiResourceId, App.clientId);
			string url = "https://graph.windows.net/" + authResult.TenantId + "/users?api-version=" + App.graphApiVersion + "&$top=10";

			if (!String.IsNullOrEmpty (inputValue)) {
				url += "&$filter=" +
					"startswith(displayName,'" + inputValue +
					"') or startswith(givenName,'" + inputValue +
					"') or startswith(surname,'" + inputValue +
					"') or startswith(userPrincipalName,'" + inputValue +
					"') or startswith(mail,'" + inputValue +
					"') or startswith(mailNickname,'" + inputValue +
					"') or startswith(jobTitle,'" + inputValue +
					"') or startswith(department,'" + inputValue +
					"') or startswith(city,'" + inputValue + "')";
			}

			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "GET";
			req.ContentType = "application/json";


			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			GraphResponse gResp = serializer.Deserialize<GraphResponse> (jreader);
			return gResp.value;
		}
	}
}

