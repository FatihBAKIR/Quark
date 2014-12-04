using System;
using System.Collections.Generic;

namespace Quark
{
    class KeyBindings
    {
        static List<IBinding> bindings = new List<IBinding>();

        public static void AddBinding(IBinding binding)
        {
            bindings.Add(binding);
        }

        public static void Register()
        {
            Messenger.AddListener("Update", Update);
        }

        static void Update()
        {
            foreach (IBinding binding in bindings)
                binding.Check();
        }
    }
}

