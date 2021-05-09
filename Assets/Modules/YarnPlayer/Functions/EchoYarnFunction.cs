using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoYarnFunction : BaseYarnFunction
{
    protected override string Name
    {
        get => "Echo";
    }
    protected override Delegate Implementation => (Func<string, string>)Method;

    string Method(string input)
    {
        Debug.Log("function" + input);
        return input;
    }
}
