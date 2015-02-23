using System.Globalization;
using System.Text;

namespace Quark.Attributes
{
    public class AttributeModifier
    {
        public float Multiplier { get; set; }

        public string AttrName { get; set; }

        public AttributeModifier(string sourceAttribute, float multiplier)
        {
            AttrName = sourceAttribute;
            Multiplier = multiplier;
        }

        public float GetValue(Character character)
        {
            if (AttrName == null)
                return Multiplier;
            return character.GetAttribute(AttrName).Value * Multiplier;
        }

        public float GetValue(AttributeCollection collection)
        {
            if (AttrName == null)
                return Multiplier;
            return collection[AttrName] * Multiplier;
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