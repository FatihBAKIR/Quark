namespace Quark.Utilities
{
    public interface Identifiable
    {
        string Identifier();
    }

    public interface IMessage
    {
        void Broadcast();
    }

    public interface IRegisterer
    {
        void Register();
    }
}