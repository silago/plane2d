using Events;
using UnityEngine;
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float ttl = 10;
    [SerializeField]
    public float speed;
    [SerializeField]
    private int damage;
    public Vector3 shooterSpeed = Vector3.zero;

    private void Update()
    {
        transform.position += -transform.up * (Time.deltaTime * speed);
        if ((ttl -= Time.deltaTime) <= 0)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        this.SendEvent(new DamageMessage {
            Damage = damage
        }, other.gameObject.GetInstanceID());
        Destroy(gameObject);
    }
}
