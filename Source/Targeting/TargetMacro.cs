using System;
using System.Collections.Generic;
using System.Text;
using Quark.Spells;
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
        //Should we add a `TargetMacro sender` argument to handlers?
        //Should we add the final selected targets to the success handler?
        public delegate void MacroSuccess(TargetCollection targets);
        public delegate void MacroError(TargetingError error);
        public delegate void TargetableHandler(Targetable target);
        public delegate void CharacterHandler(Character target);
        public delegate void PointHandler(Vector3 target);

        /// <summary>
        /// On targeting succession
        /// </summary>
        public event MacroSuccess TargetingSuccess = delegate { };
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
        
        protected Cast Context { get; private set; }
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
            Context = null;
            TargetingSuccess = null;
            TargetingFailed = null;
            TargetSelected = null;
            CharacterSelected = null;
            PointSelected = null;
        }

        public virtual void SetData(Cast context)
        {
            TargetingSuccess += context.TargetingDone;
            TargetingFailed += context.TargetingFail;
            TargetSelected += context.AddTarget;
            CharacterSelected += context.AddTarget;
            PointSelected += context.AddTarget;
            Context = context;
        }

        public Character Caster
        {
            get
            {
                return Context.Caster;
            }
        }

        /// <summary>
        /// On Target Selection
        /// </summary>
        /// <param name="target">Selected target</param>
        protected void OnTargetSelected(Character target)
        {
            _targets.Add(target);
            CharacterSelected(target);
        }

        protected void OnTargetSelected(Vector3 target)
        {
            _targets.Add(target);
            PointSelected(target);
        }

        protected void OnTargetSelected(Targetable target)
        {
            _targets.Add(target);
            TargetSelected(target);
        }

        protected void OnTargetingSuccess()
        {
            TargetingSuccess(_targets);
        }

        protected void OnTargetingFail(TargetingError error)
        {
            TargetingFailed(error);
        }

        TargetCollection _targets = new TargetCollection();
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
