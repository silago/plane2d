#region
using Events;
using Modules.YarnPlayer;
using UnityEngine;
#endregion

// maybe later we want to give the unique id
internal class HintMessage : BoolMessage
{
    public string text { get; set; }
}

internal class LocationEnterMsg : Message
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

        [SerializeField] private string text;
        private void OnTriggerEnter2D(Collider2D other)
        {
            this.SendEvent(new HintMessage {
                active = true,
                text = text
            });
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            this.SendEvent(new HintMessage {
                active = false
            });
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (Input.GetKeyDown(KeyCode.E))
                //this.SendEvent(new LocationEnterMsg {dialogueId = DialogueId});
                this.SendEvent(new StartDialogueMessage {
                    NodeName = DialogueId
                });
        }
    }
}
