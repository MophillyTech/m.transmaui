using System;
using System.Windows.Input;

namespace m.transport.ViewModels
{
	public class DelegateCommand<T> : ICommand
	{
		readonly Func<T, bool> canExecute;
		readonly Action<T> executeAction;

		bool canExecuteCache;

		public DelegateCommand (Action<T> executeAction, Func<T, bool> canExecute)
		{
			this.executeAction = executeAction;
			this.canExecute = canExecute;
		}

		public DelegateCommand (Action<T> executeAction)
		{
			this.executeAction = executeAction;
		}

		#region ICommand Members

		public bool CanExecute (object parameter)
		{
			if (canExecute != null) {
				bool temp = canExecute ((T)parameter);
				if (canExecuteCache != temp) {
					canExecuteCache = temp;
					if (CanExecuteChanged != null) {
						CanExecuteChanged (this, new EventArgs ());
					}
				}
				return canExecuteCache;
			}
			return true;
		}

		public void Execute (object parameter)
		{
			executeAction ((T)parameter);
		}

		public event EventHandler CanExecuteChanged;

		#endregion

	}

	//Non generic
	public class DelegateCommand : DelegateCommand<object>
	{
		public DelegateCommand (Action<object> executeAction, Func<object,bool> canExecute)
			: base (executeAction, canExecute)
		{
			
		}

		public DelegateCommand (Action<object> executeAction)
			: base (executeAction)
		{
			
		}
	}
}


