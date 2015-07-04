using System;
using Quark.Conditions;
using Quark.Contexts;
using Quark.Effects;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Buffs
{
    public interface IBuff : IDisposable, Identifiable
    {
        /// <summary>
        /// Life ratio of this buff.
        /// </summary>
        float LifeRatio { get; }

        /// <summary>
        /// Total duration of this buff.
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// This property stores how this buff should respond to stacking.
        /// </summary>
        StackBehavior StackBehavior { get; }

        /// <summary>
        /// This field stores the current stack count of this Buff.
        /// </summary>
        int CurrentStacks { get; set; }

        /// <summary>
        /// This field stores the maximum stack count of this Buff.
        /// </summary>
        int MaximumStacks { get; }

        /// <summary>
        /// This property determines whether this Buff should be hidden or not.
        /// </summary>
        bool Hidden { get; }

        /// <summary>
        /// This method determines whether this buff is ready for disposal.
        /// </summary>
        /// <returns>Whether this buff is ready for disposal.</returns>
        bool ShouldDispose();

        /// <summary>
        /// Register proper events to the Messenger.
        /// This method should <b>not</b> contain any gameplay related logic
        /// Refer to the <c>OnPossess()</c> for gameplay logic on possession
        /// </summary>
        void Register();

        /// <summary>
        /// Deregister pre registered events from the messenger.
        /// </summary>
        void Deregister();

        /// <summary>
        /// This event handler is called right after the owning <c>BuffContainer</c> possesses this buff
        /// </summary>
        void OnPossess();

        /// <summary>
        /// This event is raised when an existing Buff is attached again
        /// </summary>
        void OnStack();

        /// <summary>
        /// Executes the finalization logic of this buff
        /// </summary>
        void OnDone();

        /// <summary>
        /// Executes the termination logic of this buff
        /// </summary>
        void OnTerminate();

        /// <summary>
        /// Handles the tick event
        /// </summary>
        void OnTick();

        /// <summary>
        /// Begin the posession logic of this Buff
        /// </summary>
        /// <param name="possessor"></param>
        void Possess(Character possessor);

        /// <summary>
        /// Immediately terminates this Buff.
        /// Termination assures no other Tick will take place in this instance.
        /// </summary>
        void Terminate();

        /// <summary>
        /// Resets the possession time of this Buff, practically resetting its lifetime
        /// </summary>
        void ResetBeginning();
    }

    public interface IBuff<in T> : IBuff, IContextful<T> where T : IContext
    {
    }

    public class Buff<T> : IBuff<T>, ITagged where T : class, IContext
    {
        /// <summary>
        /// Name of this Buff.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// This field stores the interval of calling the Tick method in seconds.
        /// </summary>
        protected float Interval;

        public float Duration { get; set; }

        /// <summary>
        /// This flag determines whether the Tick method should be called every frame or not.
        /// </summary>
        protected bool Continuous;

        /// <summary>
        /// This flag determines whether this Buff ticks while its Possessor Character is suspended or not.
        /// </summary>
        protected bool TicksWhileSuspended;

        public bool Hidden { get; protected set; }

        /// <summary>
        /// This property stores the possessor of this Buff instance.
        /// </summary>
        protected Character Possessor { get; private set; }

        public int MaximumStacks { get; protected set; }

        public int CurrentStacks { get; set; }

        /// <summary>
        /// This field stores the stacking behavior of this Buff.
        /// See <see cref="Buffs.StackBehavior"/>
        /// </summary>
        public StackBehavior StackBehavior { get; set; }

        /// <summary>
        /// This flag stores whether this Buff is ready to be garbage collected
        /// </summary>
        public bool CleanedUp { get; protected set; }

        /// <summary>
        /// This ratio indicates the rate of its alive time to its total duration 
        /// </summary>
        public float LifeRatio
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
        /// This flag stores whether this Buff got terminated in the last Tick
        /// </summary>
        private bool _terminated;

        public void Dispose()
        {
            Terminate();
        }

        public Buff()
        {
#if DEBUG
            Logger.GC("Buff::ctor");
#endif
        }

        ~Buff()
        {
#if DEBUG
            Logger.GC("Buff::dtor");
#endif
        }

        public void Terminate()
        {
            _terminated = true;
            _lastTick = Mathf.Infinity;
        }

        public void ResetBeginning()
        {
            _posessionTime = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// This function controls the state of the buff for whether it should call the OnTick function in this frame or not and also it checks if it has completed its lifespan or not
        /// </summary>
        private void Tick()
        {
            /*
             * Tick logic:
             *
             * ---
             * Check for whether the custom OnTick logic should be executed or not:
             * 
             * A buff should not tick if it is already cleaned up or terminated.
             * If the possessor is suspended and the buff is not explicitly flagged to tick, it should not tick.
             * ---
             * 
             * ---
             * Check whether this buff should terminate or prematurely get to the done stage.
             * If either of the conditions met, finalize appropriately.
             * ---
             */

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

            ConditionCollection<T> collection = DoneConditions;
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

        /// <summary>
        /// This method determines whether this Buff instance should get disposed in the current frame.
        /// </summary>
        /// <returns>Whether this buff should get disposed.</returns>
        public bool ShouldDispose()
        {
            return (LifeRatio >= 1 || _terminated) && CleanedUp;
        }

        public T Context { get; private set; }

        public virtual void SetContext(IContext context)
        {
            Context = (T)context;
        }

        public void Possess(Character possessor)
        {
            Possessor = possessor;
            _posessionTime = Time.timeSinceLevelLoad;
            _lastTick = Time.timeSinceLevelLoad;
            Register();
            OnPossess();
        }

        public virtual void Register()
        {
            Messenger.AddListener("Update", Tick);
        }

        public virtual void Deregister()
        {
            Messenger.RemoveListener("Update", Tick);
        }

        public virtual void OnPossess()
        {
            PossessEffects.Run(Possessor, Context);
        }

        public virtual void OnStack()
        {
            StackEffects.Run(Possessor, Context);
        }

        public virtual void OnTick()
        {
            TickEffects.Run(Possessor, Context);
        }

        public virtual void OnDone()
        {
            DoneEffects.Run(Possessor, Context);
            CleanedUp = true;
        }

        public virtual void OnTerminate()
        {
            Logger.Debug("Buff::OnTerminate");
            CleanedUp = true;
            TerminateEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// These effects are applied when this Buff is first possessed
        /// </summary>
        protected virtual EffectCollection<T> PossessEffects { get { return new EffectCollection<T>(); } }

        /// <summary>
        /// These effects are applied when another instance of this Buff is attached to the possessor
        /// </summary>
        protected virtual EffectCollection<T> StackEffects { get { return new EffectCollection<T>(); } }

        /// <summary>
        /// These effects are applied on every interval
        /// </summary>
        protected virtual EffectCollection<T> TickEffects { get { return new EffectCollection<T>(); } }

        /// <summary>
        /// These effects are applied when this Buff finishes its life time successfully
        /// </summary>
        protected virtual EffectCollection<T> DoneEffects { get { return new EffectCollection<T>(); } }

        /// <summary>
        /// These effeccts are applied when this Buff terminates (fails finish its life time)
        /// </summary>
        protected virtual EffectCollection<T> TerminateEffects { get { return new EffectCollection<T>(); } }

        /// <summary>
        /// These conditions are checked to determine whether this Buff should be done
        /// </summary>
        protected virtual ConditionCollection<T> DoneConditions { get { return new ConditionCollection<T> { new FalseCondition() }; } }

        /// <summary>
        /// These conditions are checked to determine whether this Buff should terminate
        /// </summary>
        protected virtual ConditionCollection<T> TerminateConditions { get { return new ConditionCollection<T> { new FalseCondition() }; } }

        #region Tagging
        /// <summary>
        /// Stores the static tags of this buff.
        /// </summary>
        public StaticTags Tags { get; protected set; }

        /// <summary>
        /// Checks whether this Buff is tagged with the given string or not.
        /// </summary>
        /// <param name="tag">Tag to check.</param>
        /// <returns>Whether the buff is tagged.</returns>
        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion

        /// <summary>
        /// Returns this Buffs identifier
        /// </summary>
        public string Identifier
        {
            get { return Name + "@" + Context.Identifier; }
        }
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