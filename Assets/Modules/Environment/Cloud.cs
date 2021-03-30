#region
using UnityEngine;
#endregion
public class Cloud : MonoBehaviour
{
    [SerializeField]
    private Transform center;
    [SerializeField]
    private float speed;
    private float radius;
    private float t;


    // Start is called before the first frame update
    private void Start()
    {
        radius = Vector3.Distance(transform.position, center.position);
        t = -Vector3.Angle(transform.position - center.position, center.up) * Mathf.Deg2Rad;
    }

    // Update is called once per frame
    private void Update()
    {
        t += Time.deltaTime * speed;
        var x = radius * Mathf.Sin(t);
        var y = radius * Mathf.Cos(t);
        transform.position = center.position + new Vector3(x, y, 0);

        var targetPos = center.position - transform.position;
        var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    private void OnValidate()
    {
    }
}
