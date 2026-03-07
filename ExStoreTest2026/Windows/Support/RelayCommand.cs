
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


// user name: jeffs
// created:   2/22/2026 6:20:33 PM

namespace ExStoreTest2026.Windows
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Func<object, bool> _canExecute;

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

		public void Execute(object parameter) => _execute(parameter);

		public event EventHandler? CanExecuteChanged;

		public virtual void RaiseCanExecuteChange(object? parameter)
		{
			CanExecuteChanged?.Invoke(this, new RelayCommandEvtArgs(parameter));
		}
	}

	public class RelayCommandAlwaysExecute : ICommand
	{
		private readonly Action<object> _execute;


		public RelayCommandAlwaysExecute(Action<object> execute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter) => _execute(parameter);

		public event EventHandler? CanExecuteChanged;

		public virtual void RaiseCanExecuteChange(object? parameter)
		{
			CanExecuteChanged?.Invoke(this, new RelayCommandEvtArgs(parameter));
		}
	}

	public class RelayCommandEvtArgs : EventArgs
	{
		public object? Parameter { get; }
		public RelayCommandEvtArgs(object? parameter)
		{
			Parameter = parameter;
		}
	}
}
