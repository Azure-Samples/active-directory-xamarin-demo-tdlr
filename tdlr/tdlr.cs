using System;

using Xamarin.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;

namespace tdlr
{
	public class App : Application
	{
		// App Config Values
		public static AuthenticationContext AuthContext;
		public static string clientId = "3d8c4803-ffcd-4b2a-baec-05056abdc408";
		public static string taskApiResourceId = "https://strockisdevtwo.onmicrosoft.com/tdlr";
		public static string graphApiResourceId = "https://graph.windows.net";
		public static string graphApiVersion = "1.5";
		public static string commonAuthority = "https://login.microsoftonline.com/common";
		public static Uri redirectUri = new Uri("http://tdlr");

		public App ()
		{
			App.SetADALAuthority ();
			MainPage = new NavigationPage(new TaskListPage());
		}

		public static void SetADALAuthority() 
		{
			// If there aren't any tokens cached from previous app runs, use the common authority
			App.AuthContext = new AuthenticationContext (commonAuthority);
			if (App.AuthContext.TokenCache.ReadItems ().Count () > 0) {

				// But if there was a cached token, use its authority to maintain the user's session
				string cachedAuthority = App.AuthContext.TokenCache.ReadItems ().First ().Authority;
				App.AuthContext = new AuthenticationContext (cachedAuthority);
			}
			
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

