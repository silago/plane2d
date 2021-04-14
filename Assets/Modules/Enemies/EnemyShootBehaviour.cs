using System.Collections;
using Modules.Utils.TagSelector;
using UnityEngine;
using Zenject;
using static Modules.Utils.Vector3Extensions;


namespace Modules.Enemies
{
    public class EnemyShootBehaviour : MonoBehaviour
    {

        [SerializeField] private Projectile bulletPrefab;
        [SerializeField] private float rotationSpeed;
        [Inject(Id = "Player")]
        private IMovable _player;

        private Transform _target;

        private bool active;
        [TagSelector]
        private string filterTag;

        private Vector3 prevPlayerPos;

        private readonly Vector3 t = new Vector3();

        private void Start()
        {
            prevPlayerPos = _player.Transform.position;
            StartCoroutine(ShootCo());
        }

        // Update is called once per frame
        private void Update()
        {
            Aim();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(t, 1f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform == _player.Transform)
                _target = other.transform;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform == _player.Transform) _target = null;
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

        private void StateIdle()
        {

        }
        private void Aim()
        {

            Quaternion? q;
            if ((q = PredictAim2(
                transform.position,
                _player.Transform.position,
                _player.Direction * _player.VelocityMagnitude,
                bulletPrefab.speed
            )) != null)
            {
                active = true;
                transform.rotation = q.Value;
            }
            else
            {
                active = false;
            }
        }

        private IEnumerator ShootCo()
        {
            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForSeconds(3);
                if (active == false) continue;
                var b = Instantiate(bulletPrefab, transform.parent);
                b.gameObject.SetActive(true);
                b.transform.rotation = transform.rotation;
                b.transform.position = transform.position;
            }
        }

        private void StateBattle()
        {
            RotateTowardsTarget();
        }
    }
}
