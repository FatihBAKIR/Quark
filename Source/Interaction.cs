using System;
using System.Collections.Generic;
using Quark.Attribute;
using System.Collections;

namespace Quark
{
    public class Interaction : IEnumerable<AttributeModifier>
    {
        List<AttributeModifier> _modifiers;

        /// <summary>
        /// Initialize a new effect collection
        /// </summary>
        public Interaction()
        {
            _modifiers = new List<AttributeModifier>();
        }

        /// <summary>
        /// Add a new interaction to this collection
        /// </summary>
        /// <param name="effect">The interaction to be added</param>
        public void Add(AttributeModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void Add (string tag, float multiplier)
        {
            _modifiers.Add(new AttributeModifier(tag, multiplier));
        }

        public IEnumerator<AttributeModifier> GetEnumerator()
        {
            return _modifiers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _modifiers.GetEnumerator();
        }
 
        public float Calculate(Character of)
        {
            float val = 0;
            foreach (AttributeModifier interaction in _modifiers)
                val += interaction.GetValue(of);
            return val;
        }
    }
}

