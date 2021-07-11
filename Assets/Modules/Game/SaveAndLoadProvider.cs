using UnityEngine;
namespace Modules.Game
{
    public class SaveAndLoadProvider
    {
        private const string key = "current_save_name";
        public string CurrentSaveName
        {
            get => PlayerPrefs.GetString(key);
            set => PlayerPrefs.SetString(key, value);
        }
    }
}
