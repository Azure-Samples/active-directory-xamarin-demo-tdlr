using System;

using Xamarin.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace tdlr
{
	public class SignInPage : ContentPage
	{
		public IPlatformParameters platformParams { get; set; }

		public SignInPage ()
		{
			NavigationPage.SetHasNavigationBar (this, false);

			Button signInButton = new Button { 
				Text = "Sign In",
				Font = Font.SystemFontOfSize (16),
				BorderWidth = 1,
				WidthRequest = 300,
				HeightRequest = 60,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb (240, 128, 128),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			Button AADSignInButton = new Button { 
				Text = "Work or School Account",
				Font = Font.SystemFontOfSize (16),
				BorderWidth = 1,
				WidthRequest = 300,
				HeightRequest = 60,
				Image = "msft.png",	
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb (250, 250, 250),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			Button signUpButton = new Button { 
				Text = "Don't have an account? Sign up instead.",
				Font = Font.SystemFontOfSize (12),
				BorderWidth = 1,
				WidthRequest = 300,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			Entry emailField = new Entry {
				Placeholder = "Email Address",
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HeightRequest = 40,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			Entry passwordField = new Entry {
				Placeholder = "Password",
				HeightRequest = 40,
				IsPassword = true,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			signInButton.Clicked += OnSignInClicked;
			AADSignInButton.Clicked += OnAADSignInClicked;
			signUpButton.Clicked += OnSignUpClicked;


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
							new StackLayout {
								VerticalOptions = LayoutOptions.CenterAndExpand,
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								Padding = new Thickness (0,0,0,100),
								Spacing = 10,
								Children = {
									emailField,
									passwordField,
								}
							},
							new StackLayout {
								VerticalOptions = LayoutOptions.CenterAndExpand,
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								Spacing = 10,
								Children = {
									AADSignInButton,
									signInButton,
									new Label { 
										Text = "*Not actually the best",
										HorizontalOptions = LayoutOptions.CenterAndExpand,
										TextColor = Color.White,
										FontSize = 12,
									},
									signUpButton
								}
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

		async void OnSignInClicked(object sender, EventArgs e)
		{
			Navigation.PopAsync();
		}

		async void OnSignUpClicked(object sender, EventArgs e)
		{
			Navigation.PopAsync ();
		}

		async void OnAADSignInClicked(object sender, EventArgs e)
		{
			try {
				AuthenticationResult authResult = await App.AuthContext.AcquireTokenAsync (App.taskApiResourceId, App.clientId, App.redirectUri, platformParams);
				Navigation.PushAsync(new TaskListPage());
			} catch (Exception ex) {
				// Sign In Failed, Stay on Sign In Screen
			}

		}
	}
}


