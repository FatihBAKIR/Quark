using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Attributes
{
    public class AttributeCollection : IDisposable, IEnumerable<Attribute>, IDeepCopiable<AttributeCollection>
    {
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

        public void Add(string tag, string name, bool isStat = false)
        {
            if (isStat)
                AddStat(tag, name); 
            else 
                AddAttribute(tag, name);
        }

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

        public AttributeCollection DeepCopy()
        {
            AttributeCollection newCollection = new AttributeCollection();
            newCollection._attributes = new Dictionary<string, Attribute>(_attributes);
            return newCollection;
        }

        object IDeepCopiable.DeepCopy()
        {
            return DeepCopy();
        }

        public void SetCarrier(Character carrier)
        {
            _carrier = carrier;
            foreach (KeyValuePair<string, Attribute> attribute in _attributes)
            {
                attribute.Value.SetCollection(this);
            }
        }

        public AttributeCollection()
        {
            Logger.Debug("AttributeCollection::ctor");
            _attributes = new Dictionary<string, Attribute>();
        }

        public void ApplyBases(Dictionary<string, float> bases)
        {
            foreach (KeyValuePair<string, float> b in bases)
            {
                GetAttribute(b.Key).SetBase(b.Value);
            }
        }

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