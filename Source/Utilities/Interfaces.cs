using System.Diagnostics.CodeAnalysis;

namespace Quark.Utilities
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
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

    public interface IDeepCopiable
    {
        object DeepCopy();
    }

    public interface IDeepCopiable<out T> : IDeepCopiable
    {
        new T DeepCopy();
    }
}