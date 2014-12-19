using System;
using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Buff
{
    public class BuffContainer : IDisposable
    {
        Character owner;
        List<Buff> buffs;

        public BuffContainer(Character owner)
        {
            this.owner = owner;
            this.buffs = new List<Buff>();
            Messenger.AddListener("Update", this.Update);
        }

        public void Dispose()
        {
            this.owner = null;
            this.buffs.Clear();
            this.buffs = null;
            Messenger.RemoveListener("Update", this.Update);
        }

        public void AttachBuff(Buff buff)
        {
            this.buffs.Add(buff);
            buff.OnPossess(owner);
        }

        bool CheckBuff(Buff buff)
        {
            if (buff.LifeRatio >= 1)
                return true;
            return false;
        }

        List<Buff> toDispose;

        void Update()
        {
            toDispose = new List<Buff>();
            foreach (Buff buff in buffs)
            {
                if (CheckBuff(buff))
                {
                    toDispose.Add(buff);
                }
            }
            foreach (Buff buff in toDispose)
            {
                buffs.Remove(buff);
                buff.Dispose();
            }
            toDispose.Clear();
            toDispose = null;
        }
    }
}