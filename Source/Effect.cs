using System;
using System.Collections.Generic;
using Quark.Spell;
using UnityEngine;
using Quark.Utilities;
using Quark.Targeting;

namespace Quark
{
    public class Effect : ITaggable
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name of this effect.
        /// </value>
        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description of this effect.
        /// </value>
        public virtual string Description
        {
            get
            {
                return "Sımple Mutator";
            }
        }

        public virtual string[] Tags
        {
            get
            {
                return new string[] { "effect" };
            }
            set
            {
            }
        }

        public Cast Context { get; private set; }

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
                if (Effect.Context != null)
                    return EffectSource.Spell;
                return EffectSource.Custom;
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

        public Spell.Spell Spell
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