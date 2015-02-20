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
            return _attributes.GetAttribute(tag);
        }

        public virtual Stat GetStat(string tag)
        {
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

        /// <summary>
        /// Returns a readonly collection of the Buffs being carried by this Character
        /// </summary>
        /// <value>The buffs.</value>
        public IList<Buff.Buff> Buffs
        {
            get
            {
                return _buffs.Buffs;
            }
        }

        /// <summary>
        /// If a buff with the given type exists on this Character, it will return the correct instance on the Character, otherwise it will return null.
        /// </summary>
        /// <returns>The buff instance being carried by this Character.</returns>
        /// <param name="buff">Example of the Buff to find. Only types should match.</param>
        public Buff.Buff GetBuff(Buff.Buff buff)
        {
            return _buffs.GetBuff(buff);
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
}