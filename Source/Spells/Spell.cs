using System.Diagnostics.CodeAnalysis;
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
        public virtual float CastDuration
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
        public float CastingInterval { get; protected set; }

        /// <summary>
        /// This field determines the interval of the OnTravel logic to run while a projectile belonging to this Spell is   traveling.
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

        /// <summary>
        /// Identifier of this Spell
        /// </summary>
        public string Identifier
        {
            get { return MakeID(this, Context); }
        }

        /// <summary>
        /// Generates an identifier for a given Spell in a given Context.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="context">The Context</param>
        /// <returns>Identifier</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string MakeID(Spell spell, Cast context)
        {
            return spell.Name + "@" + context.Caster.Identifier;
        }

        /// <summary>
        /// Generates an identifier for a given Spell casted by a given Character.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="caster">The Caster</param>
        /// <returns>Identifier</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string MakeID(Spell spell, Character caster)
        {
            return spell.Name + "@" + caster.Identifier;
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
        public bool IsInstant
        {
            get
            {
                return CastDuration <= 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this spell invokes projectiles.
        /// </summary>
        /// <value><c>true</c> if this instance has projectiles; otherwise, <c>false</c>.</value>
        public bool IsProjectiled
        {
            get
            {
                return ProjectileObject != null;
            }
        }

        /// <summary>
        /// The context this Spell instance resides in.
        /// </summary>
        protected Cast Context { get; private set; }

        /// <summary>
        /// Introduce the Cast context which is invoking this Spell instance.
        /// </summary>
        /// <param name="context">The Cast</param>
        public void SetContext(Cast context)
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
        protected virtual Condition InvokeCondition { get { return new Condition(); } }

        /// <summary>
        /// This method determines whether this Spell can be casted in the current Context.
        /// </summary>
        /// <returns>Whether this Spell can be casted.</returns>
        public virtual bool CanInvoke()
        {
            Condition invokeCondition = InvokeCondition;
            invokeCondition.SetContext(Context);
            return invokeCondition.Check(Context.Caster);
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
        protected virtual EffectCollection InvokeEffects { get { return new EffectCollection(); } }

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
        protected virtual EffectCollection TargetingDoneEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The casting began effects
        /// Effects within this list are applied after targeting done and casting began, these effects are not executed for instant spells
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection CastingBeginEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The casting effects
        /// Effects within this list are applied while the casting occurs
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection CastingEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The casting done effects
        /// Effects within this list are applied when the casting successfully finishe
        /// 
        /// Possible Targets:
        ///     + Target Characters
        ///     + Target Targetables
        ///     + Target Points
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection CastDoneEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The interruption effects
        /// Effects within this list are applied if the spell gets interrupted either by the caster or other Character
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection InterruptEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The traveling effects
        /// Effects within this list are applied while the projectiles of this Spell are traveling
        /// 
        /// Possible Targets:
        ///     + Points the projectiles pass from while travelling.
        ///       
        /// </summary>
        protected virtual EffectCollection TravelEffects { get { return new EffectCollection(); } }

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
        protected virtual EffectCollection HitEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The spell miss effects
        /// Effects within this list are applied when a projectile gets destroyed without hitting any characters.
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection MissEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// The finalizing effects
        /// Effects within this list are applied just before the spell gets collected by the GC.
        /// 
        /// Possible Targets:
        ///     + Void
        /// 
        /// </summary>
        protected virtual EffectCollection ClearEffects { get { return new EffectCollection(); } }

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

            if (IsProjectiled)
                CreateProjectiles();
            else
                OnFinal();
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
        public virtual void OnTravel(Vector3 position)
        {
            Logger.Debug("Spell.OnTravel");
            TravelEffects.Run(position, Context);
        }

        /// <summary>
        /// Executes the Hit logic on the given point for this Spell.
        /// </summary>
        /// <param name="position">Target of the hit.</param>>
        public virtual void OnHit(Vector3 position)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(position, Context);
        }

        /// <summary>
        /// Executes the Hit logic on the given Character for this Spell.
        /// </summary>
        /// <param name="character">Target of the hit.</param>>
        public virtual void OnHit(Character character)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(character, Context);
        }

        /// <summary>
        /// Executes the Hit logic on the given Targetable for this Spell.
        /// </summary>
        /// <param name="targetable">Target of the hit.</param>
        public virtual void OnHit(Targetable targetable)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(targetable, Context);
        }

        /// <summary>
        /// Executes the Miss logic for this Spell.
        /// </summary>
        public virtual void OnMiss()
        {
            Logger.Debug("Spell.OnMiss");
            MissEffects.Run(Context);
        }

        /// <summary>
        /// Executes the ClearEffects then purges itself from the Cast context it is associated with
        /// </summary>
        public virtual void OnFinal()
        {
            Logger.Debug("Spell.OnFinal");
            ClearEffects.Run(Context);

            Context.Clear(Stages.Done);
            Context = null;
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
        /// Invokes necessary projectiles for this spell
        /// </summary>
        protected void CreateProjectiles()
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
