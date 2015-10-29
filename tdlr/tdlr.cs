using System;

using Xamarin.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;

namespace tdlr
{
	public class App : Application
	{
		public static AuthenticationContext AuthContext;
		public static string clientId = "3d8c4803-ffcd-4b2a-baec-05056abdc408";
		public static string taskApiResourceId = "https://strockisdevtwo.onmicrosoft.com/tdlr";
		public static string graphApiResourceId = "https://graph.windows.net";
		public static string graphApiVersion = "1.5";
		public static string commonAuthority = "https://login.microsoftonline.com/common";
		public static Uri redirectUri = new Uri("http://tdlr");

		public App ()
		{
			// The root page of your application

			AuthContext = new AuthenticationContext (commonAuthority);
			if (AuthContext.TokenCache.ReadItems ().Count () > 0) {
				string cachedAuthority = AuthContext.TokenCache.ReadItems ().First ().Authority;
				AuthContext = new AuthenticationContext (cachedAuthority);
			}

			MainPage = new NavigationPage(new TaskListPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

