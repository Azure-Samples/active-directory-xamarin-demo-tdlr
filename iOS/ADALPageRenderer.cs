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
	// This class is used to pass the iOS specific context to ADAL in the Xam Forms Pages.
	// Use it for any page which will use ADAL to pop up a WebView and sign in the user.
	// (So, any page that makes a call to AcquireTokenAsync(...)

	public class ADALPageRenderer : PageRenderer
	{
		SignInPage signInPage;
		SignUpPage signUpPage;

		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);
			if (e.NewElement is SignInPage) {
				signInPage = ((SignInPage)e.NewElement);
			} else if (e.NewElement is SignUpPage) {
				signUpPage = ((SignUpPage)e.NewElement);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (signInPage != null)
				signInPage.platformParams = new PlatformParameters (this);
			if (signUpPage != null)
				signUpPage.platformParams = new PlatformParameters (this);
		}	
	}
}

