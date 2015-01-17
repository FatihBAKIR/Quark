using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Quark.Spell;
using UnityEngine;

namespace Quark
{
    /// <summary>
    /// This class is designed for containing, mutating and running some effects
    /// It is useful for storing event handling effects
    /// The family of Run functions return the instance itself so the running of the effects on multiple target types can be serialized like:
    /// Container.Run().Run(character).Run(point)... etc.
    /// </summary>
    public class EffectCollection : IEnumerable<Effect>
    {
        private List<Effect> _effects;

        /// <summary>
        /// Initialize a new effect collection
        /// </summary>
        public EffectCollection()
        {
            _effects = new List<Effect>();
        }

        /// <summary>
        /// Add a new effect to this collection
        /// </summary>
        /// <param name="effect">The effect to be added</param>
        public void Add(Effect effect)
        {
            _effects.Add(effect);
        }

        public IEnumerator<Effect> GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        /// <summary>
        /// Run the effects contained in this collection with no target
        /// </summary>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(CastData data = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.Data = data;
                effect.Apply();
            }

            return this;
        }

        /// <summary>
        /// Run the effects contained in this collection with a single Point target
        /// </summary>
        /// <param name="target">The target vector</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Vector3 target, CastData data = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.Data = data;
                effect.Apply(target);
            }

            return this;
        }

        /// <summary>
        /// Run the effects contained in this collection with a single Character target
        /// </summary>
        /// <param name="target">The target character</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Character target, CastData data = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.Data = data;
                effect.Apply(target);
            }

            return this;
        }
        
        /// <summary>
        /// Run the effects contained in this collection with a single Targetable target
        /// </summary>
        /// <param name="target">The target targetable</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Targetable target, CastData data = null)
        {
            foreach (Effect effect in _effects)
            {
                effect.Data = data;
                effect.Apply(target);
            }

            return this;
        }

        /// <summary>
        /// Run the effects contained in this collection with a collection of Point targets
        /// </summary>
        /// <param name="target">The target vectors</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Vector3[] targets, CastData data = null)
        {
            foreach (Vector3 target in targets)
                foreach (Effect effect in _effects)
                {
                    effect.Data = data;
                    effect.Apply(target);
                }

            return this;
        }

        /// <summary>
        /// Run the effects contained in this collection with a collection of Character targets
        /// </summary>
        /// <param name="target">The target characters</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Character[] targets, CastData data = null)
        {
            foreach (Character target in targets)
                foreach (Effect effect in _effects)
                {
                    effect.Data = data;
                    effect.Apply(target);
                }

            return this;
        }

        /// <summary>
        /// Run the effects contained in this collection with a collection of Targetable targets
        /// </summary>
        /// <param name="target">The target targetables</param>
        /// <param name="data">The CastData context for the Effects to run</param>
        /// <returns>This collection itself</returns>
        public EffectCollection Run(Targetable[] targets, CastData data = null)
        {
            foreach (Targetable target in targets)
                foreach (Effect effect in _effects)
                {
                    effect.Data = data;
                    effect.Apply(target);
                }

            return this;
        }
    }
}
