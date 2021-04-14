using System;
using UnityEngine;
using UnityEngine.UI;
namespace Modules.UI
{
    public class Scroller : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar _scrollbar;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private bool updateSize = false;

        private void Start()
        {
            _scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
        }
        private void OnScrollValueChanged(float value)
        {
            _scrollRect.verticalNormalizedPosition = value;
            if (updateSize) _scrollbar.size = value;
        }

        private void OnValidate()
        {
            OnScrollValueChanged(_scrollbar.value);
        }
    }
}
