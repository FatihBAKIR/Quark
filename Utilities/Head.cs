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
                    Logger.Warn("The Quark Head Must Be Unique!");
                return;
            }
            Head.head = this;
            
            TargetManager.Register();
            KeyBindings.Register();
            Logger.Info("Head Started");
        }

        void Update()
        {
            Messenger.Broadcast("Update");
        }

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