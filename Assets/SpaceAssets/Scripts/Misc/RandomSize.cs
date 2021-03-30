#region
using UnityEngine;
#endregion
public class RandomSize : MonoBehaviour
{
    [Range(1.0f, 10.0f)]
    public float multiplierMax = 3f;
    private Vector3 initialScale;

    private void Start()
    {
        //Initial scale
        initialScale = transform.localScale;
        Generate();
    }

    private void Update()
    {

    }

    public void Generate()
    {
        //Choose a random multiplied scale from the initial scale and the multiplierMax variable
        transform.localScale = initialScale * Random.Range(1f, multiplierMax);
    }
}
