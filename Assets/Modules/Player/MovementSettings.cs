using UnityEngine;
namespace Modules.Player
{
    [CreateAssetMenu(menuName = "Unit/MovementSettings", fileName = "MovementSettings", order = 0)]
    public class MovementSettings : ScriptableObject
    {
        public float rotationSpeed;
        public float accelerationSpeed;
        public float maxVelocity;
        public float minVelocity;
        public float rotationDeceleration;
        public float velocityChangeRate;
    }
}