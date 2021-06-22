using System.Collections;
using Modules.Player;
using Modules.Utils;
using Modules.Utils.TagSelector;
using UnityEngine;
using static Modules.Utils.MathUtils;
using Zenject;
namespace Modules.Enemies
{
    public class NeutralPlane : MonoBehaviour
    {
        [SerializeField]
        private Mode mode = Mode.NearRadius;
        public bool setRadius;
        [SerializeField]
        private MovementController mc;
        [SerializeField]
        private Utils.MinMax stateChange;
        [SerializeField]
        private float angleTreshhold = 1f;
        private Vector3 targetPosition;
        [SerializeField] private float desiredDistance;
        [SerializeField] private float distanceTreshold = 0.1f;

        [SerializeField]
        private State currentState = State.Around;

        [SerializeField]
        private float diffB;
        [SerializeField]
        private float diffA;
        [SerializeField]
        private float diff;
        private Vector3 startPosition;
        
        private float _prevAngle = 0;
        private void Start()
        {
            StartCoroutine(SwitchStates());
            startPosition = transform.position;
            targetPosition = GetNewTarget();
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

            mc.speedUp = true;
            mc.rotateRight = mc.rotateLeft = false;
            diff = 0;
            if (currentState == State.Around)
            {
                var position = transform.position;
                if (Vector3.Distance(position, targetPosition) <= desiredDistance) return;
                FindTangents(targetPosition, desiredDistance, position, out var pointA, out var pointB);
                var diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
                var diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, transform.forward);
                diff = Mathf.Abs(diffA) < Mathf.Abs(diffB) ? diffA : diffB;
            }

            if (currentState == State.Attack)
            {
                var currentAngle = GetAngle();
                diff = currentAngle;
                var d = Vector3.Distance(transform.position, targetPosition);
                if (d < distanceTreshold) currentState = State.Around;
            }
            
            if (currentState!=State.Forward)
                if (Mathf.Abs(diff) > angleTreshhold)
                    if (diff > 0)
                        mc.rotateRight = true;
                    else
                        mc.rotateLeft = true;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(targetPosition, 0.3f);
            /*
            var position = transform.position;
            FindTangents(targetPosition, desiredDistance, position, out var pointA, out var pointB);
            diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
            diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, Vector3.forward);
            //diffAngle = diff;

            Gizmos.DrawLine(position, pointA);
            Gizmos.DrawLine(position, pointB);
            Gizmos.DrawLine(position, position - transform.right);
            Gizmos.color = new Color(0, 0, 0, 0.3f);
            Gizmos.DrawSphere(targetPosition, desiredDistance);
            */
        }

        private void OnValidate()
        {
            if (!setRadius) return;
            desiredDistance = Vector3.Distance(transform.position, targetPosition);
            setRadius = false;
        }

        private IEnumerator SwitchStates()
        {
            for (;;)
            {
                var delay = Random.Range(stateChange.min, stateChange.max);
                yield return new WaitForSeconds(delay);
                var state = Random.Range(0, 3);
                currentState = (State)(state);
                if (currentState == State.Attack)
                {
                    targetPosition = GetNewTarget();
                }
                if (currentState == State.Forward) currentState = State.Around;
            }
        }

        [SerializeField]
        private MinMax targetRange = new MinMax() {
            Min = 1,
            Max = 2
        };
        
        private Vector3 GetNewTarget()
        {
            return (mode == Mode.NearRadius ? transform.position : startPosition) + new Vector3(
                    Random.Range(targetRange.Min, targetRange.Max),
                    Random.Range(targetRange.Min, targetRange.Max),
                    0
                );

        }
        

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private float GetAngle()
        {
            return Vector3.SignedAngle(targetPosition - transform.position, -transform.right, Vector3.forward);
        }
        
        private enum Mode
        {
            NearRadius, StartRadius
        }

        private enum State
        {
            Around, Attack, Forward
        }
    }
}
