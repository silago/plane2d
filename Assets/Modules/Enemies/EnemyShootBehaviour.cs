using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Modules.Utils;
using Modules.Utils.TagSelector;
using UnityEditor;
using UnityEngine;
using Zenject;
using static Modules.Utils.Vector3Extensions;
    

namespace Modules.Enemies
{
    public class EnemyShootBehaviour : MonoBehaviour
    {
        [Inject(Id = "Player")]
        private IMovable _player;

        [SerializeField] private Transform aim;
                
        private Transform _target;
        [TagSelector]
        private string filterTag;

        [SerializeField] private Projectile bulletPrefab;

        [SerializeField] private float rotationSpeed;

        private void Start()
        {
            prevPlayerPos = _player.Transform.position;
            StartCoroutine(ShootCo());
        }

        // Update is called once per frame
        void Update()
        {
            Aim();
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

        void StateIdle()
        {
            
        }

        private bool active = false;

        Vector3 prevPlayerPos = new Vector3();
        void Aim()
        {

            Quaternion? q;
            if ((q = Vector3Extensions.PredictAim2(
                transform.position,
                _player.Transform.position,
                _player.Direction * _player.VelocityMagnitude,
                bulletPrefab.speed
            ))!=null)
            {
                active = true;
                transform.rotation = q.Value;
            }
            else
            {
                active = false;
            }
            return;
        }

        private Vector3 t = new Vector3();
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(t, 1f);
        }

        IEnumerator ShootCo()
        {
            for (var i = 0;i<30;i++)
            {
                yield return new WaitForSeconds(3);
                if (active == false)
                {
                    continue;
                }
                var b = Instantiate(bulletPrefab, transform.parent);
                b.gameObject.SetActive(true);
                b.transform.rotation = transform.rotation;
                b.transform.position = transform.position;
            } 
        }

        private void OnDestroy()
        {
        }


        void StateBattle()
        {
            RotateTowardsTarget();
        }
    }
}
