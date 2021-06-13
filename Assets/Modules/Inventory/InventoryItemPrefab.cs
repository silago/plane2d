using Modules.Resources;
using UnityEngine;
using UnityEngine.UI;
namespace Modules.Inventory
{
    public class InventoryItemPrefab : MonoBehaviour
    {
        [SerializeField]
        private Text count;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private GameObject countContainer;
        public InventoryItemPrefab Instantiate(Transform parent, ResourceInfo info, int itemCount)
        {
            var item = Instantiate(this, parent);
            item.count.text = itemCount.ToString();
            item.countContainer.SetActive(itemCount > 1);
            item.icon.sprite = info.Icon;
            return item;
        }
    }
}
