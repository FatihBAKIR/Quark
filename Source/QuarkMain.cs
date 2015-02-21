using Quark.Targeting;
using UnityEngine;
using System;
using Quark.Utilities;

namespace Quark
{
    /// <summary>
    /// QuarkMain is the singleton object which starts the default Quark subroutines
    /// </summary>
    public class QuarkMain : MonoBehaviour
    {
        void Awake()
        {
            if (IsPresent)
            {
                DestroyImmediate(this);
                Logger.Warn("The QuarkMain object must be unique!");
                return;
            }
            _headRef = new WeakReference(this);

            TargetManager.Register();
            KeyBindings.Register();
            Logger.Debug("QuarkMain::Start");
        }

        void Update()
        {
            Messenger.Broadcast("Update");
        }

        /// <summary>
        /// Gets or sets the active player Character.
        /// </summary>
        /// <value>The player.</value>
        public Character Player { get; set; }

        public virtual QuarkConfig Config {
            get { return new QuarkConfig(); }
        }

        /// <summary>
        /// A weak reference to the singleton QuarkMain object.
        /// It is accessed via a weak reference so that it will be garbage collected after the GameObject it is attached to is destroyed
        /// </summary>
        private static WeakReference _headRef = null;

        static QuarkMain()
        {
        }

        static bool IsPresent
        {
            get
            {
                return _headRef != null && _headRef.IsAlive;
            }
        }

        /// <summary>
        /// Gets the singleton QuarkMain instance.
        /// </summary>
        /// <returns>The instance</returns>
        public static QuarkMain GetInstance()
        {
            if (IsPresent)
                return (QuarkMain)_headRef.Target;

            if (_headRef == null)
            {
                Logger.Error("Trying to access Quark without creating the QuarkMain object");
                throw new Exception("Trying to access Quark without creating the QuarkMain object");
            }

            Logger.Error("QuarkMain object is not present");
            throw new Exception("QuarkMain object is not present");
        }
    }
}