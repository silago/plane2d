using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Modules.YarnPlayer;


public class DialogueStarter : MonoBehaviour
{
    [SerializeField]
    string NodeName;
    
    // Start is called before the first frame update
    void Start()
    {
        this.SendEvent(new StartDialogueMessage { NodeName = this.NodeName });  
    }
}
