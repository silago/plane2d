#region
using System.Collections;
using Events;
using Modules.YarnPlayer;
using UnityEngine;
#endregion
public class DialogueStarter : MonoBehaviour
{
    [SerializeField]
    private string NodeName;
    
    [SerializeField]
    private string Caption;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Do());
    }

    public IEnumerator Do()
    {
        yield return new WaitForEndOfFrame();
        this.SendEvent(new StartDialogueMessage {
            NodeName = NodeName, Caption = Caption
        });
        
    }
}
