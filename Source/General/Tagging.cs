using System.Collections;
using System.Collections.Generic;

namespace Quark
{
    public class DynamicTags : IEnumerable<object>
    {
        private readonly Dictionary<string, int> _tagc;
        private readonly Dictionary<string, object> _tags;

        public DynamicTags()
        {
            _tagc = new Dictionary<string, int>();
            _tags = new Dictionary<string, object>();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _tags.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tags.Values.GetEnumerator();
        }

        public void Add(string key)
        {
            Add(key, true);
        }

        public void Add(string key, object value)
        {
            if (!Has(key))
                _tagc.Add(key, 0);

            _tagc[key]++;
            _tags.Add(key, value);
        }

        public bool Has(string key)
        {
            return _tagc.ContainsKey(key) && _tagc[key] > 0;
        }

        public object Get(string key)
        {
            return _tags[key];
        }

        public void Delete(string key)
        {
            if (!Has(key))
                return;

            _tagc[key]--;

            if (_tagc[key] < 1)
            {
                _tags.Remove(key);
                _tagc.Remove(key);
            }
        }

        public bool this[string key]
        {
            get
            {
                return Has(key);
            }
        }

        public static DynamicTags operator +(DynamicTags collection, string tag)
        {
            collection.Add(tag);
            return collection;
        }

        public static DynamicTags operator -(DynamicTags collection, string tag)
        {
            collection.Delete(tag);
            return collection;
        }
    }

    public class StaticTags : IEnumerable<string>
    {
        private readonly Dictionary<string, int> _tags;

        public StaticTags()
        {
            _tags = new Dictionary<string, int>();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _tags.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tags.Keys.GetEnumerator();
        }

        public void Add(string key)
        {
            _tags.Add(key, 1);
        }

        public bool Has(string key)
        {
            return _tags.ContainsKey(key);
        }

        public object Get(string key)
        {
            return _tags[key];
        }

        public bool this[string key]
        {
            get
            {
                return Has(key);
            }
        }
    }

    public interface ITaggable : ITagged
    {
        DynamicTags Tags { get; }
        void Tag(string tag);
        void Tag(string tag, object value);
        void Untag(string tag);
        object GetTag(string tag);
    }

    public interface ITagged
    {
        bool IsTagged(string tag);
    }
}
