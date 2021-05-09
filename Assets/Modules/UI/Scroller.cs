#region
using UnityEngine;
using UnityEngine.UI;
#endregion
namespace Modules.UI
{
    public class Scroller : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar _scrollbar;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private bool updateSize;

        private void Start()
        {
            _scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnValidate()
        {
            OnScrollValueChanged(_scrollbar.value);
        }
        private void OnScrollValueChanged(float value)
        {
            _scrollRect.verticalNormalizedPosition = value;
            if (updateSize) _scrollbar.size = value;
        }
    }
}
