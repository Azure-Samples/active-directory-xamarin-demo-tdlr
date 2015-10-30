using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace tdlr
{
	public class TaskHelper
	{
		private static string taskApi = "http://todolistreimagined.azurewebsites.net/";

		public static async System.Threading.Tasks.Task<List<Task>> GetUserTasks()
		{	
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;

			// Send the request
			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			// Parse the response from the task API
			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			return serializer.Deserialize<List<Task>> (jreader);
		}

		public static async System.Threading.Tasks.Task DeleteTask(Task task)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks/" + task.TaskID;
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "DELETE";

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			// Throw an exception if task not deleted
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.NoContent)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task UpdateTask(Task task)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks/" + task.TaskID;
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "PUT";
			req.ContentType = "application/json";

			// Write object to the request body and send
			string serialized = JsonConvert.SerializeObject (task);
			var byteArray = Encoding.UTF8.GetBytes (serialized);
			var reqStream = System.Threading.Tasks.Task.Factory.FromAsync<Stream> (
				req.BeginGetRequestStream, req.EndGetRequestStream, null).Result;
			reqStream.Write (byteArray, 0, byteArray.Length);

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			// If PUT request failed, theoq
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task CreateTask(string text)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "POST";
			req.ContentType = "application/json";

			// Write (a subset of) the object to the request body and send
			object newTask = new {
				TaskText = text
			};
			string serialized = JsonConvert.SerializeObject (newTask);
			var byteArray = Encoding.UTF8.GetBytes (serialized);
			var reqStream = System.Threading.Tasks.Task.Factory.FromAsync<Stream> (
				req.BeginGetRequestStream, req.EndGetRequestStream, null).Result;
			reqStream.Write (byteArray, 0, byteArray.Length);

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			// If the POST request failed, throw
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();
		}
			
		public static async System.Threading.Tasks.Task ShareTask(int taskID, List<AADObject> shares)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks/" + taskID + "/share";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "PUT";
			req.ContentType = "application/json";

			// Write the list of shares to the request body and send
			string serialized = JsonConvert.SerializeObject (shares);
			var byteArray = Encoding.UTF8.GetBytes (serialized);
			var reqStream = System.Threading.Tasks.Task.Factory.FromAsync<Stream> (
				req.BeginGetRequestStream, req.EndGetRequestStream, null).Result;
			reqStream.Write (byteArray, 0, byteArray.Length);

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			// If the PUT request failed, throw
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.NoContent)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task<List<AADObject>> GetShares(int taskID)
		{
			// Get a token from ADAL
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			// Construct the request with an access token
			string url = TaskHelper.taskApi + "api/tasks/" + taskID + "/share";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			// If the request failed, throw
			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			// Parse the response and return a list of shares
			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			return serializer.Deserialize<List<AADObject>> (jreader);
		}
	}
}

