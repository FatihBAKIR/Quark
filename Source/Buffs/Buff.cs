using System;
using System.Diagnostics.CodeAnalysis;
using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Buffs
{
    public class Buff : ITagged, Identifiable, IDisposable
    {
        /// <summary>
        /// Name of this Buff.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().Name; }
        }
        protected float Interval;
        protected float Duration;
        protected bool Continuous;

        protected bool TicksWhileSuspended;

        public bool Hidden { get; protected set; }

        protected Character Possessor { get; private set; }
        public Cast Context { get; private set; }

        /// <summary>
        /// This field stores the maximum stack count of this Buff.
        /// </summary>
        public int MaxStacks = 1;

        /// <summary>
        /// This field stores the current stack count of this Buff.
        /// </summary>
        public int CurrentStacks = 1;

        /// <summary>
        /// This field stores the stacking behavior of this Buff.
        /// See <see cref="Buffs.StackBehavior"/>
        /// </summary>
        public StackBehavior StackBehavior = StackBehavior.Nothing;

        public void Dispose()
        {
            Terminate();
        }

#if DEBUG
        public Buff()
        {
            Logger.GC("Buff::ctor");
        }
#endif
        ~Buff()
        {
#if DEBUG
            Logger.GC("Buff::dtor");
#endif
        }

        /// <summary>
        /// Returns this Buffs identifier
        /// </summary>
        public string Identifier
        {
            get { return MakeID(this, Context); }
        }

        /// <summary>
        /// Makes an Identifier from a Buff and Cast instances
        /// </summary>
        /// <param name="buff">The Buff instance</param>
        /// <param name="context">The Cast instance</param>
        /// <returns>Buff identifier</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string MakeID(Buff buff, Cast context)
        {
            return buff.Name + "@" + (buff.Context == null ? "NaC" : buff.Context.Identifier);
        }

        /// <summary>
        /// Makes an Identifier from a Buff and an identifier for a Cast context
        /// </summary>
        /// <param name="buff">The Buff instance</param>
        /// <param name="contextID">A context identifier</param>
        /// <returns>Buff identifier</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string MakeID(Buff buff, string contextID)
        {
            return buff.Name + "@" + contextID;
        }

        /// <summary>
        /// Sets the Cast context where this Buff runs in
        /// </summary>
        /// <param name="context">The Cast context</param>
        public void SetContext(Cast context)
        {
            Context = context;
        }

        /// <summary>
        /// This ratio indicates the rate of its alive time to its total duration 
        /// </summary>
        public virtual float LifeRatio
        {
            get
            {
                return Duration > 0 ? Alive / Duration : 0;
            }
        }

        /// <summary>
        /// The time span in seconds where this Buff was running 
        /// </summary>
        protected float Alive
        {
            get
            {
                return Time.timeSinceLevelLoad - _posessionTime;
            }
        }

        /// <summary>
        /// This variable is stored for calculating the alive time of the Buff instances
        /// </summary>
        private float _posessionTime;

        /// <summary>
        /// This variable is stored for checking whether the Tick method should be called or not in a given frame
        /// </summary>
        private float _lastTick;

        /// <summary>
        /// This flag stores whether this Buff is ready to be garbage collected
        /// </summary>
        public bool CleanedUp { get; protected set; }

        /// <summary>
        /// This flag stores whether this Buff got terminated in the last Tick
        /// </summary>
        bool _terminated;

        /// <summary>
        /// Immediately terminates this Buff.
        /// Termination assures no other Tick will take place in this instance.
        /// </summary>
        public void Terminate()
        {
            _terminated = true;
            _lastTick = Mathf.Infinity;
        }

        /// <summary>
        /// Resets the possession time of this Buff, practically resetting its lifetime
        /// </summary>
        public void ResetBeginning()
        {
            _posessionTime = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// This function controls the state of the buff for whether it should call the OnTick function in this frame or not and also it checks if it has completed its lifespan or not
        /// </summary>
        private void Tick()
        {
            if (CleanedUp)
                return;

            if (_terminated)
            {
                Deregister();
                OnTerminate();
                return;
            }

            if (!Possessor.IsSuspended || TicksWhileSuspended)
            {
                if (Continuous)
                    OnTick();
                else if (Time.timeSinceLevelLoad - _lastTick >= Interval)
                {
                    _lastTick = Time.timeSinceLevelLoad;
                    OnTick();
                }
            }

            ConditionCollection collection = DoneConditions;
            bool isDone = false;
            collection.SetContext(Context);
            if (collection.Check(Possessor))
                isDone = true;

            if ((LifeRatio >= 1 && !_terminated) || isDone)
            {
                Duration = 0.00000001f;
                Deregister();
                OnDone();
                return;
            }

            collection = TerminateConditions;
            collection.SetContext(Context);
            if (collection.Check(Possessor))
                _terminated = true;
        }

        internal bool ShouldDispose()
        {
            return (LifeRatio >= 1 || _terminated) && CleanedUp;
        }

        /// <summary>
        /// Register proper events to the Messenger.
        /// This method should <b>not</b> contain any gameplay related logic
        /// Refer to the <c>OnPossess()</c> for gameplay logic on possession
        /// </summary>
        protected virtual void Register()
        {
            Messenger.AddListener("Update", Tick);
        }

        /// <summary>
        /// Deregister pre registered events from the messenger.
        /// </summary>
        protected virtual void Deregister()
        {
            Messenger.RemoveListener("Update", Tick);
        }

        /// <summary>
        /// Begin the posession logic of this Buff
        /// </summary>
        /// <param name="possessor"></param>
        public void Possess(Character possessor)
        {
            Possessor = possessor;
            _posessionTime = Time.timeSinceLevelLoad;
            _lastTick = Time.timeSinceLevelLoad;
            Register();
            OnPossess();
        }

        /// <summary>
        /// This event handler is called right after the owning <c>BuffContainer</c> possesses this buff
        /// </summary>
        public virtual void OnPossess()
        {
            Logger.Debug("Buff::OnPossess");

            PossessEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// This event is raised when an existing Buff is attached again
        /// </summary>
        public virtual void OnStack()
        {
            Logger.Debug("Buff::OnStack");

            StackEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// Handles the tick event
        /// </summary>
        protected virtual void OnTick()
        {
            Logger.Debug("Buff::OnTick");

            TickEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// Executes the finalization logic of this buff
        /// </summary>
        protected virtual void OnDone()
        {
            Logger.Debug("Buff::OnDone");

            DoneEffects.Run(Possessor, Context);
            CleanedUp = true;
        }

        /// <summary>
        /// Executes the termination logic of this buff
        /// </summary>
        protected virtual void OnTerminate()
        {
            Logger.Debug("Buff::OnTerminate");

            TerminateEffects.Run(Possessor, Context);
            CleanedUp = true;
        }

        /// <summary>
        /// These effects are applied when this Buff is first possessed
        /// </summary>
        protected virtual EffectCollection PossessEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// These effects are applied when another instance of this Buff is attached to the possessor
        /// </summary>
        protected virtual EffectCollection StackEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// These effects are applied on every interval
        /// </summary>
        protected virtual EffectCollection TickEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// These effects are applied when this Buff finishes its life time successfully
        /// </summary>
        protected virtual EffectCollection DoneEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// These effeccts are applied when this Buff terminates (fails finish its life time)
        /// </summary>
        protected virtual EffectCollection TerminateEffects { get { return new EffectCollection(); } }

        /// <summary>
        /// These conditions are checked to determine whether this Buff should be done
        /// </summary>
        protected virtual ConditionCollection DoneConditions { get { return new ConditionCollection { new FalseCondition() }; } }

        /// <summary>
        /// These conditions are checked to determine whether this Buff should terminate
        /// </summary>
        protected virtual ConditionCollection TerminateConditions { get { return new ConditionCollection { new FalseCondition() }; } }

        #region Tagging
        public StaticTags Tags { get; protected set; }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }

    /// <summary>
    /// This enumeration dictates how a given Buff should respond in a stacking situation
    /// </summary>
    public enum StackBehavior
    {
        /// <summary>
        /// In the case of stacking, the Buff should reset its possession time.
        /// <see cref="Buff.ResetBeginning()"/>
        /// </summary>
        ResetBeginning = 1,

        /// <summary>
        /// In the case of stacking, the Buff should increase its stack count.
        /// </summary>
        IncreaseStacks = 2,

        /// <summary>
        /// In the case of stacking, the Buff shouldn't respond.
        /// </summary>
        Nothing = 4
    }
}