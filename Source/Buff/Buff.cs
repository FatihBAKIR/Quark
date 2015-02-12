using System;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Buff
{
    public class Buff : ITaggable, Identifiable
    {
        public string Name { get; set; }

        protected float Interval;

        protected float Duration;

        protected Character Possessor { get; private set; }

        protected Cast _context { get; private set; }

        public bool CleanedUp { get; protected set; }

        public int MaxStacks = 1;
        public int CurrentStacks = 1;
        public StackBehavior Behaviour = StackBehavior.Nothing;

        public Buff()
        {
            Logger.GC("Buff::ctor");
        }

        ~Buff()
        {
            Logger.GC("Buff::dtor");
        }

        public string Identifier
        {
            get
            {
                return Name + "@" + _context.Spell.Identifier;
            }
        }

        /// <summary>
        /// Sets the Cast context where this Buff runs in
        /// </summary>
        /// <param name="context">The Cast context</param>
        public void SetContext(Cast context)
        {
            _context = context;
        }

        /// <summary>
        /// This ratio indicates the rate of its alive time to its total duration 
        /// </summary>
        public virtual float LifeRatio
        {
            get
            {
                return this.Duration > 0 ? this.Alive / this.Duration : 0;
            }
        }

        /// <summary>
        /// The time span in seconds where this Buff was running 
        /// </summary>
        protected float Alive
        {
            get
            {
                return Time.timeSinceLevelLoad - this._posessionTime;
            }
        }

        /// <summary>
        /// This variable is stored for calculating the alive time of the Buff instances
        /// </summary>
        private float _posessionTime = 0;

        /// <summary>
        /// This variable is stored for checking whether the Tick method should be called or not in a given frame
        /// </summary>
        private float _lastTick = 0;

        bool _terminated = false;

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
            if (Time.timeSinceLevelLoad - _lastTick >= Interval)
            {
                _lastTick = Time.timeSinceLevelLoad;
                this.OnTick();
            }

            if (LifeRatio >= 1 || _terminated)
            {
                this.Deregister();
                this.OnDone();
            }
        }

        public virtual string[] Tags
        {
            get
            {
                return new string[] { "buff" };
            }
            set
            {
            }
        }

        /// <summary>
        /// Register proper events to the Messenger.
        /// This method should <b>not</b> contain any gameplay related logic
        /// Refer to the <c>OnPossess()</c> for gameplay logic on possession
        /// </summary>
        protected virtual void Register()
        {
            Messenger.AddListener("Update", this.Tick);
        }

        /// <summary>
        /// Deregister pre registered events from the messenger.
        /// </summary>
        protected virtual void Deregister()
        {
            Messenger.RemoveListener("Update", this.Tick);
        }

        /// <summary>
        /// This event handler is called right after the owning <c>BuffContainer</c> possesses this buff
        /// </summary>
        public virtual void OnPossess(Character possessor)
        {
            Possessor = possessor;
            _posessionTime = Time.timeSinceLevelLoad;
            _lastTick = Time.timeSinceLevelLoad;
            Register();
            PossessEffects.Run(Possessor, _context);
        }

        /// <summary>
        /// This event handler is called when the same Buff is attached again
        /// </summary>
        protected virtual void OnStack()
        {
            StackEffects.Run(Possessor, _context);
        }

        /// <summary>
        /// Handles the tick event
        /// </summary>
        protected virtual void OnTick()
        {
            TickEffects.Run(Possessor, _context);
        }

        /// <summary>
        /// Executes the finalization logic of this buff
        /// </summary>
        protected virtual void OnDone()
        {
            DoneEffects.Run(Possessor, _context);
            this.CleanedUp = true;
        }

        protected virtual EffectCollection PossessEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection StackEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection TickEffects { get { return new EffectCollection { }; } }
        protected virtual EffectCollection DoneEffects { get { return new EffectCollection { }; } }
    }

    public enum StackBehavior
    {
        ResetBeginning = 1,
        IncreaseStacks = 2,
        Nothing = 4
    }
}