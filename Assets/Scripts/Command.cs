using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CommandHandler {
    private Stack<Command> commands = new Stack<Command>();

    public bool Undo() {
        if(commands.Count > 0) {
            commands.Pop().Undo();
            return true;
        }
        return false;
    }

    public void AddCommand(Command command) {
        commands.Push(command);
        command.Execute();
    }
}


public class Command : ICommand
{
    public Action executeAction;
    public Action undoAction;
    public Command() {

    }
    public Command(Action executeAction,Action undoAction) {
        this.executeAction = executeAction;
        this.undoAction = undoAction;
    }
    public void Execute() {
        executeAction?.Invoke();
    }
    public void Undo() {
        undoAction?.Invoke();
    }

    

}
