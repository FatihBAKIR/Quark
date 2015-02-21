using Quark.Attribute;

namespace Quark
{
    public class QuarkConfig
    {
        public virtual AttributeCollection DefaultAttributes
        {
            get { return new AttributeCollection(); }
        }
    }
}
