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
        /// <summary>
        /// This method is used for initializing this QuarkMain instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// This method is called from Unity right after it is instantiated.
        /// </summary>
        void Awake()
        {
            Initialize();

            // This prevents a scene to have multiple QuarkMain objects.
            if (IsPresent)
            {
                DestroyImmediate(this);
                Logger.Warn("The QuarkMain object must be unique!");
                return;
            }

            // Create a weak reference to this object.
            _headRef = new WeakReference(this);

            Logger.Debug("QuarkMain::Start");
        }

        public List<Action> OnUpdate = new List<Action>();

        /// <summary>
        /// This method is called every frame from Unity.
        /// We use it to broadcast the Update event.
        /// </summary>
        void Update()
        {
            OnUpdate.ForEach(f => f());
            //Somehow call Update stuff
        }

        /// <summary>
        /// This property stores the configuration for this game.
        /// </summary>
        public QuarkConfig Configuration { get; protected set; } 

        /// <summary>
        /// A weak reference to the singleton QuarkMain object.
        /// It is accessed via a weak reference so that it will be garbage collected after the GameObject it is attached to is destroyed
        /// </summary>
        private static WeakReference _headRef;

        /// <summary>
        /// This property determines whether a QuarkMain object is already instantiated or not.
        /// </summary>
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