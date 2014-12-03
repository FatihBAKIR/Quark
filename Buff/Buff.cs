using System;
using UnityEngine;

namespace Quark
{
    public class Buff : IDisposable
    {
        protected float Interval;
        protected float Duration;
        protected Character Possessor;
        protected CastData Data;

        public Buff()
        {
            Debug.Log("Buff::ctor");
        }

        ~Buff()
        {
            Debug.Log("Buff::dtor");
        }

        public void Dispose()
        {

        }

        public void SetData(CastData data)
        {
            this.Data = data;
        }

        public float LifeRatio
        {
            get
            {
                return this.Alive / this.Duration;
            }
        }

        protected float Alive
        {
            get
            {
                return Time.timeSinceLevelLoad - this.posessionTime;
            }
        }

        private float posessionTime = 0;
        private float lastTick = 0;

        private void Tick()
        {
            if (Time.timeSinceLevelLoad - lastTick >= Interval)
            {
                lastTick = Time.timeSinceLevelLoad;
                this.OnTick();
            }

            if (LifeRatio >= 1)
            {
                this.Deregister();
                this.OnDone();
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
            this.posessionTime = Time.timeSinceLevelLoad;
            this.lastTick = Time.timeSinceLevelLoad;
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

        }
    }
}