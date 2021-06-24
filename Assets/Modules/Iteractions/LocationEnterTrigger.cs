#region
using System;
using Events;
using Modules.YarnPlayer;
using UnityEngine;
#endregion

// maybe later we want to give the unique id
internal class HintMessage : BoolMessage
{
    public string text { get; set; }
}

internal class LocationEnterMsg : IMessage
{
    public string dialogueId { get; set; }
}

namespace Modules.Iteractions
{
    [RequireComponent(typeof(Collision2D))]
    public class LocationEnterTrigger : MonoBehaviour
    {

        [SerializeField]
        private string DialogueId;
        [SerializeField]
        private string LocationName;

        [SerializeField] private string text;
        private bool available = false;
        private void OnTriggerEnter(Collider other)
        {
            available = true;
            this.SendEvent(new HintMessage {
                active = true,
                text = text
            });
        }

        private void OnTriggerExit(Collider other)
        {
            available = false;
            this.SendEvent(new HintMessage {
                active = false
            });
        }
        private void Update()
        {
            if (available && Input.GetKeyUp(KeyCode.E))
            {
                available = false;
                this.SendEvent(new HintMessage {
                    active = false
                });
                this.SendEvent(new StartDialogueMessage {
                    NodeName = DialogueId,
                    Caption = LocationName 
                });
                
            }
        }

        /*
        private void OnTriggerStay(Collider other)
        {
            if (Input.GetKeyUp(KeyCode.E))
                this.SendEvent(new StartDialogueMessage {
                    NodeName = DialogueId,
                    Caption = LocationName 
                });
        }
        */
    }
}
