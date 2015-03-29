using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Spells;
using UnityEngine;

namespace Quark
{
    /// <summary>
    /// This class is designed for containing, mutating and running some effects
    /// It is useful for storing event handling effects
    /// The family of Run functions return the instance itself so the running of the effects on multiple target types can be serialized like:
    /// Container.Run().Run(character).Run(point)... etc.
    /// </summary>
    public class EffectCollection : Effect, IEnumerable<Effect>, IDisposable
    {
        private List<Effect> _effects;

        /// <summary>
        /// Initialize a new effect collection
        /// </summary>
        public EffectCollection()
        {
            _effects = new List<Effect>();
        }

        ~EffectCollection()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (Effect effect in _effects)
                effect.SetContext(null);
            _effects.Clear();
            _effects = null;
        }

        /// <summary>
        /// Add a new effect to this collection
        /// </summary>
        /// <param name="effect">The effect to be added</param>
        public void Add(Effect effect)
        {
            _effects.Add(effect);
        }

        public void AddRange(EffectCollection range)
        {
            foreach (Effect effect in range._effects)
                Add(effect);
        }

        public IEnumerator<Effect> GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        public int Count
        {
            get { return _effects.Count; }
        }

        /// <summary>
        /// Run the effects contained in this collection with no target
        /// </summary>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Cast context = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.SetContext(context);
                effect.Apply();
            }

            return this;
        }

        /// <summary>
        /// Apply the effects contained in this collection with a single Point target
        /// </summary>
        /// <param name="target">The target vector</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Vector3 target, Cast context = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.SetContext(context);
                effect.Apply(target);
            }

            return this;
        }

        /// <summary>
        /// Apply the effects contained in this collection with a single Character target
        /// </summary>
        /// <param name="target">The target character</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Character target, Cast context = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.SetContext(context);
                effect.Apply(target);
            }

            return this;
        }
        
        /// <summary>
        /// Apply the effects contained in this collection with a single Targetable target
        /// </summary>
        /// <param name="target">The target targetable</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Targetable target, Cast context = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.SetContext(context);
                effect.Apply(target);
            }

            return this;
        }

        /// <summary>
        /// Apply the effects contained in this collection with a collection of Point targets
        /// </summary>
        /// <param name="targets">The target vectors</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Vector3[] targets, Cast context = null)
        {
            foreach (Vector3 target in targets)
                Run(target, context);

            return this;
        }

        /// <summary>
        /// Apply the effects contained in this collection with a collection of Character targets
        /// </summary>
        /// <param name="targets">The target characters</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Character[] targets, Cast context = null)
        {
            foreach (Character target in targets)
                Run(target, context);

            return this;
        }

        /// <summary>
        /// Apply the effects contained in this collection with a collection of Targetable targets
        /// </summary>
        /// <param name="targets">The target targetables</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Targetable[] targets, Cast context = null)
        {
            foreach (Targetable target in targets)
                Run(target, context);

            return this;
        }

        public EffectCollection Run(TargetCollection targets, Cast context = null)
        {
            return this
                .Run(context)
                .Run(targets.Points, context)
                .Run(targets.Targetables, context)
                .Run(targets.Characters, context);
        }

        public override void Apply()
        {
            Run(Context);
            base.Apply();
        }

        public override void Apply(Character target)
        {
            Run(target, Context);
            base.Apply(target);
        }

        public override void Apply(Targetable target)
        {
            Run(target, Context);
            base.Apply(target);
        }

        public override void Apply(Vector3 point)
        {
            Run(point, Context);
            base.Apply(point);
        }

        public static EffectCollection operator +(EffectCollection lhs, EffectCollection rhs)
        {
            lhs.AddRange(rhs);
            return lhs;
        }
    }
}
