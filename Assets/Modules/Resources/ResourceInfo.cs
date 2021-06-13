using System;
using UnityEngine;
namespace Modules.Resources
{
    [Serializable]
    public class ResourceInfo
    {
        public string Id;
        public string Name;
        public Sprite Icon;
        public ResourceType ResourceType;
    }

    [Flags]
    enum ResourceFlags
    {
        InvisibleInInventory
    }

    
    /* proporsoal
     
        public enumResourceType {
            Item.
        }
     
     */
    public enum ResourceType
    {
        Misc = 0,
        Resource = 1,
        Quest = 2,
        Item = 3,
    }
}
