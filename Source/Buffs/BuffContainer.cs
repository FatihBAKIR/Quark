﻿using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Buffs
{
    public class BuffContainer : IDisposable
    {
        Character _owner;
        //List<Buff> _buffs;
        private Dictionary<string, Buff> _buffs;

        public BuffContainer(Character owner)
        {
            _owner = owner;
            _buffs = new Dictionary<string, Buff>();
            Messenger.AddListener("Update", Update);
        }

        public IList<Buff> Buffs
        {
            get { return new List<Buff>(_buffs.Values); }
        }

        public void Dispose()
        {
            _owner = null;
            _buffs.Clear();
            _buffs = null;
            Messenger.RemoveListener("Update", Update);
        }

        public void AttachBuff(Buff buff)
        {
            Buff existing;
            if ((existing = GetBuff(buff)) != null)
            {
                StackBuff(existing);
                return;
            }
            _buffs.Add(buff.Identifier, buff);
            buff.Possess(_owner);
        }

        void StackBuff(Buff buff)
        {
            if (buff.StackBehaviour == StackBehavior.Nothing)
                return;
            if (Utils.Checkflag(buff.StackBehaviour, StackBehavior.IncreaseStacks))
            {
                if (buff.CurrentStacks < buff.MaxStacks)
                    buff.CurrentStacks++;
            }
            if (Utils.Checkflag(buff.StackBehaviour, StackBehavior.ResetBeginning))
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

        public T HasBuff<T>() where T : Buff
        {
            /*
            foreach (Buff buff in _buffs)
                if (buff is T)
                    return (T)buff;
             */
            return null;
        }

        public Buff GetBuff(Buff buff)
        {
            string id = buff.Identifier;
            if (_buffs.ContainsKey(id))
                return _buffs[id];
            return null;
        }

        List<string> _toDispose;

        void Update()
        {
            _toDispose = new List<string>();
            foreach (Buff buff in _buffs.Values)
            {
                if (CheckBuff(buff))
                {
                    _toDispose.Add(buff.Identifier);
                }
            }

            foreach (string id in _toDispose)
                _buffs.Remove(id);

            _toDispose.Clear();
            _toDispose = null;
        }
    }
}