using System;
using System.Collections;
using System.Collections.Generic;

namespace Quark
{
    /// <summary>
    /// TODO: let items carry other items in form of a bag
    /// </summary>

    public class ItemCollection : Item, IEnumerable<Item>, IDisposable
    {
        private Dictionary<string, Item> _items;
        private uint _maxSize = 0;

        /// <summary>
        /// Initialize a new item container
        /// </summary>
        public ItemCollection(Character carrier = null, uint size = 0)
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

        /// <summary>
        /// Add a new item to this container
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Add(Item item)
        {
            if (!CanAdd())
                return;

            item.SetCarrier(Carrier);

            if (!item.CanGrab())
                return;

            if (Has(item))
            {
                Item existing = Get(item);
                existing.CurrentStacks++;
                existing.CurrentStacks = Math.Min(existing.CurrentStacks, existing.MaxStacks);
                existing.OnStack();
                return;
            }

            _items.Add(item.Identifier, item);
            item.OnGrab();
        }

        public void Remove(Item item)
        {
            _items.Remove(MakeID(item, Carrier));
        }

        public bool Has(Item item)
        {
            if (_items.ContainsKey(MakeID(item, Carrier)))
                return true;

            foreach (Item search in _items.Values)
            {
                if (!(search is ItemCollection))
                    continue;

                ItemCollection bag = (ItemCollection) search;
                if (bag.Has(item))
                    return true;
            }

            return false;
        }

        public bool Equipped(Item item)
        {
            return _items.ContainsKey(MakeID(item, Carrier));
        }

        public Item Get(Item item)
        {
            return _items[MakeID(item, Carrier)];
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
