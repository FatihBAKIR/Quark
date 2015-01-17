﻿using System;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Buff
{
    public class Buff : ITaggable
    {
        protected float Interval;
        protected float Duration;
        protected Character Possessor;
        bool isDataSet = false;
        private CastData data;
        protected CastData Data { get { return data; } set { if (isDataSet) return; data = value; } }
        public bool CleanedUp = false;

        public Buff()
        {
            Logger.GC("Buff::ctor");
        }

        ~Buff()
        {
            Logger.GC("Buff::dtor");
        }

        /// <summary>
        /// Sets the CastData context where this Buff will run in
        /// </summary>
        /// <param name="data">The CastData context</param>
        public void SetData(CastData data)
        {
            this.Data = data;
        }

        /// <summary>
        /// This ratio indicates the rate of its alive time to its total duration 
        /// </summary>
        public float LifeRatio
        {
            get
            {
                return this.Alive / this.Duration;
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

            if (LifeRatio >= 1)
            {
                this.Deregister();
                this.OnDone();
            }
        }

        public virtual string[] Tags
        {
            get{
                return new string[] { "buff" };
            }
            set{
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
            this.Possessor = possessor;
            this._posessionTime = Time.timeSinceLevelLoad;
            this._lastTick = Time.timeSinceLevelLoad;
            this.Register();
        }

        /// <summary>
        /// Handles the tick event
        /// </summary>
        protected virtual void OnTick()
        {
        }

        /// <summary>
        /// Executes the finalization logic of this buff
        /// </summary>
        protected virtual void OnDone()
        {
            this.CleanedUp = true;
        }
    }
}