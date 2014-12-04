using System;
using UnityEngine;

namespace Quark
{
    interface IBinding
    {
        void Register();
        void Check();
    }

    public class KeyBinding<T> : IBinding where T : Spell, new()
    {
        KeyCode key;

        public KeyBinding(KeyCode Key)
        {
            this.key = Key;
        }

        public void Register()
        {
            KeyBindings.AddBinding(this);
        }

        public void Check()
        {
            if (Input.GetKeyUp(key))
                CastData.PrepareCast(Head.Player, new T());
        }
    }
}

