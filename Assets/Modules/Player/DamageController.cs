#region
using System;
using System.Collections;
using Events;
using Modules.Utils;
using UnityEngine;
#endregion
public class DestroyMessage : Message
{
    public int Damage;
}
public class DamageMessage : Message
{
    public int Id;
    public int Damage;
    public int CurrentHull;
}

public class DisplayHullMessage : Message
{
    public bool Active;
    public int Id;
    public int InitialHull;
    public int CurrentHull;
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
        private void Awake()
        {
            this.Subscribe<DamageMessage, int>(OnDamage, gameObject.GetInstanceID()).BindTo(this);
            if (currentHull == default) currentHull = initialHull;
        }
        private void OnDamage(DamageMessage obj)
        {
            currentHull -= obj.Damage;
            if (currentHull <= 0)
            {
                var p = Instantiate(destroyEffect, transform.parent);
                p.transform.position = transform.position;
                p.gameObject.SetActive(true);
                this.DoWithDelay(0.3f, () => Destroy(gameObject));
            }
            else
            {
                var p = Instantiate(damageEffect, transform);
                p.gameObject.SetActive(true);
            }
            this.SendEvent(new DamageMessage() {
                Id = gameObject.GetInstanceID(),
                Damage = obj.Damage,
                CurrentHull = currentHull
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
                CurrentHull = currentHull
            });
        }
        private void OnBecameInvisible()
        {
            this.SendEvent(new DisplayHullMessage() {
                Active = true,
                Id = gameObject.GetInstanceID()
            });
        }
    }

}
