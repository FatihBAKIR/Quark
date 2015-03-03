using System.Collections;
using System.Collections.Generic;

namespace Quark
{
    public class TagCollection : IEnumerable<object>
    {
        private Dictionary<string, int> _tagc;
        private Dictionary<string, object> _tags;

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
                _tags.Remove(key);
        }
    }

    public interface ITaggable
    {
        TagCollection Tags { get; }
        void Tag(string tag);
        void Tag(string tag, object value);
        void Untag(string tag);
        bool IsTagged(string tag);
        object GetTag(string tag);
    }
}
