using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _lockTs = 0;
    private float _energy = 1f;

    public bool Shoot;

    void Update()
    {
        if ((_lockTs-=Time.deltaTime) >= 0)
            return;
        if (_energy < shootEnergyPrice)
        {
            _energy += energyRestoration *= Time.deltaTime;
            return;
        }
        
        
        if (Shoot)
        {
            _lockTs = cooldown;
            Projectile t = Instantiate(_projectile, transform.parent);
            //t.transform.rotation = transform.rotation;
            t.transform.position = transform.position;
            t.transform.up = transform.right;
        } 
    
    }
}
