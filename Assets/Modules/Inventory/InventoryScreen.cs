using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Modules.Common;
using Modules.Resources;
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
        [SerializeField]
        private InventoryGroup inventoryGroupPrefab;

        [Inject]
        void Construct(InventoryDataProvider inventoryData)
        {
            _inventoryDataProvider = inventoryData;
        }


        private void Start()
        {
            if (Content.gameObject.activeSelf)
                OnShow();
            _inventoryDataProvider.Update +=  OnShow;
        }
        private List<GameObject> groupsGameObjects = new List<GameObject>();

        protected override void OnShow()
        {
            foreach (var groupsGameObject in groupsGameObjects)
            {
                Destroy(groupsGameObject);
            }
            var groups = _inventoryDataProvider.GetGroupedResources();
            foreach (var group  in groups)
            {
                var groupGO = Instantiate(inventoryGroupPrefab, inventory);
                groupsGameObjects.Add(groupGO.gameObject);
                groupGO.label.text = group.Key.ToString();
                foreach (var (info, count) in group.Value)
                {
                    if (count == 0) continue;
                    inventoryItemPrefab.Instantiate(groupGO.transform, info, count);
                }
            }
        }
        
        
    }
}
