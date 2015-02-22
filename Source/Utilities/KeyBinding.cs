using Quark.Spells;
using UnityEngine;

namespace Quark.Utilities
{
    interface IBinding
    {
        void Register();
        void Check();
    }

    public class KeyBinding<T> : IBinding where T : Spells.Spell, new()
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
                Cast.PrepareCast(QuarkMain.GetInstance().Player, new T());
        }
    }
}

