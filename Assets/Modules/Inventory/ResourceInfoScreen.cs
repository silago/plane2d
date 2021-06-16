using System;
using Modules.Resources;
using UnityEngine;
using UnityEngine.UI;
namespace Modules.Inventory
{
    public class ResourceInfoScreen : MonoBehaviour
    {
        [SerializeField]
        private Text description;
        //[SerializeField]
        //private Image icon;
        [SerializeField]
        private Button quitButton;
        [SerializeField]
        private Button equipButton;
        [SerializeField]
        private Button dequipButton;
        private InventoryItem _currentItem;
        public event Action<InventoryItem> Equip = item => {}; 
        public event Action<InventoryItem> Dequip = item => {}; 
        
        private void Awake()
        {
            quitButton.onClick.AddListener(Close);
            equipButton.onClick.AddListener(OnEquip);
            equipButton.onClick.AddListener(OnDequip);
        }
        private void OnDequip()
        {
            if (_currentItem != null)
                Dequip(_currentItem);
        }
        private void OnEquip()
        {
            if (_currentItem != null)
                Equip(_currentItem);
        }

        private void Close()
        {
            this.gameObject.SetActive(false); 
        }
        public void Open(InventoryItem item, bool isEquipped)
        {
            gameObject.SetActive(true);
            description.text = item.info.Description;
            _currentItem = item;
            description.text = item.info.Description;
            //icon.overrideSprite = item.info.Icon;
            if (item.info.Flags.HasFlag(ResourceFlags.Equippable))
            {
                equipButton.gameObject.SetActive(!isEquipped);
                dequipButton.gameObject.SetActive(isEquipped);
            }
            else
            {
                equipButton.gameObject.SetActive(false);
                dequipButton.gameObject.SetActive(false);
            }
        }
    }
}
