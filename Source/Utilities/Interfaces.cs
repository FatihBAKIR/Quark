namespace Quark.Utilities
{
    public interface Identifiable
    {
        string Identifier
        {
            get;
        }
    }

    public interface IMessage
    {
        void Broadcast();
    }

    public interface ITaggable
    {
        string[] Tags
        {
            get;
            set;
        }
    }
}