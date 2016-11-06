using Quark.Projectiles;
using Quark.Spells;
using Quark.Targeting;
using UnityEngine;

namespace Quark.Contexts
{
    /// <summary>
    /// This enumeration stores possible results of a hit validation
    /// </summary>
    public enum HitValidationResult
    {
        /// <summary>
        /// The hit was validated.
        /// </summary>
        Valid,

        /// <summary>
        /// The hit contains no target.
        /// This value indicates a possible bug. 
        /// Be Cautious when depending on this.
        /// </summary>
        NoTarget,

        /// <summary>
        /// Hit target wasn't targetable.
        /// </summary>
        NotTargetable,

        /// <summary>
        /// The Projectile Controller invalidated the hit.
        /// </summary>
        ProjectileInvalidated,

        /// <summary>
        /// Target Character invalidated the hit.
        /// </summary>
        CharacterInvalidated
    }

    /// <summary>
    /// This interface provides basic properties of a hit context.
    /// </summary>
    public interface IHitContext : IContext
    {
        /// <summary>
        /// This property stores the target the hit occured.
        /// </summary>
        TargetUnion HitTarget { get; }

        /// <summary>
        /// This property stores the point which the hit occured.
        /// </summary>
        Vector3 HitPosition { get; }
        
        /// <summary>
        /// This property stores the orientation of the Projectile when the hit occured.
        /// </summary>
        Vector3 HitOrientation { get; }

        /// <summary>
        /// This property stores the last position change of the Projectile before the hit occurs.
        /// </summary>
        Vector3 LastChange { get; }

        /// <summary>
        /// This method should validate whether the context represents a valid hit.
        /// </summary>
        /// <returns>Whether the hit is valid or not.</returns>
        HitValidationResult Validate();
    }

    /// <summary>
    /// The HitContext is the concrete context for any projectile hit occurence.
    /// </summary>
    public class HitContext : Context, IHitContext
    {
        /// <summary>
        /// Creates a new HitContext from a parent projectile context and the position of the hit.
        /// </summary>
        /// <param name="parent">The parent context.</param
        /// <param name="target">The hit target.</param>>
        /// <param name="hitPosition">Position of the hit.</param>
        public HitContext(IProjectileContext parent, TargetUnion target, Vector3 hitPosition)
            : base(parent)
        {
            HitPosition = hitPosition;
            HitTarget = target;
            HitOrientation = parent.Projectile.LastMovement.normalized;
            LastChange = parent.Projectile.LastMovement;
            Identifier = "hit@" + parent.Identifier;
        }

        public IProjectileContext Parent {
            get {
                return base.Parent as IProjectileContext;
            }
        }

        public TargetUnion HitTarget { get; private set; }

        public Vector3 HitPosition { get; private set; }

        public Vector3 HitOrientation { get; private set; }

        public Vector3 LastChange { get; private set; }

        public HitValidationResult Validate()
        {
            if (HitTarget.Type == TargetType.Point)
                return HitValidationResult.Valid;

            Targetable hitObject = HitTarget.AsTargetable();

            if (HitTarget.Type == TargetType.None)
                return HitValidationResult.NoTarget;

            if (!hitObject.IsTargetable)
                return HitValidationResult.NotTargetable;

            if (!Parent.Projectile.Controller.Validate(hitObject))
                return HitValidationResult.ProjectileInvalidated;

            if (hitObject is Character && !(hitObject as Character).ValidateHit(this))
                return HitValidationResult.CharacterInvalidated;

            return HitValidationResult.Valid;
        }

        public static implicit operator ProjectileContext(HitContext d)
        {
            return d.Parent as ProjectileContext;
        }
    }
}
