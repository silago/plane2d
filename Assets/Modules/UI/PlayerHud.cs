using System;
using Events;
using Modules.Resources;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;
namespace Modules.UI
{
    public class PlayerHud : MonoBehaviour
    {
        private void Awake()
        {
            this.Subscribe<ResourceChanged, string>(OnHullChanged, ResourceNames.Hull);
            this.Subscribe<ResourceChanged, string>(OnFuelChanged, ResourceNames.Fuel);
        }
        private void OnHullChanged(ResourceChanged obj)
        {
            throw new NotImplementedException();
        }
        private void OnFuelChanged(ResourceChanged obj)
        {
            throw new NotImplementedException();
        }
    }
}
