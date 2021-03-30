#region
using System.Collections.Generic;
using System.Linq;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;
#endregion
public class OrbitCollider : MonoBehaviour
{
    [GetComponent]
    [SerializeField] private EdgeCollider2D _collider;

    [SerializeField]
    private int interval = 1;

    [SerializeField] private float radius;
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnValidate()
    {

        if (interval == 0) interval = 1;
        var points = new List<Vector2>();
        if (_collider == null) return;
        for (var i = 0; i < 360; i += interval)
        {
            var t = i * Mathf.Deg2Rad;
            var x = radius * Mathf.Sin(t);
            var y = radius * Mathf.Cos(t);
            points.Add(new Vector2(x, y));
        }
        points.Add(points.First());
        _collider.points = points.ToArray();
    }
}
