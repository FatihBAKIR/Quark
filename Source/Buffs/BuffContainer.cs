using System;
using System.Collections.Generic;
using System.Linq;
using Quark.Contexts;
using Quark.Utilities;

namespace Quark.Buffs
{
    public class BuffContainer : IDisposable
    {
        Character _owner;
        private Dictionary<string, IBuff> _buffs;

        public BuffContainer(Character owner)
        {
            _owner = owner;
            _buffs = new Dictionary<string, IBuff>();
        }

        public IList<IBuff> Buffs
        {
            get { return new List<IBuff>(_buffs.Values); }
        }

        public void Dispose()
        {
            _owner = null;

            foreach (IBuff buff in _buffs.Values)
            {
                buff.Dispose();
            }

            _buffs.Clear();
            _buffs = null;
        }

        public void AttachBuff(IBuff buff)
        {
            IBuff existing;
            if ((existing = GetBuff(buff)) != null)
            {
                StackBuff(existing);
                return;
            }
            _buffs.Add(buff.Identifier, buff);
            buff.Possess(_owner);
        }

        void StackBuff(IBuff buff)
        {
            if (buff.StackBehavior == StackBehavior.Nothing)
                return;
            if (Utils.Checkflag(buff.StackBehavior, StackBehavior.IncreaseStacks))
            {
                if (buff.CurrentStacks < buff.MaximumStacks)
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

        public T GetBuff<T>() where T : class, IBuff
        {
            return _buffs.Values.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// This method finds the given Buff in this container by its Identifier.
        /// </summary>
        /// <param name="buff">Buff to find by Identifier.</param>
        /// <returns>Buff instance in the container.</returns>
        public IBuff GetBuff(IBuff buff)
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
            foreach (IBuff buff in _buffs.Values)
            {
                if (buff.ShouldDispose())
                {
                    _toDispose.Add(buff.Identifier);
                }
            }

            foreach (string id in _toDispose)
            {
                IBuff toDetach = _buffs[id];
                _buffs.Remove(id);
                BuffDetached(_owner, toDetach);
            }

            _toDispose.Clear();
            _toDispose = null;
        }
    }
}