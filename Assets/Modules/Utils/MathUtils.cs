using System;
using UnityEngine;
namespace Modules.Utils
{
    public static class MathUtils
    {
        public static int FindCircleCircleIntersections(
            float cx0, float cy0, float radius0,
            float cx1, float cy1, float radius1,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            // Find the distance between the centers.
            var dx = cx0 - cx1;
            var dy = cy0 - cy1;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            if (dist == 0 && radius0 == radius1)
            {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            // Find a and h.
            var a = (radius0 * radius0 -
                radius1 * radius1 + dist * dist) / (2 * dist);
            var h = Math.Sqrt(radius0 * radius0 - a * a);

            // Find P2.
            var cx2 = cx0 + a * (cx1 - cx0) / dist;
            var cy2 = cy0 + a * (cy1 - cy0) / dist;

            // Get the points P3.
            intersection1 = new Vector2(
                (float)(cx2 + h * (cy1 - cy0) / dist),
                (float)(cy2 - h * (cx1 - cx0) / dist));
            intersection2 = new Vector2(
                (float)(cx2 - h * (cy1 - cy0) / dist),
                (float)(cy2 + h * (cx1 - cx0) / dist));

            // See if we have 1 or 2 solutions.
            if (dist == radius0 + radius1) return 1;
            return 2;
        }

        public static bool FindTangents(Vector2 center, float radius,
            Vector2 external_point, out Vector2 pt1, out Vector2 pt2)
        {
            // Find the distance squared from the
            // external point to the circle's center.
            double dx = center.x - external_point.x;
            double dy = center.y - external_point.y;
            var D_squared = dx * dx + dy * dy;
            if (D_squared < radius * radius)
            {
                pt1 = new Vector2(-1, -1);
                pt2 = new Vector2(-1, -1);
                return false;
            }

            // Find the distance from the external point
            // to the tangent points.
            var L = Math.Sqrt(D_squared - radius * radius);

            // Find the points of intersection between
            // the original circle and the circle with
            // center external_point and radius dist.
            FindCircleCircleIntersections(
                center.x, center.y, radius,
                external_point.x, external_point.y, (float)L,
                out pt1, out pt2);

            return true;
        }
    }
}
