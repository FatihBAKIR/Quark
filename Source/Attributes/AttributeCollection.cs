using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Attributes
{
    /// <summary>
    /// This class is responsible for storing and managing the Attributes and Stats of Characters
    /// </summary>
    public class AttributeCollection : IDisposable, IEnumerable<Attribute>, IDeepCopiable<AttributeCollection>
    {
        /// <summary>
        /// This event is raised when a Stat belonging to this collection is manipulated
        /// </summary>
        public event StatDel StatManipulated = delegate { };
        Dictionary<string, Attribute> _attributes;
        Character _carrier;

        public void Dispose()
        {
            _carrier = null;
            _attributes.Clear();
            _attributes = null;
            StatManipulated = null;
        }

        /// <summary>
        /// Adds a new <see cref="Attribute"/> or <see cref="Stat"/> to this collection. 
        /// </summary>
        /// <param name="tag">Tag of the attribute or stat</param>
        /// <param name="name">Name of the attribute or stat</param>
        /// <param name="isStat">This flag determines whether the element is a stat or attribute</param>
        public void Add(string tag, string name, bool isStat = false)
        {
            if (isStat)
                AddStat(tag, name);
            else
                AddAttribute(tag, name);
        }

        /// <summary>
        /// Adds a new <see cref="Attribute"/> or <see cref="Stat"/> with an <see cref="Interaction"/> to this collection. 
        /// </summary>
        /// <param name="tag">Tag of the attribute or stat</param>
        /// <param name="name">Name of the attribute or stat</param>
        /// <param name="interaction">Interaction of the new attribute or stat</param>
        /// <param name="isStat">This flag determines whether the element is a stat or attribute</param>
        public void Add(string tag, string name, Interaction interaction, bool isStat = false)
        {
            (isStat ? AddStat(tag, name) : AddAttribute(tag, name)).SetInteractions(interaction);
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        /// <summary>
        /// Deep copies this collection by recreating it attribute and stat -wise.
        /// <remarks>This function will not preserve any event listener or the current state of the states</remarks>
        /// </summary>
        /// <returns>A new collection</returns>
        public AttributeCollection DeepCopy()
        {
            AttributeCollection newCollection = new AttributeCollection();

            foreach (KeyValuePair<string, Attribute> pair in _attributes)
            {
                if (pair.Value is Stat)
                    newCollection.AddStat(pair.Value.Tag, pair.Value.Name);
                else
                    newCollection.AddAttribute(pair.Value.Tag, pair.Value.Name);
            }

            return newCollection;
        }

        object IDeepCopiable.DeepCopy()
        {
            return DeepCopy();
        }

        /// <summary>
        /// Sets the carrier of this attribute collection.
        /// <remarks>Notice this will not remove any existing event listener related with the former carrier</remarks>
        /// </summary>
        /// <param name="carrier">The new carrier</param>
        public void SetCarrier(Character carrier)
        {
            _carrier = carrier;
            foreach (KeyValuePair<string, Attribute> attribute in _attributes)
            {
                attribute.Value.SetCollection(this);
            }
        }

        /// <summary>
        /// Initializes a new AttributeCollection instance.
        /// </summary>
        public AttributeCollection()
        {
            Logger.Debug("AttributeCollection::ctor");
            _attributes = new Dictionary<string, Attribute>();
        }

        /// <summary>
        /// Sets base values of attributes and stats for this collection
        /// </summary>
        /// <param name="bases">Base values as (tag, value) pairs</param>
        public void ApplyBases(Dictionary<string, float> bases)
        {
            foreach (KeyValuePair<string, float> b in bases)
            {
                GetAttribute(b.Key).SetBase(b.Value);
            }
        }

        /// <summary>
        /// Gets a readonly collection of the attributes and stats in this collection 
        /// </summary>
        /// <returns>Readonly collection</returns>
        public Attribute[] GetAttributes()
        {
            Attribute[] attributes = new Attribute[_attributes.Count];
            _attributes.Values.CopyTo(attributes, 0);
            return attributes;
        }

        public Attribute GetAttribute(string tag)
        {
            if (!_attributes.ContainsKey(tag))
                throw new Exception("No Such Attribute [" + tag + "]!");
            return _attributes[tag];
        }

        public Stat GetStat(string tag)
        {
            return (Stat)GetAttribute(tag);
        }

        public float GetValue(string tag)
        {
            if (tag == "constant" || string.IsNullOrEmpty(tag))
                return 1;
            return GetAttribute(tag).Value;
        }

        public float this[string key]
        {
            get
            {
                return GetValue(key);
            }
        }

        Attribute AddAttribute(string tag, string name)
        {
            _attributes.Add(tag, new Attribute(tag, name, this));
            return _attributes[tag];
        }

        Stat AddStat(string tag, string name)
        {
            _attributes.Add(tag, new Stat(tag, name, this));
            ((Stat)_attributes[tag]).Manipulated += delegate(Character source, Stat stat, float change)
            {
                StatManipulated(source, stat, change);
            };
            return (Stat)_attributes[tag];
        }

        public Character Carrier
        {
            get
            {
                return _carrier;
            }
        }

        public void Dump()
        {
            foreach (Attribute attr in GetAttributes())
                Logger.Debug(attr.ToString());
        }
    }
}