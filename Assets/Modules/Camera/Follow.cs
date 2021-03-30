#region
using UnityEngine;
#endregion
namespace Plane
{
    public class Follow : MonoBehaviour
    {
        public enum Mode
        {
            Update, Late, Fixed

        }
        [SerializeField]
        private Mode _mode;
        [SerializeField]
        private Transform target;
        // Start is called before the first frame update
        private void Start()
        {

        }
        // Update is called once per frame
        private void Update()
        {
            if (_mode == Mode.Update)
                UpdatePosition();
        }

        private void FixedUpdate()
        {
            if (_mode == Mode.Fixed)
                UpdatePosition();
        }
        private void LateUpdate()
        {
            if (_mode == Mode.Late)
                UpdatePosition();
        }

        private void UpdatePosition()
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );
        }
    }
}
