#region
using UnityEngine;
#endregion
namespace Plane
{
    public class Plane : MonoBehaviour
    {
        #if UNITY_EDITOR
        public bool UpdateRotation;
        #endif
        [SerializeField]
        private float velocity = 1f;

        [SerializeField]
        private float strafingSpeed = 1f;

        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private float accelerationSpeed;

        [SerializeField]
        private Sprite[] sprites;

        [SerializeField]
        private float maxVelocity;

        [SerializeField]
        private float minVelocity;

        [SerializeField]
        private float rotationDecceleration;

        [SerializeField]
        private float inertia;


        private Rigidbody2D body;

        private Quaternion currentVelocityRotation;

        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        // Start is called before the first frame update
        private void Start()
        {
            spriteRenderer.sprite = sprites[0];
            velocity = minVelocity;
        }

        // Update is called once per frame
        private void Update()
        {
            ProcessSpeed(Time.deltaTime);
            ProcessStrafing(Time.deltaTime);
            UpdateRepresentation();
        }

        private void FixedUpdate()
        {
            Vector2 v = transform.rotation * Vector2.right * velocity;
            body.velocity = Vector2.MoveTowards(body.velocity, v, inertia);
            ProcessRotation();
        }

        private void OnValidate()
        {
            UpdateRotation = false;
            var currentRotation = transform.rotation.eulerAngles.z;
            var currentSpriteIndex = (int)(currentRotation / 360 * sprites.Length);
            Debug.Log(currentSpriteIndex);
            Debug.Log(currentRotation);
            GetComponent<SpriteRenderer>().sprite = sprites[currentSpriteIndex];
        }

        private void foo()
        {


        }

        private void ProcessSpeed(float dt)
        {
            if (Input.GetKey(KeyCode.DownArrow))
                velocity += -dt * accelerationSpeed;
            else if (Input.GetKey(KeyCode.UpArrow))
                velocity += dt * accelerationSpeed;
            else
                return;

            velocity = Mathf.Clamp(velocity, minVelocity, maxVelocity);
        }

        private void ProcessStrafing(float dt)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var v = body.transform.up * dt * strafingSpeed;
                body.AddForce(new Vector2(v.x, v.y), ForceMode2D.Impulse);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                var v = body.transform.up * dt * strafingSpeed;
                body.AddForce(-new Vector2(v.x, v.y), ForceMode2D.Impulse);
            }
        }

        private void ProcessRotation()
        {
            var fdt = Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(0, 0, -fdt * rotationSpeed);
            else if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(0, 0, fdt * rotationSpeed);
            else
                return;

            /*
            velocity-=rotationDecceleration * Time.fixedDeltaTime;
            velocity = Mathf.Clamp(velocity , minVelocity, maxVelocity);
            */
            var v = body.velocity;
            var m = v.magnitude;
            m = m - Time.fixedDeltaTime * rotationDecceleration;
            if (m < 0) m = 0;

            body.velocity = v.normalized * m;
            UpdateRepresentation();
        }

        private void UpdateRepresentation()
        {
            var currentRotation = transform.rotation.eulerAngles.z;
            var currentSpriteIndex = (int)(currentRotation / 360 * sprites.Length);
            spriteRenderer.sprite = sprites[currentSpriteIndex];
        }
    }
}
