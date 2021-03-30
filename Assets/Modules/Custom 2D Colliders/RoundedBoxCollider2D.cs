/*
The MIT License (MIT)

Copyright (c) 2016 GuyQuad

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

You can contact me by email at guyquad27@gmail.com or on Reddit at https://www.reddit.com/user/GuyQuad
*/


#if UNITY_EDITOR
#region
using System.Collections.Generic;
using UnityEngine;
#endregion
[AddComponentMenu("Physics 2D/Rounded Box Collider 2D")]
[RequireComponent(typeof(PolygonCollider2D))]
public class RoundedBoxCollider2D : MonoBehaviour
{

    [Range(10, 90)]
    public int smoothness = 15;

    [Range(.2f, 25)] [HideInInspector]
    public float height = 2;

    [Range(.2f, 25)] [HideInInspector]
    public float width = 2;

    [HideInInspector]
    public float radius = .5f, wt, wb;

    [Range(0.05f, .95f)]
    public float trapezoid = .5f;

    [HideInInspector]
    public Vector2 offset, center1, center2, center3, center4;

    [HideInInspector]
    public bool advanced;

    private float ang;
    private List<Vector2> points;

    public Vector2[] getPoints()
    {
        points = new List<Vector2>();
        points.Clear();

        wt = width + width - (width + width) * trapezoid; // width top
        wb = (width + width) * trapezoid; // width bottom

        // vertices
        var vTR = new Vector2(wt / 2f, +(height / 2f)); // top right vertex
        var vTL = new Vector2(wt / -2f, +(height / 2f)); // top left vertex
        var vBL = new Vector2(wb / -2f, -(height / 2f)); // bottom left vertex
        var vBR = new Vector2(wb / 2f, -(height / 2f)); // bottom right vertex

        var dir = vBL - vTL;
        var hypAngleTL = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // hypertenuse angle top left corner
        hypAngleTL = (hypAngleTL + 360) % 360; // get it between 0-360 range
        hypAngleTL = 360 - hypAngleTL; // use the inside angle
        hypAngleTL /= 2f; // got our adjacent angle
        ///        
        ///    adj TL (Top Left)
        ///    _____
        ///    \    |
        ///     \   |
        ///   h  \  | opp = radius
        ///       \ |
        ///        \|
        /// 
        var adjTL = radius / Mathf.Tan(hypAngleTL * Mathf.Deg2Rad);
        center1 = new Vector3(vTR.x - adjTL, vTR.y - radius, 0);
        center2 = new Vector3(vTL.x + adjTL, vTL.y - radius, 0);



        var hypAngleBL = (180 - hypAngleTL * 2f) / 2f; // hypertenuse angle bottom left corner
        /// 
        ///        /|
        ///       / |
        ///   h  /  | opp = radius
        ///     /   |
        ///    /____|
        /// 
        ///     adj BL (Bottom Left)
        /// 
        var adjBL = radius / Mathf.Tan(hypAngleBL * Mathf.Deg2Rad);
        center3 = new Vector3(vBL.x + adjBL, vBL.y + radius, 0);
        center4 = new Vector3(vBR.x - adjBL, vBR.y + radius, 0);


        // prevent overlapping of the corners
        center1.x = Mathf.Max(0, center1.x);
        center2.x = Mathf.Min(0, center2.x);
        center3.x = Mathf.Min(0, center3.x);
        center4.x = Mathf.Max(0, center4.x);


        // curveTOP angles
        var tmpDir = vBR - vTR;
        var tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;
        var x = vTR.x + adjTL * Mathf.Cos(tmpAng * Mathf.Deg2Rad);
        var y = vTR.y + adjTL * Mathf.Sin(tmpAng * Mathf.Deg2Rad);
        var startPos = new Vector2(x, y);

        var canPlot = Vector2.Distance(startPos, center1) >= radius * .85f ? true : false;
        if (!canPlot) return null;

        tmpDir = startPos - center1;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;

        var t = tmpAng > 180 ? tmpAng - 360 : tmpAng;

        ang = tmpAng;
        var totalAngle = t < 0 ? 90f - t : 90f - tmpAng;
        calcPoints(center1, totalAngle);
        calcPoints(center2, totalAngle);



        // curveBottom angles
        tmpDir = vTL - vBL;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;
        x = vBL.x + adjBL * Mathf.Cos(tmpAng * Mathf.Deg2Rad);
        y = vBL.y + adjBL * Mathf.Sin(tmpAng * Mathf.Deg2Rad);
        startPos = new Vector2(x, y);

        canPlot = Vector2.Distance(startPos, center3) >= radius * .9f ? true : false;
        if (!canPlot) return null;

        tmpDir = startPos - center3;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;

        ang = tmpAng;
        totalAngle = 270 - tmpAng;
        calcPoints(center3, totalAngle);
        calcPoints(center4, totalAngle);

        return points.ToArray();
    }



    private void calcPoints(Vector2 ctr, float totAngle)
    {
        for (var i = 0; i <= smoothness; i++)
        {
            var a = ang * Mathf.Deg2Rad;
            var x = ctr.x - offset.x + radius * Mathf.Cos(a);
            var y = ctr.y - offset.y + radius * Mathf.Sin(a);

            points.Add(new Vector2(x, y));
            ang += totAngle / smoothness;
        }

        ang -= 90f / smoothness;
    }
}
#endif
