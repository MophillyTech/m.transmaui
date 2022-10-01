using System;
using m.transport.ViewModels;

namespace m.transport
{
	public class CustomMenuItem : BaseViewModel
	{

		public string Title { get; set; }
		private bool _enabled;

		public CustomMenuItem (string title)
		{
			Title = title;
			Enabled = true;
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				RaisePropertyChanged();
			}
		}
	}
}

