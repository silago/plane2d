using System;
using Events;
using UnityEngine;

public class LayOrbitMsg : BaseValueMessage<bool> {}
namespace Modules.Game.Player
{
    public class OrbitController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _orbitLayerMask;
        private Transform _enteredItem;
        private bool _onTheOrbit = false;
        
        [SerializeField] 
        float speed, radius, t;

        private void Update()
        {
            // if it's on the orbit we are sure that _eenteredItem is not null
            if (Input.GetKeyDown(KeyCode.E) && _onTheOrbit == false )
            {
                _onTheOrbit = true;

                var itemPosition = _enteredItem.position;
                var transformPosition = transform.position;
                radius = Vector3.Distance(itemPosition, transformPosition);
                t = -Vector3.Angle(itemPosition - transformPosition, transform.up) * Mathf.Deg2Rad;
            }

            if (_onTheOrbit) 
            {
                var itemPosition = _enteredItem.position;
                var transformPosition = transform.position;
                
                t+= Time.deltaTime * speed;
                var x = radius * Mathf.Sin(t);
                var y = radius * Mathf.Cos(t);
                _enteredItem.position= transformPosition + new Vector3(x,y,0);
                
                var targetPos = itemPosition - transformPosition;
                var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                _enteredItem.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90 ));
            } 
        }

        public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        } 
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.transform == _enteredItem)
            {
                _enteredItem = null; 
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInLayerMask(other.gameObject, _orbitLayerMask))
            {
                _enteredItem = other.transform;
            }
        }
    }
}