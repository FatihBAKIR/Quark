﻿using Quark.Projectiles;
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
        /// Projectile should continue its movement.
        /// </summary>
        ProjectileInvalidated,

        /// <summary>
        /// Target Character invalidated the hit.
        /// Projectile shouldn't continue moving.
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
        public HitContext(ProjectileContext parent, TargetUnion target, Vector3 hitPosition)
            : base(parent)
        {
            HitPosition = hitPosition;
            HitTarget = target;
        }

        public TargetUnion HitTarget { get; private set; }

        public Vector3 HitPosition { get; private set; }

        public HitValidationResult Validate()
        {
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
            get { return ((ProjectileContext)Parent).Spell; }
        }

        public CastStages Stage
        {
            get { return ((ProjectileContext)Parent).Stage; }
        }

        public TargetCollection Targets
        {
            get { return ((ProjectileContext)Parent).Targets; }
        }

        public int CastPercentage
        {
            get { return ((ProjectileContext)Parent).CastPercentage; }
        }

        public float CastTime
        {
            get { return ((ProjectileContext)Parent).CastTime; }
        }

        public float CastBeginTime
        {
            get { return ((ProjectileContext)Parent).CastBeginTime; }
        }

        public Vector3 CastBeginPosition
        {
            get { return ((ProjectileContext)Parent).CastBeginPosition; }
        }

        public Projectile Projectile
        {
            get { return ((ProjectileContext)Parent).Projectile; }
        }

        public float TravelTime
        {
            get { return ((ProjectileContext)Parent).TravelTime; }
        }

        public float TravelDistance
        {
            get { return ((ProjectileContext)Parent).TravelDistance; }
        }

        public TargetUnion Target
        {
            get { return ((ProjectileContext)Parent).Target; }
            set { ((ProjectileContext)Parent).Target = value; }
        }

        public Vector3 TravelBeginRotation
        {
            get { return ((ProjectileContext)Parent).TravelBeginRotation; }
        }

        public Vector3 TravelBeginPosition
        {
            get { return ((ProjectileContext)Parent).TravelBeginPosition; }
        }

        public float TravelBeginTime
        {
            get { return ((ProjectileContext)Parent).TravelBeginTime; }
        }

        public Vector3 TargetOffset
        {
            get { return ((ProjectileContext)Parent).TargetOffset; }
        }

        public void OnHit(TargetUnion target)
        {
            ((ProjectileContext)Parent).OnHit(target);
        }

        public void OnTravel()
        {
            ((ProjectileContext)Parent).OnTravel();
        }

        public int HitCount
        {
            get { return ((ProjectileContext)Parent).HitCount; }
            set { ((ProjectileContext)Parent).HitCount = value; }
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
