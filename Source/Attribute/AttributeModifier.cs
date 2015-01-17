using System.Globalization;
using System.Text;

namespace Quark.Attribute
{
    public class AttributeModifier
    {
        public double Multiplier { get; set; }

        public string AttrName { get; set; }

        public AttributeModifier(string sourceAttribute, double multiplier)
        {
            AttrName = sourceAttribute;
            Multiplier = multiplier;
        }

        public double GetValue(Character character)
        {
            return character.GetAttribute(AttrName).Value * Multiplier;
        }

        public double GetValue(AttributeBag bag)
        {
            return bag[AttrName] * Multiplier;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Modifier ");
            if (!string.IsNullOrEmpty(AttrName))
            {
                sb.Append(AttrName);
                sb.Append(" * ");
            }
            sb.Append(this.Multiplier.ToString(CultureInfo.InvariantCulture));
            sb.Append("]");
            return sb.ToString();
        }
    }
}