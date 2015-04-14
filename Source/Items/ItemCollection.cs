using System;
using System.Collections;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark
{
    /// TODO: let items carry other items in form of a bag

    public class ItemCollection : Item, IEnumerable<Item>, IDisposable, IBag, IBagRecursive
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

        public IList<Item> Items()
        {
            return new List<Item>(_items.Values);
        }

        public void Dispose()
        {
            Carrier = null;
            _items.Clear();
            _items = null;
        }

        public bool AddItem(Item item)
        {
            if (!CanAdd())
                return false;

            item.SetCarrier(Carrier);

            if (!item.CanGrab())
                return false;

            if (Has(item))
            {
                Item existing = GetItem(item);
                existing.CurrentStacks++;
                existing.CurrentStacks = Math.Min(existing.CurrentStacks, existing.MaxStacks);
                existing.OnStack();
                return true;
            }

            _items.Add(item.Identifier, item);
            item.OnGrab();

            return true;
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
            if (!HasItem(item)) return null;
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



        public int SizeRecursive
        {
            get
            {
                int size = _maxSize;
                foreach (KeyValuePair<string, Item> item in _items)
                {
                    if (item.Value is IBag)
                        size += ((IBag) item.Value).Size;
                    if (item.Value is IBagRecursive)
                        size += ((IBagRecursive) item.Value).SizeRecursive;
                }
                return size;
            }
        }

        public int EmptyRecursive
        {
            get
            {
                int empty = Empty;
                foreach (KeyValuePair<string, Item> item in _items)
                {
                    if (item.Value is IBag)
                        empty += ((IBag)item.Value).Empty;
                    if (item.Value is IBagRecursive)
                        empty += ((IBagRecursive)item.Value).EmptyRecursive;
                }
                return empty;
            }
        }

        public bool HasItemRecursive(Item item)
        {
            if (HasItem(item))
                return true;

            foreach (KeyValuePair<string, Item> i in _items)
            {
                if (i.Value is IBag)
                    if (((IBag) i.Value).HasItem(item))
                        return true;
                if (i.Value is IBagRecursive)
                    if (((IBagRecursive) i.Value).HasItemRecursive(item))
                        return true;
            }

            return false;
        }

        public bool AddItemRecursive(Item item)
        {
            if (AddItem(item))
                return true;

            foreach (KeyValuePair<string, Item> bag in _items)
            {
                if (bag.Value is IBag)
                    if (((IBag) bag.Value).AddItem(item)) return true;
                if (bag.Value is IBagRecursive)
                    if (((IBagRecursive) bag.Value).AddItemRecursive(item)) return true;
            }

            return false;
        }

        public Item GetItemRecursive(Item item)
        {
            Item i;
            if ((i = GetItem(item)) != null)
                return i;

            foreach (KeyValuePair<string, Item> bag in _items)
            {
                if (bag.Value is IBag)
                    if ((i = ((IBag)bag.Value).GetItem(item)) != null) return i;
                if (bag.Value is IBagRecursive)
                    if ((i = ((IBagRecursive)bag.Value).GetItemRecursive(item)) != null) return i;
            }

            return null;
        }

        public bool RemoveItemRecursive(Item item)
        {
            if (RemoveItem(item))
                return true;

            foreach (KeyValuePair<string, Item> bag in _items)
            {
                if (bag.Value is IBag)
                    if (((IBag)bag.Value).RemoveItem(item)) return true;
                if (bag.Value is IBagRecursive)
                    if (((IBagRecursive)bag.Value).RemoveItem(item)) return true;
            }

            return false;
        }

        public void SetCarrierRecursive(Character carrier)
        {
            SetCarrier(carrier);
            foreach (KeyValuePair<string, Item> bag in _items)
            {
                if (bag.Value is IBag)
                    ((IBag)bag.Value).SetCarrier(carrier);
                if (bag.Value is IBagRecursive)
                    ((IBagRecursive)bag.Value).SetCarrierRecursive(carrier);
            }
        }
    }
}
