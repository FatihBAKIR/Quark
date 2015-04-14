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

        public string Identifier
        {
            get { return MakeID(this, Context); }
        }

        public static string MakeID(Spell spell, Cast context)
        {
            return spell.Name + "@" + context.Caster.Identifier;
        }

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

        protected virtual GameObject ProjectileObject
        {
            get
            {
                return null;
            }
        }

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

        protected Cast Context { get; private set; }

        /// <summary>
        /// Introduce the Cast context which is invoking this Spell instance.
        /// </summary>
        /// <param name="context">The Cast</param>
        public void SetContext(Cast context)
        {
            Context = context;
        }

        protected virtual Condition InvokeCondition { get { return new Condition(); } }

        public bool CanInvoke()
        {
            InvokeCondition.SetContext(Context);
            return InvokeCondition.Check(Context.Caster);
        }

        #region Effect Collections

        /// <summary>
        /// The begin effects.
        /// Effects within this list are applied with the caster Character object argument when the castdata is initialized
        /// </summary>
        protected virtual EffectCollection InvokeEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The targeting done effects.
        /// Effects within this list are applied with each of the target Character objects and Vector objects, depending on the type of the spell
        /// </summary>
        protected virtual EffectCollection TargetingDoneEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The casting began effects
        /// Effects within this list are applied after targeting done and casting began, these effects are not executed for instant spells
        /// </summary>
        protected virtual EffectCollection CastingBeginEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The casting effects
        /// Effects within this list are applied while the casting occurs
        /// </summary>
        protected virtual EffectCollection CastingEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The casting done effects
        /// Effects within this list are applied when the casting successfully finishes
        /// </summary>
        protected virtual EffectCollection CastDoneEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The interruption effects
        /// Effects within this list are applied if the spell gets interrupted either by the caster or other Characters
        /// </summary>
        protected virtual EffectCollection InterruptEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The traveling effects
        /// Effects within this list are applied while the projectiles of this Projectile are traveling
        /// </summary>
        protected virtual EffectCollection TravelEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The on-hit effects
        /// Effects within this list are applied when a successful hit occurs
        /// </summary>
        protected virtual EffectCollection HitEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The spell miss effects
        /// Effects within this list are applied when a projectile gets destroyed without hitting any characters
        /// </summary>
        protected virtual EffectCollection MissEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The finalizing effects
        /// Effects within this list are applied just before the spell gets collected by the GC
        /// </summary>
        protected virtual EffectCollection ClearEffects { get { return new EffectCollection { }; } }

        #endregion

        #region Effect Handlers

        /// <summary>
        /// Executes the spell cast beginning logic.
        /// </summary>
        public virtual void OnInvoke()
        {
            /*
         * The default behavior of spell lifecycle steps are only about running preset effects with appropriate targets.
         */
            Logger.Debug("Spell.OnInvoke");
            InvokeEffects.Run(Context);
        }

        /// <summary>
        /// Executes the spell targeting done logic.
        /// </summary>
        public virtual void OnTargetingDone()
        {
            /*
         * The default behavior of spell lifecycle steps are only about running preset effects with appropriate targets.
         */
            Logger.Debug("Spell.OnTargetingDone");
            TargetingDoneEffects
                .Run(Context.Targets.Points, Context)
                .Run(Context.Targets.Characters, Context)
                .Run(Context.Targets.Targetables, Context)
                .Run(Context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastingBegan()
        {
            Logger.Debug("Spell.OnCastingBegan");
            CastingBeginEffects.Run(Context);
        }

        public virtual void OnCasting()
        {
            Logger.Debug("Spell.OnCasting");
            CastingEffects.Run(Context);
        }

        /// <summary>
        /// 
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

        public virtual void OnInterrupt()
        {
            Logger.Debug("Spell.OnCastDone");

            InterruptEffects.Run(Context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnTravel(Vector3 position)
        {
            Logger.Debug("Spell.OnTravel");
            TravelEffects.Run(position, Context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Vector3 position)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(position, Context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Character character)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(character, Context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Targetable targetable)
        {
            Logger.Debug("Spell.OnHit");
            HitEffects.Run(targetable, Context);
        }

        /// <summary>
        /// 
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

        uint _onAirMissileCount = 0;

        /// <summary>
        /// Invokes necessary projectiles for this spell
        /// </summary>
        protected virtual void CreateProjectiles()
        {
            foreach (Vector3 point in Context.Targets.Points)
            {
                _onAirMissileCount++;
                Projectile.Make(ProjectileObject, Controller, Context).Set(point);
            }

            foreach (Character target in Context.Targets.Characters)
            {
                _onAirMissileCount++;
                Projectile.Make(ProjectileObject, Controller, Context).Set(target);
            }

            foreach (Targetable target in Context.Targets.Targetables)
            {
                _onAirMissileCount++;
                Projectile.Make(ProjectileObject, Controller, Context).Set(target);
            }
        }

        #region Tagging
        public StaticTags Tags { get; protected set; }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }
}
