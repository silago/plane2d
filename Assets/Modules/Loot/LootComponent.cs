using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Events;
using Modules.Iteractions;
using UnityEngine;

public class LootComonent : MonoBehaviour
{
    [SerializeField]
    private string lootDialogueName;
    [SerializeField]
    private string caption;
    [SerializeField]
    private EnterDialogueTrigger prefab;
    private void Awake()
    {
        this.Subscribe<DestroyMessage, int>(DontDestroyMessage, this.GetInstanceID());
    }
    private void DontDestroyMessage(DestroyMessage obj)
    {
        var item = Instantiate(prefab, transform.root);
        item.transform.position = this.transform.position;
        prefab.DialogueId = lootDialogueName;
        prefab.LocationName = caption;
    }
}

