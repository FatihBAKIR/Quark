using Quark.Attributes;

namespace Quark
{
    public class QuarkConfig
    {
        public virtual AttributeCollection DefaultAttributes
        {
            get { return new AttributeCollection(); }
        }

        public virtual ConditionCollection DefaultInterruption
        {
            get { return new ConditionCollection(); }
        }
    }
}
