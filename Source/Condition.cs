using System;
using Quark.Spell;
using UnityEngine;

namespace Quark
{
    public class Condition
    {
        protected Cast Context { get; private set; }
        public Condition()
        {
        }

        public virtual void SetContext(Cast context)
        {
            Context = context;
        }

        /// <summary>
        /// Checks whether a condition is met under the context
        /// </summary>
        public virtual bool Check()
        {
            return true;
        }

        /// <summary>
        /// Check whether a condition is met under the context with the specified point.
        /// </summary>
        /// <param name="point">The Point.</param>
        public virtual bool Check(Vector3 point)
        {
            return true;
        }

        /// <summary>
        /// Check whether a condition is met under the context with the specified character.
        /// </summary>
        /// <param name="character">The Character.</param>
        public virtual bool Check(Character character)
        {
            return true;
        }

        /// <summary>
        /// Check whether a condition is met under the context with the specified target.
        /// </summary>
        /// <param name="target">The Targetable.</param>
        public virtual bool Check(Targetable target)
        {
            return true;
        }
    }
}