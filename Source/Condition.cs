using System;
using Quark.Spell;
using UnityEngine;

namespace Quark
{
    public class Condition
    {
        protected Cast _context;
        public Condition()
        {
        }

        public virtual void Introduce(Cast context)
        {
            _context = context;
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