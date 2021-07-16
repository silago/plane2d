using System;
using Modules.Player;
using Modules.Utils.TagSelector;
using UnityEngine;
namespace Modules.Enemies
{
    
    public class EnemyTrigger : MonoBehaviour
    {
        [SerializeField]
        [TagSelector]
        private string attackTag;
        [SerializeField]
        private EnemyPlaneInput _input;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("enter" + other.gameObject.name);
            if (_input.AttackTarget == null && other.gameObject.CompareTag(attackTag))
            {
                var target = other.gameObject.GetComponent<IMovable>();
                _input.SetAttackTarget(target);
            }
        }
    }
}
