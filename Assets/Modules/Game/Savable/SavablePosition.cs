using Modules.Resources;
using Modules.Utils.ScriptableObject;
using UnityEngine;
using Zenject;
namespace Modules.Game.Savable
{
    [RequireComponent(typeof(Transform))]
    public class SavablePosition : MonoBehaviour
    {
        [ScriptableObjectId]
        [SerializeField]
        public string id;
        private DataProvider _dataProvider;

        [Inject]
        public void Construct(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            Load();
        }
        private void OnApplicationQuit()
        {
            Save();
        }

        private void Load()
        {
            var data = _dataProvider.GetString(id);
            if (!string.IsNullOrEmpty(data))
            {
                var parts = data.Split(';');
                var pos = new Vector3(
                    float.Parse(parts[0]),
                    float.Parse(parts[1]),
                    float.Parse(parts[2])
                );
                transform.position = pos;
            }
        }

        private void Save()
        {
            var data = $"{transform.position.x};{transform.position.y};{transform.position.z}";
            _dataProvider.Set(id,data);
        }
    }
}
/*
public interface ISavable
{  
}
*/
