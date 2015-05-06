using System.Collections.Generic;
using Quark;
using Quark.Attributes;
using Quark.Buffs;
using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark
{
    public class Character : Targetable
    {
        AttributeCollection _attributes;
        List<Cast> _casting;
        BuffContainer _regularBuffs;
        BuffContainer _hiddenBuffs;
        ItemCollection _inventory;

        ConditionCollection _interruptConditions;

        void Awake()
        {
            IsTargetable = true;
            Tags = new DynamicTags();
            _inventory = new ItemCollection(this);
            _attributes = QuarkMain.GetInstance().Config.DefaultAttributes.DeepCopy();
            _attributes.SetCarrier(this);
            _regularBuffs = new BuffContainer(this);
            _hiddenBuffs = new BuffContainer(this);
            _casting = new List<Cast>();
            _interruptConditions = QuarkMain.GetInstance().Config.DefaultInterruption.DeepCopy();

            _attributes.StatManipulated += delegate(Character source, Stat stat, float change)
            {
                OnStatManipulated(stat, change);
            };
            _regularBuffs.BuffDetached += delegate(Character source, Buff buff) { OnBuffDetached(buff); };
            _hiddenBuffs.BuffDetached += delegate(Character source, Buff buff) { OnBuffDetached(buff); };

            Configure();
        }

        protected virtual EffectCollection ConfigurationEffects
        {
            get
            {
                return new EffectCollection();
            }
        }

        protected virtual EffectCollection DestructionEffects
        {
            get
            {
                return new EffectCollection();
            }
        }

        protected virtual void Configure()
        {
            ConfigurationEffects.Run(this);
        }

        void OnDestroy()
        {
            Destruction();
            OnCharacterDestruction();

            _inventory.Dispose();
            _inventory = null;

            _attributes.Dispose();
            _attributes = null;

            _regularBuffs.Dispose();
            _regularBuffs = null;

            _hiddenBuffs.Dispose();
            _hiddenBuffs = null;

            _casting.Clear();
            _casting = null;

            _interruptConditions.Dispose();
            _interruptConditions = null;
        }

        protected virtual void Destruction()
        {
            DestructionEffects.Run(this);
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

        /// <summary>
        /// Returns a read-only collection of the casts this Character is casting
        /// </summary>
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

        public void AddItem(Item item)
        {
            _inventory.AddItemRecursive(item);
        }

        public void RemoveItem(Item item)
        {
            _inventory.Remove(item);
        }

        public bool HasItem(Item item)
        {
            return _inventory.Has(item);
        }

        public bool EquippedItem(Item item)
        {
            return _inventory.Equipped(item);
        }

        public IList<Item> EquippedItems
        {
            get { return _inventory.Items(); }
        }

        public void AttachBuff(Buff buff)
        {
            if (buff.Hidden)
                _hiddenBuffs.AttachBuff(buff);
            else
                _regularBuffs.AttachBuff(buff);
            OnBuffAttached(buff);
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

        void OnBuffAttached(Buff buff)
        {
            Messenger<Character, Buff>.Broadcast("BuffAttached", this, buff);
            BuffAttached(this, buff);
        }

        void OnBuffDetached(Buff buff)
        {
            Messenger<Character, Buff>.Broadcast("BuffDetached", this, buff);
            BuffDetached(this, buff);
        }

        void OnStatManipulated(Stat stat, float change)
        {
            Messenger<Character, Stat, float>.Broadcast("StatManipulated", this, stat, change);
            StatManipulated(this, stat, change);
        }

        void OnCharacterDestruction()
        {
            Messenger<Character>.Broadcast("CharacterDestroyed", this); 
            CharacterDestroyed(this);
        }

        void OnCharacterInstantiation()
        {
            
        }

        /// <summary>
        /// This event is raised after the Character component is destroyed
        /// </summary>
        public event CharacterDel CharacterDestroyed = delegate {};

        /// <summary>
        /// This event is raised when a new Buff is attached to this Character
        /// </summary>
        public event BuffDel BuffAttached = delegate { };

        /// <summary>
        /// This event is raised when a Buff is detached from this Character 
        /// </summary>
        public event BuffDel BuffDetached = delegate { };

        /// <summary>
        /// This event is raised when a Stat of this Character is manipulated
        /// </summary>
        public event StatDel StatManipulated = delegate { };
    }

    public class QuarkCollision
    {
        /// <summary>
        /// The Targetable this collision was catched from
        /// </summary>
        public Targetable Source { get; private set; }

        /// <summary>
        /// Other Targetable
        /// </summary>
        public Targetable Other { get; private set; }

        /// <summary>
        /// Source Targetable's position
        /// </summary>
        public Vector3 SourcePosition { get { return Source.transform.position; } }

        /// <summary>
        /// Other Targetable's position
        /// </summary>
        public Vector3 OtherPosition { get { return Other.transform.position; } }

        public QuarkCollision(Targetable source, Targetable other)
        {
            Source = source;
            Other = other;
        }
    }
}
