using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class MeshEditor : MonoBehaviour
{
    [SerializeField] MeshFilter _meshFilter;
    private Vector3[] verts; 

    public IEnumerable<(int, Vector3)> GetVerts(Vector3 pos, float range)
        => verts.Select((v,i) => (i,v)).Where(pair => Vector3.Distance(pair.v, pos) < range);
    
    void InitInEditor()
    {
        verts = _meshFilter.mesh.vertices;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //void OnSceneGUI() 
    //{
    //    if (Event.current.type == EventType.Layout) 
    //    {
    //        HandleUtility.AddDefaultControl(0);
    //    }
    //}
}
