using System.Collections.Generic;
using System.Linq;
using Modules.Common;
using Modules.Resources;
using Modules.YarnPlayer;
using UnityEngine;
using Zenject;
namespace Modules.Inventory
{
    public class InventoryScreen : BaseScreen
    {
        private InventoryDataProvider _inventoryDataProvider;
        protected override ScreenId ScreenId => ScreenId.Inventory;
        [SerializeField]
        private ResourceInfoScreen resourceInfoScreen;
        [SerializeField]
        private Transform inventory;
        [SerializeField]
        private InventoryItem inventoryItem;
        [SerializeField]
        private InventoryGroup inventoryGroupPrefab;
        [SerializeField]
        private List<EquipSlot> slots;
        private Dictionary<ResourceSubType, string> _equipped = new Dictionary<ResourceSubType, string>();
        
        [Inject]
        void Construct(InventoryDataProvider inventoryData)
        {
            _inventoryDataProvider = inventoryData;
        }
        private void Start()
        {
            foreach (var slot in slots)
            {
                var itemId = _inventoryDataProvider.GetEquippedItem(slot.subType);
                if (!string.IsNullOrEmpty(itemId))
                {
                    _equipped[slot.subType] = itemId;
                }
            }
            
            if (Content.gameObject.activeSelf) OnShow();
            _inventoryDataProvider.Update +=  OnShow;
            resourceInfoScreen.Equip += Equip;
            resourceInfoScreen.Dequip += Dequip;
        }
        private void Dequip(InventoryItem item)
        {
            _equipped.Remove(item.info.ResourceSubType);
            _inventoryDataProvider.Equip(item.info.ResourceSubType, null);
        }
        private void Equip(InventoryItem obj)
        {
            var slot = slots.FirstOrDefault(x => x.subType == obj.info.ResourceSubType);
            if (slot == null) return;
            _inventoryDataProvider.Equip(obj.info.ResourceSubType,obj.info.Id);
            _equipped[obj.info.ResourceSubType] = obj.info.Id;
        }
        
        private readonly List<GameObject> _groupsGameObjects = new List<GameObject>();

        protected override void OnShow()
        {
            foreach (var groupsGameObject in _groupsGameObjects) Destroy(groupsGameObject);
            var groups = _inventoryDataProvider.GetGroupedResources();
            foreach (var group  in groups)
            {
                var groupGO = Instantiate(inventoryGroupPrefab, inventory);
                _groupsGameObjects.Add(groupGO.gameObject);
                groupGO.label.text = group.Key.ToString();
                foreach (var (info, count) in group.Value)
                {
                    if (count == 0) continue;
                    if (info.Flags.HasFlag(ResourceFlags.InvisibleInInventory)) continue;
                    Transform parent;
                    if (info.Flags.HasFlag(ResourceFlags.Equippable)
                        && (_equipped.TryGetValue(info.ResourceSubType, out var id))
                        &&  (info.Id == id))
                    {
                        parent = slots.First(x => x.subType == info.ResourceSubType).transform;
                    }
                    else
                    {
                        parent = groupGO.transform;
                    }
                    var item = inventoryItem.Instantiate(parent, info, count);
                    item.OnClick += ShowResourceInfo;
                }
            }
        }
        void ShowResourceInfo(InventoryItem item)
        {
            bool isEquipped = item.info.Id == _inventoryDataProvider.GetEquippedItem(item.info.ResourceSubType);
            resourceInfoScreen.Open(item, isEquipped);
        }
    }

}
