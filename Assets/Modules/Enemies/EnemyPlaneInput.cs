using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

public class EnemyPlaneInput : MonoBehaviour
{
    [SerializeField]
    private MovementController mc;
    [SerializeField]
    private ShootController sc;
        
    
    [Inject(Id = "Player")]
    private IMovable _player;

    
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
        var diff = desiredAngle - angle;
        Debug.Log(diff);
        mc.Accel = true;
        mc.Right = mc.Left = false;
        if (Mathf.Abs(diff) > angleTreshhold)   {
            if (diff > 0)
            {
                mc.Right = true;
            }
            else
            {
                mc.Left = true;
            }
        }
    }
    [SerializeField]
    float desiredAngle = 90;
    [SerializeField]
    private float currentAngle;
    [SerializeField]
    private float angleTreshhold = 1f;

    [SerializeField]
    private Transform target;
    private float prevAngle = 0;

    private float clampAngle(float a)
    {
        return a < 0 ? a += 360 : a;
    }
    
    
    

    private float GetAngle()
    {
        return  Vector3.SignedAngle(_player.Transform.position-transform.position, transform.right, -Vector3.forward)  ;
    }

    private void OnDrawGizmosSelected()
    {
        if (target==null) return;
        var desiredAngle = 90;
        var dir = target.position - transform.position;
        var angle = Vector3.SignedAngle(dir, transform.right, -Vector3.forward)  ;
        if (Math.Abs(prevAngle - angle) > 0.1f)
        {
            Debug.Log("current angle = " + angle);
            prevAngle = angle;
        }
    }
    
}
