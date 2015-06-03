using Quark.Attributes;
using Quark.Buffs;
using Quark.Targeting;
using UnityEngine;

namespace Quark
{
    /// <summary>
    /// This delegate is used for events related with Stats.
    /// </summary>
    /// <param name="source">Character that owns the Stat.</param>
    /// <param name="stat">The Stat.</param>
    /// <param name="change">The amount of change.</param>
    public delegate void StatDel(Character source, Stat stat, float change);

    /// <summary>
    /// This delegate is used for events related with Buffs.
    /// </summary>
    /// <param name="source">Character that possesses the Buff</param>
    /// <param name="buff">The Buff.</param>
    public delegate void BuffDel(Character source, Buff buff);

    /// <summary>
    /// This delegate is used for events related with collisions.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    public delegate void CollisionDel(QuarkCollision collision);

    /* 
     * Should we add a `TargetMacro sender` argument to handlers?
     * Should we add the final selected targets to the success handler?
    */

    /// <summary>
    /// This delegate is used with the event of a TargetMacro success.
    /// </summary>
    /// <param name="targets">Selected targets by the macro.</param>
    public delegate void MacroSuccess(TargetCollection targets);

    /// <summary>
    /// This delegate is used with the event of a TargetMacro failure.
    /// </summary>
    /// <param name="error">Why the targeting failed.</param>
    public delegate void MacroError(TargetingError error);

    /// <summary>
    /// This delegate is used with events related with a Character.
    /// </summary>
    /// <param name="source">The Character.</param>
    public delegate void CharacterDel(Character source);

    /// <summary>
    /// This delegate is used with events related with a Targetable. 
    /// </summary>
    /// <param name="target">The Targetable.</param>
    public delegate void TargetableDel(Targetable target);

    /// <summary>
    /// This delegate is used with events related with a Point. 
    /// </summary>
    /// <param name="target">The Point.</param>>
    public delegate void PointDel(Vector3 target);
}
