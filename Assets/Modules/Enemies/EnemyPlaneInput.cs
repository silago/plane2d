using System.Collections;
using Modules.Utils;
using UnityEngine;
using Zenject;
using static Modules.Utils.Vector3Extensions;

namespace Modules.Enemies
{
    public class EnemyPlaneInput : MonoBehaviour
    {
        public bool setRadius;
        [SerializeField]
        private ShootController shootController;
        [SerializeField]
        private MovementController movementController;
        [SerializeField]
        private Utils.MinMax attackEvery;
        [SerializeField]
        private float angleTreshhold = 1f;
        [SerializeField]
        private Transform target;
        [SerializeField] private float desiredDistance;
        [SerializeField] private float distanceTreshold = 0.1f;

        [SerializeField]
        private State currentState = State.Default;

        [SerializeField]
        private float diffB;
        [SerializeField]
        private float diffA;
        [SerializeField]
        private float diff;
        [Inject(Id = "Player")]
        private IMovable _player;
        private float _prevAngle = 0;
        [SerializeField]
        private int shootCountBeforeExitState = 3;
        private int shootCount = 0;
        private void Start()
        {
            StartCoroutine(WaitForAttack());
        }
        [SerializeField]
        private Vector3 orientation;
        [SerializeField] float angleShootThreshold = 5f;

        private bool AimingAndShootingWithPrediction()
        {
            Quaternion? q;
            if ((q = PredictAim2(
                transform.position,
                _player.Transform.position,
                _player.Direction * _player.VelocityMagnitude,
                shootController.projectileSpeed
            )) != null)
            {
                var rotation = transform.rotation;
                q *= Quaternion.Euler(orientation);
                var zDiff = Mathf.Abs(transform.eulerAngles.z - q.Value.eulerAngles.z);
                if (angleShootThreshold > zDiff && (shootController.MakeShot() && (++shootCount >= shootCountBeforeExitState)))
                {
                    shootCount = 0;
                    currentState = State.Default;
                }
                rotation = Quaternion.RotateTowards(rotation, q.Value, movementController.rotationSpeed * Time.deltaTime);
                transform.rotation = rotation;
                return true;
            }
            return false;
        }
            
        public void Update()
        {
            //mc.Slow = Input.GetKey(KeyCode.DownArrow);
            //mc.Accel = Input.GetKey(KeyCode.UpArrow);
            //mc.Right = Input.GetKey(KeyCode.RightArrow);
            //mc.Left = Input.GetKey(KeyCode.LeftArrow);
            //mc.StrafeLeft = Input.GetKey(KeyCode.Q);
            //mc.StrafeRight = Input.GetKey(KeyCode.E);
            //sc.Shoot = Input.GetMouseButton(0);



            //in a future i'll change it
            movementController.speedUp = true; 
            movementController.rotateRight = movementController.rotateLeft = false;
            diff = 0;
            if (currentState == State.Default)
            {
                var position = transform.position;
                if (Vector3.Distance(position, target.position) <= desiredDistance) return;
                MathUtils.FindTangents(target.position, desiredDistance, position, out var pointA, out var pointB);
                var diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
                var diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, transform.forward);
                diff = Mathf.Abs(diffA) < Mathf.Abs(diffB) ? diffA : diffB;
            }

            if (currentState == State.Aiming)
            {
                AimingAndShootingWithPrediction();
                //var currentAngle = GetAngle();
                //diff = currentAngle;
                //var d = Vector3.Distance(transform.position, target.position);
                //if (d < distanceTreshold) currentState = State.Default;
            }

            if (Mathf.Abs(diff) > angleTreshhold)
                if (diff > 0)
                    movementController.rotateRight = true;
                else
                    movementController.rotateLeft = true;
        }
        [SerializeField]
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            MathUtils.FindTangents(target.position, desiredDistance, position, out var pointA, out var pointB);
            diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
            diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, Vector3.forward);
            //diffAngle = diff;

            Gizmos.DrawLine(position, pointA);
            Gizmos.DrawLine(position, pointB);
            Gizmos.DrawLine(position, position - transform.right);
            Gizmos.color = new Color(0, 0, 0, 0.3f);
            Gizmos.DrawSphere(target.position, desiredDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position - transform.right);
        }

        private void OnValidate()
        {
            if (!setRadius) return;
            desiredDistance = Vector3.Distance(transform.position, target.position);
            setRadius = false;
        }

        private IEnumerator WaitForAttack()
        {
            for (;;)
            {
                var delay = Random.Range(attackEvery.min, attackEvery.max);
                yield return new WaitForSeconds(delay);
                currentState = State.Aiming;
            }
        }

        /*
        void Foo()
        {
            
        }
        */

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private float GetAngle()
        {
            return Vector3.SignedAngle(_player.Transform.position - transform.position, -transform.right, Vector3.forward);
        }

        private enum State
        {
            Default, Aiming
        }
    }
}
