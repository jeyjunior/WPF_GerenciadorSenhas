using System;
using System.Windows.Input;

public class CommandHandler : ICommand
{
    private Action _execute;
    private bool _canExecute;

    public CommandHandler(Action execute, bool canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute;
    public void Execute(object parameter) => _execute();
    public event EventHandler CanExecuteChanged;
}
