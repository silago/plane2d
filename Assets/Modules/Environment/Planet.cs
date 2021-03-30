#region
using UnityEngine;
#endregion
public class Planet : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private void Update()
    {
        transform.Rotate(0, 0, speed * Mathf.Rad2Deg * Time.deltaTime);
    }
}
