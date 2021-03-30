#region
using UnityEngine;
#endregion
public class PlayerInput : MonoBehaviour
{

    [SerializeField]
    private MovementController mc;
    [SerializeField]
    private ShootController sc;

    public void Update()
    {
        mc.Slow = Input.GetKey(KeyCode.DownArrow);
        mc.Accel = Input.GetKey(KeyCode.UpArrow);
        mc.Right = Input.GetKey(KeyCode.RightArrow);
        mc.Left = Input.GetKey(KeyCode.LeftArrow);
        mc.StrafeLeft = Input.GetKey(KeyCode.Q);
        mc.StrafeRight = Input.GetKey(KeyCode.E);
        sc.Shoot = Input.GetMouseButton(0);
    }
}
