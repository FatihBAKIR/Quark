using System;
using System.Collections.Generic;
using Quark.Spell;
using UnityEngine;
using Quark.Utilities;

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
        protected virtual string Name
        {
            get{
                return "Effect";
            }
        }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description of this effect.
        /// </value>
        protected virtual string Description
        {
            get{
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

        protected Cast _context { get; private set; }

        public void SetContext(Cast context)
        {
            _context = context;
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
}