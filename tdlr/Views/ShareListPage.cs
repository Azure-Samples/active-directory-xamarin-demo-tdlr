using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace tdlr
{
	public class ShareListPage : ContentPage
	{
		Image _done;
		Image _unselect;
		StackLayout _navbar;
		ListView _shareList;
		public ObservableCollection<AADObject> _shares;
		Task _task;
		Label _add;
		Image _delete;

		public ShareListPage (Task task)
		{
			_task = task;
			_shares = new ObservableCollection<AADObject> ();

			#region UI Init

			this.Title = "tdlr;";
			NavigationPage.SetHasNavigationBar (this, false);

			_add = new Label {
				Text = "+",
				TextColor = Color.White,
				FontSize = 30,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
				IsVisible = true,
				Opacity = 1
			};

			_done = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_done_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = true,
				Opacity = 1,
			};

			_unselect = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_done_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0,
			};

			Label vr = new Label {
				Text = "|",
				TextColor = Color.White,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
			};

			_delete = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile("ic_delete.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0
			};

			Label header = new Label {
				Text = "Currently Shared With:",
				TextColor = Color.Black,
				FontSize = 18,
				FontFamily = "Montserrat-UltraLight",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,	
			};

			_shareList = new ListView {
				ItemsSource = _shares,
			};
			var cell = new DataTemplate(typeof(TextCell));
			cell.SetValue (TextCell.TextColorProperty, Color.Black);
			_shareList.ItemTemplate = cell;
			_shareList.ItemTemplate.SetBinding(TextCell.TextProperty, "displayName");

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
					_delete,
					_add,
					vr,
					_done,
					_unselect
				}
			};

			#endregion

			#region Event Listeners

			_done.GestureRecognizers.Add (new TapGestureRecognizer (OnDoneClicked));
			_add.GestureRecognizers.Add (new TapGestureRecognizer (OnAddClicked));
			_unselect.GestureRecognizers.Add (new TapGestureRecognizer (OnUnselectClicked));
			_delete.GestureRecognizers.Add (new TapGestureRecognizer (OnDeleteClicked));
			_shareList.ItemSelected += OnShareSelected;
			menu.GestureRecognizers.Add (new TapGestureRecognizer (OnMenuClicked));
			_shareList.IsPullToRefreshEnabled = true;

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
					new ScrollView {
						Content = _shareList	
					}
				}
			};

			#endregion

		}

		// When a user is selected
		void OnShareSelected(object sender, SelectedItemChangedEventArgs e)
		{
			// If unselect occurred, show the right buttons
			if (e.SelectedItem == null) {
				_delete.FadeTo (0, 250);
				_unselect.FadeTo (0, 250);
				_delete.IsVisible = false;
				_unselect.IsVisible = false;
				_add.IsVisible = true;
				_done.IsVisible = true;
				_add.FadeTo (1, 250);
				_done.FadeTo (1, 250);
				return;
			}

			// If select occurred, show the right buttons
			_delete.FadeTo (0, 250);
			_unselect.FadeTo (0, 250);
			_add.FadeTo (0, 250);
			_done.FadeTo (0, 250);
			_add.IsVisible = false;
			_done.IsVisible = false;
			_delete.IsVisible = true;
			_unselect.IsVisible = true;
			_delete.FadeTo (1, 250);
			_unselect.FadeTo (1, 250);

		}

		// When a user is deselected
		void OnUnselectClicked(View image, object sender)
		{
			_shareList.SelectedItem = null;
		}

		// When the user is done sharing
		async void OnDoneClicked(View image, object sender)
		{
			try {
				await TaskHelper.ShareTask(_task.TaskID, _shares.ToList());
				Navigation.PopAsync();
			} catch (Exception ex) {
				this.DisplayAlert ("Error updating task", ex.Message, "OK");
			}
		}

		// Menu is used for logout
		async void OnMenuClicked(View image, object sender)
		{
			App.SignOut ();
			Navigation.PopAsync ();
		}

		// Add a new user to share with
		void OnAddClicked(View image, object sender)
		{
			Navigation.PushAsync (new SharePage ());
		}

		// Remove user from the list
		void OnDeleteClicked(View image, object sender)
		{
			_shares.Remove (((AADObject)_shareList.SelectedItem));
			_shareList.SelectedItem = null;
		}

		// Populate the list of current shares
		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			if (_shares.Count () == 0) {
				try {
					List<AADObject> shares = await TaskHelper.GetShares(_task.TaskID);
					foreach (AADObject share in shares) {
						_shares.Add(share);
					}
				} catch (Exception ex) {
					await this.DisplayAlert ("Error getting shares.", ex.Message, "Go Back");
					Navigation.PopAsync ();
				}
			}

		}
	}
}


