using System.Collections.Generic;
using Modules.Resources;
namespace Modules.Inventory
{
    public class InventoryDataProvider
    {
        // make cache
        private ResourceSettings _resourceSettings;
        private UserDataProvider _userDataProvider;
        
        public  InventoryDataProvider(UserDataProvider userDataProvider, ResourceSettings resourceSettings)
        {
            _resourceSettings = resourceSettings;
            _userDataProvider = userDataProvider;
        }

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
        
    }
}
