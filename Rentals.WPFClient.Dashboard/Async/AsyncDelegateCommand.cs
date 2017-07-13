using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

public sealed class AsyncDelegateCommand : ICommand
{
    private readonly Action _command;

    public AsyncDelegateCommand(Action command)
    {
        _command = command;
    }

    public void Execute(object parameter)
    {
        _command();
    }

    bool ICommand.CanExecute(object parameter)
    {
        return true;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add { }
        remove { }
    }
}
