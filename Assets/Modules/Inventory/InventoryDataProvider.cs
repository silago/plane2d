using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Resources;
namespace Modules.Inventory
{
    public class InventoryDataProvider
    {
        public event Action Update;
        // make cache
        private ResourceSettings _resourceSettings;
        private UserDataProvider _userDataProvider;
        // slotId => id maybe
        private StringStorage<ResourceSubType> _equippedItems = new StringStorage<ResourceSubType>();
        
        public  InventoryDataProvider(UserDataProvider userDataProvider, ResourceSettings resourceSettings)
        {
            _resourceSettings = resourceSettings;
            _userDataProvider = userDataProvider;
        }
        public void Equip(ResourceSubType type, string itemId) => _equippedItems[type] = itemId;
        public string GetEquippedItem(ResourceSubType type) => _equippedItems[type]; 

        public (ResourceInfo, int)[] GetUserResources()
        {
            List<(ResourceInfo, int)> result = new List<(ResourceInfo, int)>(); 
            var resources = _resourceSettings.Resources;
            foreach (var resourceInfo in resources)
            {
                result.Add((resourceInfo.Value, _userDataProvider[resourceInfo.Key]));
            }
            return result.ToArray();
        }

        public Dictionary<ResourceType, (ResourceInfo, int)[]> GetGroupedResources()
        {
            return GetUserResources().Where(x=>x.Item2>0).GroupBy(x => x.Item1.ResourceType).ToDictionary(x=>x.Key,x=>x.ToArray());
        }

        public virtual void OnUpdate()
        {
            Update?.Invoke();
        }
    }
}
