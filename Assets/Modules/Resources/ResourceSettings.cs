using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Modules.Resources
{
    [CreateAssetMenu(menuName = "Settings/ResourceSettings", fileName = "ResourceSettings", order = 0)]
    public class ResourceSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        public ResourceInfo[] data;
        [SerializeField]
        private Sprite defaultResourceSprite;


        public Dictionary<string, ResourceInfo> Resources => _resourceDict;

        private Dictionary<string, ResourceInfo> _resourceDict;

        public ResourceInfo this[string index] => _resourceDict.TryGetValue(index, out var resource) ? resource : new ResourceInfo {
            Id = index,
            Name = index,
            Icon = defaultResourceSprite
        };

        public void OnAfterDeserialize()
        {
            _resourceDict = data.ToDictionary(x => x.Id, x => x);
        }
        public void OnBeforeSerialize() {}
    }
}
