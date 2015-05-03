using System;
using System.Collections.Generic;
using System.Text;
using Quark.Attributes;
using Quark.Buffs;

namespace Quark
{
    public delegate void StatDel(Character source, Stat stat, float change);
    public delegate void BuffDel(Character source, Buff buff);
    public delegate void CollisionDel(QuarkCollision collision);
}
