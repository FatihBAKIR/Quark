using System;
using System.Collections.Generic;
using System.Text;
using Quark.Spell;
using UnityEngine;

namespace Quark.Targeting
{
    public interface IRanged
    {
        float CastRange
        {
            get;
        }
    }

    public class TargetMacro
    {
        protected CastData Data { get; private set; }
        public TargetMacro()
        {
        }

        public virtual void Run()
        {
            Cancel();
        }

        public virtual void Cancel()
        {
            TargetManager.FreeTargeter();
            Data.TargetingFail();
            Clear();
        }

        protected void Clear()
        {
            Data = null;
        }

        public void SetData(CastData data)
        {
            Data = data;
        }

        public Character Caster
        {
            get
            {
                return Data.Caster;
            }
        }
    }
}
