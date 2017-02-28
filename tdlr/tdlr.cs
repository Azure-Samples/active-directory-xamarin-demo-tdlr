using System;

using Xamarin.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;

namespace tdlr
{
	public class App : Application
	{
		// App Config Values
		public static string clientId = "3b4c1be3-8c00-4162-935e-61b4601bf1d9"; // Enter your client ID as registered in the Azure Management Portal if setting up your own app
		public static string taskApiResourceId = "https://strockisdevtwo.onmicrosoft.com/tdlr";
		public static string graphApiResourceId = "https://graph.windows.net";
		public static string graphApiVersion = "1.6";
		public static string commonAuthority = "https://login.microsoftonline.com/common";
		public static Uri redirectUri = new Uri("http://tdlr");

		public App ()
		{
			MainPage = new NavigationPage(new TaskListPage());
		}

		public static IPlatformParameters PlatformParameters { get; set; }

		private static AuthenticationContext authContext;
		public static AuthenticationContext AuthContext
		{
			get
			{
				if (authContext == null)
				{
					SetADALAuthority();
				}
				return authContext;
			}
		}

		public static void SignOut()
		{
			AuthContext.TokenCache.Clear();
			authContext = null;
		}

		private static void SetADALAuthority() 
		{
			// If there aren't any tokens cached from previous app runs, use the common authority
			authContext = new AuthenticationContext (commonAuthority);
			if (authContext.TokenCache.ReadItems ().Count () > 0) {

				// But if there was a cached token, use its authority to maintain the user's session
				string cachedAuthority = authContext.TokenCache.ReadItems ().First ().Authority;
				authContext = new AuthenticationContext (cachedAuthority);
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

