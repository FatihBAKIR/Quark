using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark
{
    public class Effect : ITagged
    {
        /// <summary>
        /// The context this Effect should apply in.
        /// </summary>
        public Cast Context { get; private set; }

        /// <summary>
        /// Sets the context of this Effect.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SetContext(Cast context)
        {
            Context = context;
        }

        /// <summary>
        /// Applies this effect without a target.
        /// </summary>
        public virtual void Apply()
        {
        }

        /// <summary>
        /// Applies this effect on the specified target Character.
        /// </summary>
        /// <param name='target'>
        /// Target Character.
        /// </param>
        public virtual void Apply(Character target)
        {
        }

        /// <summary>
        /// Applies this effect on the specified target Vector3.
        /// </summary>
        /// <param name='point'>  
        /// Target Point.
        /// </param>
        public virtual void Apply(Vector3 point)
        {
        }

        /// <summary>
        /// Applies this effect on the specified non character targetable
        /// </summary>
        /// <param name="target">Targetable.</param>
        public virtual void Apply(Targetable target)
        {
        }

        #region Tagging
        /// <summary>
        /// The tags of this Effect.
        /// </summary>
        public StaticTags Tags { get; protected set; }

        /// <summary>
        /// Checks whether this Effect is tagged with a particular string or not.
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <returns>Whether this Effect is tagged or not.</returns>
        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }

    public enum EffectSource
    {
        Spell,
        Custom
    }

    public class EffectArgs : IMessage
    {
        public Effect Effect
        {
            get;
            protected set;
        }

        public TargetUnion Target
        {
            get;
            protected set;
        }

        public EffectSource Source
        {
            get
            {
                return Effect.Context != null ? EffectSource.Spell : EffectSource.Custom;
            }
        }

        public Cast Context
        {
            get
            {
                return Effect.Context;
            }
        }

        public Character Caster
        {
            get
            {
                return Context.Caster;
            }
        }

        public Spell Spell
        {
            get
            {
                return Context.Spell;
            }
        }

        public EffectArgs(Effect effect)
        {
            Effect = effect;
            Target = new TargetUnion();
        }

        public EffectArgs(Effect effect, Vector3 target)
        {
            Effect = effect;
            Target = new TargetUnion(target);
        }

        public EffectArgs(Effect effect, Targetable target)
        {
            Effect = effect;
            Target = new TargetUnion(target);
        }

        public EffectArgs(Effect effect, Character target)
        {
            Effect = effect;
            Target = new TargetUnion(target);
        }

        public virtual void Broadcast()
        {
            Messenger<EffectArgs>.Broadcast("EffectApplied", this);
        }
    }
}