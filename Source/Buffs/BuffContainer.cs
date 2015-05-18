using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Buffs
{
    public class BuffContainer : IDisposable
    {
        Character _owner;
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
            Messenger.RemoveListener("Update", Update);
            _owner = null;

            foreach (Buff buff in _buffs.Values)
            {
                buff.Dispose();
            }

            _buffs.Clear();
            _buffs = null;
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
            if (buff.StackBehavior == StackBehavior.Nothing)
                return;
            if (Utils.Checkflag(buff.StackBehavior, StackBehavior.IncreaseStacks))
            {
                if (buff.CurrentStacks < buff.MaxStacks)
                {
                    buff.CurrentStacks++;
                    buff.OnStack();
                }
            }
            if (Utils.Checkflag(buff.StackBehavior, StackBehavior.ResetBeginning))
            {
                buff.ResetBeginning();
            }
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

        /// <summary>
        /// This method finds the given Buff in this container by its Identifier.
        /// </summary>
        /// <param name="buff">Buff to find by Identifier.</param>
        /// <returns>Buff instance in the container.</returns>
        public Buff GetBuff(Buff buff)
        {
            string id = buff.Identifier;
            return _buffs.ContainsKey(id) ? _buffs[id] : null;
        }

        List<string> _toDispose;

        /// <summary>
        /// This event is raised whenever a Buff was detached from this container.
        /// </summary>
        public event BuffDel BuffDetached = delegate { }; 

        void Update()
        {
            _toDispose = new List<string>();
            foreach (Buff buff in _buffs.Values)
            {
                if (buff.ShouldDispose())
                {
                    _toDispose.Add(buff.Identifier);
                }
            }

            foreach (string id in _toDispose)
            {
                Buff toDetach = _buffs[id];
                _buffs.Remove(id);
                BuffDetached(_owner, toDetach);
            }

            _toDispose.Clear();
            _toDispose = null;
        }
    }
}