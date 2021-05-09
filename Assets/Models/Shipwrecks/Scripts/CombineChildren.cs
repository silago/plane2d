#region
using System;
using System.Collections;
using UnityEngine;
#endregion
/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{

    /// Usually rendering with triangle strips is faster.
    /// However when combining objects with very low triangle counts, it can be faster to use triangles.
    /// Best is to try out which value is faster in practice.
    public bool generateTriangleStrips = true;

    /// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    private void Start()
    {
        var filters = GetComponentsInChildren(typeof(MeshFilter));
        var myTransform = transform.worldToLocalMatrix;
        var materialToMesh = new Hashtable();

        for (var i = 0; i < filters.Length; i++)
        {
            var filter = (MeshFilter)filters[i];
            var curRenderer = filters[i].GetComponent<Renderer>();
            var instance = new MeshCombineUtility.MeshInstance();
            instance.mesh = filter.sharedMesh;
            if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
            {
                instance.transform = myTransform * filter.transform.localToWorldMatrix;

                var materials = curRenderer.sharedMaterials;
                for (var m = 0; m < materials.Length; m++)
                {
                    instance.subMeshIndex = Math.Min(m, instance.mesh.subMeshCount - 1);

                    var objects = (ArrayList)materialToMesh[materials[m]];
                    if (objects != null)
                    {
                        objects.Add(instance);
                    }
                    else
                    {
                        objects = new ArrayList();
                        objects.Add(instance);
                        materialToMesh.Add(materials[m], objects);
                    }
                }

                curRenderer.enabled = false;
            }
        }

        foreach (DictionaryEntry de in materialToMesh)
        {
            var elements = (ArrayList)de.Value;
            var instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

            // We have a maximum of one material, so just attach the mesh to our own game object
            if (materialToMesh.Count == 1)
            {
                // Make sure we have a mesh filter & renderer
                if (GetComponent(typeof(MeshFilter)) == null)
                    gameObject.AddComponent(typeof(MeshFilter));
                if (!GetComponent("MeshRenderer"))
                    gameObject.AddComponent<MeshRenderer>();

                var filter = (MeshFilter)GetComponent(typeof(MeshFilter));
                filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                GetComponent<Renderer>().material = (Material)de.Key;
                GetComponent<Renderer>().enabled = true;
            }
            // We have multiple materials to take care of, build one mesh / gameobject for each material
            // and parent it to this object
            else
            {
                var go = new GameObject("Combined mesh");
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent(typeof(MeshFilter));
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = (Material)de.Key;
                var filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
                filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
            }
        }
    }
}
