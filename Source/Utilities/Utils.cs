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
        
        public static Vector3 RotateVector(Vector3 vector, Vector3 align)
        {
            float angle = Mathf.Atan(align.z / align.x);
            
            int isNegative = (align.z < 0 && align.x > 0) || (align.x > 0 && align.z > 0) ? 1 : -1;
            
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            Vector3 temp = new Vector3
            {
                x = vector.x*cos - vector.z*sin, 
                z = vector.x*sin + vector.z*cos
            };

            temp = temp * isNegative;

            temp.y = vector.y;

            return temp;
        }

        public static float Distance2(Vector3 v1, Vector3 v2)
        {
            v1.y = 0;
            v2.y = 0;
            return Vector3.Distance(v1, v2);
        }
    }
}

