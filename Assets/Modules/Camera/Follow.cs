using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane {
public class Follow : MonoBehaviour
{
    [SerializeField]
        private Mode _mode;
        public enum Mode
        {
            Update,Late,Fixed
            
        }
        [SerializeField]
        Transform target;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        void UpdatePosition()
        {
            transform.position = new Vector3(
                    target.position.x,
                    target.position.y,
                    transform.position.z
                    );
        }
        // Update is called once per frame
        void Update() {
            if (_mode == Mode.Update)
                UpdatePosition();
        }
        
        void FixedUpdate() {
            if (_mode == Mode.Fixed)
                UpdatePosition();
        }
        private void LateUpdate()
        {
            if (_mode == Mode.Late)
                UpdatePosition();
        }

}
}
