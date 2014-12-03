using System;
using System.Text;

namespace Quark
{
    public class AttributeModifier
    {
        string sourceAttr;
        double multiplier;

        public double Multiplier
        {
            get
            {
                return this.multiplier;
            }
            set
            {
                multiplier = value;
            }
        }

        public string AttrName
        {
            get
            {
                return this.sourceAttr;
            }
            set
            {
                sourceAttr = value;
            }
        }

        public AttributeModifier(string SourceAttribute, double Multiplier)
        {
            this.sourceAttr = SourceAttribute;
            this.multiplier = Multiplier;
        }

        public double GetValue(Character character)
        {
            return character.GetAttribute(this.AttrName).Value * this.Multiplier;
        }

        public double GetValue(AttributeBag bag)
        {
            return bag[this.AttrName] * this.Multiplier;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Modifier ");
            if (!string.IsNullOrEmpty(sourceAttr))
            {
                sb.Append(this.sourceAttr.ToString());
                sb.Append(" * ");
            }
            sb.Append(this.multiplier.ToString());
            sb.Append("]");
            return sb.ToString();
        }
    }
}