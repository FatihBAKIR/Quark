using System;
using UnityEngine;
namespace Quark
{
    public class Events
    {
        public static void StatChange(Character of, Stat stat, double Difference)
        {
            Messenger<Character, double>.Broadcast(stat.Tag + ".Changed", of, Difference);
            Messenger<double>.Broadcast(StatChangeEvent(of, stat.Tag), Difference);
        }

        public static string StatChangeEvent(Character of, string stat)
        {
            return of.GetHashCode() + "." + stat + ".Changed";
        }
    }
}
    