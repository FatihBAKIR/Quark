namespace Quark.Contexts
{
    public class BuffContext : Context
    {
        public BuffContext(Context parent) : base(parent.Source)
        {
            Parent = parent;
            Parent.AddChild(this);
        }
    }
}
