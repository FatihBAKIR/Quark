using System.Collections.Generic;
using Quark.Attributes;
using System.Collections;
using Quark.Utilities;

namespace Quark
{
    public class Interaction : IEnumerable<AttributeModifier>, IDeepCopiable<Interaction>
    {
        readonly List<AttributeModifier> _modifiers;

        /// <summary>
        /// Initialize a new effect collection
        /// </summary>
        public Interaction()
        {
            _modifiers = new List<AttributeModifier>();
        }

        public Interaction(Interaction src)
        {
            _modifiers = new List<AttributeModifier>(src._modifiers);
        }

        /// <summary>
        /// Add a new interaction to this collection.
        /// </summary>
        /// <param name="modifier">The interaction to be added</param>
        public void Add(AttributeModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void Add(string tag, float multiplier)
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

        /// <summary>
        /// Calculates the current value of this Interaction for a given Character
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public float Calculate(Character of)
        {
            float val = 0;
            foreach (AttributeModifier interaction in _modifiers)
                val += interaction.GetValue(of);
            return val;
        }


        /// <summary>
        /// Deep copies this collection by recreating it attribute and stat -wise.
        /// <remarks>This function will not preserve any event listener or the current state of the states</remarks>
        /// </summary>
        /// <returns>A new collection</returns>
        public Interaction DeepCopy()
        {
            return new Interaction(this);
        }

        object IDeepCopiable.DeepCopy()
        {
            return DeepCopy();
        }
    }
}

