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
		Image _done;
		StackLayout _navbar;
		ListView _shareList;
		ListView _searchList;
		Label _vr;
		ObservableCollection<Share> _shares;
		ObservableCollection<AADObject> _results;
		Entry _search;
		Image _cancel;
		Task _task;

		public SharePage (Task task)
		{
			NavigationPage.SetHasNavigationBar (this, false);

			this.Title = "tdlr;";

			_task = task;

			_cancel = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_close_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};
			_done = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_done_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};
			_vr = new Label {
				Text = "|",
				TextColor = Color.White,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
			};

			Label add = new Label {
				Text = "(+)",
				TextColor = Color.Black,
				FontSize = 30,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
			};

			Label addHeader = new Label {
				Text = "Add Users",
				TextColor = Color.Black,
				FontSize = 30,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
			};

			Label removeHeader = new Label {
				Text = "Currently Shared With:",
				TextColor = Color.Black,
				FontSize = 18,
				FontFamily = "Montserrat-UltraLight",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,	
			};

			_search = new Entry {
				Placeholder = "Search to add users...",
				BackgroundColor = Color.Transparent,
				TextColor = Color.Black,
			};

			_search.TextChanged += OnSearchTermChange;

			_cancel.GestureRecognizers.Add (new TapGestureRecognizer (OnCancelClicked));
			_done.GestureRecognizers.Add (new TapGestureRecognizer (OnDoneClicked));

			_shares = new ObservableCollection<Share> ();
			_results = new ObservableCollection<AADObject> ();

			_shareList = new ListView {
				ItemsSource = _shares,
			};
			_searchList = new ListView {
				ItemsSource = _results,
			};

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetValue (TextCell.TextColorProperty, Color.Black);
			_shareList.ItemTemplate = cell;
			_shareList.ItemTemplate.SetBinding(TextCell.TextProperty, "displayName");
			_searchList.ItemTemplate = cell;
			_searchList.ItemTemplate.SetBinding(TextCell.TextProperty, "displayName");

			_shareList.ItemSelected += OnShareSelected;
			_searchList.ItemSelected += OnResultSelected;

			Image menu = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_menu_white.png"),
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
			};

			menu.GestureRecognizers.Add (new TapGestureRecognizer (OnMenuClicked));

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
					_vr,
					_done,
				}
			};

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
							removeHeader,
						}						
					},
					new ScrollView {
						Content = _shareList	
					},
					new BoxView {
						Color = Color.Gray,
						HeightRequest = 1,
						BackgroundColor = Color.Black
					},
					_search,
					new ScrollView {
						Content = _searchList
					},
				}
			};
		}

		async void OnSearchTermChange (object sender, TextChangedEventArgs e)
		{
			_results.Clear ();
			try {
				List<AADObject> results = await GraphHelper.SearchUsers (e.NewTextValue);
				foreach (AADObject result in results) {
					_results.Add(result);
				};
			} catch (Exception ex) {
				// Error calling Graph
			}
		}

		void OnShareSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
				return;

			_shares.Remove ((Share)e.SelectedItem);
			((ListView)sender).SelectedItem = null;
		}

		void OnResultSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
				return;
			
			AADObject user = (AADObject)e.SelectedItem;
			if (_shares.Where (s => s.objectID == user.objectId).Count() == 0) {
				_shares.Add (new Share {
					objectID = user.objectId,
					displayName = user.displayName
				});
			}
			((ListView)sender).SelectedItem = null;
		}

		async void OnCancelClicked(View image, object sender)
		{
			Navigation.PopAsync ();
		}
			

		async void OnDoneClicked(View image, object sender)
		{
			try {
				TaskHelper.ShareTask(_task.TaskID, _shares.ToList());
				Navigation.PopAsync();
			} catch (Exception ex) {
				// Error Sharing Tasks
			}
		}

		async void OnMenuClicked(View image, object sender)
		{
			App.AuthContext.TokenCache.Clear ();
			Navigation.PopAsync ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			_shares.Clear ();
			_results.Clear ();
			_search.Focus ();
			try {
				List<Share> shares = await TaskHelper.GetShares(_task.TaskID);
				foreach (Share share in shares) {
					_shares.Add(share);
				}
			} catch (Exception ex) {
				Navigation.PopAsync ();
			}
		}
	}
}


