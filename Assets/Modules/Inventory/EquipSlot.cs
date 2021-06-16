using Modules.Resources;
using Modules.Utils.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;
namespace Modules.Inventory
{
    public class EquipSlot : MonoBehaviour
    {
        [ScriptableObjectId]
        public string id;
        [ScriptableObjectId]
        public ResourceSubType subType;
        [SerializeField]
        public Image icon;
    }
}
