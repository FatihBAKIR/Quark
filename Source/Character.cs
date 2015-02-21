using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Quark.Attribute;
using Quark.Buff;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;
using System.ComponentModel;
using AttributeCollection = Quark.Attribute.AttributeCollection;

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
        AttributeCollection _attributes;
        List<Cast> _casting;
        BuffContainer _regularBuffs;
        BuffContainer _hiddenBuffs;
        //TODO: items

        void Awake()
        {
            _attributes = QuarkMain.GetInstance().Config.DefaultAttributes.DeepCopy();
            _attributes.SetCarrier(this);
            _regularBuffs = new BuffContainer(this);
            _hiddenBuffs = new BuffContainer(this);
            _casting = new List<Cast>();
            Configure();
        }

        protected virtual void Configure()
        {
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

        public IList<Cast> Casts
        {
            get
            {
                return _casting.AsReadOnly();
            }
        }

        public virtual bool CanCast(Spell.Spell spell)
        {
            return _casting.Count == 0;
        }

        public void AddCast(Cast cast)
        {
            if (CanCast(cast.Spell))
                _casting.Add(cast);
        }

        public void ClearCast(Cast cast)
        {
            _casting.Remove(cast);
        }

        public void AttachBuff(Buff.Buff buff)
        {
            if (buff.Hidden)
                _hiddenBuffs.AttachBuff(buff);
            else
                _regularBuffs.AttachBuff(buff);
        }

        /// <summary>
        /// Returns a readonly collection of the Buffs being carried by this Character
        /// </summary>
        /// <value>The buffs.</value>
        public IList<Buff.Buff> Buffs
        {
            get
            {
                return _regularBuffs.Buffs;
            }
        }

        /// <summary>
        /// If a buff with the given type exists on this Character, it will return the correct instance on the Character, otherwise it will return null.
        /// </summary>
        /// <returns>The buff instance being carried by this Character.</returns>
        /// <param name="buff">Example of the Buff to find. Only types should match.</param>
        public Buff.Buff GetBuff(Buff.Buff buff)
        {
            return _regularBuffs.GetBuff(buff);
        }

        public Buff.Buff GetHidden(Buff.Buff hidden)
        {
            return _hiddenBuffs.GetBuff(hidden);
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