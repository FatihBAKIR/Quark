using System;
using Quark.Spell;

namespace Quark
{
    public class Condition
    {
        protected CastData _data;
        public Condition()
        {
        }

        public void Introduce(CastData data)
        {
            _data = data;
        }

    }
}

