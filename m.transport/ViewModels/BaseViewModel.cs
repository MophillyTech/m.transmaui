using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace m.transport.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
	
		public BaseViewModel ()
		{
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected virtual void RaisePropertyChanged ([CallerMemberName] string propertyName = "")
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}

