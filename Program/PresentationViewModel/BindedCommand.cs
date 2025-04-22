using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TP.ConcurrentProgramming.PresentationViewModel
{
  class BindedCommand: ICommand
  {
    public delegate void ICommandOnExecute(object? parameter);
    public delegate bool ICommandOnCanExecute(object? parameter);

    private ICommandOnExecute _execute;
    private ICommandOnCanExecute _canExecute;

    public BindedCommand(ICommandOnExecute onExecute, ICommandOnCanExecute onCanExecute) {
      this._execute = onExecute;
      this._canExecute = onCanExecute;
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
      return _canExecute.Invoke(parameter);
    }

    public void Execute(object? parameter)
    {
      _execute.Invoke(parameter);
    }
  }
}
