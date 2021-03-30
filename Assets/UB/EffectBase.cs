#region
using System.Collections.Generic;
using UnityEngine;
#endregion
namespace UB
{
    public class EffectBase : MonoBehaviour
    {
        public static Dictionary<string, RenderTexture> AlreadyRendered = new Dictionary<string, RenderTexture>();

        private static bool _insiderendering;
        public static bool InsideRendering
        {
            get => _insiderendering;
            set => _insiderendering = value;
        }
    }
}
