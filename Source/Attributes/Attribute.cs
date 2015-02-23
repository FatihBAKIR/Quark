using Quark.Utilities;

namespace Quark.Attributes
{
    public class Attribute : Identifiable
    {
        /// <summary>
        /// The attribute collection which this attribute is hold in.
        /// </summary>
        AttributeCollection _collection;

        readonly string _tag;
        readonly string _name;
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
                return _collection.Carrier;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Attribute"/> class.
        /// </summary>
        /// <param name='tag'>
        /// Tag.
        /// </param>
        /// <param name='name'>
        /// Name.
        /// </param>
        /// <param name='collection'>
        /// The attribute collection which this attribute should be held in.
        /// </param>
        public Attribute(string tag, string name, AttributeCollection collection)
        {
            _collection = collection;
            _tag = tag;
            _name = name;
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

        public void SetCollection(AttributeCollection collection)
        {
            _collection = collection;
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

        public void SetInteractions(Interaction interaction)
        {
            _interactions = interaction;
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