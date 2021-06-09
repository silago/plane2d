using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

/* Original code stolen from http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_IcoSphere
   Heavily modified to fit my needs.
*/

static class IcoSphere
{
    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    // return index of point in the middle of p1 and p2
    private static int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }
    
    

    public static Mesh Create(int recursionLevel, bool flatShaded, Func<Vector3, float> heightFunc, float normalsAngle = 0, float trashHold = 1.4f)
    {
        var mesh = new Mesh();
        var vertList = new List<Vector3>();
        var middlePointIndexCache = new Dictionary<long, int>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized);
        vertList.Add(new Vector3(1f, t, 0f).normalized);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized);
        vertList.Add(new Vector3(1f, -t, 0f).normalized);

        vertList.Add(new Vector3(0f, -1f, t).normalized);
        vertList.Add(new Vector3(0f, 1f, t).normalized);
        vertList.Add(new Vector3(0f, -1f, -t).normalized);
        vertList.Add(new Vector3(0f, 1f, -t).normalized);

        vertList.Add(new Vector3(t, 0f, -1f).normalized);
        vertList.Add(new Vector3(t, 0f, 1f).normalized);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized);


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));


        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = GetMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, 1);
                int b = GetMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, 1);
                int c = GetMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, 1);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        for (int i = 0; i < vertList.Count; i++)
        {
            var v = vertList[i];
            vertList[i] = v.normalized * heightFunc(v);
        }

        var newVerts = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        var tangents = new List<Vector4>();
        for (int i = 0; i < faces.Count; i++)
        {
            var a = vertList[faces[i].v1];
            var b = vertList[faces[i].v2];
            var c = vertList[faces[i].v3];

            if (a.magnitude < trashHold && b.magnitude < trashHold && c.magnitude < trashHold)
                continue;

            var ab = a - b;
            var ac = a - c;
            var n1 = Vector3.Cross(ab, ac);

            var ca = c - a;
            var cb = c - b;
            var n2 = Vector3.Cross(ca, cb);

            var avg = (n1 + n2)/2;
            normals.Add(avg);
            normals.Add(avg);
            normals.Add(avg);

            tangents.Add(a.normalized);
            tangents.Add(b.normalized);
            tangents.Add(c.normalized);

            newVerts.Add(a);
            triangles.Add(newVerts.Count-1);
            newVerts.Add(b);
            triangles.Add(newVerts.Count-1);
            newVerts.Add(c);
            triangles.Add(newVerts.Count-1);
        }

        if (!flatShaded)
        {
            triangles.Clear();
            for (int i = 0; i < faces.Count; i++)
            {
                triangles.Add(faces[i].v1);
                triangles.Add(faces[i].v2);
                triangles.Add(faces[i].v3);
            }
        }

        mesh.vertices = (flatShaded ? newVerts : vertList).ToArray();
        mesh.triangles = triangles.ToArray();
        var uv = new Vector2[mesh.vertices.Length];
        for(var i= 0; i < mesh.vertices.Length; i++){
            var unitVector = mesh.vertices[i].normalized;
            Vector2 ICOuv = new Vector2(0, 0);
            ICOuv.x = (Mathf.Atan2(unitVector.x, unitVector.z) + Mathf.PI) / Mathf.PI / 2;
            ICOuv.y = (Mathf.Acos(unitVector.y) + Mathf.PI) / Mathf.PI - 1;
            uv[i] = new Vector2(ICOuv.x, ICOuv.y);
        }
 
        mesh.uv = uv; 
        
        
        if(flatShaded)
            mesh.tangents = tangents.ToArray();
        
        if(flatShaded)
            mesh.normals = normals.ToArray();
        else
        {
            mesh.RecalculateNormals(normalsAngle);
        }
         
        
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.Optimize();
        return mesh;
    }
}