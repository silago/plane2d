#region
using UnityEngine;
#endregion
public class RandomRotation : MonoBehaviour
{
    //The rotation speed of the object
    public float rotationSpeedMax = 35f;
    //Set whether the rotation speed should be randomized or not
    public bool randomize = true;
    private float rotationSpeed;

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    //Assign a new speed for the rotation
    public void Generate()
    {
        if (randomize)
            //Select randomly if the rotation is clockwise or counterclockwise and assign a random speed
            rotationSpeed = (Random.Range(0, 100) < 50 ? -1f : 1f) * Random.Range(0f, rotationSpeedMax);
        else
            rotationSpeed = rotationSpeedMax;
    }
}
