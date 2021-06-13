using System;
using Events;
using UnityEngine;
using Zenject;
namespace Modules.Map
{
    public class MapElement : MonoBehaviour, IMessage
    {
        [SerializeField]
        public string caption;
        [SerializeField]
        public Sprite sprite;
        private void Start()
        {
            this.SendEvent(this);
        }
    }
}
