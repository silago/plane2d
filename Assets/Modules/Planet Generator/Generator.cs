using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Vector2[] uv;
    [HideInInspector] [SerializeField] private Vector3[] vertices;
    [HideInInspector] [SerializeField] private Vector3[] normals;
    [HideInInspector] [SerializeField] private int[] triangles;

	[SerializeField]
	private MeshFilter _meshFilter;
	[SerializeField]
	private MinMax RandomRange = new MinMax();
	[SerializeField]
	private float trashHold = 1.4f;
	[SerializeField]
	[Range(0,3)]
	private float scale = 0.8f;
	[SerializeField]
	[Range(0,10)]
	private float noiseScale = 0.1f;
	[SerializeField]
	float normalsAngle;
	[SerializeField]
	private MinMax clamp = new MinMax();
	[SerializeField]
	[Range(0,3)]
	private float offset = 1f;
	[SerializeField]
	private bool flatShaded;

	public bool regenerate;
	private float random = 1000f;

	private void Start()
	{
		Rebuild();
	}
	private void OnValidate()
	{
		if (regenerate == true)
		{
			regenerate = false;
			Generate();
			Serialize();
		}
	}

	void Generate()
    {
	    _meshFilter.sharedMesh = null;
        random = Random.Range(RandomRange.Min, RandomRange.Max);
	    _meshFilter.sharedMesh = IcoSphere.Create(4, flatShaded, HeigtFunc, normalsAngle, trashHold);
    }


	float HeigtFunc(Vector3 v)
	{
		v += Vector3.one * random;
		v *= noiseScale;
        var result =  Noise.Noise.GetOctaveNoise(v.x , v.y , v.z , 8) * scale + offset;
        result = Mathf.Clamp(result, clamp.Min, clamp.Max);
        return result;
	}
	
	public void Serialize()
	{
		var mesh = GetComponent<MeshFilter>().sharedMesh;
 
		uv = mesh.uv;
		vertices = mesh.vertices;
		triangles = mesh.triangles;
		normals = mesh.normals;
	}
	
	public Mesh Rebuild()
	{
		Mesh mesh = new Mesh {
			vertices = vertices,
			triangles = triangles,
			uv = uv
		};
		mesh.RecalculateBounds();
		return mesh;
	}
	
	
}
