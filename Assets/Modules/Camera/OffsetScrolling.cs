#region
using UnityEngine;
#endregion
public class OffsetScrolling : MonoBehaviour
{
    public float scrollSpeed;

    private Renderer renderer;
    private Vector2 savedOffset;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        var x = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = transform.position;
        renderer.sharedMaterial.mainTextureOffset = offset;
    }
}
