#region
using System.Collections.Generic;
using UnityEngine;
#endregion
namespace UB
{
    public class EffectBase : MonoBehaviour
    {
        public static Dictionary<string, RenderTexture> AlreadyRendered = new Dictionary<string, RenderTexture>();

        public static bool InsideRendering
        {
            get;
            set;
        }
    }
}
