using UnityEngine;
namespace Modules.Player
{
    [CreateAssetMenu(menuName = "Unit/ShootSettings", fileName = "ShootSettings", order = 0)]
    public class ShootSettings : ScriptableObject
    {
        public float cooldown;
        public float energyRestoration = 1f;
        public float shootEnergyPrice = 0.1f;
        public float projectileSpeed = 2f;
        public float damage = 1f;
    }
}
