using System;
using System.Collections;
using UnityEngine;
namespace Modules.Utils
{
    public static class MonobehaviourExtension
    {
    
        private static IEnumerator DoWithDelayCo(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }
         
        public static void DoWithDelay(this MonoBehaviour @this, float seconds, Action action)
        {
            @this.StartCoroutine(DoWithDelayCo(seconds, action));
        }
    }
}
