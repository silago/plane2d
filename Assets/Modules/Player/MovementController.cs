#region
using UnityEngine;
using Zenject;
#endregion
namespace Modules.Player
{

    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour, IMovable
    {
        public bool speedUp;
        public bool slowDown;
        public bool rotateRight;
        public bool rotateLeft;
        //todo: move to settings (scriptrable)
        [SerializeField]
        public float rotationSpeed;
        [SerializeField]
        private float accelerationSpeed;
        [SerializeField]
        private float maxVelocity;
        [SerializeField]
        private float minVelocity;
        [SerializeField]
        private float rotationDeceleration;
        [SerializeField]
        private float velocityChangeRate;
        private Rigidbody _body;
        private float Speed { get; set; }
        public Vector3 Direction => _body.velocity.normalized;
        public float VelocityMagnitude
        {
            get
            {
                return _body.velocity.magnitude;
            }
        }
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            ProcessSpeed(Time.deltaTime);
        }
        protected virtual void FixedUpdate()
        {
            TargetVelocity = transform.rotation * -Vector2.right * Speed;
            _body.velocity = Vector2.MoveTowards(_body.velocity, TargetVelocity, velocityChangeRate * Time.fixedDeltaTime);
            ProcessRotation();
        }
        protected virtual Vector3 TargetVelocity
        {
            get;
            set;
        }

        public Transform Transform => transform;
        private void ProcessSpeed(float dt)
        {
            if (slowDown) Speed += -dt * accelerationSpeed;
            else if (speedUp) Speed += dt * accelerationSpeed;
            else return;
            Speed = Mathf.Clamp(Speed, minVelocity, maxVelocity);
        }
        private void ProcessRotation()
        {
            if (!rotateRight && !rotateLeft) return;
            var fdt = Time.fixedDeltaTime;
            transform.Rotate(0, 0, (rotateRight ? -fdt : fdt) * rotationSpeed);

            var velocity = _body.velocity;
            var magnitude = Mathf.Max(0, _body.velocity.magnitude - fdt * rotationDeceleration);
            _body.velocity = velocity.normalized * magnitude;
        }
    }

}
