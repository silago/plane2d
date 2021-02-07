using System;
using Events;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Modules.Notifications
{
    public class HintWidget : MonoBehaviour
    {
        [SerializeField] private Text text;
        private void Awake()
        {
            this.Subscribe<HintMessage>(OnHintMessage);
            gameObject.SetActive(false);
        }

        private void OnHintMessage(HintMessage obj)
        {
            gameObject.SetActive(obj.active);
            if (false == string.IsNullOrEmpty(obj.text))
            {
                text.text = obj.text;
            }
        }
    }
}