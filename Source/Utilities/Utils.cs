using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Quark.Utilities
{
    /// <summary>
    /// This enumeration is used by the 2 dimensional geometry functions 
    /// to determine the correct plane to work on in Quark utilities.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Planes
    {
        /// <summary>
        /// The X-Y plane.
        /// </summary>
        XY,
        /// <summary>
        /// The X-Z plane.
        /// </summary>
        XZ,
        /// <summary>
        /// The Y-Z plane.
        /// </summary>
        YZ
    }
    public class Utils
    {
        /// <summary>
        /// This function checks whether a value is included in a given enumeration.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="checkfor">The value to check for</param>
        /// <returns></returns>
        public static bool Checkflag(Enum value, Enum checkfor)
        {
            ulong longVal = Convert.ToUInt64(value);
            ulong longOpt = Convert.ToUInt64(checkfor);
            return (longVal & longOpt) == longOpt;
        }

        /// <summary>
        /// Aligns a vector in the given direction in a two dimensional plane
        /// </summary>
        /// <param name="vector">Vector to align</param>
        /// <param name="align">Direction</param>
        /// <returns>Aligned vector</returns>
        public static Vector3 Align2(Vector3 vector, Vector3 align)
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
        /// Sets the unnecessary component of a 3d vector to zero.
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <param name="plane">The plane</param>
        /// <returns>A new vector on the given plane.</returns>
        public static Vector3 VectorOnPlane(Vector3 vector, Planes plane)
        {
            switch (plane)
            {
                case Planes.XY:
                    vector.z = 0;
                    break;
                case Planes.XZ:
                    vector.y = 0;
                    break;
                case Planes.YZ:
                    vector.x = 0;
                    break;
            }

            return vector;
        }

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <param name="plane">The plane this function should work on (default XZ)</param>
        /// <returns>Calculated distance</returns>
        public static float Distance2(Vector3 v1, Vector3 v2, Planes plane = Planes.XZ)
        {
            v1 = VectorOnPlane(v1, plane);
            v2 = VectorOnPlane(v2, plane);

            return Vector3.Distance(v1, v2);
        }

        /// <summary>
        /// Returns the angle between 2 points in the range of [0,360] degrees.
        /// </summary>
        /// <param name="v1">First Point</param>
        /// <param name="v2">Second Point</param>
        /// <param name="plane">The plane this function should work on (default XZ)</param>
        /// <returns>Angle between them in degrees</returns>
        public static float Angle2(Vector3 v1, Vector3 v2, Planes plane = Planes.XZ)
        {
            v1 = VectorOnPlane(v1, plane);
            v2 = VectorOnPlane(v2, plane);

            v1.Normalize();
            v2.Normalize();

            Vector3 normal = Vector3.up;
            switch (plane)
            {
                case Planes.XY:
                    normal = Vector3.forward;
                    break;
                case Planes.XZ:
                    normal = Vector3.up;
                    break;
                case Planes.YZ:
                    normal = Vector3.right;
                    break;
            }

            float angle = Vector3.Angle(v1, v2);

            float sign = Mathf.Sign(Vector3.Dot(v1, Vector3.Cross(normal, v2)));

            return sign * angle + (sign > 0 ? 0 : 360);
        }

        /// <summary>
        /// Returns the slope in degrees from a vector to another 
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <param name="plane">The plane this function should work on (default XZ)</param>
        /// <returns>Slope from the first point to the other in degrees</returns>
        public static float Slope2(Vector3 v1, Vector3 v2, Planes plane = Planes.XZ)
        {
            v1 = VectorOnPlane(v1, plane);
            v2 = VectorOnPlane(v2, plane);

            float nominator = 0;
            float denominator = 0;

            switch (plane)
            {
                case Planes.XY:
                    nominator = v2.y - v1.y;
                    denominator = v2.x - v1.x;
                    break;
                case Planes.XZ:
                    nominator = v2.z - v1.z;
                    denominator = v2.x - v1.x;
                    break;
                case Planes.YZ:
                    nominator = v2.z - v1.z;
                    denominator = v2.y - v1.y;
                    break;
            }

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

