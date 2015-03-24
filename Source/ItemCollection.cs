using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark
{
    /// TODO: let items carry other items in form of a bag

    public class ItemCollection : Item, IEnumerable<Item>, IDisposable, IBag
    {
        private Dictionary<string, Item> _items;
        private int _maxSize = 0;

        /// <summary>
        /// Initialize a new item container
        /// </summary>
        public ItemCollection(Character carrier = null, int size = 0)
        {
            _maxSize = size;
            Carrier = carrier;
            _items = new Dictionary<string, Item>();
        }

        ~ItemCollection()
        {
            Dispose();
        }

        public void Dispose()
        {
            Carrier = null;
            _items.Clear();
            _items = null;
        }

        public void AddItem(Item item)
        {
            if (!CanAdd())
                return;

            item.SetCarrier(Carrier);

            if (!item.CanGrab())
                return;

            if (Has(item))
            {
                Item existing = GetItem(item);
                existing.CurrentStacks++;
                existing.CurrentStacks = Math.Min(existing.CurrentStacks, existing.MaxStacks);
                existing.OnStack();
                return;
            }

            _items.Add(item.Identifier, item);
            item.OnGrab();
        }

        public bool HasItem(Item item)
        {
            if (_items.ContainsKey(MakeID(item, Carrier)))
                return true;

            foreach (Item search in _items.Values)
            {
                if (!(search is ItemCollection))
                    continue;

                ItemCollection bag = (ItemCollection)search;
                if (bag.Has(item))
                    return true;
            }

            return false;
        }

        public Item GetItem(Item item)
        {
            return _items[MakeID(item, Carrier)];
        }

        public bool RemoveItem(Item item)
        {
            return _items.Remove(MakeID(item, Carrier));
        }

        public int Size
        {
            get { return _maxSize; }
        }

        public int Empty
        {
            get { return _maxSize - _items.Count; }
        }

        /// <summary>
        /// Add a new item to this container
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Add(Item item)
        {
            AddItem(item);
        }

        public bool Remove(Item item)
        {
            return RemoveItem(item);
        }

        public bool Has(Item item)
        {
            return HasItem(item);
        }

        public bool Equipped(Item item)
        {
            return _items.ContainsKey(MakeID(item, Carrier));
        }

        public bool CanAdd()
        {
            return _maxSize == 0 || _items.Count < _maxSize;
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
