using Quark.Spells;
using Quark.Targeting;
using UnityEngine;

namespace Quark.Utilities
{
    interface IBinding
    {
        void Register();
        void Check();
    }

    public class KeyBinding<T> : IBinding where T : Spell, new()
    {
        readonly KeyCode _key;

        public KeyBinding(KeyCode key)
        {
            _key = key;
        }

        public void Register()
        {
            KeyBindings.GetInstance().AddBinding(this);
        }

        public void Check()
        {
            if (Input.GetKeyUp(_key))
                Cast.PrepareCast(QuarkMain.GetInstance().Player, new T());
        }
    }
}

