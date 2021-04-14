#region
using System.Collections;
using System.Collections.Generic;
using Modules.Utils.TagSelector;
using UnityEngine;
using Zenject;
using static Modules.Utils.Vector3Extensions;
#endregion


namespace Modules.Enemies
{
    public class EnemyRamBehaviour : MonoBehaviour
    {

        [SerializeField] private Transform aim;

        [SerializeField] private Transform bulletPrefab;

        [SerializeField] private float rotationSpeed;

        [SerializeField]
        private float bulletSpeed = 10f;

        private bool _active;
        [Inject(Id = "Player")]
        private IMovable _player;

        private Transform _target;
        private readonly List<Transform> bullets = new List<Transform>();
        [TagSelector]
        private string filterTag;

        private readonly Vector3 t = new Vector3();

        private void Start()
        {
            StartCoroutine(ShootCo());
        }

        // Update is called once per frame
        private void Update()
        {
            Aim();
            foreach (var bullet in bullets) bullet.position += -bullet.up * (Time.deltaTime * bulletSpeed);
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(t, 1f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (false == other.CompareTag(filterTag)) return;
            if (other.transform == _player)
                _target = other.transform;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform == _player) _target = null;
            //if (false == other.CompareTag(filterTag)) return;
            _target = null;
        }

        private void RotateTowardsTarget()
        {
            if (_target == null) return;
            ;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(Vector3.forward, transform.position - _target.position),
                Time.deltaTime * rotationSpeed
            );
        }
        private void Aim()
        {

            Quaternion? q;
            if ((q = PredictAim2(
                transform.position,
                _player.Transform.position,
                _player.Direction * _player.VelocityMagnitude,
                bulletSpeed
            )) != null)
            {
                _active = true;
                transform.rotation = q.Value;
            }
            else
            {
                _active = false;
            }
        }

        private IEnumerator ShootCo()
        {
            for (;;)
            {
                yield return new WaitForSeconds(3);
                if (_active == false) continue;
                var b = Instantiate(bulletPrefab, transform.parent);
                b.rotation = transform.rotation;
                b.position = transform.position;
                bullets.Add(b);
            }
        }


        private void StateBattle()
        {
            RotateTowardsTarget();
        }
    }
}
