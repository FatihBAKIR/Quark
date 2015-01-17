using Quark.Targeting;
using UnityEngine;

namespace Quark.Utilities
{
    public class Head : MonoBehaviour
    {
        void Start()
        {
            if (Head.head != null)
            {
                DestroyImmediate(this);
                    Logger.Warn("The Head Object Must Be Unique!");
                return;
            }
            Head.head = this;
            
            TargetManager.Register();
            KeyBindings.Register();
            Logger.Info("Head::Start");
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
                return Head.player;
            }
            set
            {
                Head.player = value;
            }
        }

        static Character player = null;

        public static Head head = null;
    }
}