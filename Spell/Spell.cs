using Quark.Missile;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spell
{

    public class Spell : ITaggable
    {
        /// <summary>
        /// Gets the duration of the cast.
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

        public virtual string Name
        {
            get
            {
                return "Spell";
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
        public virtual TargetForm TargetForm
        {
            get
            {
                return TargetForm.Singular;
            }
        }

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

        private void RunEffects(Effect[] Effects)
        {
            foreach (Effect effect in Effects)
            {
                effect.Data = Data;
                effect.Apply();
            }
        }

        private void RunEffects(Targetable[] Targets, Effect[] Effects)
        {
            foreach (Targetable target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = Data;
                    effect.Apply(target);
                }
        }

        private void RunEffects(Character[] Targets, Effect[] Effects)
        {
            foreach (Character target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = Data;
                    effect.Apply(target);
                }
        }

        private void RunEffects(Vector3[] Targets, Effect[] Effects)
        {
            foreach (Vector3 target in Targets)
                foreach (Effect effect in Effects)
                {
                    effect.Data = Data;
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
            RunEffects(BeginEffects);
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
            RunEffects(Data.TargetPoints, TargetingDoneEffects);
            RunEffects(Data.TargetCharacters, TargetingDoneEffects);
            RunEffects(Data.Targetables, TargetingDoneEffects);
            RunEffects(TargetingDoneEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastingBegan()
        {
            Logger.Debug("Spell.OnCastingBegan");
            RunEffects(CastingEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCastDone()
        {
            Logger.Debug("Spell.OnCastDone");

            RunEffects(CastDoneEffects);

            if (this.IsProjectiled)
                this.CreateProjectiles();
            else
            {
                RunEffects(Data.TargetPoints, CastDoneEffects);
                RunEffects(Data.TargetCharacters, CastDoneEffects);
                RunEffects(Data.Targetables, CastDoneEffects);

                OnFinal();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnTravel(Vector3 position)
        {
            RunEffects(new Vector3[] { position }, TravelEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Vector3 position)
        {
            RunEffects(new Vector3[] { position }, HitEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Character character)
        {
            RunEffects(new Character[] { character }, HitEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnHit(Targetable targetable)
        {
            RunEffects(new Targetable[] { targetable }, HitEffects);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMiss()
        {
            RunEffects(MissEffects);
        }

        /// <summary>
        /// Executes the ClearEffects then purges itself from the CastData it is associated with
        /// </summary>
        public virtual void OnFinal()
        {
            RunEffects(ClearEffects);

            this.Data = null;
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
            foreach (Vector3 point in Data.TargetPoints)
            {
                Missile_Count++;
                Missile.Missile.Make(this.MissileObject, this.Controller, this.Data).Set(point);
            }

            foreach (Character target in Data.TargetCharacters)
            {
                Missile_Count++;
                Missile.Missile.Make(this.MissileObject, this.Controller, this.Data).Set(target);
            }
        }
    }
}