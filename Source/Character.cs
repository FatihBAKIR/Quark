using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Quark.Attribute;
using Quark.Buff;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;
using System.ComponentModel;

namespace Quark
{
    public class Targetable : MonoBehaviour, Identifiable
    {
        public bool IsTargetable { get; set; }

        public string Identifier
        {
            get
            {
                return GetHashCode().ToString();
            }
        }
    }

    public class Character : Targetable
    {
        AttributeBag _attributes;
        List<Cast> _casting;
        BuffContainer _buffs;
        //TODO: items

        void Awake()
        {
            _attributes = new AttributeBag(this);
            _buffs = new BuffContainer(this);
            _casting = new List<Cast>();
        }

        public virtual void Start()
        {
            IsTargetable = true;
        }

        public Character()
        {
#if DEBUG
            Logger.GC("Character::ctor");
#endif
        }

        public Character(Character obj)
        {
#if DEBUG
            Logger.GC("Character::cctor");
#endif
        }

        ~Character()
        {
#if DEBUG
            Logger.GC("Character::dtor");
#endif
        }

        public virtual Attribute.Attribute GetAttribute(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");
            return _attributes.GetAttribute(tag);
        }

        public virtual Stat GetStat(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");
            return _attributes.GetStat(tag);
        }

        public Cast[] GetCasts
        {
            get
            {
                return _casting.ToArray();
            }
        }

        public virtual bool CanCast
        {
            get
            {
                return _casting.Count == 0;
            }
        }

        public void AddCast(Cast cd)
        {
            if (CanCast)
                _casting.Add(cd);
        }

        public void ClearCast(Cast cd)
        {
            _casting.Remove(cd);
        }

        public void AttachBuff(Buff.Buff buff)
        {
            _buffs.AttachBuff(buff);
        }

        public List<Buff.Buff> Buffs
        {
            get
            {
                return _buffs.Buffs;
            }
        }

        public void ApplyBases(Dictionary<string, float> bases)
        {
            _attributes.ApplyBases(bases);
        }

        public Attribute.Attribute[] GetAttributes
        {
            get
            {
                return _attributes.GetAttributes();
            }
        }
    }

    struct MouseArgs : IMessage
    {
        public Character Character { get; private set; }

        public Vector3 Point { get; private set; }

        public MouseEventType Type { get; private set; }

        public bool IsCharacter { get; private set; }

        public MouseArgs(Character character, MouseEventType type)
            : this()
        {
            this.Character = character;
            this.Type = type;
            this.Point = Vector3.zero;
            this.IsCharacter = true;
        }

        public MouseArgs(Vector3 point)
            : this()
        {
            this.Character = null;
            this.Point = point;
            this.IsCharacter = false;
            this.Type = MouseEventType.Click;
        }

        public void Broadcast()
        {
            Messenger<MouseArgs>.Broadcast("Mouse", this);
            Messenger<MouseArgs>.Broadcast(this.Type + ".Mouse", this);
            if (this.IsCharacter)
                Messenger<MouseArgs>.Broadcast(this.Character.Identifier + "." + this.Type + ".Mouse", this);
        }
    }

    enum MouseEventType
    {
        Enter,
        Exit,
        Click,
        Hover
    }
}