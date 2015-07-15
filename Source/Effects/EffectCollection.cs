using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Contexts;
using UnityEngine;

namespace Quark.Effects
{
    /// <summary>
    /// This class is designed for containing, mutating and running some effects
    /// It is useful for storing event handling effects
    /// The family of Run functions return the instance itself so the running of the effects on multiple target types can be serialized like:
    /// Container.Run().Run(character).Run(point)... etc.
    /// </summary>
    public class EffectCollection<T> : Effect<T>, IEnumerable<IEffect>, IDisposable where T : class, IContext
    {
        private List<IEffect> _effects;

        /// <summary>
        /// Initialize a new effect collection
        /// </summary>
        public EffectCollection()
        {
            _effects = new List<IEffect>();
        }

        ~EffectCollection()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (IEffect effect in _effects)
                effect.SetContext(null);
            _effects.Clear();
            _effects = null;
        }

        /// <summary>
        /// Add a new effect to this collection
        /// </summary>
        /// <param name="effect">The effect to be added</param>
        public void Add(IEffect<T> effect)
        {
            _effects.Add(effect);
        }

        /// <summary>
        /// Add multiple effects from another collection.
        /// </summary>
        /// <param name="range">Other collection.</param>
        public void AddRange(EffectCollection<T> range)
        {
            foreach (IEffect effect in range._effects)
                _effects.Add(effect);
        }

        public IEnumerator<IEffect> GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        /// <summary>
        /// The count of the Effects in this collection.
        /// </summary>
        public int Count
        {
            get { return _effects.Count; }
        }

        /// <summary>
        /// Run the effects contained in this collection with no target
        /// </summary>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection<T> Run(T context = null)
        {
            foreach (IEffect effect in _effects)
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
        public EffectCollection<T> Run(Vector3 target, T context = null)
        {
            foreach (IEffect effect in _effects)
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
        public EffectCollection<T> Run(Character target, T context = null)
        {
            foreach (IEffect effect in _effects)
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
        public EffectCollection<T> Run(Targetable target, T context = null)
        {
            foreach (IEffect effect in _effects)
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
        public EffectCollection<T> Run(Vector3[] targets, T context = null)
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
        public EffectCollection<T> Run(Character[] targets, T context = null)
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
        public EffectCollection<T> Run(Targetable[] targets, T context = null)
        {
            foreach (Targetable target in targets)
                Run(target, context);

            return this;
        }


        /// <summary>
        /// Apply the effects contained in this collection with a collection of targets
        /// </summary>
        /// <param name="targets">The targets</param>
        /// <param name="context">The Cast context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection<T> Run(TargetCollection targets, T context = null)
        {
            return Run(context)
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

        /// <summary>
        /// This operator adds 2 EffectCollection instances by adding the Effects in the right hand side operand to the left hand side operand.
        /// </summary>
        /// <param name="lhs">Collection to add to.</param>
        /// <param name="rhs">Collection to add from.</param>
        /// <returns>The left hand side collection.</returns>
        public static EffectCollection<T> operator +(EffectCollection<T> lhs, EffectCollection<T> rhs)
        {
            lhs.AddRange(rhs);
            return lhs;
        }
    }
}
