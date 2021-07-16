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
    public enum AttackType
    {
        Ram, Shoot
    }


    public class EnemyPlaneInput : MonoBehaviour, ISetTarget
    {
        [SerializeField]
        private PlaneSettings.PlaneSettings _settings;
        public bool setRadius;
        [SerializeField]
        private ShootController shootController;
        [SerializeField]
        private MovementController movementController;
        [SerializeField]
        private Transform[] targets;
        [SerializeField]
        private Transform _currentTarget;
        [SerializeField]
        private State currentState = State.Default;
        
        private float diff;
        public IMovable AttackTarget { get; private set; }
         
        private float _prevAngle = 0;
        [SerializeField]
        private int shootCountBeforeExitState = 3;
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
                yield return new WaitForSeconds(Random.Range(_settings.targetChangeInterval.Min, _settings.targetChangeInterval.Max));
                if (AttackTarget == null)
                    _currentTarget = targets[Random.Range(0, targets.Length)];
                //else _currentTarget = AttackTarget;
            }
        }

        [SerializeField]
        private Vector3 orientation;
        [SerializeField] float angleShootThreshold = 5f;
        
        private float GetAngle()
        {
            return Vector3.SignedAngle(_currentTarget.transform.position - transform.position, -transform.right, Vector3.forward);
        }

        private void OnRam()
        {
            movementController.speedUp = true; 
            
            var currentAngle = GetAngle();
            diff = currentAngle;
            var d = Vector3.Distance(transform.position, _currentTarget.position);
            if (d < 0.005) currentState = State.Default;
            
            
            if (Mathf.Abs(diff) > _settings.angleTreshhold)
                if (diff > 0)
                    movementController.rotateRight = true;
                else
                    movementController.rotateLeft = true;
        }
        private bool OnAiming()
        {
            var distance = Vector3.Distance(transform.position, AttackTarget.Transform.position);
            if ( distance > _settings.surrenderDistance)
            {
                SetAttackTarget(null);
                currentState = State.Default;
                return false;
            }
            var distanceDiff = (distance -_settings. desiredDistance);
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
            movementController.slowDown =  movementController.speedUp = false;
            RaycastHit hit;
            if (!(
                    Physics.Raycast(transform.position, -transform.right, out hit, obstacleDistance) ||
                    Physics.Raycast(transform.position - transform.up *_settings. sideRaycastOffset, -transform.right - transform.up *_settings. sideRaycastOffset, out hit, obstacleDistance) ||
                    Physics.Raycast(transform.position + transform.up *_settings. sideRaycastOffset, -transform.right + transform.up *_settings. sideRaycastOffset, out hit, obstacleDistance)
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
            if (Vector3.Distance(position, _currentTarget.position) <=_settings. desiredDistance) return;
            MathUtils.FindTangents(_currentTarget.position,_settings. desiredDistance, position, out var pointA, out var pointB);
            var right = transform.right;
            var forward = transform.forward;
            var diffA = Vector3.SignedAngle(pointA - (Vector2)position, -right, forward);
            var diffB = Vector3.SignedAngle(pointB - (Vector2)position, -right, forward);
            diff = Mathf.Abs(diffA) < Mathf.Abs(diffB) ? diffA : diffB;
            
            if (Mathf.Abs(diff) >_settings. angleTreshhold)
                if (diff > 0)
                    movementController.rotateRight = true;
                else
                    movementController.rotateLeft = true;
        }

        protected LayerMask AvoidingObstacles;
        public void Update()
        {

            movementController.rotateRight = movementController.rotateLeft = false;
            movementController.speedUp = movementController.slowDown = movementController.rotateLeft = movementController.rotateRight = false;
            var transform = this.transform;
            var ignoreProjectile = AvoidingObstacles;//~LayerMask.GetMask("Projectile");
                RaycastHit hit;
            if (
                currentState!=State.Avoid && (
                Physics.Raycast(transform.position, -transform.right , out hit, obstacleDistance, ignoreProjectile) ||
                Physics.Raycast(transform.position-transform.up*_settings.sideRaycastOffset, -transform.right - transform.up *_settings.sideRaycastOffset , out hit, obstacleDistance,ignoreProjectile) ||
                Physics.Raycast(transform.position+transform.up*_settings.sideRaycastOffset, -transform.right + transform.up * _settings.sideRaycastOffset, out hit, obstacleDistance,ignoreProjectile)
                ))
            {
                currentState = State.Avoid;
                return;
            }
            if (currentState == State.Avoid)
            {
                OnAvoid();
            }
            
            if (currentState == State.Default)
            {
                OnDefault();
            }
            
            if (currentState == State.Ram)
            {
                    OnRam();
            }

            if (currentState == State.Aiming)
            {
                    OnAiming();
            }
        }

        private void OnValidate()
        {
            if (!setRadius) return;
            _settings.desiredDistance = Vector3.Distance(transform.position, _currentTarget.position);
            setRadius = false;
        }

        private IEnumerator ChangeStateCo()
        {
            for (;;)
            {
                var delay = Random.Range(_settings.attackEvery.min, _settings.attackEvery.max);
                yield return new WaitForSeconds(delay);
                if (AttackTarget != null) currentState = _settings.attackType == AttackType.Shoot ? State.Aiming : State.Ram;
            }
        }
        
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    var t = transform;
        //    var up = t.up;
        //    var right = t.right;
        //    var position = transform.position;
        //    DrawLine(position, -right, obstacleDistance);
        //    DrawLine(position - up * sideRaycastOffset, -right - up * sideRaycastOffset, obstacleDistance);
        //    DrawLine(position + up * sideRaycastOffset, -right + up * sideRaycastOffset, obstacleDistance);
        //    
        //    Gizmos.color =  new Color(1, 1, 1, 0.2f);
        //    Gizmos.DrawSphere(transform.position, surrenderDistance);
        //}

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
            Default, Aiming, Avoid, Ram
        }
    }
}

public interface ISetTarget
{
    public void SetAttackTarget(IMovable item);
}
