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
            Logger.GC(GetType().Name + "::ctor");
        }

        ~TargetMacro()
        {
            Logger.GC(GetType().Name + "::dtor");
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

        public virtual void SetContext(Cast context, bool withoutCallbacks = false)
        {
            Context = context;
            if (withoutCallbacks)
                return;
            TargetingSuccess += context.TargetingDone;
            TargetingFailed += context.TargetingFail;
            TargetSelected += context.AddTarget;
            CharacterSelected += context.AddTarget;
            PointSelected += context.AddTarget;
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
            _targets = new TargetCollection();
        }

        protected void OnTargetingFail(TargetingError error)
        {
            TargetingFailed(error);
            _targets = new TargetCollection();
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
