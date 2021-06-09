using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

[CustomEditor(typeof(MeshFilter))]
public class VertsMeshEditor : Editor
{
    private static float radius = 0.5f;
    private static float scrollSpeed = 0.05f;
    private static float moveAmount = 0.05f;
    private static float normalsAngle = 15f;
    private static bool  edit = false;
    
    
    [MenuItem("Window/Mesh Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(VertsMeshEditor));
    }

    private List<Vector3> gotVerts = new List<Vector3>();
    public IEnumerable<(int, Vector3)> GetVerts(Mesh mesh, Vector3 pos, float range)
        => mesh.vertices.Select((v,i) => (i,v)).Where(pair => Vector3.Distance(pair.v, pos) < range);
    void OnSceneGUI()
    {
        Event e = Event.current;
        MeshFilter filter = target as MeshFilter;
        if (filter != null && filter.sharedMesh != null)
        {
            Handles.BeginGUI();
            Mesh mesh = filter.sharedMesh;
            GUI.Label(new Rect(10,10,300,25), "Mesh Editor is ready");
            GUI.Label(new Rect(10,40,100,25), "Scroll Speed: ");
            scrollSpeed = float.Parse(GUI.TextField(new Rect(110, 40, 100, 25), scrollSpeed.ToString()));
            
            GUI.Label(new Rect(10,70,100,25), "Change Amount: ");
            moveAmount = float.Parse(GUI.TextField(new Rect(110, 70, 100, 25), moveAmount.ToString()));
            
            GUI.Label(new Rect(10,100,100,25), "Normals Angole: ");
            normalsAngle = float.Parse(GUI.TextField(new Rect(110, 100, 100, 25), normalsAngle.ToString()));
            
            GUI.Label(new Rect(10,130,100,25), "Edit: ");
            edit = GUI.Toggle(new Rect(110, 130, 100, 25), edit, "");
            Handles.EndGUI();
            if (!edit)
            {
                return;
            }
            e.Use();
            Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
                     
            RaycastHit hit;
            if( Physics.Raycast( ray, out hit ) )
            {
                if (e.type == EventType.ScrollWheel)
                {
                    Debug.Log(e.delta);
                    radius += e.delta.y * scrollSpeed;
                    radius = Mathf.Max(radius, 0.01f);
                }
                
                Handles.color = new Color(1, 1, 1, 0.3f);
                var x =HandleUtility.GetHandleSize(hit.point);
                Handles.DrawSolidDisc(hit.point, hit.normal,radius);
                
                
                if (e.type == EventType.MouseDown)
                {
                    var meshTransform = filter.transform;
                    var point = meshTransform.InverseTransformPoint(hit.point);
                    var verts = GetVerts(mesh, point, radius);
                    var original = filter.sharedMesh.vertices;
                    if (e.button == 0)
                    {
                        foreach (var vert in verts)
                        {
                            original[vert.Item1] += (original[vert.Item1].normalized * moveAmount);
                        }    
                    }
                    else if (e.button == 1)
                    {
                        foreach (var vert in verts)
                        {
                            original[vert.Item1] -= (original[vert.Item1].normalized * moveAmount);
                        }    
                    }
                    
                    filter.sharedMesh.vertices = original;
                    filter.sharedMesh.RecalculateBounds();
                    filter.sharedMesh.RecalculateNormals(15f);
                    filter.sharedMesh.RecalculateTangents();
                    filter.sharedMesh.RecalculateUVDistributionMetrics();
                }
            }
        }
        else
        {
            Handles.BeginGUI();
            GUI.Label(new Rect(10,10,300,25), "Mesh Editor Is not ready");
            Handles.EndGUI();
        }
        //foreach (var vector3 in gotVerts)
        //{
        //    Handles.color = new Color(1, 0, 0, 1f);
        //    Handles.DrawSolidDisc(vector3,Vector3.forward, 0.05f);
        //}
    }
    
}
