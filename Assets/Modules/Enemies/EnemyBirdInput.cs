using System;
using System.Collections;
using Modules.Player;
using Modules.Utils;
using UnityEngine;
using Zenject;
using static Modules.Utils.MathUtils;
using Random = UnityEngine.Random;
namespace Modules.Enemies
{
    public class EnemyBirdInput : MonoBehaviour
    {
        public bool setRadius;
        [SerializeField]
        private MovementController mc;
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
        private void Start()
        {
            StartCoroutine(WaitForAttack());
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
            if (currentState == State.Default)
            {
                var position = transform.position;
                if (Vector3.Distance(position, target.position) <= desiredDistance) return;
                FindTangents(target.position, desiredDistance, position, out var pointA, out var pointB);
                var diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
                var diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, transform.forward);
                diff = Mathf.Abs(diffA) < Mathf.Abs(diffB) ? diffA : diffB;
            }

            if (currentState == State.Attack)
            {
                var currentAngle = GetAngle();
                diff = currentAngle;
                var d = Vector3.Distance(transform.position, target.position);
                if (d < distanceTreshold) currentState = State.Default;
            }

            if (Mathf.Abs(diff) > angleTreshhold)
                if (diff > 0)
                    mc.rotateRight = true;
                else
                    mc.rotateLeft = true;
        }
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            FindTangents(target.position, desiredDistance, position, out var pointA, out var pointB);
            diffA = Vector3.SignedAngle(pointA - (Vector2)position, -transform.right, transform.forward);
            diffB = Vector3.SignedAngle(pointB - (Vector2)position, -transform.right, Vector3.forward);
            //diffAngle = diff;

            Gizmos.DrawLine(position, pointA);
            Gizmos.DrawLine(position, pointB);
            Gizmos.DrawLine(position, position - transform.right);
            Gizmos.color = new Color(0, 0, 0, 0.3f);
            Gizmos.DrawSphere(target.position, desiredDistance);
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
                currentState = State.Attack;
            }
        }

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
            Default, Attack
        }
        
        
    }
}
