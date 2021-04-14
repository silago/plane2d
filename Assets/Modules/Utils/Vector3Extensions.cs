#region
using UnityEngine;
#endregion
namespace Modules.Utils
{
    public static class Vector3Extensions
    {
        private static float PredictiveAimWildGuessAtImpactTime()
        {
            return Random.Range(1, 5);
        }

        public static float Squared(this float @this)
        {
            return Mathf.Pow(@this, 2);
        }

        public static bool PredictAimAndCheck(Vector3 shootPos, Vector3 targetPos, Vector3 targetVel, float projSpeed,
            out Vector3 result)
        {
            result = PredictAim(shootPos, targetPos, targetVel, projSpeed);
            return !float.IsNaN(result.x);

        }

        /*
        public static Vector3 PredictAim3(Vector3 shootPos, Vector3 targetPos, Vector3 targetVel, float projSpeed)
        {
            var a = shootPos;
            var b = targetPos;
            var av = projSpeed;
        }
        */

        public static Vector3 PredictAim(Vector3 shootPos, Vector3 targetPos, Vector3 targetVel, float projSpeed)
        {
            if (targetVel.sqrMagnitude <= 0f) return targetPos;
            var targetToBullet = shootPos - targetPos;
            var distToTargetSqr = (shootPos - targetPos).sqrMagnitude;
            var distToTarget = (shootPos - targetPos).magnitude;
            var targetToBulletNorm = targetToBullet / distToTarget;
            var tarSpeed = targetVel.magnitude;
            var tarSpeedSqr = targetVel.sqrMagnitude;
            var tarVelNorm = targetVel / tarSpeed;
            var projSpeedSqr = projSpeed.Squared();

            var cosTheta = Vector3.Dot(targetToBulletNorm, tarVelNorm);

            var offset = Mathf.Sqrt((2 * distToTarget * tarSpeed * cosTheta).Squared() + 4 * (projSpeedSqr - tarSpeedSqr) * distToTargetSqr);

            var estimatedTravelTime = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));
                    #region Offset should be ±, but works fine with just +
            //float estimatedTravelTimeTwo = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));

            /*if (estimatedTravelTimeOne < 0 && estimatedTravelTimeTwo < 0 ||
                        estimatedTravelTimeOne == float.NaN && estimatedTravelTimeTwo == float.NaN)*/
                    #endregion

            if (estimatedTravelTime < 0 || estimatedTravelTime == float.NaN) return targetPos;
 #region Offset should be ±, but works fine with just +
            /*if (estimatedTravelTimeOne < estimatedTravelTimeTwo)
						{
                            return targetPos + tarVelNorm * tarSpeed * estimatedTravelTimeOne;
						}
                        else
						{
                            return targetPos + tarVelNorm * tarSpeed * estimatedTravelTimeTwo;
                        }*/
						#endregion

            return targetPos + tarVelNorm * (tarSpeed * estimatedTravelTime);
        }

        public static Quaternion? PredictAim2(Vector3 shootPos, Vector3 targetPos, Vector3 targetVelocity, float projSpeed)
        {
            var a = targetVelocity.x * targetVelocity.x + targetVelocity.y * targetVelocity.y - projSpeed * projSpeed;
            var b = 2 * (targetVelocity.x * (targetPos.x - shootPos.x)
                + targetVelocity.y * (targetPos.y - shootPos.y));
            var c = (targetPos.x - shootPos.x) * (targetPos.x - shootPos.x) +
                (targetPos.y - shootPos.y) * (targetPos.y - shootPos.y);

            var disc = b * b - 4 * a * c;
            if (disc < 0) return null;
            var t1 = (-1 * b + Mathf.Sqrt(disc)) / (2 * a);
            var t2 = (-1 * b - Mathf.Sqrt(disc)) / (2 * a);
            var t = Mathf.Max(t1, t2); // let us take the larger time value 
            var aimX = targetVelocity.x * t + targetPos.x;
            var aimY = targetPos.y + targetVelocity.y * t;

            var turretAngle = Mathf.Atan2(aimY - shootPos.y, aimX - shootPos.x) * Mathf.Rad2Deg;
            turretAngle += 90; //art correction

            return Quaternion.Euler(0, 0, turretAngle);
        }
    }
}
