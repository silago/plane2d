using Modules.Enemies;
using UnityEngine;
namespace Modules.PlaneSettings
{
    [CreateAssetMenu(menuName = "Planes/CommonSettings", fileName = "CommonPlaneSettings", order = 0)]
    public class PlaneSettings : ScriptableObject
    {
        public AttackType attackType;
        public Utils.MinMax attackEvery;
        public float angleTreshhold = 1f;
        public float desiredDistance;
        public float sideRaycastOffset;
        public float surrenderDistance = 7;
        public MinMax targetChangeInterval;
        
        
    }
}
