#region
using Events;
using UnityEngine;
using UnityEngine.UI;
#endregion
namespace Modules.Notifications
{
    public class HintWidget : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField]
        private GameObject container;
        private void Awake()
        {
            this.Subscribe<HintMessage>(OnHintMessage);
            container.SetActive(false);
        }

        private void OnHintMessage(HintMessage obj)
        {
            container.SetActive(obj.active);
            if (false == string.IsNullOrEmpty(obj.text)) text.text = obj.text;
        }
    }
}
