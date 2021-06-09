using UnityEngine;
namespace Modules.Resources
{
    public class BaseStorage
    {
        protected readonly string Prefix;
        protected string Key(string k) => $"{Prefix}_{k}";
        protected string Key(object k) => $"{Prefix}_{k}";
        
        public BaseStorage(string prefix = "")
        {
            Prefix = prefix;
        }
        public bool HasKey(string key) => PlayerPrefs.HasKey(Key(key));
    }
}
