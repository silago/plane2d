using UnityEngine;
using UnityEngine.UI;

public class ResourceChangeInfo : MonoBehaviour
{
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private Text text;

    [SerializeField]
    private string addPattern = "You you've gain {0} {1}. Now you have {2}";
    [SerializeField]
    private string losePatter = "You you've lost {0} {1}. Now you have {2}";
    //private string unchagedPattern = $"{1} amount is unchaged. You gave {0}";
    
    public void Init(DialogueDataStorage.DialogueResourceChanged<int> data)
    {

        if (data.Current == data.Prev)
        {
            Destroy(gameObject);
            return;
        }
        string pattern; 
        pattern = data.Current > data.Prev ? addPattern : losePatter;
        text.text = string.Format(pattern, Mathf.Abs(data.Current - data.Prev), data.ResourceInfo.Name, data.Current);
        Icon.sprite = data.ResourceInfo.Icon;
    }
}