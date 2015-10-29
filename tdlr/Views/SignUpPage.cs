using System;

using Xamarin.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Text.RegularExpressions;

namespace tdlr
{
	public class SignUpPage : ContentPage
	{
		public IPlatformParameters platformParams { get; set; }
		Button _AADSignInButton;
		Entry _emailField;
		Entry _passwordField;
		Entry _passwordConfirmField;
		Entry _firstNameField;
		Entry _lastNameField;
		Button _signUpButton;


		public SignUpPage ()
		{
			NavigationPage.SetHasNavigationBar (this, false);

			_signUpButton = new Button { 
				Text = "Sign Up",
				Font = Font.SystemFontOfSize (16),
				BorderWidth = 1,
				WidthRequest = 300,
				HeightRequest = 60,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb (240, 128, 128),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Opacity = .5,
				IsEnabled = false
			};

			_AADSignInButton = new Button { 
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
				Opacity = 0,
			};

			Button signInButton = new Button { 
				Text = "Already have an account? Sign in instead.",
				Font = Font.SystemFontOfSize (12),
				BorderWidth = 1,
				WidthRequest = 300,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			_emailField = new Entry {
				Placeholder = "Email Address",
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HeightRequest = 40,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			_emailField.Unfocused += OnEmailEntered;

			_passwordField = new Entry {
				Placeholder = "Password",
				HeightRequest = 40,
				IsPassword = true,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			_passwordConfirmField = new Entry {
				Placeholder = "Confirm Password",
				HeightRequest = 40,
				IsPassword = true,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			_firstNameField = new Entry {
				Placeholder = "First Name",
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HeightRequest = 40,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			_lastNameField = new Entry {
				Placeholder = "Last Name",
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HeightRequest = 40,
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			signInButton.Clicked += OnSignInClicked;
			_AADSignInButton.Clicked += OnAADSignInClicked;
			_signUpButton.Clicked += OnSignUpClicked;
			_emailField.TextChanged += OnFormEdit;
			_passwordField.TextChanged += OnFormEdit;
			_passwordConfirmField.TextChanged += OnFormEdit;
			_firstNameField.TextChanged += OnFormEdit;
			_lastNameField.TextChanged += OnFormEdit;


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
								Padding = new Thickness (0,0,0,30),
								Spacing = 10,
								Children = {
									_emailField,
									_passwordField,
									_passwordConfirmField,
									_firstNameField,
									_lastNameField,
								}
							},
							new StackLayout {
								VerticalOptions = LayoutOptions.CenterAndExpand,
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								Spacing = 10,
								Children = {
									_AADSignInButton,
									_signUpButton,
									new Label { 
										Text = "*Not actually the best",
										HorizontalOptions = LayoutOptions.CenterAndExpand,
										TextColor = Color.White,
										FontSize = 12,
									},
									signInButton
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
			if (isValidForm())
				Navigation.PushAsync (new TaskListPage ());
		}

		async void OnEmailEntered(object sender, FocusEventArgs e)
		{
			string email = ((Entry)sender).Text;
			Regex regex = new Regex (@"^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$");
			Match match = regex.Match (email);
			if (match.Success) {
				string domain = email.Substring(email.IndexOf('@') + 1).ToLower();
				if (GraphHelper.isAADDomain (domain)) {
					_AADSignInButton.FadeTo (1, 400);
					return;
				}
			}
			_AADSignInButton.FadeTo (0, 400);
		}

		async void OnAADSignInClicked(object sender, EventArgs e)
		{
			if (((Button)sender).Opacity >= 1) {
				try {
					AuthenticationResult authResult = await App.AuthContext.AcquireTokenAsync (App.taskApiResourceId, App.clientId, App.redirectUri, platformParams, new UserIdentifier(_emailField.Text, UserIdentifierType.OptionalDisplayableId), null);
					_emailField.Text = authResult.UserInfo.DisplayableId;
					_passwordField.IsEnabled = false;
					_passwordField.BackgroundColor = Color.Gray;
					_passwordConfirmField.IsEnabled = false;
					_passwordConfirmField.BackgroundColor = Color.Gray;
					_firstNameField.Text = authResult.UserInfo.GivenName;
					_firstNameField.Text = authResult.UserInfo.FamilyName;
				} catch (Exception ex) {
					// Sign In Failed, Stay on Sign In Screen
				}
			}
		}

		async void OnFormEdit(object sender, TextChangedEventArgs e)
		{
			if (isValidForm ()) {
				_signUpButton.IsEnabled = true;
				_signUpButton.FadeTo (1, 250);
			} else {
				_signUpButton.FadeTo (.5, 250);
				_signUpButton.IsEnabled = false;
			}
		}

		private bool isValidForm () {
			return !String.IsNullOrEmpty (_emailField.Text) &&
			(!String.IsNullOrEmpty (_passwordField.Text) || !_passwordField.IsEnabled) &&
			(!String.IsNullOrEmpty (_passwordConfirmField.Text) || !_passwordConfirmField.IsEnabled) &&
			(!String.IsNullOrEmpty (_firstNameField.Text) || !_passwordField.IsEnabled) &&
			(!String.IsNullOrEmpty (_lastNameField.Text) || !_passwordField.IsEnabled);
		}
	}
}


