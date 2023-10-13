using System;
using UnityEngine;

public class DebugCommandBase
{
    public string cmdId { get; private set; }
    public string cmdDescription { get; private set; }
    public string cmdFormat { get; private set; }

    public DebugCommandBase(string id, string description, string format)
    {
        cmdId = id;
        cmdDescription = description;
        cmdFormat = format;
    }

}

public class DebugCommand : DebugCommandBase
{
    private Action cmd;
    
    public DebugCommand(string id, string description, string format, Action cmd) : base(id, description, format)
    {
        this.cmd = cmd;
    }

    public void Invoke()
    {
        cmd?.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> cmd;
    
    public DebugCommand(string id, string description, string format, Action<T> cmd) : base(id, description, format)
    {
        this.cmd = cmd;
    }

    public void Invoke(T value)
    {
        cmd?.Invoke(value);
    }
}