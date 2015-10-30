using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;

namespace tdlr
{
	public class TaskListPage : ContentPage
	{
		public IPlatformParameters platformParams { get; set; }
		Image _delete;
		Image _edit;
		Image _share;
		Image _done;
		StackLayout _navbar;
		ListView _taskList;
		Label _vr;
		Label _add;
		ObservableCollection<Task> _tasks;
		Entry _entry;
		Image _cancel;
		string _userObjectId;

		public TaskListPage ()
		{
			_tasks = new ObservableCollection<Task> ();

			#region UI Init
			NavigationPage.SetHasNavigationBar (this, false);

			this.Title = "tdlr;";

			_cancel = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_close_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0,
			};
			_delete = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile("ic_delete.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0
			};
			_edit = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile("ic_mode_edit.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0
			};
			_share = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_share.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0
			};
			_done = new Image {
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile ("ic_done_white.png"),
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				IsVisible = false,
				Opacity = 0
			};
			_vr = new Label {
				Text = "|",
				TextColor = Color.White,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
				IsVisible = false,
				Opacity = 0
			};

			_add = new Label {
				Text = "+",
				TextColor = Color.White,
				FontSize = 30,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,	
				IsVisible = true,
				Opacity = 1
			};

			_entry = new Entry {
				Placeholder = "Enter your task...",
				BackgroundColor = Color.Transparent,
				TextColor = Color.Black,
				IsEnabled = false,
				Opacity = 0,
				IsVisible = false,
			};

