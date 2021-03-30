#region
using System;
using UnityEngine;
#endregion
namespace Modules.Player
{
    public class RotationController3D : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 360)]
        private float rotationZ;

        [SerializeField]
        private Transform model;
        [SerializeField]
        private Vector3 initialModelRotation;
        public Vector3 settedRotation;
        public float diffRotationZ;
        private float _prevRotation;


        private void Start()
        {
            //well, that is totally wrong
            //initialModelRotation = (model.localRotation.eulerAngles);
        }

        //[GetComponent]

        private void Update()
        {
            var currentZRotation = transform.rotation.eulerAngles.z;
            if (Math.Abs(currentZRotation - _previousZRotation) > 1f)
            {
                UpdateRepresentation();
                _previousZRotation = currentZRotation;
            }
        }

        private void OnValidate()
        {
            if (UpdateRotation)
            {
                _prevRotation = transform.rotation.eulerAngles.z;
                transform.rotation = Quaternion.Euler(0, 0, rotationZ);
                UpdateRepresentation();
            }
            //UpdateRotation = false;

        }

        private void UpdateRepresentation()
        {
            var currentRotation = transform.rotation.eulerAngles;
            var diff = currentRotation.z - _prevRotation;
            diffRotationZ = diff;
            //model.transform.Rotate(model.rotation*new Vector3(diff,0,0),Space.Self);
            model.localRotation = Quaternion.Euler(currentRotation.z, 0, 0);
            settedRotation = model.transform.localRotation.eulerAngles;
        }
        #if UNITY_EDITOR
        public bool UpdateRotation;
        protected float _previousZRotation;
        #endif
    }
}
