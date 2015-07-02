﻿using Quark.Spells;
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
        /// Gets the cast done percentage.
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
                return Time.timeSinceLevelLoad - CastBeginTime;
            }
        }

        public int CastPercentage
        {
            get
            {
                return (int)(CastTime * 100 / Spell.CastDuration);
            }
        }

        /// <summary>
        /// This method executes the initialization logic of this CastContext and the Spell being casted.
        /// </summary>
        void Initialize()
        {
            Stage = CastStages.Initialization;

            if (!Source.CanCast(Spell))
            {
                Messenger<CastContext>.Broadcast("CasterBusy", this);
                return;
            }

            if (!Spell.CanInvoke())
            {
                Messenger<CastContext>.Broadcast("CannotCast", this);
                return;
            }

            Source.Context.AddChild(this); // Add this CastContext to the children of the Caster's Context.

            Spell.OnInvoke();

            BeginTargeting();
        }

        /// <summary>
        /// Begin targeting logic
        /// </summary>
        void BeginTargeting()
        {
            Stage = CastStages.Targeting;

            Targets = new TargetCollection();

            TargetMacro macro = Spell.TargetMacro;

            macro.SetContext(this);

            macro.TargetingSuccess += delegate(TargetCollection targets)
            {
                Targets.AddRange(targets);
                PostTargeting();
                macro = null;
            };

            macro.TargetingFailed += delegate(TargetingError error)
            {
                Stage = CastStages.TargetingFailed;
                if (error == TargetingError.NotFound)
                    Messenger<CastContext>.Broadcast("TargetingFailed", this);
                macro = null;
            };

            macro.Run();
        }

        void PostTargeting()
        {
            Spell.OnTargetingDone();
            if (Spell.IsInstant)
            {
                CastSuccess();
            }
            else
            {
                PreCasting();
            }
        }

        private ConditionCollection<ICastContext> _interruptConditions;
        private float _lastCast;
        void PreCasting()
        {
            Stage = CastStages.PreCasting;
            Spell.OnCastingBegan();
            _interruptConditions = Source.InterruptConditions.DeepCopy();  // Store the interrupt conditions on a member field.

            CastBeginTime = Time.timeSinceLevelLoad;
            CastBeginPosition = Source.transform.position;

            Messenger.AddListener("Update", Casting);
        }

        void Casting()
        {
            if (CheckInterrupt())
            {
                Interrupt();
                return;
            }

            if (CastPercentage >= 100)
            {
                CastSuccess();
                return;
            }

            if (Time.timeSinceLevelLoad > _lastCast + Spell.CastingInterval)
            {
                Spell.OnCasting();
                _lastCast = Time.timeSinceLevelLoad;
            }
        }

        bool CheckInterrupt()
        {
            _interruptConditions.SetContext(this);
            return _interruptConditions.Check();
        }

        void CastSuccess()
        {
            Stage = CastStages.CastSuccess;
            Spell.OnCastDone();
            Clear();
        }

        public void Interrupt()
        {
            Stage = CastStages.CastFail;
            Spell.OnInterrupt();
            Clear();
        }

        public void Clear()
        {
            _interruptConditions = null;
            if (!Spell.IsInstant)
                Messenger.RemoveListener("Update", Casting);
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
}
