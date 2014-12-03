using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quark
{

    public class Spell
    {
        /// <summary>
        /// The duration of the casting.
        /// </summary>
        protected virtual float castDuration
        {
            get
            {
                return 1;
            }
        }

        public virtual string Name
        {
            get
            {
                return "Spell";
            }
        }

        protected virtual TargetType targetables
        {
            get
            {
                return TargetType.Enemy;
            }
        }

        protected virtual TargetForm targetForm
        {
            get
            {
                return TargetForm.Singular;
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

        //
        // Properties
        //

        /// <summary>
        /// Gets the duration of the cast.
        /// </summary>
        /// <value>
        /// The duration of the cast.
        /// </value>
        public float CastDuration
        {
            get
            {
                return this.castDuration;
            }
        }

        /// <summary>
        /// Gets the possible target kinds for this spell
        /// </summary>
        public TargetType Targetables
        {
            get
            {
                return this.targetables;
            }
        }

        /// <summary>
        /// Gets the target form of this spell
        /// </summary>
        public TargetForm TargetForm
        {
            get
            {
                return this.targetForm;
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
                return castDuration <= 0;
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

        protected CastData Data { get; set; }

        public void SetData(ref CastData data)
        {
            this.Data = data;
        }

        #region Effect Holders

        /// <summary>
        /// The begin effects.
        /// Effects within this list are applied with the caster Character object argument when the castdata is initialized
        /// </summary>
        protected virtual Effect[] BeginEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The targeting done effects.
        /// Effects within this list are applied with each of the target Character objects and Vector objects, depending on the type of the spell
        /// </summary>
        protected virtual Effect[] TargetingDoneEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The casting began effects
        /// Effects within this list are applied after targeting done and casting began, these effects are not executed for instant spells
        /// </summary>
        protected virtual Effect[] CastingEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The interruption effects
        /// Effects within this list are applied if the spell gets interrupted either by the caster or other Characters
        /// </summary>
        protected virtual Effect[] InterruptEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The casting done effects
        /// Effects within this list are applied when the casting successfully finishes
        /// </summary>
        protected virtual Effect[] CastDoneEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The travelling effects
        /// Effects within this list are applied while the projectiles of this missile are travelling
        /// </summary>
        protected virtual Effect[] TravelEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The on-hit effects
        /// Effects within this list are applied when a successful hit occurs
        /// </summary>
        protected virtual Effect[] HitEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The spell miss effects
        /// Effects within this list are applied when a projectile gets destroyed without hitting any characters
        /// </summary>
        protected virtual Effect[] MissEffects { get { return new Effect[] { }; } }

        /// <summary>
        /// The finalizing effects
        /// Effects within this list are applied just before the spell gets collected by the GC
        /// </summary>
        protected virtual Effect[] ClearEffects { get { return new Effect[] { }; } }

        #endregion

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
            foreach (Effect effect in this.BeginEffects)
            {
                effect.Data = Data;
                effect.Apply();
            }
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
            foreach (Vector3 targetp in Data.TargetPoints)
                foreach (Effect effect in this.TargetingDoneEffects)
                {
                    effect.Data = Data;
                    effect.Apply(targetp);
                }
            foreach (Character target in Data.TargetCharacters)
                foreach (Effect effect in this.TargetingDoneEffects)
                {
                    effect.Data = Data;
                    effect.Apply(target);
                }
            foreach (Effect effect in this.TargetingDoneEffects)
            {  
                effect.Data = Data;
                effect.Apply();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastingBegan()
        {
            Logger.Debug("Spell.OnCastingBegan");
            foreach (Effect effect in this.CastingEffects)
            {
                effect.Data = Data;
                effect.Apply();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastDone()
        {
            Logger.Debug("Spell.OnCastDone");
            foreach (Effect effect in this.CastDoneEffects)
            {
                effect.Data = Data;
                effect.Apply();
            }
            if (this.IsProjectiled)
                this.CreateProjectiles();
            else
            {
                foreach (Vector3 targetp in Data.TargetPoints)
                    foreach (Effect effect in this.CastDoneEffects)
                    {
                        effect.Data = Data;
                        effect.Apply(targetp);
                    }
                foreach (Character target in Data.TargetCharacters)
                    foreach (Effect effect in this.CastDoneEffects)
                    {
                        effect.Data = Data;
                        effect.Apply(target);
                    }
                OnFinal();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnTravel(Vector3 position)
        {
            foreach (Effect effect in this.TravelEffects)
            {
                effect.Data = Data;
                effect.Apply(position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Vector3 point)
        {
            foreach (Effect effect in this.HitEffects)
            {
                effect.Data = Data;
                effect.Apply(point);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Character character)
        {
            foreach (Effect effect in this.HitEffects)
            {
                effect.Data = Data;
                effect.Apply(character);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMiss()
        {
            foreach (Effect effect in this.MissEffects)
            {
                effect.Data = Data;
                effect.Apply();
            }
        }

        /// <summary>
        /// Executes the ClearEffects then purges itself from the CastData it is associated with
        /// </summary>
        public virtual void OnFinal()
        {
            foreach (Effect effect in this.ClearEffects)
            {
                effect.Data = Data;
                effect.Apply();
            }
            this.Data.Clear();
            this.Data = null;
        }

        #endregion

        /// <summary>
        /// Collects a projectile which were created by this spell
        /// </summary>
        /// <param name="m">Missile to collect.</param>
        public void CollectProjectile(Missile m)
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
            foreach (Vector3 point in Data.TargetPoints)
            {
                Missile_Count++;
                Missile.Make(this.MissileObject, this.Controller, this.Data).Set(point);
            }

            foreach (Character target in Data.TargetCharacters)
            {
                Missile_Count++;
                Missile.Make(this.MissileObject, this.Controller, this.Data).Set(target);
            }
        }
    }

}