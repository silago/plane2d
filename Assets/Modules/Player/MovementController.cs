using System;
using UnityEngine;

public class MovementController : MonoBehaviour, IMovable
{
        public bool Accel;
        public bool Slow;
        public bool Shoot;
        public bool Right;
        public bool Left;
        public bool StrafeLeft;
        public bool StrafeRight;

        Quaternion currentVelocityRotation;

        SpriteRenderer spriteRenderer;
        public Vector3 Direction => body.velocity.normalized;
        public float Speed { get; set; }
        public float VelocityMagnitude => body.velocity.magnitude;
        public Transform Transform => transform;
        [SerializeField]
        private Vector3 forward;

        [SerializeField]
        float strafingSpeed = 1f;

        [SerializeField]
        float rotationSpeed;

        [SerializeField]
        float accelerationSpeed;


        [SerializeField]
        float maxVelocity;

        [SerializeField]
        float minVelocity;

        [SerializeField]
        float rotationDecceleration;
        
        [SerializeField]
        float inertia;


        Rigidbody2D body;

        private Transform _transform;
        public MovementController(Transform transform)
        {
           Speed = minVelocity; 
        }

        void Awake() {
            body  = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public  void Update()
        {
            ProcessSpeed(Time.deltaTime);
            ProcessStrafing(Time.deltaTime);
        }

        void FixedUpdate() 
        {
            Vector2 targetVeclocity =transform.rotation * -Vector2.right * (Speed );
            body.velocity = Vector2.MoveTowards(body.velocity, targetVeclocity, inertia * Time.fixedDeltaTime);
            ProcessRotation();
        }

        void ProcessSpeed(float dt) {
            if (Slow) {
                Speed+=-dt*accelerationSpeed;
            } else if (Accel) {
                Speed+=dt*accelerationSpeed;
            } else {
                return;
            }
            Speed = Mathf.Clamp(Speed , minVelocity, maxVelocity);
        }

        public void SetSpeed()
        {
            
        }
        

        public void ProcessStrafing(float dt)
        {
            if ( StrafeLeft) {
                var v = body.transform.up * (dt * strafingSpeed);
                body.AddForce(  new Vector2(v.x, v.y), ForceMode2D.Impulse);
            } else if ( StrafeRight ) 
            {
                var v = body.transform.up * (dt * strafingSpeed);
                body.AddForce( - new Vector2(v.x, v.y), ForceMode2D.Impulse);
            }
        }

        void ProcessRotation() {
            var fdt = Time.fixedDeltaTime;
            if (Right) {
                transform.Rotate(0,0,-fdt*rotationSpeed);
            } else if (Left) {
                transform.Rotate(0,0,fdt*rotationSpeed);
            } else {
                return;
            }

            var velocity = body.velocity;
            var magnitude = velocity.magnitude;
            magnitude = magnitude - Time.fixedDeltaTime * rotationDecceleration;
            if (magnitude<0) magnitude = 0;

            body.velocity = velocity.normalized *  magnitude ; 
        }
        
        

    }

public interface IMovable
{
    Vector3 Direction { get; }
    float VelocityMagnitude { get; }
    Transform Transform { get; }
}
