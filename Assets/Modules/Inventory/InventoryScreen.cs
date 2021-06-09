using System;
using Modules.Common;
using Modules.Utils;
using UnityEngine;
using Zenject;
namespace Modules.Inventory
{
    public class InventoryScreen : BaseScreen
    {
        private InventoryDataProvider _inventoryDataProvider;
        protected override ScreenId ScreenId => ScreenId.Inventory;
        [SerializeField]
        private Transform inventory;
        [SerializeField]
        private InventoryItemPrefab inventoryItemPrefab;
        
        [Inject]
        public  void Construct(InventoryDataProvider inventoryDataProvider)
        {
            _inventoryDataProvider = inventoryDataProvider;
        }

        protected override void OnShow()
        {
            var items = _inventoryDataProvider.GetUserResources();
            inventory.ClearChildren();
            foreach (var (info, count) in items)
            {
                inventoryItemPrefab.Instantiate(inventory, info, count);
            }
        }
    }
}
