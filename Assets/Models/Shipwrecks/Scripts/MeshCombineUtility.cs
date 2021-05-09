#region
using UnityEngine;
#endregion
public class MeshCombineUtility
{

    public static Mesh Combine(MeshInstance[] combines, bool generateStrips)
    {
        var vertexCount = 0;
        var triangleCount = 0;
        var stripCount = 0;
        foreach (var combine in combines)
            if (combine.mesh)
            {
                vertexCount += combine.mesh.vertexCount;

                if (generateStrips)
                {
                    // SUBOPTIMAL FOR PERFORMANCE
                    var curStripCount = combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                    if (curStripCount != 0)
                    {
                        if (stripCount != 0)
                        {
                            if ((stripCount & 1) == 1)
                                stripCount += 3;
                            else
                                stripCount += 2;
                        }
                        stripCount += curStripCount;
                    }
                    else
                    {
                        generateStrips = false;
                    }
                }
            }

        // Precomputed how many triangles we need instead
        if (!generateStrips)
            foreach (var combine in combines)
                if (combine.mesh)
                    triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;

        var vertices = new Vector3[vertexCount];
        var normals = new Vector3[vertexCount];
        var tangents = new Vector4[vertexCount];
        var uv = new Vector2[vertexCount];
        var uv1 = new Vector2[vertexCount];
        var colors = new Color[vertexCount];

        var triangles = new int[triangleCount];
        var strip = new int[stripCount];

        int offset;

        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);

        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
            {
                var invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
            }
        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
            {
                var invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
            }
        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);

        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv2, uv1, ref offset);

        offset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
                CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset);

        var triangleOffset = 0;
        var stripOffset = 0;
        var vertexOffset = 0;
        foreach (var combine in combines)
            if (combine.mesh)
            {
                if (generateStrips)
                {
                    var inputstrip = combine.mesh.GetTriangles(combine.subMeshIndex);
                    if (stripOffset != 0)
                    {
                        if ((stripOffset & 1) == 1)
                        {
                            strip[stripOffset + 0] = strip[stripOffset - 1];
                            strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                            strip[stripOffset + 2] = inputstrip[0] + vertexOffset;
                            stripOffset += 3;
                        }
                        else
                        {
                            strip[stripOffset + 0] = strip[stripOffset - 1];
                            strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                            stripOffset += 2;
                        }
                    }

                    for (var i = 0; i < inputstrip.Length; i++) strip[i + stripOffset] = inputstrip[i] + vertexOffset;
                    stripOffset += inputstrip.Length;
                }
                else
                {
                    var inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                    for (var i = 0; i < inputtriangles.Length; i++) triangles[i + triangleOffset] = inputtriangles[i] + vertexOffset;
                    triangleOffset += inputtriangles.Length;
                }

                vertexOffset += combine.mesh.vertexCount;
            }

        var mesh = new Mesh();
        mesh.name = "Combined Mesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.uv2 = uv1;
        mesh.tangents = tangents;
        if (generateStrips)
            mesh.SetTriangles(strip, 0);
        else
            mesh.triangles = triangles;

        return mesh;
    }

    private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (var i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        offset += vertexcount;
    }

    private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (var i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
        offset += vertexcount;
    }

    private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    {
        for (var i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
    {
        for (var i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    {
        for (var i = 0; i < src.Length; i++)
        {
            var p4 = src[i];
            var p = new Vector3(p4.x, p4.y, p4.z);
            p = transform.MultiplyVector(p).normalized;
            dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
        }

        offset += vertexcount;
    }

    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }
}
