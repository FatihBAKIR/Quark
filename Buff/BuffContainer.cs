using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Buff
{
    public class BuffContainer : IDisposable
    {
        Character _owner;
        List<Buff> _buffs;

        public BuffContainer(Character owner)
        {
            _owner = owner;
            _buffs = new List<Buff>();
            Messenger.AddListener("Update", this.Update);
        }

        public void Dispose()
        {
            _owner = null;
            _buffs.Clear();
            _buffs = null;
            Messenger.RemoveListener("Update", this.Update);
        }

        public void AttachBuff(Buff buff)
        {
            _buffs.Add(buff);
            buff.OnPossess(_owner);
        }

        bool CheckBuff(Buff buff)
        {
            if (buff.LifeRatio >= 1)
                return true;
            return false;
        }

        List<Buff> _toDispose;
        void Update()
        {
            _toDispose = new List<Buff>();
            foreach (Buff buff in _buffs)
            {
                if (CheckBuff(buff))
                {
                    _toDispose.Add(buff);
                }
            }
            foreach (Buff buff in _toDispose)
            {
                _buffs.Remove(buff);
                buff.Dispose();
            }
            _toDispose.Clear();
            _toDispose = null;
        }
    }
}