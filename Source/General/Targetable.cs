using Quark.Utilities;
using UnityEngine;

namespace Quark
{
    public class Targetable : MonoBehaviour, Identifiable, ITaggable
    {
        public bool IsTargetable { get; set; }
        public string Identifier
        {
            get
            {
                return GetHashCode().ToString();
            }
        }
        public DynamicTags Tags { get; protected set; }

        public void Tag(string tag)
        {
            Tags.Add(tag);
        }

        public void Tag(string tag, object value)
        {
            Tags.Add(tag, value);
        }

        public void Untag(string tag)
        {
            Tags.Delete(tag);
        }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }

        public object GetTag(string tag)
        {
            return Tags.Get(tag);
        }
    }
}
