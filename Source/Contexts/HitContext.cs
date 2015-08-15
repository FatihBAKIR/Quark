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
    public interface IHitContext : IProjectileContext
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
            Identifier = "hit@" + parent.Identifier;
        }

        public TargetUnion HitTarget { get; private set; }

        public Vector3 HitPosition { get; private set; }

        public Vector3 HitOrientation { get; private set; }

        public HitValidationResult Validate()
        {
            if (HitTarget.Type == TargetType.Point)
                return HitValidationResult.Valid;

            Targetable hitObject = HitTarget.AsTargetable();

            if (HitTarget.Type == TargetType.None)
                return HitValidationResult.NoTarget;

            if (!hitObject.IsTargetable)
                return HitValidationResult.NotTargetable;

            if (!Projectile.Controller.Validate(hitObject))
                return HitValidationResult.ProjectileInvalidated;

            if (hitObject is Character && !(hitObject as Character).ValidateHit(this))
                return HitValidationResult.CharacterInvalidated;

            return HitValidationResult.Valid;
        }

        public Spell Spell
        {
            get { return ((IProjectileContext)Parent).Spell; }
        }

        public CastStages Stage
        {
            get { return ((IProjectileContext)Parent).Stage; }
        }

        public TargetCollection Targets
        {
            get { return ((IProjectileContext)Parent).Targets; }
        }

        public int CastPercentage
        {
            get { return ((IProjectileContext)Parent).CastPercentage; }
        }

        public float CastTime
        {
            get { return ((IProjectileContext)Parent).CastTime; }
        }

        public float CastBeginTime
        {
            get { return ((IProjectileContext)Parent).CastBeginTime; }
        }

        public Vector3 CastBeginPosition
        {
            get { return ((IProjectileContext)Parent).CastBeginPosition; }
        }

        public int CurrentProjectileCount
        {
            get { return ((IProjectileContext)Parent).CurrentProjectileCount; }
            set { ((IProjectileContext)Parent).CurrentProjectileCount = value; }
        }
        public int TotalProjectileCount
        {
            get { return ((IProjectileContext)Parent).TotalProjectileCount; }
            set { ((IProjectileContext)Parent).TotalProjectileCount = value; }
        }
        public Projectile Projectile
        {
            get { return ((IProjectileContext)Parent).Projectile; }
        }

        public float TravelTime
        {
            get { return ((IProjectileContext)Parent).TravelTime; }
        }

        public float TravelDistance
        {
            get { return ((IProjectileContext)Parent).TravelDistance; }
        }

        public TargetUnion Target
        {
            get { return ((IProjectileContext)Parent).Target; }
            set { ((IProjectileContext)Parent).Target = value; }
        }

        public Vector3 TravelBeginRotation
        {
            get { return ((IProjectileContext)Parent).TravelBeginRotation; }
        }

        public Vector3 TravelBeginPosition
        {
            get { return ((IProjectileContext)Parent).TravelBeginPosition; }
        }

        public float TravelBeginTime
        {
            get { return ((IProjectileContext)Parent).TravelBeginTime; }
        }

        public Vector3 TargetOffset
        {
            get { return ((IProjectileContext)Parent).TargetOffset; }
        }

        public void OnHit(TargetUnion target)
        {
            ((IProjectileContext)Parent).OnHit(target);
        }

        public void OnTravel()
        {
            ((IProjectileContext)Parent).OnTravel();
        }

        public int HitCount
        {
            get { return ((IProjectileContext)Parent).HitCount; }
            set { ((IProjectileContext)Parent).HitCount = value; }
        }

        public void Interrupt()
        {
            ((IProjectileContext)Parent).Interrupt();
        }

        public void Clear()
        {
            ((IProjectileContext)Parent).Clear();
        }
    }
}
