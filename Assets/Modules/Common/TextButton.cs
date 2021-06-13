using UnityEngine;
using UnityEngine.UI;
namespace Modules.Common
{
    public class TextButton : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        public Button button;
        public string Text
        {
            set => text.text = value;
        }
    }
}
