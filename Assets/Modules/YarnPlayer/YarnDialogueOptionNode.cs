#region
using System.Collections.Generic;
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
        private Color activeColor;
        [SerializeField]
        private Color inactiveColor;

        [SerializeField]
        private Button _button;
        [SerializeField]
        private Transform requirementContainer;
        [SerializeField]
        private RequirementOptionView reqPrefab;
        private List<RequirementOptionView> _options = new List<RequirementOptionView>();
        private OptionLine _line;

        private OptionLine _option;
        public OptionLine optionLine
        {
            get => _option;
            set
            {
                SetLine(value);
            }
        }

        public void ClearRequirements()
        {
            foreach (var child in _options) Destroy(child.gameObject);
            _options.Clear();
            foreach (Transform child in requirementContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void SetLine(OptionLine line)
        {
            _line = line;
            _label.text = line.Text;
            _label.color = line.IsSatisfied ? activeColor : inactiveColor;
            _option = line;
            ClearRequirements();
            
            if (line.LineRequirements?.Length > 0)
            {
                requirementContainer.gameObject.SetActive(true);
                foreach (LineRequirement requirement in line.LineRequirements)
                {
                    var item = Instantiate(reqPrefab, requirementContainer);
                    item.Init(requirement);
                    _options.Add(item);
                }
            }
            else
            {
                requirementContainer.gameObject.SetActive(false);
            }
        }

        public int OptionId { get; set; }

        public void Awake()
        {
            _button.onClick.AddListener(OnPressed);
        }

        private void OnEnable()
        {
        }

        public void DisableButton()
        {
            _button.enabled = false;
        }

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
            if (_line.IsSatisfied)
                this.SendEvent(new OptionSelectedMessage {
                    ID = OptionId,
                    Text = _option.Text
                });
        }
    }
}
