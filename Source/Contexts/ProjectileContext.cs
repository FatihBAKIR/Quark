using Quark.Projectiles;
using Quark.Spells;
using Quark.Targeting;
using UnityEngine;

namespace Quark.Contexts
{
    /// <summary>
    /// This interface provides the basis properties of a context for a projectile.
    /// </summary>
    public interface IProjectileContext : ICastContext
    {
        /// <summary>
        /// The projectile object of this context.
        /// </summary>
        Projectile Projectile { get; }

        /// <summary>
        /// This property stores the time spent travelling by this context.
        /// </summary>
        float TravelTime { get; }

        /// <summary>
        /// This property stores the -approximate- distance travelled by this context.
        /// </summary>
        float TravelDistance { get; }

        /// <summary>
        /// This property stores the actual target of this context.
        /// </summary>
        TargetUnion Target { get; set; }

        /// <summary>
        /// This property stores the source's initial rotation at the time of this context's creation.
        /// </summary>
        Vector3 TravelBeginRotation { get; }

        /// <summary>
        /// This property stores the source's initial position at the time of this context's creation.
        /// </summary>
        Vector3 TravelBeginPosition { get; }

        /// <summary>
        /// This property stores the seconds since level load at the time of this context's creation.
        /// </summary>
        float TravelBeginTime { get; }

        /// <summary>
        /// This field stores the Y axis (ie. height) offset to properly hit the target in a proper point.
        /// </summary>
        Vector3 TargetOffset { get; }

        /// <summary>
        /// This method handles hitting event of its projectile. 
        /// </summary>
        /// <param name="target">The hit object.</param>
        void OnHit(TargetUnion target);

        /// <summary>
        /// This method handles the travel event of its projectile.
        /// </summary>
        void OnTravel();

        /// <summary>
        /// This property stores the count of successful hits in this context.
        /// </summary>
        int HitCount { get; set; }
    }

    public class ProjectileContext : Context, IProjectileContext
    {
        /// <summary>
        /// Creates a new ProjectileContext from a cast context and a projectile object.
        /// </summary>
        /// <param name="parent">Parent cast context.</param>
        /// <param name="projectile">The projectile object.</param>
        public ProjectileContext(ICastContext parent, Projectile projectile, TargetUnion target)
            : base(parent)
        {
            Parent = parent;
            Projectile = projectile;
            Target = target;
            TravelBeginRotation = Source.transform.rotation.eulerAngles;
            TravelBeginPosition = Source.transform.position;
            TravelBeginTime = Time.timeSinceLevelLoad;
        }

        public Projectile Projectile { get; protected set; }

        public float TravelTime
        {
            get
            {
                return Time.deltaTime - TravelBeginTime;
            }
        }

        public float TravelDistance
        {
            get
            {
                return Vector3.Distance(Projectile.transform.position, TravelBeginPosition);
            }
        }

        private TargetUnion _target;

        public TargetUnion Target
        {
            get { return _target; }
            set
            {
                _target = value;
                switch (Target.Type)
                {
                    case TargetType.Targetable:
                        TargetOffset = new Vector3(0, Target.Targetable.HeightOffset, 0);
                        break;
                    case TargetType.Character:
                        TargetOffset = new Vector3(0, Target.Character.HeightOffset, 0);
                        break;
                    default:
                        TargetOffset = Vector3.zero;
                        break;
                }
            }
        }

        public Vector3 TravelBeginRotation { get; protected set; }

        public Vector3 TravelBeginPosition { get; protected set; }

        public float TravelBeginTime { get; protected set; }

        public Vector3 TargetOffset { get; protected set; }

        public virtual void OnHit(TargetUnion target)
        {
            if (target.Type != TargetType.Point)
            {
                IHitContext hit = new HitContext(this, target, Projectile.transform.position);
                HitValidationResult validation = hit.Validate();
                if (validation == HitValidationResult.Valid)
                {
                    switch (target.Type)
                    {
                        case TargetType.Point:
                            Spell.OnHit(target.Point, hit);
                            break;
                        case TargetType.Targetable:
                            Spell.OnHit(target.Targetable, hit);
                            break;
                        case TargetType.Character:
                            Spell.OnHit(target.Character, hit);
                            break;
                    }

                    if ((Target.Type != TargetType.Point &&
                        target.AsTargetable().Equals(Target.AsTargetable())) ||
                        (Spell.TargetForm == TargetForm.Singular))
                    {
                        if (HitCount == 0)
                            Spell.OnMiss(this);

                        Collect();
                    }

                    HitCount++;
                }
            }
            else
                Collect();
        }

        void Collect()
        {
            Spell.CollectProjectile(Projectile);
            Object.Destroy(Projectile.gameObject);
        }

        public virtual void OnTravel()
        {
            Spell.OnTravel(Projectile.transform.position, this);     
        }

        public int HitCount { get; set; }

        public Spell Spell { get { return ((ICastContext)Parent).Spell; } }
        public CastStages Stage { get { return ((ICastContext)Parent).Stage; } }
        public TargetCollection Targets { get { return ((ICastContext)Parent).Targets; } }
        public int CastPercentage { get { return ((ICastContext)Parent).CastPercentage; } }
        public float CastTime { get { return ((ICastContext)Parent).CastTime; } }
        public float CastBeginTime { get { return ((ICastContext)Parent).CastBeginTime; } }
        public Vector3 CastBeginPosition { get { return ((ICastContext)Parent).CastBeginPosition; } }
        public void Interrupt() { ((ICastContext)Parent).Interrupt(); }
        public void Clear()
        {
            ((ICastContext)Parent).Clear();
        }
    }
}
