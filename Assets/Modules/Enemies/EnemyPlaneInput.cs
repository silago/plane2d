#region
using System;
using System.Collections;
using UnityEngine;
using Zenject;
#endregion
public class EnemyPlaneInput : MonoBehaviour
{
    [SerializeField]
    private MovementController mc;
    [SerializeField]
    private ShootController sc;

    [SerializeField]
    private float diff;

    [SerializeField]
    private float attackEvery = 3f;






    [SerializeField]
    private float desiredAngle = 90;
    [SerializeField]
    private float currentAngle;
    [SerializeField]
    private float angleTreshhold = 1f;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private float minDistance;

    [SerializeField] private float desiredDistance;
    [SerializeField] private float distanceTreshold = 0.1f;

    [SerializeField]
    private State currentState = State.Default;
    [SerializeField]
    private float diffAngle;
    [SerializeField]
    private float tAngle;


    [Inject(Id = "Player")]
    private IMovable _player;
    private float prevAngle = 0;

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


        var angle = GetAngle();
        currentAngle = angle;
        mc.Accel = true;
        mc.Right = mc.Left = false;

        float diff = 0;
        if (currentState == State.Default)
        {
            if (Vector3.Distance(transform.position, target.position) <= desiredDistance) return;
            FindTangents(target.position, desiredDistance, transform.position, out var pointA, out var PointB);
            diff = Vector3.Angle(pointA - (Vector2)transform.position, -transform.right);
        }

        if (currentState == State.Attack)
        {
            var currentAngle = GetAngle();
            diff = currentAngle;
            if (Vector3.Distance(transform.position, target.position) < 0.3f) currentState = State.Default;
        }

        diffAngle = diff;
        if (Mathf.Abs(diff) > angleTreshhold)
        {
            if (diff > 0)
                mc.Right = true;
            else
                mc.Left = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        FindTangents(target.position, desiredDistance, transform.position, out var pointA, out var pointB);
        var diff = Vector3.Angle(pointA - (Vector2)transform.position, -transform.right);
        //diffAngle = diff;

        Gizmos.DrawLine(transform.position, pointA);
        Gizmos.DrawLine(transform.position, pointA);
        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        Gizmos.color = new Color(0, 0, 0, 0.3f);
        Gizmos.DrawSphere(target.position, desiredDistance);
    }

    private IEnumerator WaitForAttack()
    {
        for (;;)
        {
            yield return new WaitForSeconds(attackEvery);
            currentState = State.Attack;
        }
    }

    private float clampAngle(float a)
    {
        return a < 0 ? a += 360 : a;
    }


    private float GetAngle()
    {
        return Vector3.SignedAngle(_player.Transform.position - transform.position, transform.right, -Vector3.forward);
    }

    public Vector2 GetTangentPointCoordinate(Vector2 position, Vector2 centerPosition, float rad)
    {
        var X = position.x;
        var Y = position.y;
        var Xo = centerPosition.x;
        var Yo = centerPosition.y;
        var dx = X - Xo;
        var dy = Y - Yo;
        var L = Math.Sqrt(dx * dx + dy * dy);
        var itg = rad / L;
        var jtg = Math.Sqrt(1 - itg * itg);
        var Xtg = -itg * dx * itg + itg * dy * jtg;
        var Ytg = -itg * dx * jtg - itg * dy * itg;
        return centerPosition + new Vector2((float)Xtg, (float)Ytg);
    }

    private int FindCircleCircleIntersections(
        float cx0, float cy0, float radius0,
        float cx1, float cy1, float radius1,
        out Vector2 intersection1, out Vector2 intersection2)
    {
        // Find the distance between the centers.
        var dx = cx0 - cx1;
        var dy = cy0 - cy1;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        // See how many solutions there are.
        if (dist > radius0 + radius1)
        {
            // No solutions, the circles are too far apart.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        if (dist < Math.Abs(radius0 - radius1))
        {
            // No solutions, one circle contains the other.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        if (dist == 0 && radius0 == radius1)
        {
            // No solutions, the circles coincide.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        // Find a and h.
        var a = (radius0 * radius0 -
            radius1 * radius1 + dist * dist) / (2 * dist);
        var h = Math.Sqrt(radius0 * radius0 - a * a);

        // Find P2.
        var cx2 = cx0 + a * (cx1 - cx0) / dist;
        var cy2 = cy0 + a * (cy1 - cy0) / dist;

        // Get the points P3.
        intersection1 = new Vector2(
            (float)(cx2 + h * (cy1 - cy0) / dist),
            (float)(cy2 - h * (cx1 - cx0) / dist));
        intersection2 = new Vector2(
            (float)(cx2 - h * (cy1 - cy0) / dist),
            (float)(cy2 + h * (cx1 - cx0) / dist));

        // See if we have 1 or 2 solutions.
        if (dist == radius0 + radius1) return 1;
        return 2;
    }

    private bool FindTangents(Vector2 center, float radius,
        Vector2 external_point, out Vector2 pt1, out Vector2 pt2)
    {
        // Find the distance squared from the
        // external point to the circle's center.
        double dx = center.x - external_point.x;
        double dy = center.y - external_point.y;
        var D_squared = dx * dx + dy * dy;
        if (D_squared < radius * radius)
        {
            pt1 = new Vector2(-1, -1);
            pt2 = new Vector2(-1, -1);
            return false;
        }

        // Find the distance from the external point
        // to the tangent points.
        var L = Math.Sqrt(D_squared - radius * radius);

        // Find the points of intersection between
        // the original circle and the circle with
        // center external_point and radius dist.
        FindCircleCircleIntersections(
            center.x, center.y, radius,
            external_point.x, external_point.y, (float)L,
            out pt1, out pt2);

        return true;
    }


    private enum State
    {
        Default, Attack
    }
}
