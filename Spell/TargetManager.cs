using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Quark
{
    public class TargetManager
    {
        public static void GetTargets(CastData data)
        {
            if (Utils.Checkflag(data.Spell.Targetables, TargetType.Character))
            {
                data.AddTarget(GameObject.Find("Cylinder").GetComponent<Character>());
            }
            if (data.Spell.TargetForm == TargetForm.Plural && Utils.Checkflag(data.Spell.Targetables, TargetType.Point))
            {
                data.AddTarget(new UnityEngine.Vector3(0, 0, 0));
                data.AddTarget(new UnityEngine.Vector3(10, 0, 10));
                data.AddTarget(new UnityEngine.Vector3(10, 0, 0));
                data.AddTarget(new UnityEngine.Vector3(0, 0, 10));
            }

            data.TargetingDone();
        }
    }

    [Flags]
    public enum TargetType
    {
        Point = 1,
        Character = 2,
        Allied = 4,
        Neutral = 8,
        Enemy = 16,
        Self = 32
    }

    public enum TargetForm
    {
        Singular,
        Plural
    }
}