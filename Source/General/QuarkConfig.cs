using Quark.Attributes;

namespace Quark
{
    /// <summary>
    /// This class is used for configuring a Quark game.
    /// It includes the base Attributes and Stats and the common interruption conditions.
    /// </summary>
    public class QuarkConfig
    {
        public AttributeCollection DefaultAttributes = new AttributeCollection();
        public ConditionCollection DefaultInterruption = new ConditionCollection();
    }
}