			_taskList = new ListView {
				ItemsSource = _tasks,
				IsPullToRefreshEnabled = true,
			};
			_taskList.ItemTemplate = new DataTemplate (typeof(TaskCell));

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
					_add,
					_cancel,
					_edit,
					_share,
					_delete,
					_vr,
					_done,
				}
			};

			#endregion

			#region Event Listeners

			_entry.Completed += OnTaskEntered;
			_cancel.GestureRecognizers.Add (new TapGestureRecognizer (OnCancelClicked));
			_add.GestureRecognizers.Add (new TapGestureRecognizer (OnAddClicked));
			_delete.GestureRecognizers.Add (new TapGestureRecognizer (OnDeleteClicked));
			_edit.GestureRecognizers.Add (new TapGestureRecognizer (OnEditClicked));
			_share.GestureRecognizers.Add (new TapGestureRecognizer (OnShareClicked));
			_done.GestureRecognizers.Add (new TapGestureRecognizer (OnDoneClicked));
			_taskList.ItemSelected += OnTaskSelected;
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
					_entry,
					new ScrollView {
						Content = _taskList
					}
				}
			};

			#endregion
		}

		// When a task is selected or deselected, show the right actions
		void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				_delete.Opacity = 0;
				_edit.Opacity = 0;
				_share.Opacity = 0;
				_vr.Opacity = 0;
				_done.Opacity = 0;

				if (((Task)e.SelectedItem).Creator == _userObjectId) {
					_delete.IsVisible = true;
					_share.IsVisible = true;
					_delete.FadeTo (1, 250);
					_share.FadeTo (1, 250);
				}

				_edit.IsVisible = true;
				_done.IsVisible = true;
				_vr.IsVisible = true;
				_add.FadeTo (0, 250);
				_add.IsVisible = false;
				_edit.FadeTo (1, 250);
				_done.FadeTo (1, 250);
				_vr.FadeTo (1, 250);
			} else {
				_delete.FadeTo (0, 250);
				_edit.FadeTo (0, 250);
				_share.FadeTo (0, 250);
				_done.FadeTo (0, 250);
				_vr.FadeTo (0, 250);
				_add.IsVisible = true;
				_delete.IsVisible = false;
				_edit.IsVisible = false;
				_share.IsVisible = false;
				_done.IsVisible = false;
				_vr.IsVisible = false;
				_add.FadeTo (1, 250);
			} 
		}

		// When the user creates a task, try to create & add it to the list
		async void OnTaskEntered(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty (_entry.Text)) {
				try {

					// Create the task
					await TaskHelper.CreateTask(_entry.Text);

					// Update the UI
					_entry.FadeTo(0, 250);
					_entry.IsEnabled = false;
					_entry.IsVisible = false;
					_entry.Text = null;
					_cancel.FadeTo (0, 250);
					_cancel.IsVisible = false;
					_add.IsVisible = true;
					_add.FadeTo (1, 250);

					// Refresh the list data
					OnAppearing();

				} catch (Exception ex) {
					this.DisplayAlert ("Error creating task", ex.Message, "OK");
				}
			}
		}

		// When the user cancels adding a task
		async void OnCancelClicked(View image, object sender)
		{
			_entry.FadeTo(0, 250);
			_entry.IsEnabled = false;
			_entry.IsVisible = false;
			_entry.Text = null;
			_cancel.FadeTo (0, 250);
			_cancel.IsVisible = false;
			_add.IsVisible = true;
			_add.FadeTo (1, 250);
		}

		// When the user deletes a task
		async void OnDeleteClicked(View image, object sender)
		{
			try {
				// Delete the task
				Task task = ((Task)_taskList.SelectedItem);
				await TaskHelper.DeleteTask(task);

				// Remove it from the list, and deselect the task
				_tasks.Remove(task);
				_taskList.SelectedItem = null;

			} catch (Exception ex) {
				this.DisplayAlert ("Error deleting task", ex.Message, "OK");
			}
		}

		// Unselect the task
		async void OnDoneClicked(View image, object sender)
		{
			_taskList.SelectedItem = null;					
		}

		// Show the add task UI
		async void OnAddClicked(View image, object sender)
		{
			_entry.IsEnabled = true;
			_entry.IsVisible = true;
			_entry.FadeTo (1, 250);
			_entry.Focus ();
			_add.FadeTo (0, 250);
			_add.IsVisible = false;
			_cancel.IsVisible = true;
			_cancel.FadeTo (1, 250);
		}

		// Bring up the options for task status
		async void OnEditClicked(View image, object sender)
		{
			// Show status options
			Task task = ((Task)_taskList.SelectedItem);
			string oldStatus = task.Status;
			var status = await DisplayActionSheet (task.TaskText, "Cancel", null, new string [] {"NotStarted", "InProgress", "Complete", "Blocked"}); 

			if (status.Equals ("Cancel"))
				return;

			task.Status = status;
			try {

				// Update the status on the server
				await TaskHelper.UpdateTask(task);
			} catch (Exception ex) {
				task.Status = oldStatus;
				DisplayAlert ("Error updating task", ex.Message, "OK");
			}
		}

		// Launch the share list page
		async void OnShareClicked(View image, object sender)
		{
			Navigation.PushAsync (new ShareListPage ((Task)_taskList.SelectedItem));
		}

		// Logout
		async void OnMenuClicked(View image, object sender)
		{
			App.AuthContext.TokenCache.Clear ();
			App.SetADALAuthority ();
			OnAppearing ();
		}

		protected override async void OnAppearing ()
		{
			_tasks.Clear ();
			base.OnAppearing ();
			try {
				// Make sure the user is signed in and we can get token for calling APIs
				AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync (App.taskApiResourceId, App.clientId);
				_userObjectId = authResult.UserInfo.UniqueId;

			} catch (Exception ex) {
				// Send the user back to the sign in screen, they need to sign in again.	
				App.AuthContext.TokenCache.Clear ();
				Navigation.InsertPageBefore (new WelcomePage (), this);
				Navigation.PopAsync ().ConfigureAwait (false);
				return;
			}

			try {
				// Update the list of tasks
				List<Task> tasks = await TaskHelper.GetUserTasks();
				foreach (Task task in tasks) {
					_tasks.Add (task);
				}

			} catch (Exception ex) {
				await this.DisplayAlert ("Error getting tasks", ex.Message, "OK");
				Navigation.InsertPageBefore (new WelcomePage (), this);
				Navigation.PopAsync ().ConfigureAwait (false);
			}
		}
	}
}


