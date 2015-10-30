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
			string discovery_template = "https://login.microsoftonline.com/{0}/.well-known/openid-configuration";
			string issuer_template = "https://sts.windows.net/{0}/";
			List<string> ignored_tenants = new List<string> {
				String.Format(issuer_template, "9cd80435-793b-4f48-844b-6b3f37d1c1f3"),
				String.Format(issuer_template, "f8cdef31-a31e-4b4a-93e4-5f571e91255a"),
			};

			// Construct the discovery request
			string url = String.Format (discovery_template, domain);
			var req = WebRequest.CreateHttp (url);

			try {
				// Send a request to the AAD Discovery Endpoint
				var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
					req.BeginGetResponse, req.EndGetResponse, null).Result;

				// If the request fails, there's no AAD tenant
				HttpWebResponse resp = res as HttpWebResponse;
				if (resp.StatusCode != HttpStatusCode.OK)
					return false;

				// Ignore the two AAD tenants with consumer domains, like gmail.com
				var stream = res.GetResponseStream ();
				JsonSerializer serializer = new JsonSerializer ();
				StreamReader reader = new StreamReader (stream);
				JsonTextReader jreader = new JsonTextReader (reader);
				DiscoveryResponse dResp = serializer.Deserialize<DiscoveryResponse> (jreader);
				return !ignored_tenants.Contains(dResp.issuer);
				
			} catch (Exception ex) {
				return false;
			}


		}

		public static async System.Threading.Tasks.Task<List<AADObject>> SearchUsers(string inputValue)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.graphApiResourceId, App.clientId);

			// Construct the AAD Graph API query
			string url = "https://graph.windows.net/" + authResult.TenantId + "/users?api-version=" + App.graphApiVersion + "&$top=10";

			// Add filters to the query (10 max)
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

			// Send GET request with access token
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "GET";
			req.ContentType = "application/json";
			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;
			
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			// Parse response from AAD Graph API
			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			GraphResponse gResp = serializer.Deserialize<GraphResponse> (jreader);
			return gResp.value;
		}
	}

	public class GraphResponse
	{
		[JsonProperty("odata.metadata")]
		public string metadata { get; set; }
		public List<AADObject> value { get; set; }
		[JsonProperty("odata.nextLink")]
		public string nextLink { get; set; }
	}

	public class DiscoveryResponse
	{
		public string issuer { get; set; }
	}
}

