using System;
using System.Windows.Input;

namespace CN.Desktop.Display.Helpers;

public class CommandHandler : ICommand
{
    public CommandHandler(Action<object?> action, bool canExecute)
    {
        this._action = action;
        this._canExecute = canExecute;
    }

    public CommandHandler(Action action, bool canExecute) {
        this._action = (a) => action.Invoke();
        this._canExecute = canExecute;
    }

    private readonly Action<object?> _action;
    private readonly bool _canExecute;
    public event EventHandler? CanExecuteChanged = delegate { };
    
    public bool CanExecute(object? parameter) => this._canExecute;
    public void Execute(object? parameter) => this._action(parameter);
}