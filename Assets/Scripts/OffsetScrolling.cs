using UnityEngine;
public class OffsetScrolling : MonoBehaviour {
    public float scrollSpeed;

    private Renderer renderer;
    private Vector2 savedOffset;

    void Start () {
        renderer = GetComponent<Renderer> ();
    }

    void Update () {
        float x = Mathf.Repeat (Time.time * scrollSpeed, 1);
        Vector2 offset = transform.position; 
        renderer.sharedMaterial.mainTextureOffset = offset;
    }
}
