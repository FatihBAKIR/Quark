using Quark.Targeting;
using UnityEngine;
using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark
{
    /// <summary>
    /// QuarkMain is the singleton object which starts the default Quark subroutines
    /// </summary>
    public class QuarkMain : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (IsPresent)
            {
                DestroyImmediate(this);
                Logger.Warn("The QuarkMain object must be unique!");
                return;
            }
            _headRef = new WeakReference(this);

            _daemons = new List<Daemon>
            {
                new KeyBindings(),
                new TargetManager()
            };

            foreach (Daemon daemon in _daemons)
                daemon.Register();

            Logger.Debug("QuarkMain::Start");
        }

        void Update()
        {
            Messenger.Broadcast("Update");
        }

        private List<Daemon> _daemons = new List<Daemon>();

        public T GetDaemon<T>() where T : Daemon
        {
            foreach (Daemon daemon in _daemons)
            {
                if (daemon is T)
                    return (T)daemon;
            }
            return null;
        }

        public void AddDaemon(Daemon daemon)
        {
            _daemons.Add(daemon);
            daemon.Register();
        }

        public void TerminateDaemon(Daemon daemon)
        {
            daemon.Terminate();
            _daemons.Remove(daemon);
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