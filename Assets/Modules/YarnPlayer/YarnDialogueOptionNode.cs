using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.YarnPlayer {
public class YarnDialogueOptionNode : MonoBehaviour {
    [SerializeField]
	Text _label;

    [SerializeField]
    Button _button;

    private OptionLine _option;
    public OptionLine optionLine
    {
	    get => _option;
	    set
	    {
		    _label.text = value.Text;
		    _option = value;
	    }
	    //return _label.text = value;
    }
    
    public int OptionId {get; set;}

    public void DisableButton()
    {
	    _button.enabled = false;
    }

    public void Awake() {
        _button.onClick.AddListener(call: OnPressed); 
    }

    void OnEnable() {
    }

	//void OnEnable() {
	//	//this.SubscribeOnce<OptionSelectedMessage>( _ => QueueFree() );
	//}

    public void Show() 
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

	void OnDisable() {
	}
	
	void OnPressed()
	{
		this.SendEvent( new OptionSelectedMessage { ID = OptionId, Text = _option.Text } );
	}

}
}
