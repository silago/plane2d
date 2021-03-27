using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Modules.Utils
{
    public static class Vector3Extensions
    {
 static float PredictiveAimWildGuessAtImpactTime() {
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
             return !(float.IsNaN(result.x));

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
                if (targetVel.sqrMagnitude <= 0f)
				{
                    return targetPos;
                }
                else
				{
                    Vector3 targetToBullet = shootPos - targetPos;
                    float distToTargetSqr = (shootPos - targetPos).sqrMagnitude;
                    float distToTarget = (shootPos - targetPos).magnitude;
                    Vector3 targetToBulletNorm = targetToBullet / distToTarget;
                    float tarSpeed = targetVel.magnitude;
                    float tarSpeedSqr = targetVel.sqrMagnitude;
                    Vector3 tarVelNorm = targetVel / tarSpeed;
                    float projSpeedSqr = projSpeed.Squared();

                    float cosTheta = Vector3.Dot(targetToBulletNorm, tarVelNorm);

                    float offset = Mathf.Sqrt((2 * distToTarget * tarSpeed * cosTheta).Squared() + 4 * (projSpeedSqr - tarSpeedSqr) * distToTargetSqr);

                    float estimatedTravelTime = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));
                    #region Offset should be ±, but works fine with just +
                    //float estimatedTravelTimeTwo = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));

                    /*if (estimatedTravelTimeOne < 0 && estimatedTravelTimeTwo < 0 ||
                        estimatedTravelTimeOne == float.NaN && estimatedTravelTimeTwo == float.NaN)*/
                    #endregion

                    if (estimatedTravelTime < 0 || estimatedTravelTime == float.NaN)
					{
                        return targetPos;
                    }
                    else
					{
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
                }
            }
            
            public static Quaternion? PredictAim2(Vector3 shootPos, Vector3 targetPos, Vector3 targetVelocity, float projSpeed)
            {
                float a=(targetVelocity.x*targetVelocity.x)+(targetVelocity.y*targetVelocity.y)-(projSpeed*projSpeed);
                float b=2*(targetVelocity.x*(targetPos.x-shootPos.x) 
                    +targetVelocity.y*(targetPos.y-shootPos.y));
                float c= ((targetPos.x-shootPos.x)*(targetPos.x-shootPos.x))+
                    ((targetPos.y-shootPos.y)*(targetPos.y-shootPos.y));
 
                float disc= b*b -(4*a*c);
                if(disc<0){
                    return null;
                }
                float t1=(-1*b+Mathf.Sqrt(disc))/(2*a);
                float t2=(-1*b-Mathf.Sqrt(disc))/(2*a);
                float t= Mathf.Max(t1,t2);// let us take the larger time value 
                float aimX=(targetVelocity.x*t)+targetPos.x;
                float aimY=targetPos.y+(targetVelocity.y*t);
                    
                float turretAngle=Mathf.Atan2(aimY-shootPos.y,aimX-shootPos.x)*Mathf.Rad2Deg;
                turretAngle+=90;//art correction
                    
                return Quaternion.Euler(0,0,turretAngle);
            }
    }
}