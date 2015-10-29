using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using tdlr;
using tdlr.iOS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

[assembly: ExportRenderer(typeof(SignUpPage), typeof(ADALPageRenderer))]
[assembly: ExportRenderer(typeof(SignInPage), typeof(ADALPageRenderer))]

namespace tdlr.iOS
{
	public class ADALPageRenderer : PageRenderer
	{
		TaskListPage taskListPage;
		SignInPage signInPage;
		SignUpPage signUpPage;

		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);
			if (e.NewElement is TaskListPage) {
				taskListPage = ((TaskListPage)e.NewElement);
			} else if (e.NewElement is SignInPage) {
				signInPage = ((SignInPage)e.NewElement);
			} else if (e.NewElement is SignUpPage) {
				signUpPage = ((SignUpPage)e.NewElement);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (taskListPage != null)
				taskListPage.platformParams = new PlatformParameters (this);
			if (signInPage != null)
				signInPage.platformParams = new PlatformParameters (this);
			if (signUpPage != null)
				signUpPage.platformParams = new PlatformParameters (this);
		}	
	}
}

