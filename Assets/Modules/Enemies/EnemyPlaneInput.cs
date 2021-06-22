using System;
using System.Collections;
using System.Linq;
using Modules.Player;
using Modules.Utils;
using UnityEngine;
using static Modules.Utils.Vector3Extensions;
using Random = UnityEngine.Random;

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
        private Transform[] targets;
        [SerializeField]
        private Transform _currentTarget;
        [SerializeField] private float desiredDistance;
        [SerializeField] private float distanceTreshold = 0.1f;

        [SerializeField]
        private State currentState = State.Default;
        [SerializeField]
        private float sideRaycastOffset;
        [SerializeField]
        private float surrenderDistance = 7;

        [SerializeField]
        private float diff;
        //[Inject(Id = "Player")]
        public IMovable AttackTarget { get; private set; }
         
        private float _prevAngle = 0;
        [SerializeField]
        private int shootCountBeforeExitState = 3;
        [SerializeField]
        private MinMax targetChangeInterval;
        private int shootCount = 0;
        private void Start()
        {
            StartCoroutine(ChangeStateCo());
            StartCoroutine(ChangeTargetsCo());
            _currentTarget = targets.First();
        }

        public void SetAttackTarget(IMovable item)
        {
            AttackTarget = item;
            if (item != null)
            {
                _currentTarget = item.Transform;
                Debug.Log("Set target");
                    
            }
            else
            {
                _currentTarget = targets[Random.Range(0, targets.Length)];
                currentState = State.Default;
            }
        }
        private IEnumerator ChangeTargetsCo()
        {
            for (;;)
            {
                yield return new WaitForSeconds(Random.Range(targetChangeInterval.Min, targetChangeInterval.Max));
                if (AttackTarget!=null)
                    _currentTarget = targets[Random.Range(0, targets.Length)];
            }
        }

        [SerializeField]
        private Vector3 orientation;
        [SerializeField] float angleShootThreshold = 5f;

        private bool OnAiming()
        {

            var distance = Vector3.Distance(transform.position, AttackTarget.Transform.position);
            if ( distance > surrenderDistance)
            {
                SetAttackTarget(null);
                currentState = State.Default;
                return false;
            }
            var distanceDiff = (distance - desiredDistance);
            movementController.speedUp = movementController.slowDown = false;
            if (distance > 0.2)
            {
                movementController.slowDown = true;
            }
            else if (distance < -.2 )
            {
                movementController.speedUp = true;
            }
            
            Quaternion? q;
            if ((q = PredictAim2(
                transform.position,
                AttackTarget.Transform.position,
                AttackTarget.Direction * AttackTarget.VelocityMagnitude,
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
        [SerializeField]
        private float obstacleDistance = 1;

        public void OnAvoid()
        {
            //if (currentState == State.Aiming) currentState = State.Default;
            //his.DoWithDelay(3, () =>
            //{
            //    if (AttackTarget != null) currentState = State.Aiming;
            //});
            //movementController.slowDown = true;
            movementController.slowDown =  movementController.speedUp = false;
            
            
            RaycastHit hit;
            if (!(
                    Physics.Raycast(transform.position, -transform.right, out hit, obstacleDistance) ||
                    Physics.Raycast(transform.position - transform.up * sideRaycastOffset, -transform.right - transform.up * sideRaycastOffset, out hit, obstacleDistance) ||
                    Physics.Raycast(transform.position + transform.up * sideRaycastOffset, -transform.right + transform.up * sideRaycastOffset, out hit, obstacleDistance)
                ))
            {
                currentState = State.Default;
                return;
            }
            RaycastHit RaycastHitDown;
            RaycastHit RaycastHitUp;
            if (!Physics.Raycast(transform.position, (-transform.right - transform.up), out RaycastHitDown, obstacleDistance))
            {
                movementController.rotateLeft = true;
            } else if (!Physics.Raycast(transform.position, (-transform.right + transform.up), out RaycastHitUp,obstacleDistance))
            {
                movementController.rotateRight = true;
            } else if (RaycastHitDown.distance > RaycastHitUp.distance)
            {
                movementController.rotateLeft = true;
            }
            else
            {
                movementController.rotateRight = true;
            }
        }

        // patrol
        public void OnDefault()
        {
            movementController.speedUp = true; 
            var position = transform.position;
            if (Vector3.Distance(position, _currentTarget.position) <= desiredDistance) return;
            MathUtils.FindTangents(_currentTarget.position, desiredDistance, position, out var pointA, out var pointB);
            var right = transform.right;
            var forward = transform.forward;
            var diffA = Vector3.SignedAngle(pointA - (Vector2)position, -right, forward);
            var diffB = Vector3.SignedAngle(pointB - (Vector2)position, -right, forward);
            diff = Mathf.Abs(diffA) < Mathf.Abs(diffB) ? diffA : diffB;
            
            if (Mathf.Abs(diff) > angleTreshhold)
                if (diff > 0)
                    movementController.rotateRight = true;
                else
                    movementController.rotateLeft = true;
        }

        public void Update()
        {

            movementController.rotateRight = movementController.rotateLeft = false;
            movementController.speedUp = movementController.slowDown = movementController.rotateLeft = movementController.rotateRight = false;
            var transform = this.transform;
            var ignoreProjectile = ~LayerMask.GetMask("Projectile");
                RaycastHit hit;
            if (
                currentState!=State.Avoid && (
                Physics.Raycast(transform.position, -transform.right , out hit, obstacleDistance, ignoreProjectile) ||
                Physics.Raycast(transform.position-transform.up*sideRaycastOffset, -transform.right - transform.up *sideRaycastOffset , out hit, obstacleDistance,ignoreProjectile) ||
                Physics.Raycast(transform.position+transform.up*sideRaycastOffset, -transform.right + transform.up * sideRaycastOffset, out hit, obstacleDistance,ignoreProjectile)
                ))
            {
                currentState = State.Avoid;
                Debug.Log("Stay Away Of Obstacels");
                return;
            }
            if (currentState == State.Avoid)
            {
                OnAvoid();
            }
            // fly around target
            if (currentState == State.Default)
            {
                OnDefault();
            }

            if (currentState == State.Aiming)
            {
                //else
                //{
                    OnAiming();
               // }
            }

        }

        private void OnValidate()
        {
            if (!setRadius) return;
            desiredDistance = Vector3.Distance(transform.position, _currentTarget.position);
            setRadius = false;
        }

        private IEnumerator ChangeStateCo()
        {
            for (;;)
            {
                var delay = Random.Range(attackEvery.min, attackEvery.max);
                yield return new WaitForSeconds(delay);
                if (AttackTarget != null)
                {
                    Debug.Log("set state to aiming");
                    currentState = State.Aiming;
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var t = transform;
            var up = t.up;
            var right = t.right;
            var position = transform.position;
            DrawLine(position, -right, obstacleDistance);
            DrawLine(position - up * sideRaycastOffset, -right - up * sideRaycastOffset, obstacleDistance);
            DrawLine(position + up * sideRaycastOffset, -right + up * sideRaycastOffset, obstacleDistance);
            
            //Gizmos.color =  new Color(1, 1, 1, 0.2f);
            //Gizmos.DrawSphere(transform.position, surrenderDistance);
        }

        private static void DrawLine(Vector3 start, Vector3 dir, float distance)
        {
            var a = start;
            var b = start + dir * distance;
            Gizmos.DrawLine(a, b);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private enum State
        {
            Default, Aiming, Avoid
        }
    }
}
