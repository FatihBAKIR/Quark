using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quark
{
    public class Effect
    {
        protected string name;
        protected string description;

        public CastData Data { get; set; }

        public Effect(CastData Data = null)
        {
            this.Data = Data;
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
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description of this effect.
        /// </value>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name of this effect.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}