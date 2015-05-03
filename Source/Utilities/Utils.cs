using System;
using UnityEngine;

namespace Quark.Utilities
{
    public class Utils
    {
        public static bool Checkflag(Enum value, Enum checkfor)
        {
            ulong longVal = Convert.ToUInt64(value);
            ulong longOpt = Convert.ToUInt64(checkfor);
            return (longVal & longOpt) == longOpt;
        }

        /// <summary>
        /// Aligns a vector in the given direction
        /// </summary>
        /// <param name="vector">Vector to align</param>
        /// <param name="align">Direction</param>
        /// <returns>Aligned vector</returns>
        public static Vector3 AlignVector(Vector3 vector, Vector3 align)
        {
            float angle = Mathf.Atan(align.z / align.x);

            int isNegative = (align.z < 0 && align.x > 0) || (align.x > 0 && align.z > 0) ? 1 : -1;

            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            Vector3 temp = new Vector3
            {
                x = vector.x * cos - vector.z * sin,
                z = vector.x * sin + vector.z * cos
            };

            temp = temp * isNegative;

            temp.y = vector.y;

            return temp;
        }

        /// <summary>
        /// Calculates the distance between two points on XZ plane
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <returns>Calculated distance</returns>
        public static float Distance2(Vector3 v1, Vector3 v2)
        {
            v1.y = 0;
            v2.y = 0;
            return Vector3.Distance(v1, v2);
        }

        /// <summary>
        /// Returns the angle between 2 points in degrees on XY plane using cross product
        /// </summary>
        /// <param name="v1">First Point</param>
        /// <param name="v2">Second Point</param>
        /// <returns>Angle between them in degrees</returns>
        public static float Angle2(Vector3 v1, Vector3 v2)
        {
            v1.z = 0;
            v2.z = 0;

            float nominator = v1.x * v2.x + v1.y * v2.y;
            float denominator = v1.magnitude * v2.magnitude;

            float radians = Mathf.Acos(nominator / denominator);

            return radians * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns the slope in degrees from a vector to another on the XY plane
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <returns>Slope from the first point to the other in degrees</returns>
        public static float Slope2(Vector3 v1, Vector3 v2)
        {
            v1.z = 0;
            v2.z = 0;

            float nominator = v2.y - v1.y;
            float denominator = v2.x - v1.x;

            float angle = denominator == 0 ? 90 : Mathf.Atan(nominator / denominator) * Mathf.Rad2Deg;

            if (v2.y > v1.y)
            {
                if (angle > 0)
                    return angle;
                return angle + 180;
            }

            if (angle < 0)
                return angle + 360;
            return angle + 180;
        }
    }
}

