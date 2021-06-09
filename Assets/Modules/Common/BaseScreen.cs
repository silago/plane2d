using System;
using Events;
using UnityEngine;
using Zenject;
namespace Modules.Common
{

    public abstract class BaseScreen : MonoBehaviour
    {
        protected abstract ScreenId ScreenId { get;  }
        [SerializeField]
        protected Transform Content;
        private void Awake()
        {
            this.Subscribe<ChangeScreenState,ScreenId>(OnChangeScreenStateMessage,ScreenId.Inventory).BindTo(this);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
        protected void OnChangeScreenStateMessage(ChangeScreenState msg)
        {
            Content.gameObject.SetActive(msg.Active);
            if (msg.Active) OnShow(); else OnHide();
        }
    }

    public class ChangeScreenState : Message
    {
        public bool Active;
        public ScreenId ScreenId;
    }
    public enum ScreenId
    {
        None = 0,
        Inventory = 1,
    }
}
