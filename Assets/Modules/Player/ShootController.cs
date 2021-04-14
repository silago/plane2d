#region
using UnityEngine;
#endregion
public class ShootController : MonoBehaviour
{
    [SerializeField]
    private Projectile _projectile;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float energyRestoration = 1f;
    [SerializeField]
    private float shootEnergyPrice = 0.1f;
    [SerializeField]
    public float projectileSpeed = 2f;

    public bool Shoot;
    private float _energy = 1f;
    private float _lockTs;

    public bool MakeShot()
    {
        if (_lockTs >= 0)
            return false;
        if (_energy < shootEnergyPrice)
        {
            return false;
        }
        
        _lockTs = cooldown;
        var t = Instantiate(_projectile, transform.parent);
        t.speed = projectileSpeed;
        //t.transform.rotation = transform.rotation;
        t.transform.position = transform.position;
        t.transform.up = transform.right;
        return true;

    }

    private void Update()
    {
        if (_lockTs>=0)
            _lockTs -= Time.deltaTime;
        if (_energy < 1)
            _energy += energyRestoration *= Time.deltaTime;


        if (Shoot)
        {
            MakeShot();
        }

    }
}
