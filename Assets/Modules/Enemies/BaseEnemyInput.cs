using System.Collections;
using System.Linq;
using Modules.Player;
using Modules.Utils;
using UnityEngine;
namespace Modules.Enemies
{
    public abstract class BaseEnemyInput : MonoBehaviour 
    {
        [SerializeField]
        protected PlaneSettings.PlaneSettings _settings;
        public bool setRadius;
        [SerializeField]
        protected ShootController shootController;
        [SerializeField]
        protected MovementController movementController;
        [SerializeField]
        protected Transform[] targets;
        [SerializeField]
        protected Transform _currentTarget;
        [SerializeField]
        protected State currentState = State.Default;
        
        protected float diff;
        public IMovable AttackTarget { get; protected set; }
         
        protected float _prevAngle = 0;
        [SerializeField]
        protected int shootCountBeforeExitState = 3;
        protected int shootCount = 0;
        protected void Start()
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
        protected IEnumerator ChangeTargetsCo()
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
        protected Vector3 orientation;
        [SerializeField]
        protected float angleShootThreshold = 5f;
        
        protected float GetAngle()
        {
            return Vector3.SignedAngle(_currentTarget.transform.position - transform.position, -transform.right, Vector3.forward);
        }

        protected abstract bool Attack();
        [SerializeField]
        protected float obstacleDistance = 1;

        public virtual void Avoid()
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
        public virtual void Default()
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
                Avoid();
            }
            
            if (currentState == State.Default)
            {
                Default();
            }
            

            if (currentState == State.Attack)
            {
                    Attack();
            }
        }

        protected void OnValidate()
        {
            if (!setRadius) return;
            _settings.desiredDistance = Vector3.Distance(transform.position, _currentTarget.position);
            setRadius = false;
        }

        protected IEnumerator ChangeStateCo()
        {
            for (;;)
            {
                var delay = Random.Range(_settings.attackEvery.min, _settings.attackEvery.max);
                yield return new WaitForSeconds(delay);
                if (AttackTarget != null) currentState = State.Attack;
            }
        }

        protected static void DrawLine(Vector3 start, Vector3 dir, float distance)
        {
            var a = start;
            var b = start + dir * distance;
            Gizmos.DrawLine(a, b);
        }

        protected void OnDestroy()
        {
            StopAllCoroutines();
        }

        protected enum State
        {
            Default, Attack, Avoid
        }
        
    }
}
