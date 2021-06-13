using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    float speedStep = 5f;
    private float speed = 0.0f;
    [SerializeField]
    private float maxSpeed = 20f;
    [SerializeField]
    private float minSpeed = 0f;
    
    [SerializeField]
    float rotationSpeed = 30f;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0,0,Time.deltaTime * rotationSpeed);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0,0,-Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            speed += speedStep;
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            speed -= speedStep;
        }
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        var pos = transform.position;
        transform.Translate(Vector3.up * (Time.deltaTime * speed));
    }
}
