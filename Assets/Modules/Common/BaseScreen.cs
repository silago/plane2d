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
        [SerializeField]
        protected KeyCode keyCode = KeyCode.None;
        protected void Awake()
        {
            this.Subscribe<ChangeScreenState,ScreenId>(OnChangeScreenStateMessage,ScreenId).BindTo(this);
        }

        protected virtual void Update()
        {
            if (keyCode != KeyCode.None && Input.GetKeyDown(keyCode))
            {
                Content.gameObject.SetActive(!Content.gameObject.activeSelf);
                if (Content.gameObject.activeSelf) 
                    OnShow();
            } 
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

    public class ChangeScreenState : IMessage
    {
        public bool Active;
    }
    public enum ScreenId
    {
        None = 0,
        Inventory = 1,
        Map = 2,
        Cheats =3,
    }
}
