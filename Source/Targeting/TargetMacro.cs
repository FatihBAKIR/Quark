using System;
using System.Collections.Generic;
using System.Text;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Targeting
{
    public interface IRanged
    {
        float CastRange
        {
            get;
        }
    }

    public class TargetMacro
    {
        public delegate void MacroHandler();
        public delegate void MacroError(TargetingError error);
        public delegate void TargetableHandler(Targetable target);
        public delegate void CharacterHandler(Character target);
        public delegate void PointHandler(Vector3 target);

        /// <summary>
        /// On targeting succession
        /// </summary>
        public event MacroHandler TargetingSuccess = delegate { };
        /// <summary>
        /// On targeting failure
        /// </summary>
        public event MacroError TargetingFailed = delegate { };
        /// <summary>
        /// On a Targetable object selection
        /// </summary>
        public event TargetableHandler TargetSelected = delegate { };
        /// <summary>
        /// On a Characater object selection
        /// </summary>
        public event CharacterHandler CharacterSelected = delegate { };
        /// <summary>
        /// On a Point selection
        /// </summary>
        public event PointHandler PointSelected = delegate { };
        
        protected CastData Data { get; private set; }
        public TargetMacro()
        {
            Logger.GC(this.GetType().Name + "::ctor");
        }

        ~TargetMacro()
        {
            Logger.GC(this.GetType().Name + "::dtor");
        }

        /// <summary>
        /// Begins the targeting procedure for this macro
        /// </summary>
        public virtual void Run()
        {
            Cancel();
        }

        /// <summary>
        /// Cancels the target selection
        /// </summary>
        public virtual void Cancel()
        {
            TargetManager.FreeTargeter();
            TargetingFailed(TargetingError.Cancelled);
            Clear();
        }

        protected void Clear()
        {
            Data = null;
            TargetingSuccess = null;
            TargetingFailed = null;
            TargetSelected = null;
            CharacterSelected = null;
            PointSelected = null;
        }

        public virtual void SetData(CastData data)
        {
            TargetingSuccess += data.TargetingDone;
            TargetingFailed += data.TargetingFail;
            TargetSelected += data.AddTarget;
            CharacterSelected += data.AddTarget;
            PointSelected += data.AddTarget;
            Data = data;
        }

        public Character Caster
        {
            get
            {
                return Data.Caster;
            }
        }

        /// <summary>
        /// On Target Selection
        /// </summary>
        /// <param name="target">Selected target</param>
        protected void OnTargetSelected(Character target)
        {
            CharacterSelected(target);
        }

        protected void OnTargetSelected(Vector3 target)
        {
            PointSelected(target);
        }

        protected void OnTargetSelected(Targetable target)
        {
            TargetSelected(target);
        }

        protected void OnTargetingSuccess()
        {
            TargetingSuccess();
        }

        protected void OnTargetingFail(TargetingError error)
        {
            TargetingFailed(error);
        }
    }
    
    public enum TargetingError
    {
        /// <summary>
        /// User cancelled targeting
        /// </summary>
        Cancelled,
        /// <summary>
        /// Targeting got interrupted
        /// </summary>
        Interrupted,
        /// <summary>
        /// No viable target was found
        /// </summary>
        NotFound
    }
}
