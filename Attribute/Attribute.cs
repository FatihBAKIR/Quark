using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Attribute
{
    public class Attribute : Identifiable
    {
        /// <summary>
        /// The attribute bag which this attribute is hold in.
        /// </summary>
        AttributeBag bag;
        string tag;
        string name;
        /// <summary>
        /// Other attribute interactions are held in this list.
        /// </summary>
        List<AttributeModifier> interactions;

        /// <summary>
        /// Gets the owner of this attribute.
        /// </summary>
        /// <value>The owner.</value>
        protected Character Owner
        {
            get
            {
                return bag.Carrier;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Attribute"/> class.
        /// </summary>
        /// <param name='Tag'>
        /// Tag.
        /// </param>
        /// <param name='Name'>
        /// Name.
        /// </param>
        /// <param name='Bag'>
        /// The attribute bag which this attribute should be held in.
        /// </param>
        public Attribute(string Tag, string Name, AttributeBag Bag)
        {
            this.bag = Bag;
            this.tag = Tag;
            this.name = Name;
            this.interactions = new List<AttributeModifier>();
        }

        /// <summary>
        /// Sets the multiplier with the null attribute for this attribute.
        /// </summary>
        /// <param name='Base'>
        /// Base value.
        /// </param>
        public void SetBase(double Base)
        {
            foreach (AttributeModifier modifier in interactions)
                if (string.IsNullOrEmpty(modifier.AttrName))
                {
                    modifier.Multiplier = Base;
                    return;
                }
            AddInteraction(null, Base);
        }

        /// <summary>
        /// Gets or sets the interactions.
        /// </summary>
        /// <value>
        /// The interactions.
        /// </value>
        public List<AttributeModifier> Interactions
        {
            get
            {
                return this.interactions;
            }
        }

        /// <summary>
        /// Gets the name of this attribute.
        /// </summary>
        /// <value>
        /// The name of this attribute.
        /// </value>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the tag of this attribute.
        /// </summary>
        /// <value>
        /// The tag of this attribute.
        /// </value>
        public string Tag
        {
            get
            {
                return this.tag;
            }
        }

        /// <summary>
        /// Gets the calculated value of this attribute
        /// </summary>
        /// <value>
        /// The value this attribute has.
        /// </value>
        public virtual double Value
        {
            get
            {
                return CalculateInteractions();
            }
        }

        /// <summary>
        /// Adds an attribute interaction to this attribute.
        /// </summary>
        /// <param name='Tag'>
        /// Tag of the other related attribute.
        /// </param>
        /// <param name='Multiplier'>
        /// The amoubt that will be multiplied with the value of the related attribute on calculation.
        /// </param>
        public void AddInteraction(string Tag, double Multiplier)
        {
            this.interactions.Add(new AttributeModifier(Tag, Multiplier));
        }

        protected double CalculateInteractions()
        {
            double val = 0;
            foreach (AttributeModifier interaction in interactions)
            {
                val += interaction.GetValue(bag);
            }
            return val;
        }

        public override string ToString()
        {
            return string.Format("[Attribute {0}: {1}]", Name, Value);
        }

        public string Identifier()
        {
            return this.Tag;
        }
    }
}