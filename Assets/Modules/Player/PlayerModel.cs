using System;
using System.Collections;
using Modules.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Modules.Player
{
    public class PlayerModel 
    {
        private readonly DataProvider _data;
        public float CurrentFuelConsumeProgress = 0;
        private float fuelConsumeSpeed = 0.05f;
        private Vector3 _targetVelocity;
        private readonly Updater _updater;
        public event Action<float> FuelConsumeProgress = f => {};
        public int CurrentFuel
        {
            get => _data[ResourceNames.Fuel];
            protected set => _data[ResourceNames.Fuel] = value;
        }
        public int CurrentHull => _data[ResourceNames.Hull];

        public PlayerModel(DataProvider dataProvider, Updater updater)
        {
            _data = dataProvider;
            _updater = updater;
            _updater.Started += () =>
            {
                updater.StartCoroutine(FuelConsuming());
            };
        }
        public Vector3 TargetVelocity
        {
            get => CurrentFuel > 0 ? _targetVelocity : Vector3.zero;
            set => _targetVelocity = value;
        }

        IEnumerator FuelConsuming()
        {
            for (;;)
            {
                yield return new WaitForSeconds(1);
                var current = _data[ResourceNames.Fuel];
                if (current <=0 ) continue;
                FuelConsumeProgress.Invoke(CurrentFuelConsumeProgress);
                if ((CurrentFuelConsumeProgress += fuelConsumeSpeed * _targetVelocity.magnitude) >= 1)
                {
                    CurrentFuelConsumeProgress = 0;
                    CurrentFuel -= 1;
                }
            }
        }
    }
}
