#region
using UnityEngine;
#endregion
public class MovementController : MonoBehaviour, IMovable
{
    public bool Accel;
    public bool Slow;
    public bool Shoot;
    public bool Right;
    public bool Left;
    public bool StrafeLeft;
    public bool StrafeRight;
    [SerializeField]
    private Vector3 forward;

    [SerializeField]
    private float strafingSpeed = 1f;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float accelerationSpeed;


    [SerializeField]
    private float maxVelocity;

    [SerializeField]
    private float minVelocity;

    [SerializeField]
    private float rotationDecceleration;

    [SerializeField]
    private float inertia;

    public Vector3 targetVelocity;

    private Transform _transform;


    private Rigidbody2D body;

    private Quaternion currentVelocityRotation;

    private SpriteRenderer spriteRenderer;
    public MovementController(Transform transform)
    {
        Speed = minVelocity;
    }
    public float Speed { get; set; }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        ProcessSpeed(Time.deltaTime);
        ProcessStrafing(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        targetVelocity = transform.rotation * -Vector2.right * Speed;
        body.velocity = Vector2.MoveTowards(body.velocity, targetVelocity, inertia * Time.fixedDeltaTime);
        ProcessRotation();
    }
    public Vector3 Direction => body.velocity.normalized;
    public float VelocityMagnitude => body.velocity.magnitude;
    public Transform Transform => transform;

    private void ProcessSpeed(float dt)
    {
        if (Slow)
            Speed += -dt * accelerationSpeed;
        else if (Accel)
            Speed += dt * accelerationSpeed;
        else
            return;
        Speed = Mathf.Clamp(Speed, minVelocity, maxVelocity);
    }

    public void SetSpeed()
    {

    }


    public void ProcessStrafing(float dt)
    {
        if (StrafeLeft)
        {
            var v = body.transform.up * (dt * strafingSpeed);
            body.AddForce(new Vector2(v.x, v.y), ForceMode2D.Impulse);
        }
        else if (StrafeRight)
        {
            var v = body.transform.up * (dt * strafingSpeed);
            body.AddForce(-new Vector2(v.x, v.y), ForceMode2D.Impulse);
        }
    }

    private void ProcessRotation()
    {
        var fdt = Time.fixedDeltaTime;
        if (Right)
            transform.Rotate(0, 0, -fdt * rotationSpeed);
        else if (Left)
            transform.Rotate(0, 0, fdt * rotationSpeed);
        else
            return;

        var velocity = body.velocity;
        var magnitude = velocity.magnitude;
        magnitude = magnitude - Time.fixedDeltaTime * rotationDecceleration;
        if (magnitude < 0) magnitude = 0;

        body.velocity = velocity.normalized * magnitude;
    }
}

public interface IMovable
{
    Vector3 Direction { get; }
    float VelocityMagnitude { get; }
    Transform Transform { get; }
}
