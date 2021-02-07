using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;

public class OrbitCollider : MonoBehaviour
{
    [GetComponent]
    [SerializeField] private EdgeCollider2D _collider;

    [SerializeField]
    int interval = 1;

    [SerializeField] private float radius; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        
        if (interval == 0) interval = 1;
        List<Vector2> points = new List<Vector2>();
        if (_collider == null) return;
        for (var i = 0; i < 360; i += interval)
        {
            var t = i * Mathf.Deg2Rad;
            var x = radius * Mathf.Sin(t);
            var y = radius * Mathf.Cos(t);
            points.Add(new Vector2(x,y));
        }
        points.Add(points.First());
        _collider.points = points.ToArray(); 
    }
}
