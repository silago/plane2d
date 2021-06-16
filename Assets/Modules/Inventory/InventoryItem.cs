using System;
using System.Collections.Generic;
using Modules.Resources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
namespace Modules.Inventory
{
    public class InventoryItemPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField]
        private Text count;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private GameObject countContainer;
        [SerializeField]
        private List<Graphic> graphics;
        [SerializeField]
        private Color selectedColor;
        [SerializeField]
        private Color regularColor;
        public event Action<ResourceInfo> OnClick = resourceInfo => {};
        
        public ResourceInfo info;
        public InventoryItemPrefab Instantiate(Transform parent, ResourceInfo info, int itemCount)
        {
            var item = Instantiate(this, parent);
            item.count.text = itemCount.ToString();
            item.countContainer.SetActive(itemCount > 1);
            item.icon.sprite = info.Icon;
            item.info = info;
            return item;
        }

        void OnIn() => graphics.ForEach(x=>x.color = selectedColor);
        void OnOut() => graphics.ForEach(x=>x.color = regularColor);


        public void OnPointerEnter(PointerEventData eventData) => OnIn();
        public void OnPointerExit(PointerEventData eventData) => OnOut();
        public void OnPointerDown(PointerEventData eventData) => OnClick(info);
    }
}
