using System;
using System.Windows.Input;

namespace CN.Desktop.Display.Helpers;

public class CommandHandler(Action action, bool canExecute) : ICommand
{
    private readonly Action _action = action;
    private readonly bool _canExecute = canExecute;
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => this._canExecute;

    public void Execute(object? parameter) => this._action();
}
