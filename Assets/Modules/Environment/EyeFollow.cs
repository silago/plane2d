using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.Utils;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;
using Zenject;

public class EyeFollow : MonoBehaviour
{
    private Transform _player;
    [SerializeField]
    private Vector3 bounds;
    [SerializeField]
    private Vector3 boundsRotation;
    private Vector3 _rotatedBounds;
    
    [SerializeField]
    private Transform center;

    [SerializeField]
    public bool UpdateInEditor;
    [SerializeField]
    public bool DrawAlways;

    private bool Active { get; set; } = false;

    #if UNITY_EDITOR
    public Transform EditorPlayerCheck;
    public bool Check;
    [SerializeField]
    [Range(-1, 1)]
    private float Offset = 0;
    private void OnValidate()
    {
        if (EditorPlayerCheck != null && Check == true)
        {
            _player = EditorPlayerCheck;
            CalcBounds();
            Follow();
            Check = true;
        }
    }
    #endif

    void CalcBounds()
    {
            _rotatedBounds = (Quaternion.Euler(boundsRotation) * new Vector3(bounds.x, bounds.y, 0));

    }


    public void Activate(Transform transform)
    {
        _player = transform;
        Active = true;
    }
    
    
    public void Deactivate()
    {
        Active = false;
    }
    
    

    [SerializeField]
    private float Radius;

    private void Start()
    {
        var position = center.position;
        CalcBounds();
    }

    void Follow()
    {
        transform.position = GetPosition(center, _player.position, bounds, Quaternion.Euler(boundsRotation) ); //center.position + dir;
    }
    

    private void Update()
    {
        if (Active) Follow();
    }

    private void OnDrawGizmos()
    {
        if (DrawAlways)
        {
            OnDrawGizmosSelected();
        }
        if (UpdateInEditor)
        {
            Follow();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var position = center.position;
        Gizmos.DrawSphere(position, 0.3f);
        
        var rot = Quaternion.Euler(boundsRotation);
        var points = new List<Vector3>();
        for (var i = 0; i < 360; i++)
        {
            
            var a =  i * Mathf.Deg2Rad;
            var x = bounds.x * Mathf.Cos(a);
            var y = bounds.y * Mathf.Sin(a);
            points.Add(center.position +  rot * new Vector3(x,y));
        }
        
        
        
        
        
        var prev = points.First();
        foreach (var next in points.Skip(1))
        {
            Gizmos.DrawLine(prev,next);
            prev = next;
        }


        if (_player != null)
        {
            
            var player = _player.transform.position;
            var cpos = center.position;
            var dir = player - center.transform.position;
            var right = cpos + new Vector3(bounds.x, 0, 0);
            var top = cpos + new Vector3( 0, bounds.y, 0);
            var pos = GetPosition(center, player, bounds, rot);
            Gizmos.DrawLine(cpos, pos);
        }
    }
    

    public Vector3 GetPosition(Transform centerTransform, Vector3 playerPosition, Vector3 viewBounds, Quaternion rotation)
    {
            var dir = playerPosition - centerTransform.transform.position;
            var a = Vector3.SignedAngle(dir, rotation*centerTransform.right, -Vector3.forward)  * Mathf.Deg2Rad;
            var x = viewBounds.x * Mathf.Cos(a);
            var y = viewBounds.y * Mathf.Sin(a);
            return centerTransform.position + rotation * new Vector3(x, y);
    }
    

}

