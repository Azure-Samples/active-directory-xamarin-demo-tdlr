using System;

using Xamarin.Forms;

namespace tdlr
{
	public class WelcomePage : ContentPage
	{
		public WelcomePage ()
		{
			NavigationPage.SetHasNavigationBar (this, false);

			Button signUpButton = new Button { 
				Text = "Sign Up",
				Font = Font.SystemFontOfSize (16),
				BorderWidth = 1,
				WidthRequest = 200,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb (240, 128, 128),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			Button signInButton = new Button { 
				Text = "Sign In",
				Font = Font.SystemFontOfSize (16),
				BorderWidth = 1,
				WidthRequest = 200,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb (240, 128, 128),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			signUpButton.Clicked += OnSignUpClicked;
			signInButton.Clicked += OnSignInClicked;


			Content = new StackLayout {
				BackgroundColor = Color.Black,
				Children = {
					new StackLayout {
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						Children = {
							new StackLayout {
								VerticalOptions = LayoutOptions.CenterAndExpand,
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								Spacing = 0,
								Children= {
									new Label { 
										Text = "tdlr;",
										FontFamily = "Pacifico",
										VerticalOptions = LayoutOptions.CenterAndExpand,
										HorizontalOptions = LayoutOptions.CenterAndExpand,
										TextColor = Color.FromRgb(240,128,128),
										FontSize = 48,
									},
									new Label { 
										Text = "To-Do List (Reimagined)",
										FontFamily = "Montserrat-UltraLight",
										HorizontalOptions = LayoutOptions.CenterAndExpand,
										TextColor = Color.White,
										FontSize = 24,
									},
									new Label { 
										Text = "The best* way to keep track of your team's work.",
										HorizontalOptions = LayoutOptions.CenterAndExpand,
										TextColor = Color.White,
										FontSize = 12,
									},
								}
							}
						}
					},
					new StackLayout { 
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						Spacing = 10,
						Padding = new Thickness(0,0,0,50),
						Children = {
							signUpButton,
							signInButton,
							new Label { 
								Text = "*Not actually the best",
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								TextColor = Color.White,
								FontSize = 12,
							},
						}
					},
				}
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}

		async void OnSignUpClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync (new SignUpPage ());
		}

		async void OnSignInClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync (new SignInPage ());
		}
	}
}


