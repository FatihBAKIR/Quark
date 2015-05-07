using System;
using System.Collections.Generic;
using System.Text;
using Quark.Attributes;
using Quark.Buffs;
using Quark.Targeting;
using UnityEngine;

namespace Quark
{
    public delegate void StatDel(Character source, Stat stat, float change);

    public delegate void BuffDel(Character source, Buff buff);

    public delegate void CollisionDel(QuarkCollision collision);

    public delegate void CharacterDel(Character source);

    //Should we add a `TargetMacro sender` argument to handlers?
    //Should we add the final selected targets to the success handler?
    public delegate void MacroSuccess(TargetCollection targets);
    public delegate void MacroError(TargetingError error);
    public delegate void TargetableHandler(Targetable target);
    public delegate void CharacterHandler(Character target);
    public delegate void PointHandler(Vector3 target);
}
