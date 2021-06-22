using System;
using Events;
using Modules.Player;
using Modules.Resources;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Modules.UI
{
    public class PlayerHud : MonoBehaviour
    {
        [SerializeField]
        private Text HullText;
        [SerializeField]
        private Text FuelText;
        [SerializeField]
        private Image FuelImage;
        private PlayerModel _model;

        [Inject]
        void Construct(PlayerModel model)
        {
            _model = model;
            _model.FuelConsumeProgress += OnFuelConsumeProgress;
            HullText.text = model.CurrentHull.ToString();
            FuelText.text = model.CurrentFuel.ToString();
            FuelImage.fillAmount = model.CurrentFuelConsumeProgress;
        }
        private void OnFuelConsumeProgress(float val)
        {
            FuelImage.fillAmount = 1-val;
        }

        private void Awake()
        {
            this.Subscribe<ResourceChanged, string>(OnHullChanged, ResourceNames.Hull);
            this.Subscribe<ResourceChanged, string>(OnFuelChanged, ResourceNames.Fuel);
        }
        private void OnHullChanged(ResourceChanged obj)
        {
            HullText.text = obj.Value.ToString();
        }
        private void OnFuelChanged(ResourceChanged obj)
        {
            FuelText.text = obj.Value.ToString();
        }
    }
}
