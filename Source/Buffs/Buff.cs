using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Buffs
{
    public class Buff : ITagged, Identifiable
    {
        public virtual string Name
        {
            get { return GetType().Name; }
            set { }
        }
        protected float Interval;
        protected float Duration;
        protected bool Continuous;

        public bool Hidden { get; protected set; }

        protected Character Possessor { get; private set; }
        public Cast Context { get; private set; }

        public int MaxStacks = 1;
        public int CurrentStacks = 1;
        public StackBehavior StackBehaviour = StackBehavior.Nothing;

#if DEBUG
        public Buff()
        {
            Logger.GC("Buff::ctor");
        }
#endif

#if DEBUG
        ~Buff()
        {
            Logger.GC("Buff::dtor");
        }
#endif

        public string Identifier
        {
            get { return MakeID(this, Context); }
        }

        public static string MakeID(Buff buff, Cast context)
        {
            return buff.Name + "@" + (buff.Context == null ? "NaC" : buff.Context.Identifier);
        }

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

        public bool CleanedUp { get; protected set; }
        bool _terminated;

        /// <summary>
        /// Immediately finalize this Buff
        /// </summary>
        protected void Terminate()
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
            if (Continuous)
            {
                OnTick();
            }
            else if (Time.timeSinceLevelLoad - _lastTick >= Interval)
            {
                _lastTick = Time.timeSinceLevelLoad;
                OnTick();
            }

            if (LifeRatio >= 1 && !_terminated)
            {
                Deregister();
                OnDone();
            }

            if (_terminated)
            {
                Deregister();
                OnTerminate();
            }
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
            PossessEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// This event is raised when an existing Buff is attached again
        /// </summary>
        protected virtual void OnStack()
        {
            StackEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// Handles the tick event
        /// </summary>
        protected virtual void OnTick()
        {
            TickEffects.Run(Possessor, Context);
        }

        /// <summary>
        /// Executes the finalization logic of this buff
        /// </summary>
        protected virtual void OnDone()
        {
            DoneEffects.Run(Possessor, Context);
            CleanedUp = true;
        }

        protected virtual void OnTerminate()
        {
            TerminateEffects.Run(Possessor, Context);
            CleanedUp = true;
        }

        protected virtual EffectCollection PossessEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection StackEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection TickEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection DoneEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection TerminateEffects { get { return new EffectCollection { }; } }

        #region Tagging
        public StaticTags Tags { get; protected set; }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }

    public enum StackBehavior
    {
        ResetBeginning = 1,
        IncreaseStacks = 2,
        Nothing = 4
    }
}