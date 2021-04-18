using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public Vector3 sectorSize;
    private void OnDrawGizmosSelected()
    {
        var color = Gizmos.color;
        Gizmos.color = Color.green;
        OnDrawGizmos();
        Gizmos.color = color;
    }
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var halfSize = sectorSize / 2;
        var a = pos + MulAxis(new Vector3(-1, 1), halfSize);
        var b = pos + MulAxis(new Vector3(1, 1), halfSize);
        var c = pos + MulAxis(new Vector3(1, -1), halfSize);
        var d = pos + MulAxis(new Vector3(-1, -1), halfSize);
        Gizmos.DrawLine(a,b);
        Gizmos.DrawLine(b,c);
        Gizmos.DrawLine(c,d);
        Gizmos.DrawLine(d,a);
    }

    Vector3 MulAxis(Vector3 dir, Vector3 val)
    {
        return new Vector3(
            dir.x * val.x,
            dir.y * val.y
        );
    }
}
