#region
using UnityEngine;
#endregion
public class Rotatator : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotation;
    [SerializeField]
    private Transform meshObject;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private bool randomize;

    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float minSpeed;

    public bool Randomize => randomize;

    // Use this for initialization
    private void Start()

    {

        if (meshObject == null)

        {
            meshObject = transform.Find("planet");
            if (meshObject == null)
                meshObject = transform.Find("w2");
        }


        if (randomize)

        {
            rotation = new Vector3(RandFloat(), RandFloat(), RandFloat());
            rotationSpeed = Random.Range(minSpeed, maxSpeed);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()

    {
        if (meshObject != null)
            meshObject.Rotate(rotation, rotationSpeed * Time.deltaTime);
    }

    private float RandFloat()

    {
        return Random.Range(0f, 1.01f);
    }
}
