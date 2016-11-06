using Quark.Contexts;
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
        public event TargetableDel TargetSelected = delegate { };

        /// <summary>
        /// On a Characater object selection
        /// </summary>
        public event CharacterDel CharacterSelected = delegate { };

        /// <summary>
        /// On a Point selection
        /// </summary>
        public event PointDel PointSelected = delegate { };

        /// <summary>
        /// The context this TargetMacro resides in.
        /// </summary>
        protected IContext Context { get; private set; }

#if DEBUG
        public TargetMacro()
        {
            Logger.GC(GetType().Name + "::ctor");
        }

        ~TargetMacro()
        {
            Logger.GC(GetType().Name + "::dtor");
        }
#endif

        /// <summary>
        /// This method should start the targeting logic for this target macro.
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
            TargetingFailed(TargetingError.Cancelled);
        }

        /// <summary>
        /// Clear this TargetMacro from its context.
        /// </summary>
        protected void Clear()
        {
            Context = null;
            TargetingSuccess = null;
            TargetingFailed = null;
            TargetSelected = null;
            CharacterSelected = null;
            PointSelected = null;
        }

        /// <summary>
        /// This method sets the context for this TargetMacro to run in.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void SetContext(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// This property stores the source Character for this targeting.
        /// </summary>
        public Character Caster
        {
            get
            {
                return Context.Source;
            }
        }

        /// <summary>
        /// This property stores whether this Macro should clear its listeners and context after running.
        /// </summary>
        public bool DontClearUponFinish { get; set; }

        /// <summary>
        /// On Target Selection
        /// </summary>
        /// <param name="target">Selected Character</param>
        protected void OnTargetSelected(Character target)
        {
            _targets.Add(target);
            CharacterSelected(target);
        }

        /// <summary>
        /// On Target Selection
        /// </summary>
        /// <param name="target">Selected Point</param>>
        protected void OnTargetSelected(Vector3 target)
        {
            _targets.Add(target);
            PointSelected(target);
        }

        /// <summary>
        /// On Target Selection
        /// </summary>
        /// <param name="target">Selected Targetable</param>
        protected void OnTargetSelected(Targetable target)
        {
            _targets.Add(target);
            TargetSelected(target);
        }

        /// <summary>
        /// On Targeting Success
        /// </summary>
        protected void OnTargetingSuccess()
        {
            TargetingSuccess(_targets);
            _targets = new TargetCollection();
            if (!DontClearUponFinish)
                Clear();
        }

        /// <summary>
        /// On Targeting Fail
        /// </summary>
        /// <param name="error">The reason for this targeting to fail.</param>
        protected void OnTargetingFail(TargetingError error)
        {
            TargetingFailed(error);
            _targets = new TargetCollection();
            if (!DontClearUponFinish)
                Clear();
        }

        TargetCollection _targets = new TargetCollection();
    }

    /// <summary>
    /// This enumeration represents the reason a targeting has failed.
    /// </summary>
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

    /// <summary>
    /// This enumeration represents a Spells target form.
    /// </summary>
    public enum TargetForm
    {
        /// <summary>
        /// May not hit multiple characters
        /// </summary>
        Singular,
        /// <summary>
        /// May hit multiple characters
        /// </summary>
        Plural
    }

    /// <summary>
    /// This enumeration represents a Targets type.
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// There is no target.
        /// </summary>
        None,

        /// <summary>
        /// Target is a Point.
        /// </summary>
        Point,

        /// <summary>
        /// The target is a Targetable.
        /// </summary>
        Targetable,

        /// <summary>
        /// The target is a Character
        /// </summary>
        Character
    }
}
