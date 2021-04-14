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
        mc.slowDown = Input.GetKey(KeyCode.DownArrow);
        mc.speedUp = Input.GetKey(KeyCode.UpArrow);
        mc.rotateRight = Input.GetKey(KeyCode.RightArrow);
        mc.rotateLeft = Input.GetKey(KeyCode.LeftArrow);
        mc.strafeLeft = Input.GetKey(KeyCode.Q);
        mc.strafeRight = Input.GetKey(KeyCode.E);
        sc.Shoot = Input.GetMouseButton(0);
    }
}
