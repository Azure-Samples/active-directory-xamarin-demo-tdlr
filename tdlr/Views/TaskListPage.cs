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

			_entry.Completed += OnTaskEntered;
			_cancel.GestureRecognizers.Add (new TapGestureRecognizer (OnCancelClicked));
			_add.GestureRecognizers.Add (new TapGestureRecognizer (OnAddClicked));
			_delete.GestureRecognizers.Add (new TapGestureRecognizer (OnDeleteClicked));
			_edit.GestureRecognizers.Add (new TapGestureRecognizer (OnEditClicked));
			_share.GestureRecognizers.Add (new TapGestureRecognizer (OnShareClicked));
			_done.GestureRecognizers.Add (new TapGestureRecognizer (OnDoneClicked));

			_tasks = new ObservableCollection<Task> ();

			_taskList = new ListView {
				ItemsSource = _tasks,
			};

			_taskList.ItemTemplate = new DataTemplate (typeof(TaskCell));
			_taskList.ItemSelected += OnTaskSelected;

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
					_add,
					_cancel,
					_edit,
					_share,
					_delete,
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
					_entry,
					new ScrollView {
						Content = _taskList
					},
					new BoxView {
						Color = Color.Gray,
						HeightRequest = 10,
						BackgroundColor = Color.Black
					},
				}
			};
		}

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

		async void OnTaskEntered(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty (_entry.Text)) {
				try {
					await TaskHelper.CreateTask(_entry.Text);
					_entry.FadeTo(0, 250);
					_entry.IsEnabled = false;
					_entry.IsVisible = false;
					_entry.Text = null;
					_cancel.FadeTo (0, 250);
					_cancel.IsVisible = false;
					_add.IsVisible = true;
					_add.FadeTo (1, 250);
					OnAppearing();
				} catch (Exception ex) {
					// Error Creating Task
				}
			}
		}

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

		async void OnDeleteClicked(View image, object sender)
		{
			try {
				Task task = ((Task)_taskList.SelectedItem);
				await TaskHelper.DeleteTask(task);
				_tasks.Remove(task);
			} catch (Exception e) {
				// Error on delete
			}
		}

		async void OnDoneClicked(View image, object sender)
		{
			_taskList.SelectedItem = null;					
		}

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

		async void OnEditClicked(View image, object sender)
		{
			Task task = ((Task)_taskList.SelectedItem);
			string oldStatus = task.Status;
			var status = await DisplayActionSheet (task.TaskText, "Cancel", null, new string [] {"NotStarted", "InProgress", "Complete", "Blocked"}); 

			if (status.Equals ("Cancel"))
				return;

			task.Status = status;
			try {
				await TaskHelper.UpdateTask(task);
			} catch (Exception ex) {
				// Error updating
				task.Status = oldStatus;
				return;
			}
		}

		async void OnShareClicked(View image, object sender)
		{
			Navigation.PushAsync (new SharePage ((Task)_taskList.SelectedItem));
		}

		async void OnMenuClicked(View image, object sender)
		{
			App.AuthContext.TokenCache.Clear ();
			OnAppearing ();
		}

		protected override async void OnAppearing ()
		{
			_tasks.Clear ();
			base.OnAppearing ();
			try {
				AuthenticationResult authResult = await App.AuthContext.AcquireTokenSilentAsync (App.taskApiResourceId, App.clientId);
				_userObjectId = authResult.UserInfo.UniqueId;
				List<Task> tasks = await TaskHelper.GetUserTasks();
				foreach (Task task in tasks) {
					_tasks.Add (task);
				}
			} catch (Exception ex) {
				Navigation.InsertPageBefore (new WelcomePage (), this);
				Navigation.PopAsync ().ConfigureAwait (false);
			}
		}
	}
}


