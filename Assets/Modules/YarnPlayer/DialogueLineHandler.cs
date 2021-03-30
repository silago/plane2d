#region
using UnityEngine;
using UnityEngine.UI;
using Yarn;
#endregion
namespace Modules.YarnPlayer
{
    public class DialogueLineHandler : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Image image;


        public void SetLine(Line line)
        {
        }
        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}
