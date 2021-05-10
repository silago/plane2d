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

    public enum ResourceType
    {
        Default, Quest
    }
}
