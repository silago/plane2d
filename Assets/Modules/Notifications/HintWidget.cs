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
        private void Awake()
        {
            this.Subscribe<HintMessage>(OnHintMessage);
            gameObject.SetActive(false);
        }

        private void OnHintMessage(HintMessage obj)
        {
            gameObject.SetActive(obj.active);
            if (false == string.IsNullOrEmpty(obj.text)) text.text = obj.text;
        }
    }
}
