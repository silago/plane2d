using System.Collections;
using System.Collections.Generic;
using Modules.Player;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxSkyboxCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3  center = Vector3.zero;
    //[SerializeField]
    //private IMovable target;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float speed = 1f;
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            return;
        }
        var distance = center - mainCamera.transform.position;
        distance *= speed;
        transform.rotation = Quaternion.Euler(
            new Vector3(distance.y,-distance.x)
            ); 
    }
}
