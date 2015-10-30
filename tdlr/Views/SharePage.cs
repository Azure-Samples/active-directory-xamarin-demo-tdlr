using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;
using System.Collections.Generic;

namespace tdlr
{
	public class SharePage : ContentPage
	{
		StackLayout _navbar;
		ListView _searchList;
		ObservableCollection<AADObject> _results;
		Entry _search;
		Image _cancel;

		public SharePage ()
		{
			_results = new ObservableCollection<AADObject> ();

			#region UI Init

			this.Title = "tdlr;";
			NavigationPage.SetHasNavigationBar (this, false);

			_cancel = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_close_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};

			Label header = new Label {
				Text = "Select a user to add:",
				TextColor = Color.Black,
				FontSize = 18,
				FontFamily = "Montserrat-UltraLight",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,	
			};

			_search = new Entry {
				Placeholder = "Search users...",
				BackgroundColor = Color.Transparent,
				TextColor = Color.Black,
			};
			_searchList = new ListView {
				ItemsSource = _results,
			};
			var cell = new DataTemplate(typeof(TextCell));
			cell.SetValue (TextCell.TextColorProperty, Color.Black);
			_searchList.ItemTemplate = cell;
			_searchList.ItemTemplate.SetBinding(TextCell.TextProperty, "displayName");

			Image menu = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_menu_white.png"),
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
			};

			_navbar = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HeightRequest = 50,
				Padding = new Thickness(10, 0, 20, 0),
				Spacing = 5,
				BackgroundColor = Color.Black,
				Children = {
					menu,
					new Label {
						Text = " tdlr;",
						HorizontalOptions = LayoutOptions.StartAndExpand,
						VerticalOptions = LayoutOptions.Center,
						FontFamily = "Pacifico",
						FontSize = 24,
						TextColor = Color.FromRgb(240,128,128)
					},
					_cancel,
				}
			};

			#endregion

			#region Event Handlers

			_search.TextChanged += OnSearchTermChange;
			_cancel.GestureRecognizers.Add (new TapGestureRecognizer (OnCancelClicked));
			_searchList.ItemSelected += OnResultSelected;
			menu.GestureRecognizers.Add (new TapGestureRecognizer (OnMenuClicked));

			#endregion

			#region Main Layout

			Content = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = new Thickness(0,0,0,0),
				Children = {
					_navbar, 
					new BoxView {
						Color = Color.Gray,
						HeightRequest = 1,
						BackgroundColor = Color.Black
					},
					new StackLayout {
						Padding = new Thickness(0,10,0,0),
						Children = {
							header,
						}						
					},
					_search,
					new ScrollView {
						Content = _searchList	
					},
				}
			};

			#endregion
		}

		// Search for users
		async void OnSearchTermChange (object sender, TextChangedEventArgs e)
		{
			_results.Clear ();
			try {
				List<AADObject> results = await GraphHelper.SearchUsers (e.NewTextValue);
				foreach (AADObject result in results) {
					_results.Add(result);
				};
			} catch (Exception ex) {
				this.DisplayAlert ("Error searching for users.", ex.Message, "OK");
			}
		}

		// Add user to list on previous page
		void OnResultSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
				return;

			// Add selection to share list on previous page if it's not already added
			AADObject user = (AADObject)e.SelectedItem;
			ShareListPage previousPage = (ShareListPage)Navigation.NavigationStack.ElementAt (Navigation.NavigationStack.Count () - 2);
			bool exists = false;
			foreach (AADObject share in previousPage._shares) {
				if (share.objectId == user.objectId) {
					exists = true;
				}
			}
			if (!exists) {
				previousPage._shares.Add (user);				
			}

			Navigation.PopAsync ();
		}

		// Return to last page w/o changes
		void OnCancelClicked(View image, object sender)
		{
			Navigation.PopAsync ();
		}

		// Logout
		void OnMenuClicked(View image, object sender)
		{
			App.AuthContext.TokenCache.Clear ();
			App.SetADALAuthority ();
			Navigation.PopAsync ();
		}

		// Clear results and focus in search window
		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			_searchList.SelectedItem = null;
			_results.Clear ();
			_search.Focus ();
		}
	}
}


