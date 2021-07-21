using System;
using System.Collections;
using Modules.Player;
using Modules.Utils;
using UnityEngine;
using Zenject;
using static Modules.Utils.MathUtils;
using Random = UnityEngine.Random;
namespace Modules.Enemies
{
    public class EnemyBirdInput : BaseEnemyInput//, ISetTarget
    {
        protected override bool Attack()
        {
            movementController.speedUp = true; 
            
            var currentAngle = GetAngle();
            diff = currentAngle;
            var d = Vector3.Distance(transform.position, _currentTarget.position);
            if (d < 0.005) currentState = State.Default;
            
            
            if (Mathf.Abs(diff) > _settings.angleTreshhold)
                if (diff > 0)
                    movementController.rotateRight = true;
                else
                    movementController.rotateLeft = true;
            return false;
        }
    }
}
