using System;
using Events;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float ttl = 10;
    [SerializeField]
    public float speed;
    [SerializeField]
    private int damage;
    public Vector3 shooterSpeed = Vector3.zero;
    private void OnTriggerEnter2D(Collider2D other)
    {
        this.SendEvent(new DamageMessage { Damage = damage }, other.transform.GetInstanceID());
        Destroy(this.gameObject);
    }

    private void Update()
    {
         transform.position += -transform.up * (Time.deltaTime * speed);
         if ((ttl -= Time.deltaTime) <= 0)
             Destroy(gameObject);
    }
}
