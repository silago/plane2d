#region
using Events;
using Modules.Player;
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
        if (mc.Speed <= mc.minVelocity && mc.speedUp && EngineSoundState==null)
        {
            StartEngine();
        } else if (mc.Speed > mc.minVelocity && mc.slowDown && EngineSoundState!=null)
        {
            StopEngine();
        }
        
        mc.rotateRight = Input.GetKey(KeyCode.RightArrow);
        mc.rotateLeft = Input.GetKey(KeyCode.LeftArrow);
        //mc.strafeLeft = Input.GetKey(KeyCode.Q);
        //mc.strafeRight = Input.GetKey(KeyCode.E);
        sc.ShooterSpeed = mc.VelocityMagnitude;
        sc.Shoot = Input.GetMouseButton(2);
    }
    private PlaySoundLong EngineSoundState = null;
    
    void StartEngine()
    {
        this.SendEvent(new PlaySoundOnce("Common.EngineStart") {
            Callback = () =>
            {
                var engineSound = new PlaySoundLong("Common.Engine");
                EngineSoundState = engineSound;
                this.SendEvent(EngineSoundState);
            }
        });
    }


    void StopEngine()
    {
        if (EngineSoundState != null)
        {
            EngineSoundState.Stop();
            EngineSoundState = null;
        }
        
    }
}
