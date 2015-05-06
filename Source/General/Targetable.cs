﻿using Quark.Utilities;
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

        void OnCollisionEnter(Collision hit)
        {
            if (hit.gameObject.Equals(gameObject))
                return;
            if (hit.gameObject.GetComponent<Targetable>() != null)
                OnQuarkCollision(new QuarkCollision(this, hit.gameObject.GetComponent<Targetable>()));
        }

        void OnTriggerEnter(Collider hit)
        {
            if (hit.gameObject.Equals(gameObject))
                return;
            if (hit.gameObject.GetComponent<Targetable>() != null)
                OnQuarkCollision(new QuarkCollision(this, hit.gameObject.GetComponent<Targetable>()));
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.Equals(gameObject))
                return;
            if (hit.gameObject.GetComponent<Targetable>() != null)
                OnQuarkCollision(new QuarkCollision(this, hit.gameObject.GetComponent<Targetable>()));
        }

        void OnQuarkCollision(QuarkCollision collision)
        {
            Messenger<QuarkCollision>.Broadcast("QuarkCollision", collision);
            QuarkCollision(collision);
        }

        //TODO: Event for tagging and untagging

        /// <summary>
        /// This event is raised when this Character collides with another Targetable
        /// </summary>
        public event CollisionDel QuarkCollision = delegate { };
    }
}
