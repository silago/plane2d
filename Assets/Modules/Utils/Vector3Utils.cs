#region
using UnityEngine;
#endregion
namespace Modules.Utils
{
    public static class Vector3Utils
    {

        public static void Clamp(this ref Vector3 value, Vector3 Max)
        {
            value.Clamp(Vector3.zero, Max);
        }
        public static void Clamp(this ref Vector3 value, Vector3 Min, Vector3 Max)
        {
            value.x = Mathf.Clamp(value.x, Min.x, Max.x);
            value.y = Mathf.Clamp(value.y, Min.y, Max.y);
            value.z = Mathf.Clamp(value.z, Min.z, Max.z);
        }
    }
}
