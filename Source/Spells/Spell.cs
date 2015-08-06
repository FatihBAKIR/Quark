using System;
using Quark.Conditions;
using Quark.Contexts;
using Quark.Effects;
using Quark.Projectiles;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spells
{
    public class Spell : ITagged, Identifiable
    {
#if DEBUG
        ~Spell()
        {
            Logger.GC("Spell::dtor");
        }
#endif

        /// <summary>
        /// Gets or Sets the duration of the cast.
        /// </summary>
        /// <value>
        /// The duration of the cast.
        /// </value>
        [Obsolete("CastDuration is deprecated. Use MaxCastDuration or MinCastDuration instead.", true)]
        public virtual float CastDuration
        {
            get { return MaxCastDuration; }
            set { MaxCastDuration = value; }
        }

        /// <summary>
        /// This property stores the cast order type of this Spell.
        /// </summary>
        public virtual CastOrder CastOrder { get; set; }

        private float _minCastDuration = -1;

        /// <summary>
        /// This property stores the least amount of time in seconds for a cast of this Spell to be considered successful.
        /// </summary>
        public virtual float MinCastDuration
        {
            get { return _minCastDuration < 0 ? MaxCastDuration : _minCastDuration; }
            set { _minCastDuration = value; }
        }

        /// <summary>
        /// This property stores the maximum amount of casting time in seconds for this Spell.
        /// 
        /// <remarks>Setting this property to 0 makes this spell a variable casted Spell.</remarks>
        /// </summary>
        public virtual float MaxCastDuration
        {
            get;
            set;
        }

        /// <summary>
        /// This field determines the interval of the OnCasting logic to run while this Spell is being casted.
        /// </summary>
        /// <value>
        /// Interval in seconds.
        /// </value>
        public virtual float CastingInterval { get; protected set; }

        /// <summary>
        /// This field determines the interval of the OnTravel logic to run while a projectile belonging to this Spell is traveling.
        /// </summary>
        /// <value>
        /// Interval in distance units.
        /// </value> 
        public virtual float TravelingInterval { get; protected set; }

        /// <summary>
        /// Name of the Spell
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>s
        /// Identifier of this Spell
        /// </summary>
        public string Identifier
        {
            get { return Name; }
        }

        /// <summary>
        /// Gets the target form of this spell
        /// </summary>
        /// <value>The target form.</value>
        public virtual TargetForm TargetForm
        {
            get
            {
                return TargetForm.Singular;
            }
        }

        /// <summary>
        /// Gets the target macro of this Spell
        /// </summary>
        /// <value>The target macro.</value>
        public virtual TargetMacro TargetMacro
        {
            get
            {
                return new TargetMacro();
            }
        }

        /// <summary>
        /// This property stores the GameObject for the projectiles of this Spell.
        /// If this property returns null, the Projectile stage of this Spell will be skipped.
        /// Necessary components will be added by Quark upon instantiation on the projectiles.
        /// </summary>
        protected virtual GameObject ProjectileObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// This property stores the ProjectileController for the projectiles of this Spell.
        /// </summary>
        protected virtual ProjectileController Controller
        {
            get
            {
                return new ProjectileController();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this spell is instant.
        /// </summary>
        /// <value><c>true</c> if this spell is instant; otherwise, <c>false</c>.</value>
        public virtual bool IsInstant
        {
            get
            {
                return MaxCastDuration <= 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this spell invokes projectiles.
        /// </summary>
        /// <value><c>true</c> if this instance has projectiles; otherwise, <c>false</c>.</value>
        public virtual bool IsProjectiled
        {
            get
            {
                return ProjectileObject != null;
            }
        }

        /// <summary>
        /// The CastContext this Spell is being casetd in.
        /// </summary>
        protected ICastContext Context { get; private set; }

        /// <summary>
        /// Sets the CastContext this Spell is being casted in.
        /// </summary>
        /// <param name="context">The CastContext.</param>
        public virtual void SetContext(ICastContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The invoke conditions.
        /// Conditions in this collection are checked to determine whether this Spell can be casted.
        /// 
        /// Possible Targets:
        ///     + Caster Character 
        /// 
        /// </summary>
        protected virtual ConditionCollection<ICastContext> InvokeConditions { get { return new ConditionCollection<ICastContext>(); } }

        /// <summary>
        /// The interrupt conditions.
        /// Conditions in this collection are checked to determine whether this cast should be interrupted or not.
        /// 
        /// Possible Targets:
        ///     + Caster Character 
        /// 
        /// </summary>
        protected virtual ConditionCollection<ICastContext> InterruptConditions { get { return new ConditionCollection<ICastContext> { new FalseCondition() }; } }

        /// <summary>
        /// This method should determine whether the cast for this Spell should be interrupted or not.
        /// </summary>
        /// <returns>Whether this Spell cast should be interrupted.</returns>
        public virtual bool CheckInterrupt()
        {
            ConditionCollection<ICastContext> interruptConditions = InterruptConditions;
            interruptConditions.SetContext(Context);
            return interruptConditions.Check(Context.Source);
        }

        /// <summary>
        /// This method determines whether this Spell can be casted in the current CastContext.
        /// </summary>
        /// <returns>Whether this Spell can be casted.</returns>
        public virtual bool CanInvoke()
        {
            Condition<ICastContext> invokeCondition = InvokeConditions;
            invokeCondition.SetContext(Context);
            return invokeCondition.Check(Context.Source);
        }

        #region Effect Collections

        /// <summary>
        /// The begin effects.
        /// Effects within this list are applied with the caster Character object argument when the castdata is initialized
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> InvokeEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The targeting done effects.
        /// Effects within this list are applied with each of the target Character objects and Vector objects, depending on the type of the spell
        /// 
        /// Possible Targets:
        ///     + Target Characters
        ///     + Target Targetables
        ///     + Target Points
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> TargetingDoneEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The casting began effects
        /// Effects within this list are applied after targeting done and casting began, these effects are not executed for instant spells
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> CastingBeginEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The casting effects
        /// Effects within this list are applied while the casting occurs
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> CastingEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The casting done effects
        /// Effects within this list are applied when the casting successfully finishes
        /// 
        /// Possible Targets:
        ///     + Target Characters
        ///     + Target Targetables
        ///     + Target Points
        ///     + Void
        /// 
        /// Notice:
        ///     In the CastFirst ordering, only the void overload of Apply will be called.
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> CastDoneEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The interruption effects
        /// Effects within this list are applied if the spell gets interrupted either by the caster or other Character
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> InterruptEffects { get { return new EffectCollection<ICastContext>(); } }

        /// <summary>
        /// The traveling effects
        /// Effects within this list are applied while the projectiles of this Spell are traveling
        /// 
        /// Possible Targets:
        ///     + Points the projectiles pass from while travelling.
        ///       
        /// </summary>
        protected virtual EffectCollection<ProjectileContext> TravelEffects { get { return new EffectCollection<ProjectileContext>(); } }

        /// <summary>
        /// The on-hit effects
        /// Effects within this list are applied when a successful hit occurs.
        /// 
        /// Possible Targets:
        ///     + Target Characters
        ///     + Target Targetables
        ///     + Target Points
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<IHitContext> HitEffects { get { return new EffectCollection<IHitContext>(); } }

        /// <summary>
        /// The spell miss effects
        /// Effects within this list are applied when a projectile gets destroyed without hitting any characters.
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<IProjectileContext> MissEffects { get { return new EffectCollection<IProjectileContext>(); } }

        /// <summary>
        /// The finalizing effects
        /// Effects within this list are applied just before the spell gets collected by the GC.
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection<ICastContext> ClearEffects { get { return new EffectCollection<ICastContext>(); } }

        #endregion

        #region Effect Handlers

        /// <summary>
        /// Executes the spell cast beginning logic of this Spell without a target.
        /// </summary>
        public virtual void OnInvoke()
        {
            Logger.Debug("Spell.OnInvoke");
            InvokeEffects.Run(Context);
        }

        /// <summary>
        /// Executes the Targeting Done logic of this Spell on every possible target.
        /// </summary>
        public virtual void OnTargetingDone()
        {
            Logger.Debug("Spell.OnTargetingDone");
            TargetingDoneEffects
                .Run(Context.Targets.Points, Context)
                .Run(Context.Targets.Characters, Context)
                .Run(Context.Targets.Targetables, Context)
                .Run(Context);
        }

        /// <summary>
        /// Executes the Casting Begin logic of this Spell without a target.
        /// </summary>
        public virtual void OnCastingBegan()
        {
            Logger.Debug("Spell.OnCastingBegan");
            CastingBeginEffects.Run(Context);
        }

        /// <summary>
        /// Executes the Casting logic of this Spell without a target.
        /// <remarks>This method is called every frame while casting.</remarks>
        /// </summary>
        public virtual void OnCasting()
        {
            Logger.Debug("Spell.OnCasting");
            CastingEffects.Run(Context);
        }

        /// <summary>
        /// Executes the Casting Done logic of this Spell on every possible target.
        /// </summary>
        public virtual void OnCastDone()
        {
            Logger.Debug("Spell.OnCastDone");

            CastDoneEffects
                .Run(Context)
                .Run(Context.Targets.Points, Context)
                .Run(Context.Targets.Characters, Context)
                .Run(Context.Targets.Targetables, Context);
        }

        /// <summary>
        /// Executes the Interruption logic of this Spell without a target.
        /// </summary>
        public virtual void OnInterrupt()
        {
            Logger.Debug("Spell.OnInterrupt");

            InterruptEffects.Run(Context);
        }

        /// <summary>
        /// Executes the Travel logic on thge position of the projectile this stage was triggered from. 
        /// </summary>
        /// <param name="position">Target of the hit.</param>
        public virtual void OnTravel(Vector3 position, ProjectileContext context)
        {
            Logger.Debug("Spell.OnTravel");
            TravelEffects.Run(position, context);
        }

        /// <summary>
        /// Executes the Hit logic on the given point for this Spell.
        /// </summary>
        /// <param name="position">Target of the hit.</param>>
        /// <param name="context">The context for this hit.</param>
        public virtual void OnHit(Vector3 position, IHitContext context)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(position, context);
        }

        /// <summary>
        /// Executes the Hit logic on the given Character for this Spell.
        /// </summary>
        /// <param name="character">Target of the hit.</param>>
        /// <param name="context">The context for this hit.</param>
        public virtual void OnHit(Character character, IHitContext context)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(character, context);
        }

        /// <summary>
        /// Executes the Hit logic on the given Targetable for this Spell.
        /// </summary>
        /// <param name="targetable">Target of the hit.</param>
        /// <param name="context">The context for this hit.</param>
        public virtual void OnHit(Targetable targetable, IHitContext context)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(targetable, context);
        }

        /// <summary>
        /// Executes the Miss logic for this Spell.
        /// </summary>
        public virtual void OnMiss(IProjectileContext context)
        {
            Logger.Debug("Spell.OnMiss");
            MissEffects.Run(context);
        }

        /// <summary>
        /// Executes the ClearEffects then purges itself from the Cast context it is associated with
        /// </summary>
        public virtual void OnFinal()
        {
            Logger.Debug("Spell.OnFinal");
            ClearEffects.Run(Context);

            SetContext(null);
        }

        #endregion

        /// <summary>
        /// Collects a projectile which were created by this spell
        /// </summary>
        /// <param name="projectile">Projectile to collect.</param>
        public void CollectProjectile(Projectile projectile)
        {
            Logger.Debug("Collecting Projectile");
            _onAirMissileCount--;
            if (_onAirMissileCount == 0)
                OnFinal();
        }

        /// <summary>
        /// This field stores the current travelling missiles originating from this Spell.
        /// </summary>
        uint _onAirMissileCount;

        /// <summary>
        /// Creates necessary projectiles for this spell
        /// </summary>
        public virtual void CreateProjectiles()
        {
            foreach (TargetUnion target in Context.Targets)
            {
                Projectile.Make(ProjectileObject, Controller, Context, target);
                _onAirMissileCount++;
            }
        }

        #region Tagging

        /// <summary>
        /// This property stores the static tags of this Spell.
        /// </summary>
        public StaticTags Tags { get; protected set; }

        /// <summary>
        /// This method determines whether this Spell is tagged with a certain string or not.
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <returns>Whether this spell is tagged or not.</returns>
        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }
}
