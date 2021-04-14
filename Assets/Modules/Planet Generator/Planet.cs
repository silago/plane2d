using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

/*
public class Planet : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        float random = Random.Range(100f, 1000f);
        var meshFilter = GetComponent<MeshFilter>();
	    meshFilter.sharedMesh = null;

	    float HeightFunc(Vector3 v)
	    {
		    return Noise.Noise.GetOctaveNoise(v.x + random, v.y + random, v.z + random, 8) * 0.8f + 1;
	    }

	    meshFilter.sharedMesh = IcoSphere.Create(4, true, HeightFunc);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    transform.rotation *=
	        Quaternion.AngleAxis(Time.deltaTime*20, Vector3.up)
	        *Quaternion.AngleAxis(Time.deltaTime*20, Vector3.right);
	}
	public bool regenerate;

	private void OnValidate()
	{
		if (regenerate == true)
		{
			regenerate = false;
			Start();
		}
	}
}
*/
