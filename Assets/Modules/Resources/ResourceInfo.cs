using System;
using UnityEngine;
namespace Modules.Resources
{
    [Serializable]
    public class ResourceInfo
    {
        public string Id;
        public string Name;
        public string Description;
        public Sprite Icon;
        public ResourceType ResourceType; // Group
        public ResourceSubType ResourceSubType; // Type
        public ResourceFlags Flags;
    }

}
