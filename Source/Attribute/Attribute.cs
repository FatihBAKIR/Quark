using Quark.Utilities;

namespace Quark.Attribute
{
    public class Attribute : Identifiable
    {
        /// <summary>
        /// The attribute bag which this attribute is hold in.
        /// </summary>
        AttributeBag _bag;
        string _tag;
        string _name;
        /// <summary>
        /// Other attribute interactions are held in this list.
        /// </summary>
        Interaction _interactions;

        /// <summary>
        /// Gets the owner of this attribute.
        /// </summary>
        /// <value>The owner.</value>
        protected Character Owner
        {
            get
            {
                return _bag.Carrier;
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
            _bag = Bag;
            _tag = Tag;
            _name = Name;
            _interactions = new Interaction();
        }

        /// <summary>
        /// Sets the multiplier with the null attribute for this attribute.
        /// </summary>
        /// <param name='Base'>
        /// Base value.
        /// </param>
        public void SetBase(float Base)
        {
            foreach (AttributeModifier modifier in _interactions)
                if (string.IsNullOrEmpty(modifier.AttrName))
                {
                    modifier.Multiplier = Base;
                    return;
                }
            AddInteraction(null, Base);
        }

        /// <summary>
        /// Gets the interactions.
        /// </summary>
        /// <value>
        /// The interactions.
        /// </value>
        public Interaction Interactions
        {
            get
            {
                return _interactions;
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
                return _name;
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
                return _tag;
            }
        }

        /// <summary>
        /// Gets the calculated value of this attribute
        /// </summary>
        /// <value>
        /// The value this attribute has.
        /// </value>
        public virtual float Value
        {
            get
            {
                return _interactions.Calculate(Owner);
            }
        }

        /// <summary>
        /// Adds an attribute interaction to this attribute.
        /// </summary>
        /// <param name='tag'>
        /// Tag of the other related attribute.
        /// </param>
        /// <param name='multiplier'>
        /// The amoubt that will be multiplied with the value of the related attribute on calculation.
        /// </param>
        public void AddInteraction(string tag, float multiplier)
        {
            _interactions.Add(tag, multiplier);
        }

        public override string ToString()
        {
            return string.Format("[Attribute {0}: {1}]", Name, Value);
        }

        public string Identifier
        {
            get
            {
                return Tag;
            }
        }
    }
}