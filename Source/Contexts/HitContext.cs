using Quark.Projectiles;
using Quark.Spells;
using UnityEngine;

namespace Quark.Contexts
{
    /// <summary>
    /// This interface provides basic properties of a hit context.
    /// </summary>
    public interface IHitContext : IProjectileContext
    {
        /// <summary>
        /// This property stores the point which the collision occured.
        /// </summary>
        Vector3 HitPosition { get; }
    }

    /// <summary>
    /// The HitContext is the concrete context for any projectile hit occurence.
    /// </summary>
    public class HitContext : Context, IHitContext
    {
        /// <summary>
        /// Creates a new HitContext from a parent projectile context and the position of the hit.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <param name="hitPosition">Position of the hit.</param>
        public HitContext(ProjectileContext parent, Vector3 hitPosition)
            : base(parent)
        {
            HitPosition = hitPosition;
        }

        public Vector3 HitPosition { get; private set; }

        public Spell Spell
        {
            get { return ((ProjectileContext) Parent).Spell; }
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

        public int HitCount
        {
            get { return ((ProjectileContext)Parent).HitCount; }
            set { ((ProjectileContext) Parent).HitCount = value; }
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
