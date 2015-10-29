using System;
using System.ComponentModel;

namespace tdlr
{
	public class Task : INotifyPropertyChanged
	{
		private string _status;
		public event PropertyChangedEventHandler PropertyChanged;
		public int TaskID { get; set;}
		public string TaskText { get; set;}
		public string Status { 
			get {
				return _status;
			} set {
				_status = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this,
						new PropertyChangedEventArgs("StatusImage"));
				}
			}}
		public string Creator { get; set;}
		public string CreatorName { get; set;}
		public string StatusImage {
			get { 
				return this.MapStatusToIcon (this.Status);
			}
		}

		private string MapStatusToIcon(string status)
		{
			switch (status)
			{
			case "NotStarted":
				return "whiteLightCircle.png";
			case "InProgress":
				return "yellowLightCircle.png";
			case "Complete":
				return "greenLightIcon.png";
			case "Blocked":
				return "redLightCircle.png";
			default:
				return "";
			}
		}
	}
}

