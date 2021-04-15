#region
using UnityEngine;
#endregion
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour, IMovable
{
    public bool speedUp;
    public bool slowDown;
    public bool shoot;
    public bool rotateRight;
    public bool rotateLeft;
    public bool strafeLeft;
    public bool strafeRight;
    [SerializeField]
    private float strafingSpeed = 1f;
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

    public Vector3 targetVelocity;
    private Rigidbody _body;
    private float Speed { get; set; }
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        ProcessSpeed(Time.deltaTime);
        ProcessStrafing(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        targetVelocity = transform.rotation * -Vector2.right * Speed;
        _body.velocity = Vector2.MoveTowards(_body.velocity, targetVelocity, velocityChangeRate * Time.fixedDeltaTime);
        ProcessRotation();
    }
    public Vector3 Direction => _body.velocity.normalized;
    public float VelocityMagnitude => _body.velocity.magnitude;
    public Transform Transform => transform;

    private void ProcessSpeed(float dt)
    {
        if (slowDown)
            Speed += -dt * accelerationSpeed;
        else if (speedUp)
            Speed += dt * accelerationSpeed;
        else
            return;
        Speed = Mathf.Clamp(Speed, minVelocity, maxVelocity);
    }

    public void ProcessStrafing(float dt)
    {
        //if (strafeLeft || strafeRight)
        //{
        //    var v = (Vector2)_body.transform.up * (dt * strafingSpeed);
        //    if (strafeRight) v = -v;
        //    _body.AddForce(v, ForceMode2D.Impulse);
        //}
    }

    private void ProcessRotation()
    {
        if (!rotateRight && !rotateLeft) return;
        var fdt = Time.fixedDeltaTime;
        transform.Rotate(0, 0, (rotateRight ? -fdt : fdt) * rotationSpeed);

        var velocity = _body.velocity;
        var magnitude = Mathf.Max(0,_body.velocity.magnitude - fdt * rotationDeceleration);
        _body.velocity = velocity.normalized * magnitude;
    }
}

public interface IMovable
{
    Vector3 Direction { get; }
    float VelocityMagnitude { get; }
    Transform Transform { get; }
}
