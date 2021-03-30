#region
using UnityEngine;
#endregion
[RequireComponent(typeof(SpriteRenderer))]
public class RotationController : MonoBehaviour
{

    [SerializeField]
    private Sprite[] sprites;

    //[GetComponent]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        var currentZRotation = transform.rotation.eulerAngles.z;
        if (currentZRotation != _previousZRotation)
        {
            UpdateRepresentation();
            _previousZRotation = currentZRotation;
        }
    }

    private void OnValidate()
    {
        UpdateRotation = false;
        var currentRotation = transform.rotation.eulerAngles.z;
        var currentSpriteIndex = (int)(currentRotation / 360 * sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[currentSpriteIndex];
    }

    private void UpdateRepresentation()
    {
        var currentRotation = transform.rotation.eulerAngles.z;
        var currentSpriteIndex = (int)(currentRotation / 360 * sprites.Length);
        spriteRenderer.sprite = sprites[currentSpriteIndex];
    }

    #if UNITY_EDITOR
    public bool UpdateRotation;
    protected float _previousZRotation;
    #endif
}
