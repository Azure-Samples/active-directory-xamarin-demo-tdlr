using System;
using Xamarin.Forms;

namespace tdlr
{
	public class TaskCell : ViewCell
	{
		public TaskCell ()
		{
			Label task = new Label {
				LineBreakMode = LineBreakMode.TailTruncation,
				HorizontalOptions = LayoutOptions.Start,
			};
			Label statusText = new Label {
				LineBreakMode = LineBreakMode.TailTruncation,
				HorizontalOptions = LayoutOptions.End,
			};
			Image status = new Image {
				Source = ImageSource.FromFile("ic_more_horiz.png"),
				VerticalOptions = LayoutOptions.Center,
			};
			task.SetBinding (Label.TextProperty, "TaskText");
			statusText.SetBinding (Label.TextProperty, "Status");
			status.SetBinding(Image.SourceProperty, "StatusImage");
			status.HorizontalOptions = LayoutOptions.EndAndExpand;

			StackLayout horizontalLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness (10, 10, 10, 10),
				HorizontalOptions = LayoutOptions.StartAndExpand,
				Children = {
					task,
					new StackLayout {
						Padding = new Thickness (10, 5, 10, 0),
						HorizontalOptions = LayoutOptions.EndAndExpand,
						Children = {
							status
						}
					}
				}
			};

			StackLayout cellWrapper = new StackLayout {
				HorizontalOptions = LayoutOptions.StartAndExpand,
			};
			cellWrapper.Children.Add (horizontalLayout);
//			View.HorizontalOptions = LayoutOptions.StartAndExpand;
			View = cellWrapper;
		}


	}
}

