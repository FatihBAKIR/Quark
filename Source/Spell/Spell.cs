using Quark.Missile;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spell
{

    public class Spell : ITaggable
    {
        /// <summary>
        /// Gets or Sets the duration of the cast.
        /// </summary>
        /// <value>
        /// The duration of the cast.
        /// </value>
        public virtual float CastDuration
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        /// <summary>
        /// Name of the Spell
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public virtual string[] Tags
        {
            get
            {
                return new string[] { "spell" };
            }
            set
            {
            }
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

        protected virtual GameObject MissileObject
        {
            get
            {
                return null;
            }
        }

        protected virtual MissileController Controller
        {
            get
            {
                return new MissileController();
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
                return this.MissileObject != null;
            }
        }

        protected CastData _data { get; set; }

        /// <summary>
        /// Introduce the CastData object which is invoking this Spell instance.
        /// </summary>
        /// <param name="data">Invoking CastData</param>
        public void Introduce(CastData data)
        {
            this._data = data;
        }

        #region Effect Holders

        /// <summary>
        /// The begin effects.
        /// Effects within this list are applied with the caster Character object argument when the castdata is initialized
        /// </summary>
        protected virtual EffectCollection BeginEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The targeting done effects.
        /// Effects within this list are applied with each of the target Character objects and Vector objects, depending on the type of the spell
        /// </summary>
        protected virtual EffectCollection TargetingDoneEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The casting began effects
        /// Effects within this list are applied after targeting done and casting began, these effects are not executed for instant spells
        /// </summary>
        protected virtual EffectCollection CastingEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The interruption effects
        /// Effects within this list are applied if the spell gets interrupted either by the caster or other Characters
        /// </summary>
        protected virtual EffectCollection InterruptEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The casting done effects
        /// Effects within this list are applied when the casting successfully finishes
        /// </summary>
        protected virtual EffectCollection CastDoneEffects { get { return new EffectCollection { }; } }

        /// <summary>
        /// The travelling effects
        /// Effects within this list are applied while the projectiles of this missile are travelling
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

        private void RunEffects(Effect[] Effects)
        {
            foreach (Effect effect in Effects)
            {
                effect.Data = _data;
                effect.Apply();
            }
        }

        private void RunEffects(Targetable[] Targets, Effect[] Effects)
        {
            foreach (Targetable target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = _data;
                    effect.Apply(target);
                }
        }

        private void RunEffects(Character[] Targets, Effect[] Effects)
        {
            foreach (Character target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = _data;
                    effect.Apply(target);
                }
        }

        private void RunEffects(Vector3[] Targets, Effect[] Effects)
        {
            foreach (Vector3 target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = _data;
                    effect.Apply(target);
                }
        }

        #region Effect Handlers

        /// <summary>
        /// Executes the spell cast beginning logic.
        /// </summary>
        public virtual void OnBegin()
        {
            /*
         * The default behavior of spell lifecycle steps are only about running preset effects with appropriate targets.
         */
            Logger.Debug("Spell.OnBegin");
            BeginEffects.Run(_data);
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
                .Run(_data.TargetPoints, _data)
                .Run(_data.TargetCharacters, _data)
                .Run(_data.Targetables, _data)
                .Run(_data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastingBegan()
        {
            Logger.Debug("Spell.OnCastingBegan");
            CastingEffects.Run(_data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastDone()
        {
            Logger.Debug("Spell.OnCastDone");

            CastDoneEffects.Run();

            if (this.IsProjectiled)
                this.CreateProjectiles();
            else
            {
                CastDoneEffects
                    .Run(_data.TargetPoints, _data)
                    .Run(_data.TargetCharacters, _data)
                    .Run(_data.Targetables, _data);

                OnFinal();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnTravel(Vector3 position)
        {
            TravelEffects.Run(position, _data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Vector3 position)
        {
            HitEffects.Run(position, _data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Character character)
        {
            HitEffects.Run(character, _data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Targetable targetable)
        {
            HitEffects.Run(targetable, _data);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMiss()
        {
            MissEffects.Run(_data);
        }

        /// <summary>
        /// Executes the ClearEffects then purges itself from the CastData it is associated with
        /// </summary>
        public virtual void OnFinal()
        {
            ClearEffects.Run(_data);

            this._data = null;
        }

        #endregion

        /// <summary>
        /// Collects a projectile which were created by this spell
        /// </summary>
        /// <param name="m">Missile to collect.</param>
        public void CollectProjectile(Missile.Missile m)
        {
            Logger.Debug("Collecting Projectile");
            Missile_Count--;
            if (Missile_Count == 0)
                OnFinal();
        }

        uint Missile_Count = 0;

        /// <summary>
        /// Invokes necessary projectiles for this spell
        /// </summary>
        protected virtual void CreateProjectiles()
        {
            foreach (Vector3 point in _data.TargetPoints)
            {
                Missile_Count++;
                Missile.Missile.Make(this.MissileObject, this.Controller, this._data).Set(point);
            }

            foreach (Character target in _data.TargetCharacters)
            {
                Missile_Count++;
                Missile.Missile.Make(this.MissileObject, this.Controller, this._data).Set(target);
            }
        }
    }
}