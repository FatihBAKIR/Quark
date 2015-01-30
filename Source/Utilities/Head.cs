using Quark.Targeting;
using UnityEngine;
using System;

namespace Quark.Utilities
{
    /// <summary>
    /// Head is the singleton object which starts the default Quark subroutines
    /// </summary>
    public class Head : MonoBehaviour
    {
        void Start()
        {
            if (IsHeadPresent)
            {
                DestroyImmediate(this);
                Logger.Warn("The Head object must be unique!");
                return;
            }
            Head._headRef = new WeakReference(this);
            
            TargetManager.Register();
            KeyBindings.Register();
            Logger.Debug("Head::Start");
        }

        void Update()
        {
            Messenger.Broadcast("Update");
        }

        /// <summary>
        /// Gets or sets the active player Character.
        /// </summary>
        /// <value>The player.</value>
        public static Character Player
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
            }
        }

        static Character _player = null;

        /// <summary>
        /// A weak reference to the singleton Head object.
        /// It is accessed via a weak reference so that it will be garbage collected after the GameObject it is attached to is destroyed
        /// </summary>
        private static WeakReference _headRef = null;

        static bool IsHeadPresent
        {
            get
            {
                return _headRef != null && _headRef.IsAlive;
            }
        }

        /// <summary>
        /// Gets the singleton Head instance.
        /// </summary>
        /// <returns>The instance</returns>
        public static Head GetInstance()
        {
            if (IsHeadPresent)
                return (Head)_headRef.Target;

            if (_headRef == null)
            {
                Logger.Error("Trying to access Quark without creating the Head object");
                throw new Exception("Trying to access Quark without creating the Head object");
            }

            Logger.Error("Head object is not present");
            throw new Exception("Head object is not present");
        }
    }
}