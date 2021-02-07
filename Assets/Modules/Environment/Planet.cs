using UnityEngine;

public class Planet : MonoBehaviour {
    [SerializeField]
    float speed;

    void Update() {
        transform.Rotate(0,0,speed * Mathf.Rad2Deg * Time.deltaTime);
    }
}
