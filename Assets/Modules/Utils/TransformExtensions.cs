using UnityEngine;
namespace Modules.Utils
{
    public static class TransformExtensions
    {
        public static void ClearChildren(this Transform item)
        {
            foreach (Transform child in item)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
