using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utils;
using Modules.Utils.TagSelector;
using UnityEditor;
using UnityEngine;
using Zenject;
using static Modules.Utils.Vector3Extensions;
    

namespace Modules.Enemies
{
    public class EnemyRamBehaviour : MonoBehaviour
    {
        [Inject(Id = "Player")]
        private IMovable _player;

        [SerializeField] private Transform aim;
                
        private Transform _target;
        [TagSelector]
        private string filterTag;

        [SerializeField] private Transform bulletPrefab;
        private List<Transform> bullets = new List<Transform>();

        [SerializeField] private float rotationSpeed;

        private void Start()
        {
            StartCoroutine(ShootCo());
        }

        // Update is called once per frame
        void Update()
        {
            Aim();
            foreach (var bullet in bullets)
            {
                bullet.position += -bullet.up * (Time.deltaTime * bulletSpeed);
            }
        }

        void RotateTowardsTarget()
        {
            if (_target == null) return;;
            transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                Quaternion.LookRotation(Vector3.forward, transform.position - _target.position),
                Time.deltaTime * rotationSpeed
                );
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (false == other.CompareTag(filterTag)) return;
            if (other.transform == _player)
                _target = other.transform;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform == _player)
            {
                _target = null;
            }
            //if (false == other.CompareTag(filterTag)) return;
            _target = null;
        }

        private bool _active = false;

        [SerializeField]
        private float bulletSpeed = 10f;
        void Aim()
        {

            Quaternion? q;
            if ((q = Vector3Extensions.PredictAim2(
                transform.position,
                _player.Transform.position,
                _player.Direction * _player.VelocityMagnitude,
                bulletSpeed
            ))!=null)
            {
                _active = true;
                transform.rotation = q.Value;
            }
            else
            {
                _active = false;
            }
        }

        private Vector3 t = new Vector3();
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(t, 1f);
        }

        IEnumerator ShootCo()
        {
            for (;;)
            {
                yield return new WaitForSeconds(3);
                if (_active == false)
                {
                    continue;
                }
                var b = Instantiate(bulletPrefab, transform.parent);
                b.rotation = transform.rotation;
                b.position = transform.position;
                bullets.Add(b);
            } 
        }


        void StateBattle()
        {
            RotateTowardsTarget();
        }
    }
}
