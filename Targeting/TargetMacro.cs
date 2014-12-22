using System;
using System.Collections.Generic;
using System.Text;
using Quark.Spell;
using UnityEngine;

namespace Quark.Targeting
{
    public class TargetMacro
    {
        protected CastData Data { get; set; }
        public TargetMacro()
        {
        }

        public virtual void Run()
        {

        }

        public void SetData(CastData data)
        {
            this.Data = data;
        }
    }

    public class PointTarget : TargetMacro
    {
        public override void Run()
        {
            TargetManager.ReserveTargeter(this);
            TargetManager.RequestPoint(HandlePoint);
        }

        private void HandlePoint(Vector3 point)
        {
            Data.AddTarget(point);
            TargetManager.FreeTargeter();
            Data.TargetingDone();
        }
    }
}
