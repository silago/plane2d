using System;
using UnityEngine;

    public class MovementController : MonoBehaviour
    {

        Quaternion currentVelocityRotation;

        SpriteRenderer spriteRenderer;
        [SerializeField]
        float velocity = 1f;

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
        void Awake() {
            body  = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
            // Start is called before the first frame update
        void Start()
        {
           velocity = minVelocity; 
        }

        // Update is called once per frame
        void Update()
        {
            ProcessSpeed(Time.deltaTime);
            ProcessStrafing(Time.deltaTime);
        }

        void foo()
        {
            

        }

        void FixedUpdate() 
        {
            Vector2 v =transform.rotation * Vector2.right * (velocity );
            body.velocity = Vector2.MoveTowards(body.velocity, v, inertia);
            ProcessRotation();
        }

        void ProcessSpeed(float dt) {
            if (Input.GetKey(KeyCode.DownArrow)) {
                velocity+=-dt*accelerationSpeed;
            } else if (Input.GetKey(KeyCode.UpArrow)) {
                velocity+=dt*accelerationSpeed;
            } else {
                return;
            }

            velocity = Mathf.Clamp(velocity , minVelocity, maxVelocity);
        }

        void ProcessStrafing(float dt)
        {
            if ( Input.GetKeyDown(KeyCode.Q)) {
                var v = body.transform.up * dt * strafingSpeed;
                body.AddForce(  new Vector2(v.x, v.y), ForceMode2D.Impulse);
            } else if ( Input.GetKeyDown(KeyCode.E)) 
            {
                var v = body.transform.up * dt * strafingSpeed;
                body.AddForce( - new Vector2(v.x, v.y), ForceMode2D.Impulse);
            }
        }

        void ProcessRotation() {
            var fdt = Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Rotate(0,0,-fdt*rotationSpeed);
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Rotate(0,0,fdt*rotationSpeed);
            } else {
                return;
            }

            /*
            velocity-=rotationDecceleration * Time.fixedDeltaTime;
            velocity = Mathf.Clamp(velocity , minVelocity, maxVelocity);
            */
            var v = body.velocity;
            var m = v.magnitude;
            m = m - Time.fixedDeltaTime * rotationDecceleration;
            if (m<0) m = 0;

            body.velocity = v.normalized *  m ; 
        }

    }
