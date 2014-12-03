using System.Collections.Generic;
using System;

namespace Quark
{
    public class AttributeBag : IDisposable
    {
        Dictionary<string, Attribute> Attributes;
        Character carrier;

        public void Dispose()
        {
            this.carrier = null;
            this.Attributes.Clear();
            this.Attributes = null;
        }

        public AttributeBag(Character carrier)
        {
            Quark.Logger.Debug("AttributeBag::ctor");
            this.carrier = carrier;

            this.Attributes = new Dictionary<string, Attribute>();
            NewAttribute("str", "Strength");
            NewAttribute("int", "Intellect");
            NewAttribute("agi", "Agility");
            NewAttribute("will", "Willpower");
            NewAttribute("haste", "Haste");
            NewAttribute("sta", "Stamina");
            NewAttribute("armor", "Armor");
            NewAttribute("mres", "Magic Resist");

            Stat health = NewStat("hp", "Health");
            health.AddInteraction("str", 3);
            health.AddInteraction("sta", 10);

            Stat mana = NewStat("mana", "Mana");
            mana.AddInteraction("int", 5);
            mana.AddInteraction("will", 20);
        }

        public void ApplyBases(Dictionary<string, double> Bases)
        {
            foreach (KeyValuePair<string, double> b in Bases)
            {
                this.GetAttribute(b.Key).SetBase(b.Value);
            }
        }

        public Attribute[] GetAttributes()
        {
            Attribute[] attributes = new Attribute[Attributes.Count];
            Attributes.Values.CopyTo(attributes, 0);
            return attributes;
        }

        public Attribute GetAttribute(string Tag)
        {
            if (!this.Attributes.ContainsKey(Tag))
                throw new Exception("No Such Attribute!");
            return Attributes[Tag];
        }

        public Stat GetStat(string Tag)
        {
            return (Stat)GetAttribute(Tag);
        }

        public double GetValue(string Tag)
        {
            if (Tag == "constant" || string.IsNullOrEmpty(Tag))
                return 1;
            return GetAttribute(Tag).Value;
        }

        public double this [string key]
        {
            get
            {
                return GetValue(key);
            }
        }

        Attribute NewAttribute(string tag, string name)
        {
            Attributes.Add(tag, new Attribute(tag, name, this));
            return Attributes[tag];
        }

        Stat NewStat(string tag, string name)
        {
            Attributes.Add(tag, (Attribute)new Stat(tag, name, this));
            return (Stat)Attributes[tag];
        }

        public Character Carrier
        {
            get
            {
                return this.carrier;
            }
        }

        public void Dump()
        {
            foreach (Attribute attr in this.GetAttributes())
                Quark.Logger.Debug(attr.ToString());        
        }
    }
}