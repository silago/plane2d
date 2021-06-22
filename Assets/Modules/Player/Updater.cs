using System;
using UnityEngine;
namespace Modules.Player
{
    public class Updater : MonoBehaviour
    {
        public event Action Started = () => {};
        private void Start()
        {
            Started();
        }
    }
}
