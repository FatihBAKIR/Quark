using System.Diagnostics.CodeAnalysis;

namespace Quark.Utilities
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface Identifiable
    {
        /// <summary>
        /// This property gets the identifier string for the object.
        /// </summary>
        string Identifier
        {
            get;
        }
    }

    public interface IDeepCopiable
    {
        object DeepCopy();
    }

    public interface IDeepCopiable<out T> : IDeepCopiable
    {
        new T DeepCopy();
    }

    public interface IBag
    {
        int Size { get; }
        int Empty { get; }

        bool HasItem(Item item);
        bool AddItem(Item item);
        Item GetItem(Item item);
        bool RemoveItem(Item item);

        void SetCarrier(Character carrier);
    }

    public interface IBagRecursive : IBag
    {
        int SizeRecursive { get; }
        int EmptyRecursive { get; }

        bool HasItemRecursive(Item item);
        bool AddItemRecursive(Item item);
        Item GetItemRecursive(Item item);
        bool RemoveItemRecursive(Item item);

        void SetCarrierRecursive(Character carrier);
    }
}