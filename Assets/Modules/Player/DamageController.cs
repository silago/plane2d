#region
using System;
using System.Collections;
using Events;
using Modules.Utils;
using UnityEngine;
#endregion
public class DestroyMessage : IMessage
{
    public int Damage;
}
public class DamageMessage : IMessage
{
    public int Id;
    public int Damage;
    public int CurrentHull;
}

/*
public class VisibleStateChangeEmitter {

}
*/

public class DisplayHullMessage : IMessage
{
    public bool Active;
    public int Id;
    public int InitialHull;
    public int CurrentHull;
    public Transform Target;
}

namespace Modules.Game.Player
{

    public class DamageController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem destroyEffect;
        [SerializeField]
        private ParticleSystem damageEffect;
        [SerializeField]
        private int initialHull;
        [SerializeField]
        private int currentHull;
        protected virtual int Hull
        {
            get => currentHull;
            set => currentHull = value;
        } 
        protected virtual void Awake()
        {
            this.Subscribe<DamageMessage, int>(OnDamage, gameObject.GetInstanceID()).BindTo(this);
            if (Hull == default) Hull = initialHull;
            
            this.SendEvent(new DisplayHullMessage() {
                Active = true,
                Id = gameObject.GetInstanceID(),
                InitialHull = initialHull,
                CurrentHull = Hull,
                Target = transform
            });
        }

        protected virtual void OnDestroy()
        {
            this.SendEvent(new DisplayHullMessage() {
                Active = false,
                Id = gameObject.GetInstanceID()
            });
        }
        protected void OnDamage(DamageMessage obj)
        {
            if ((Hull-=obj.Damage) <= 0)
            {
                var p = Instantiate(destroyEffect, transform.parent);
                p.transform.position = transform.position;
                p.gameObject.SetActive(true);
                this.DoWithDelay(0.3f, () => Destroy(gameObject));
            }
            else
            {
                var p = Instantiate(damageEffect, transform.parent);
                p.transform.position = transform.position;
                p.gameObject.SetActive(true);
            }
            this.SendEvent(new DamageMessage() {
                Id = gameObject.GetInstanceID(),
                Damage = obj.Damage,
                CurrentHull = Hull
            });
        }

        private IEnumerator OnEffectEnd(ParticleSystem p, Action cb)
        {
            while (p.IsAlive())
                yield return new WaitForSeconds(1);
            cb();
        }

        private IEnumerator DestroyEffectOnEnd(ParticleSystem p)
        {
            while (p.IsAlive())
                yield return new WaitForSeconds(1);
            Destroy(p.gameObject);
        }
        
        private void OnBecameVisible()
        {
            this.SendEvent(new DisplayHullMessage() {
                Active = true,
                Id = gameObject.GetInstanceID(),
                InitialHull = initialHull,
                CurrentHull = Hull,
                Target = transform
            });
        }
        private void OnBecameInvisible()
        {
            this.SendEvent(new DisplayHullMessage() {
                Active = false,
                Id = gameObject.GetInstanceID()
            });
        }
    }

}
