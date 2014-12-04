using System;
using UnityEngine;

namespace Quark
{
    public class Head : MonoBehaviour
    {
        void Start()
        {
            if (Head.head != null)
            {
                DestroyImmediate(this);
                Quark.Logger.Warn("Multiple Head Objects Are Forbidden");
                return;
            }
            Head.head = this;
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