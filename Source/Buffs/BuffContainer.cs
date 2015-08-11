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
            Messenger.AddListener("Update", Update);
        }

        public IList<IBuff> Buffs
        {
            get { return new List<IBuff>(_buffs.Values); }
        }

        public void Dispose()
        {
            Messenger.RemoveListener("Update", Update);
            _owner = null;

            foreach (IBuff buff in _buffs.Values)
            {
                buff.Dispose();
            }

            _buffs.Clear();
            _buffs = null;
        }

        /// <summary>
        /// This method attaches a given buff to this collection.
        /// If the given buff already exists, then the stacking logic will take place for the buff.
        /// </summary>
        /// <param name="buff">Buff to attach</param>
        public void AttachBuff(IBuff buff)
        {
            IBuff existing;

            if ((existing = GetBuff(buff)) != null)
            {
                if (StackBuff(existing))
                    return;
            }

            buff.Terminated += BuffTerminated;

            _buffs.Add(buff.Identifier, buff);
            buff.Possess(_owner);
        }

        void BuffTerminated(Character source, IBuff buff)
        {
            buff.Terminated -= BuffTerminated;

            if (_owner == null)
            {
                // The collection is disposing, don't remove it yet.
                return;
            }

            Remove(buff.Identifier);
        }

        bool StackBuff(IBuff buff)
        {
            if (buff.StackBehavior == StackBehavior.Replace)
            {
                buff.Terminate(true);
                return false;
            }

            if (buff.StackBehavior == StackBehavior.Nothing)
                return true;

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

            return true;
        }

        /// <summary>
        /// This method returns the first instance of a buff type in this collection.
        /// </summary>
        /// <typeparam name="T">Type of the buff</typeparam>
        /// <returns>The instance of the buff</returns>
        public T GetBuff<T>() where T : class, IBuff
        {
            return _buffs.Values.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// This method returns the collection of a buff type instances in this collection.
        /// </summary>
        /// <typeparam name="T">Type of the collection</typeparam>
        /// <returns>All instances of the buff</returns>
        public T[] GetBuffs<T>() where T : class, IBuff
        {
            return _buffs.Values.OfType<T>().ToArray();
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

        /// <summary>
        /// This method will remove a buff with a given ID from this collection.
        /// 
        /// <remarks>This method will not check whether the buff already exists or not!</remarks>
        /// </summary>
        /// <param name="id">Identifier of the buff</param>
        void Remove(string id)
        {
            IBuff toDetach = _buffs[id];
            _buffs.Remove(id);
            BuffDetached(_owner, toDetach);
        }

        void Update()
        {
            _toDispose = new List<string>();
            foreach (KeyValuePair<string, IBuff> pair in _buffs)
            {
                if (pair.Value.ShouldDispose())
                {
                    _toDispose.Add(pair.Key);
                }
            }

            foreach (string id in _toDispose)
            {
                Remove(id);
            }

            _toDispose.Clear();
            _toDispose = null;
        }
    }
}