using System.Collections.Generic;

namespace Quark.Utilities
{
    class KeyBindings : Daemon<KeyBindings>
    {
        readonly List<IBinding> _bindings = new List<IBinding>();

        public void AddBinding(IBinding binding)
        {
            _bindings.Add(binding);
        }

        public override void Update()
        {
            foreach (IBinding binding in _bindings)
                binding.Check();
        }
    }
}

