using System;
using System.Collections;
using System.Collections.Generic;
using Modules.YarnPlayer;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;

[RequireComponent(typeof(YarnController))]
public abstract class BaseYarnFunction : MonoBehaviour
{
    [GetComponent]
    [SerializeField]
    private YarnController yarnController;

    protected abstract string Name { get; }
    protected abstract Delegate Implementation { get; }
    private void Start()
    {
        yarnController.AddFunction(Name, Implementation);
    }
}
