using Quark.Spells;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Contexts
{
    /// <summary>
    /// This interface provides basic getters for CastContext compatible classes.
    /// </summary>
    public interface ICastContext : IContext
    {
        /// <summary>
        /// The Spell object being casted in this Cast Context.
        /// </summary>
        Spell Spell { get; }

        /// <summary>
        /// This property stores the current stage of this CastContext.
        /// </summary>
        CastStages Stage { get; }

        /// <summary>
        /// This property stores all of the Targets acquired by this CastContext.
        /// </summary>
        TargetCollection Targets { get; }

        /// <summary>
        /// Gets the cast done percentage, respective to the minimum casting time.
        /// </summary>
        int CastPercentage { get; }

        /// <summary>
        /// This property gets the time spent casting up to now.
        /// </summary>
        float CastTime { get; }

        /// <summary>
        /// This property stores the time this CastContext has transitioned to the Casting stage.
        /// </summary>
        float CastBeginTime { get; }

        /// <summary>
        /// This property stores the position this CastContext has transitioned to the Casting stage.
        /// </summary>
        Vector3 CastBeginPosition { get; }

        int ProjectilesOnAir { get; set; }

        /// <summary>
        /// Interrupts this cast context.
        /// Can only cancel the casts in the Casting stage.
        /// </summary>
        void Interrupt();

        /// <summary>
        /// Executes the clearing logic for this context.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// CastContexts provide state for a Spell cast.
    /// </summary>
    public class CastContext : Context, ICastContext
    {
        /// <summary>
        /// Creates a new CastContext instance from a caster Character and a Spell to be casted.
        /// </summary>
        /// <param name="caster">The caster Character object.</param>
        /// <param name="spell">The Spell object to be casted.</param>
        public CastContext(Character caster, Spell spell)
            : base(caster.Context)
        {
            Spell = spell;
            Spell.SetContext(this);
            Identifier = Spell.Identifier + "@" + caster.Identifier;
            Initialize();
        }

        public Spell Spell
        {
            get;
            private set;
        }

        public CastStages Stage
        {
            get;
            private set;
        }

        public TargetCollection Targets
        {
            get;
            private set;
        }

        public Vector3 CastBeginPosition
        {
            get;
            private set;
        }

        public float CastBeginTime
        {
            get;
            private set;
        }

        public float CastTime
        {
            get
            {
                return _lastCast - CastBeginTime;
            }
        }

        public int CastPercentage
        {
            get
            {
                if (Spell.MinCastDuration <= 0) return 100;
                return (int)(CastTime * 100 / Spell.MinCastDuration);
            }
        }

        public int ProjectilesOnAir { get; set; }

        /// <summary>
        /// This method executes the initialization logic of this CastContext and the Spell being casted.
        /// </summary>
        void Initialize()
        {
            Stage = CastStages.Initialization;

            if (!Source.CanCast(Spell))
            {
                //Messenger<ICastContext>.Broadcast("CasterBusy", this);
                return;
            }

            if (!Spell.CanInvoke())
            {
                //Messenger<ICastContext>.Broadcast("CannotCast", this);
                return;
            }

            Source.AddCast(this);

            Spell.OnInvoke();

            Targets = new TargetCollection();

            //Messenger<ICastContext>.Broadcast("Cast.Initialize", this);

            if (Spell.CastOrder == CastOrder.TargetFirst)
                BeginTargeting();
            else
                PreCasting();
        }

        /// <summary>
        /// Begin targeting logic
        /// </summary>
        void BeginTargeting()
        {
            Stage = CastStages.Targeting;

            TargetMacro macro = Spell.TargetMacro;

            macro.SetContext(this);

            macro.TargetingSuccess += delegate(TargetCollection targets)
            {
                Targets.AddRange(targets);
                //Messenger<ICastContext>.Broadcast("Cast.TargetingSuccess", this);
                PostTargeting();
                macro = null;
            };

            macro.TargetingFailed += delegate(TargetingError error)
            {
                Stage = CastStages.TargetingFailed;
                //Messenger<ICastContext, TargetingError>.Broadcast("Cast.TargetingFailed", this, error);
                macro = null;
                Clear();
            };

            //Messenger<ICastContext>.Broadcast("Cast.BeginTargeting", this);

            macro.Run();
        }

        void PostTargeting()
        {
            Spell.OnTargetingDone();

            if (Spell.CastOrder == CastOrder.TargetFirst)
            {
                PreCasting();
            }
        }

        private ConditionCollection<ICastContext> _interruptConditions;
        private float _lastCast;
        void PreCasting()
        {
            Stage = CastStages.PreCasting;

            CastBeginTime = Time.timeSinceLevelLoad;
            CastBeginPosition = Source.transform.position;
            _lastCast = Time.timeSinceLevelLoad;

            if (Spell.IsInstant)
            {
                PostCasting();
                return;
            }

            //Messenger<ICastContext>.Broadcast("Cast.CastingBegin", this);

            Spell.OnCastingBegan();
            _interruptConditions = Source.InterruptConditions.DeepCopy();  // Store the interrupt conditions on a member field...

            QuarkMain.GetInstance().OnUpdate.Add(Casting);
            //Messenger.AddListener("Update", Casting);
        }

        void Casting()
        {
            if (CheckInterrupt() || CastTime >= Spell.MaxCastDuration)
            {
                PostCasting();
                return;
            }

            //Messenger<ICastContext>.Broadcast("Cast.CastingTick", this);

            if (Time.timeSinceLevelLoad > _lastCast + Spell.CastingInterval)
            {
                Spell.OnCasting();
                _lastCast = Time.timeSinceLevelLoad;
            }
        }

        bool CheckInterrupt()
        {
            _interruptConditions.SetContext(this);
            return _interruptConditions.Check() || Spell.CheckInterrupt();
        }

        void PostCasting()
        {
            if (!Spell.IsInstant)
            {
                //Messenger.RemoveListener("Update", Casting);
                QuarkMain.GetInstance().OnUpdate.Remove(Casting);
            }

            if (CastPercentage >= 100)
            {
                //Messenger<ICastContext>.Broadcast("Cast.CastSuccess", this);

                // Cast is successful, run the cast success event.
                CastSuccess();

                if (Spell.CastOrder != CastOrder.TargetFirst)
                {
                    BeginTargeting();
                }
            }
            else
            {
                //Messenger<ICastContext>.Broadcast("Cast.CastInterrupt", this);

                // Cast got interrupted somehow. Run the interruption event.
                Interrupt();
            }

            Clear();
        }

        void CastSuccess()
        {
            Stage = CastStages.CastSuccess;
            Spell.OnCastDone();
        }

        public void Interrupt()
        {
            Stage = CastStages.CastFail;
            Spell.OnInterrupt();
        }

        void BeginProjectiles()
        {
            if (Spell is ProjectiledSpell)
                ((ProjectiledSpell)Spell).CreateProjectiles();
            else
                Spell.OnFinal();

            Clear();
        }

        public void Clear()
        {
            Source.ClearCast(this);
            _interruptConditions = null;
        }
    }


    /// <summary>
    /// This enumeration represents the state of a CastContext.
    /// </summary>
    public enum CastStages
    {
        /// <summary>
        /// The cast is invalid.
        /// </summary>
        Null,
        /// <summary>
        /// The cast is in initialization stage.
        /// </summary>
        Initialization,
        /// <summary>
        /// The cast is in targeting stage.
        /// </summary>
        Targeting,
        /// <summary>
        /// The cast has failed due to targeting.
        /// </summary>
        TargetingFailed,
        /// <summary>
        /// The cast is in the precasting stage.
        /// </summary>
        PreCasting,
        /// <summary>
        /// The cast is in the casting stage.
        /// </summary>
        Casting,
        /// <summary>
        /// The cast is in the interruption stage.
        /// </summary>
        CastFail,
        /// <summary>
        /// The cast has succeeded.
        /// </summary>
        CastSuccess
    }

    /// <summary>
    /// This enumeration represents the order of casting of a Spell.
    /// </summary>
    public enum CastOrder
    {
        /// <summary>
        /// In this ordering, casting will occur after targeting is done.
        /// </summary>
        TargetFirst,

        /// <summary>
        /// In this ordering, casting will occur right after initialization.
        /// </summary>
        CastFirst,
    }
}
