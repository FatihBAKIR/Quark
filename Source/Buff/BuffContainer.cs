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

        public List<Buff> Buffs
        {
            get
            {
                return _buffs;
            }
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
            Buff existing;
            if ((existing = GetBuff(buff)) != null)
            {
                Logger.Error("ALREADY HAS");
                StackBuff(existing);
                return;
            }
            _buffs.Add(buff);
            buff.OnPossess(_owner);
        }

        void StackBuff(Buff buff)
        {
            if (buff.Behaviour == StackBehavior.Nothing)
                return;
            if (Utils.Checkflag(buff.Behaviour, StackBehavior.IncreaseStacks))
            {
                if (buff.CurrentStacks < buff.MaxStacks)
                    buff.CurrentStacks++;
            }
            if (Utils.Checkflag(buff.Behaviour, StackBehavior.ResetBeginning))
            {
                buff.ResetBeginning();
            }
        }

        bool CheckBuff(Buff buff)
        {
            if (buff.LifeRatio >= 1)
                return true;
            return false;
        }

        T HasBuff<T>() where T : Buff
        {
            foreach (Buff buff in _buffs)
                if (buff is T)
                    return (T)buff;
            return null;
        }

        Buff GetBuff(Buff buff)
        {
            string id = buff.Identifier;
            foreach (Buff existing in _buffs)
                if (existing.Identifier == id)
                    return existing;
            return null;
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
                _buffs.Remove(buff);

            _toDispose.Clear();
            _toDispose = null;
        }
    }
}