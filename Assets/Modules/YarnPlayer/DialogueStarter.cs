#region
using Events;
using Modules.YarnPlayer;
using UnityEngine;
#endregion
public class DialogueStarter : MonoBehaviour
{
    [SerializeField]
    private string NodeName;

    // Start is called before the first frame update
    private void Start()
    {
        this.SendEvent(new StartDialogueMessage {
            NodeName = NodeName
        });
    }
}
