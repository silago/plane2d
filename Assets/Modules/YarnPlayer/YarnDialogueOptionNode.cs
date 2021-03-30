#region
using Events;
using UnityEngine;
using UnityEngine.UI;
#endregion
namespace Modules.YarnPlayer
{
    public class YarnDialogueOptionNode : MonoBehaviour
    {
        [SerializeField]
        private Text _label;

        [SerializeField]
        private Button _button;

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

        public int OptionId { get; set; }

        public void Awake()
        {
            _button.onClick.AddListener(OnPressed);
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void DisableButton()
        {
            _button.enabled = false;
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

        private void OnPressed()
        {
            this.SendEvent(new OptionSelectedMessage {
                ID = OptionId,
                Text = _option.Text
            });
        }
    }
}
