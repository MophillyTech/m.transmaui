using System;
using m.transport.ViewModels;

namespace m.transport
{
	public class PopupAlertTextViewModel : BaseViewModel
	{
		public string Title { get; set; }
		public string Msg { get; set; }

		private bool isInvalidInput = false;

		public PopupAlertTextViewModel(string title, string msg, string value= "")
		{
			Title = title;
			Msg = msg;
		}

		public bool IsInvalidInput
		{
			get { return isInvalidInput; }
			set
			{
				if (isInvalidInput != value)
				{
					isInvalidInput = value;
					RaisePropertyChanged();
				}
			}
		}
	}
}

