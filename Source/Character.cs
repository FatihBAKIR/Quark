using System.Collections.Generic;
using Quark.Attributes;
using Quark.Buffs;
using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark
{
    public class Targetable : MonoBehaviour, Identifiable, ITaggable
    {
        public bool IsTargetable { get; set; }
        public string Identifier
        {
            get
            {
                return GetHashCode().ToString();
            }
        }
        public TagCollection Tags { get; protected set; }

        public void Tag(string tag)
        {
            Tags.Add(tag);
        }

        public void Tag(string tag, object value)
        {
            Tags.Add(tag, value);
        }

        public void Untag(string tag)
        {
            Tags.Delete(tag);
        }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }

        public object GetTag(string tag)
        {
            return Tags.Get(tag);
        }

    }

    public class Character : Targetable
    {
        AttributeCollection _attributes;
        List<Cast> _casting;
        BuffContainer _regularBuffs;
        BuffContainer _hiddenBuffs;

        ConditionCollection _interruptConditions;

        //TODO: items

        void Awake()
        {
            Tags = new TagCollection();
            _attributes = QuarkMain.GetInstance().Config.DefaultAttributes.DeepCopy();
            _attributes.SetCarrier(this);
            _regularBuffs = new BuffContainer(this);
            _hiddenBuffs = new BuffContainer(this);
            _casting = new List<Cast>();
            _interruptConditions = QuarkMain.GetInstance().Config.DefaultInterruption.DeepCopy();
            Configure();
        }

        protected virtual void Configure()
        {
        }

        public virtual void Start()
        {
            IsTargetable = true;
        }

#if DEBUG
        public Character()
        {
            Logger.GC("Character::ctor");
        }
#endif

#if DEBUG
        ~Character()
        {
            Logger.GC("Character::dtor");
        }
#endif

        public virtual Attribute GetAttribute(string tag)
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

        public virtual bool CanCast(Spell spell)
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

        public void AttachBuff(Buff buff)
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
        public IList<Buff> Buffs
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
        public Buff GetBuff(Buff buff)
        {
            return _regularBuffs.GetBuff(buff);
        }

        public Buff GetHidden(Buff hidden)
        {
            return _hiddenBuffs.GetBuff(hidden);
        }

        public void ApplyBases(Dictionary<string, float> bases)
        {
            _attributes.ApplyBases(bases);
        }

        public Attribute[] GetAttributes
        {
            get
            {
                return _attributes.GetAttributes();
            }
        }

        public ConditionCollection InterruptConditions
        {
            get
            {
                return _interruptConditions;
            }
        }

        public static Character operator +(Character character, Buff buff)
        {
            character.AttachBuff(buff);
            return character;
        }
    }
}