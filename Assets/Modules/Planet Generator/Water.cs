using UnityEngine;

public class Water : MonoBehaviour
{
    private MeshFilter _meshFilter;

    // Use this for initialization
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = null;
        _meshFilter.sharedMesh = IcoSphere.Create(4, true, v => 1.4f);
    }
}
