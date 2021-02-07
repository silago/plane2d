using System;
using System.Collections;
using System.Collections.Generic;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RotationController : MonoBehaviour
{
    
    #if UNITY_EDITOR
    public bool UpdateRotation = false;
    protected float _previousZRotation = 0f;
    #endif
    
    [SerializeField]
    Sprite[] sprites;
    
    [GetComponent]
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

    void UpdateRepresentation() {
            var currentRotation = transform.rotation.eulerAngles.z;
            var currentSpriteIndex = (int)((currentRotation /360) * sprites.Length); 
            spriteRenderer.sprite = sprites[currentSpriteIndex];
        }

    private void OnValidate()
    {
            UpdateRotation = false;
            var currentRotation = transform.rotation.eulerAngles.z;
            var currentSpriteIndex = (int)((currentRotation /360) * sprites.Length); 
            Debug.Log(currentSpriteIndex);
            Debug.Log(currentRotation);
            GetComponent<SpriteRenderer>().sprite = sprites[currentSpriteIndex];
        }
}
