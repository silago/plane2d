using UnityEngine;
using UnityEngine.UI;
namespace Modules.UI
{
    public class HullWidget : MonoBehaviour
    {
        [SerializeField]
        private Text current;
        [SerializeField]
        private Text initial;
        public int Current { set => current.text = value.ToString(); }
        public int Initial { set => initial.text = value.ToString(); }
    }
}
