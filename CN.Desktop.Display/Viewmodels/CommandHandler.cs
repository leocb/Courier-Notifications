using System;
using System.Windows.Input;

namespace CN.Desktop.Display.Viewmodels;

public class CommandHandler : ICommand
{
    private readonly Action _action;
    private readonly bool _canExecute;
    public event EventHandler? CanExecuteChanged;

    public CommandHandler(Action action, bool canExecute)
    {
        this._action = action;
        this._canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => this._canExecute;

    public void Execute(object? parameter) => this._action();
}
