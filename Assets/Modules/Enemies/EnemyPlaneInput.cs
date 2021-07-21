using System;

using System.Collections;
using System.Linq;
using Modules.Utils;
using UnityEngine;
using static Modules.Utils.Vector3Extensions;
using Random = UnityEngine.Random;





namespace Modules.Enemies
{

    public class EnemyPlaneInput : BaseEnemyInput 
    {
        protected override bool Attack()
        {
            var distance = Vector3.Distance(transform.position, AttackTarget.Transform.position);
            if ( distance > _settings.surrenderDistance)
            {
                SetAttackTarget(null);
                currentState = State.Default;
                return false;
            }
            var distanceDiff = (distance -_settings. desiredDistance);
            movementController.speedUp = movementController.slowDown = false;
            if (distance > 0.2)
            {
                movementController.slowDown = true;
            }
            else if (distance < -.2 )
            {
                movementController.speedUp = true;
            }
            
            Quaternion? q;
            if ((q = PredictAim2(
                transform.position,
                AttackTarget.Transform.position,
                AttackTarget.Direction * AttackTarget.VelocityMagnitude,
                shootController.projectileSpeed
            )) != null)
            {
                var rotation = transform.rotation;
                q *= Quaternion.Euler(orientation);
                var zDiff = Mathf.Abs(transform.eulerAngles.z - q.Value.eulerAngles.z);
                if (angleShootThreshold > zDiff && (shootController.MakeShot() && (++shootCount >= shootCountBeforeExitState)))
                {
                    shootCount = 0;
                    currentState = State.Default;
                }
                rotation = Quaternion.RotateTowards(rotation, q.Value, movementController.rotationSpeed * Time.deltaTime);
                transform.rotation = rotation;
                return true;
            }
            return false;
        }
    }
}