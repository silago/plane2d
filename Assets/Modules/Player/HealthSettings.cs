using UnityEngine;
namespace Modules.Player
{
    [CreateAssetMenu(menuName = "Unit/HealthSettings", fileName = "HealthSettings", order = 0)]
    public class HealthSettings : ScriptableObject
    {
        public int hull;
    }
}
