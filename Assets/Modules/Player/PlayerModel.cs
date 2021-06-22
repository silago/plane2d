using System;
using System.Collections;
using Modules.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Modules.Player
{
    public class PlayerModel 
    {
        private readonly UserDataProvider _userData;
        public float CurrentFuelConsumeProgress = 0;
        private float fuelConsumeSpeed = 0.05f;
        private Vector3 _targetVelocity;
        private readonly Updater _updater;
        public event Action<float> FuelConsumeProgress = f => {};
        public int CurrentFuel
        {
            get => _userData[ResourceNames.Fuel];
            protected set => _userData[ResourceNames.Fuel] = value;
        }
        public int CurrentHull => _userData[ResourceNames.Hull];

        public PlayerModel(UserDataProvider userDataProvider, Updater updater)
        {
            _userData = userDataProvider;
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
                var current = _userData[ResourceNames.Fuel];
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
