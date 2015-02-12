using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Attribute
{
    public class AttributeBag : IDisposable
    {
        Dictionary<string, Attribute> _Attributes;
        Character _carrier;

        public void Dispose()
        {
            _carrier = null;
            _Attributes.Clear();
            _Attributes = null;
        }

        public AttributeBag(Character carrier)
        {
            Logger.Debug("AttributeBag::ctor");
            _carrier = carrier;

            _Attributes = new Dictionary<string, Attribute>();
            NewAttribute("str", "Strength");
            NewAttribute("int", "Intellect");
            NewAttribute("agi", "Agility");
            NewAttribute("will", "Willpower");
            NewAttribute("haste", "Haste");
            NewAttribute("sta", "Stamina");
            NewAttribute("armor", "Armor");
            NewAttribute("mres", "Magic Resist");
            NewAttribute("crit", "Critical Strike Chance");

            Stat health = NewStat("hp", "Health");
            health.AddInteraction("str", 3);
            health.AddInteraction("sta", 10);

            Stat mana = NewStat("mana", "Mana");
            mana.AddInteraction("int", 5);
            mana.AddInteraction("will", 20);
        }

        public void ApplyBases(Dictionary<string, float> Bases)
        {
            foreach (KeyValuePair<string, float> b in Bases)
            {
                this.GetAttribute(b.Key).SetBase(b.Value);
            }
        }

        public Attribute[] GetAttributes()
        {
            Attribute[] attributes = new Attribute[_Attributes.Count];
            _Attributes.Values.CopyTo(attributes, 0);
            return attributes;
        }

        public Attribute GetAttribute(string Tag)
        {
            if (!this._Attributes.ContainsKey(Tag))
                throw new Exception("No Such Attribute!");
            return _Attributes[Tag];
        }

        public Stat GetStat(string Tag)
        {
            return (Stat)GetAttribute(Tag);
        }

        public float GetValue(string Tag)
        {
            if (Tag == "constant" || string.IsNullOrEmpty(Tag))
                return 1;
            return GetAttribute(Tag).Value;
        }

        public float this [string key]
        {
            get
            {
                return GetValue(key);
            }
        }

        Attribute NewAttribute(string tag, string name)
        {
            _Attributes.Add(tag, new Attribute(tag, name, this));
            return _Attributes[tag];
        }

        Stat NewStat(string tag, string name)
        {
            _Attributes.Add(tag, (Attribute)new Stat(tag, name, this));
            return (Stat)_Attributes[tag];
        }

        public Character Carrier
        {
            get
            {
                return this._carrier;
            }
        }

        public void Dump()
        {
            foreach (Attribute attr in this.GetAttributes())
                Logger.Debug(attr.ToString());        
        }
    }
}