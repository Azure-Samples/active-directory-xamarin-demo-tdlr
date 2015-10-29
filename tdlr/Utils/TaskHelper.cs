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
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			return serializer.Deserialize<List<Task>> (jreader);
		}

		public static async System.Threading.Tasks.Task DeleteTask(Task task)
		{
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks/" + task.TaskID;
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "DELETE";

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.NoContent)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task UpdateTask(Task task)
		{
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks/" + task.TaskID;
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "PUT";
			req.ContentType = "application/json";

			string serialized = JsonConvert.SerializeObject (task);
			var byteArray = Encoding.UTF8.GetBytes (serialized);
			var reqStream = System.Threading.Tasks.Task.Factory.FromAsync<Stream> (
				req.BeginGetRequestStream, req.EndGetRequestStream, null).Result;
			reqStream.Write (byteArray, 0, byteArray.Length);

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task CreateTask(string text)
		{
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "POST";
			req.ContentType = "application/json";

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

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();
		}

		public static async void ShareTask(int taskID, List<Share> shares)
		{
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks/" + taskID + "/share";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;
			req.Method = "PUT";
			req.ContentType = "application/json";

			string serialized = JsonConvert.SerializeObject (shares);
			var byteArray = Encoding.UTF8.GetBytes (serialized);
			var reqStream = System.Threading.Tasks.Task.Factory.FromAsync<Stream> (
				req.BeginGetRequestStream, req.EndGetRequestStream, null).Result;
			reqStream.Write (byteArray, 0, byteArray.Length);

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.NoContent)
				throw new Exception ();
		}

		public static async System.Threading.Tasks.Task<List<Share>> GetShares(int taskID)
		{
			AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync(App.taskApiResourceId, App.clientId);

			string url = TaskHelper.taskApi + "api/tasks/" + taskID + "/share";
			var req = WebRequest.CreateHttp (url);
			req.Headers ["Authorization"] = "Bearer " + authResult.AccessToken;

			var res = System.Threading.Tasks.Task.Factory.FromAsync<WebResponse> (
				req.BeginGetResponse, req.EndGetResponse, null).Result;

			HttpWebResponse resp = res as HttpWebResponse;
			if (resp.StatusCode != HttpStatusCode.OK)
				throw new Exception ();

			var stream = res.GetResponseStream ();
			JsonSerializer serializer = new JsonSerializer ();
			StreamReader reader = new StreamReader (stream);
			JsonTextReader jreader = new JsonTextReader (reader);
			return serializer.Deserialize<List<Share>> (jreader);
		}
	}
}

