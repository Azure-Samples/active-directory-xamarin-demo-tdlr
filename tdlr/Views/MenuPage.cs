using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace tdlr
{
	public class MenuPage : MasterDetailPage
	{
		public MenuPage ()
		{
			Label header = new Label
			{
				Text = "MasterDetailPage",
				FontSize = 30,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};

			ObservableCollection<Task> tasks = new ObservableCollection<Task> {
				new Task {
					TaskID = 1,
					TaskText = "fdsafdasfdkfjadfasfasdfadsfdsafdanmvcnxznv,cdfasfzvn,zxcv",
					Status = "NotStarted"
				},
				new Task {
					TaskID = 2,
					TaskText = "Task 1",
					Status = "InProgress",
				},
				new Task {
					TaskID = 3,
					TaskText = "Task 1",
					Status = "Complete"
				},
				new Task {
					TaskID = 4,
					TaskText = "Task 1",
					Status = "Blocked"
				},
				new Task {
					TaskID = 5,
					TaskText = "Task 1"
				},
			};

			// Create ListView for the master page.
			ListView listView = new ListView
			{
				ItemsSource = tasks
			};

			// Create the master page with the ListView.
			this.Master = new ContentPage
			{
				Title = "Color List",       // Title required!
				Content = new StackLayout
				{
					Children = 
					{
						header, 
						listView
					}
					}
			};

			// Create the detail page using NamedColorPage
			TaskListPage detailPage = new TaskListPage();
			this.Detail = detailPage;
		}
	}
}


