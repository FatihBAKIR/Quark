using Quark.Attributes;
using Quark.Conditions;
using Quark.Contexts;
using Quark.Utilities;

namespace Quark
{
    /// <summary>
    /// This class is used for configuring a Quark game.
    /// It includes the base Attributes and Stats and the common interruption conditions.
    /// </summary>
    public class QuarkConfig
    {
        public AttributeCollection DefaultAttributes = new AttributeCollection();
        public ConditionCollection<ICastContext> DefaultInterruption = new ConditionCollection<ICastContext> { new FalseCondition() };
        public LogLevel LogLevel
        {
            get { return Logger.Level; }
            set { Logger.Level = value; }
        }
    }
}
