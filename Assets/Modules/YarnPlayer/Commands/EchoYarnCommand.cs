using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoYarnCommand : BaseYarnCommand
{
    protected override string Name
    {
        get => "Echo";
    }
    protected override Delegate Implementation => (Action<string>)Method;
    void Method(string input)
    {
        Debug.Log($"command {Name} {input}");
    }
}
