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
    public int Damage;
}

public class DisplayHullMessage : Message
{
    public bool Active;
    public int Id;
    public int InitialHull;
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
        private int Hull;
        private void Awake()
        {
            this.Subscribe<DamageMessage, int>(OnDamage, transform.GetInstanceID()).BindTo(this);
        }
        private void OnDamage(DamageMessage obj)
        {
            Hull -= obj.Damage;
            if (Hull <= 0)
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
                Id = GetInstanceID()
            });
        }
        private void OnBecameInvisible()
        {
            this.SendEvent(new DisplayHullMessage() {
                Active = true,
                Id = GetInstanceID()
            });
        }
    }

}
