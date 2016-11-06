using System;
using System.Collections.Generic;
using Quark.Attributes;
using Quark.Buffs;
using Quark.Contexts;
using Quark.Effects;
using Quark.Spells;
using Quark.Utilities;
using UnityEngine;
using Attribute = Quark.Attributes.Attribute;

// ReSharper disable ParameterHidesMember

namespace Quark
{

    public class Character : Targetable
    {
        AttributeCollection _attributes;
        List<ICastContext> _casting;
        BuffContainer _regularBuffs;
        BuffContainer _hiddenBuffs;
        ItemCollection _inventory;

        ConditionCollection<ICastContext> _interruptConditions;


        /// <summary>
        /// This property stores whether this Character is suspended or not.
        /// </summary>
        public bool IsSuspended { get; private set; }

        /// <summary>
        /// This method suspends this Character, practically disabling it.
        /// </summary>
        public virtual void Suspend()
        {
            IsSuspended = true;
        }

        /// <summary>
        /// This method continues this Character if it was suspended.
        /// </summary>
        public virtual void Continue()
        {
            IsSuspended = false;
        }

        void Awake()
        {
            IsTargetable = true;
            Tags = new DynamicTags();
            _inventory = new ItemCollection(this);
            _attributes = QuarkMain.GetInstance().Configuration.DefaultAttributes.DeepCopy();
            _attributes.SetCarrier(this);
            _regularBuffs = new BuffContainer(this);
            _hiddenBuffs = new BuffContainer(this);
            _casting = new List<ICastContext>();
            _interruptConditions = QuarkMain.GetInstance().Configuration.DefaultInterruption.DeepCopy();
            Context = new Context(this);

            _attributes.StatManipulated += delegate(Character source, Stat stat, float change)
            {
                Logger.Debug("Character::HandleStatManipulation");
                OnStatManipulated(stat, change);
            };
            _regularBuffs.BuffDetached += delegate(Character source, IBuff buff) { OnBuffDetached(buff); };
            _hiddenBuffs.BuffDetached += delegate(Character source, IBuff buff) { OnBuffDetached(buff); };

            Configure();
        }

        /// <summary>
        /// These effects are applied when this Character is instantiated.
        /// </summary>
        protected virtual EffectCollection<IContext> ConfigurationEffects
        {
            get
            {
                return new EffectCollection<IContext>();
            }
        }

        /// <summary>
        /// These effects are applied when the GameObject this Character belongs is destroyed.
        /// </summary>
        protected virtual EffectCollection<IContext> DestructionEffects
        {
            get
            {
                return new EffectCollection<IContext>();
            }
        }

        /// <summary>
        /// Configure this Character.
        /// </summary>
        protected virtual void Configure()
        {
            ConfigurationEffects.Run(this, Context);
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

            List<ICastContext> casts = _casting;

            _casting.Clear();
            _casting = null;

            foreach (ICastContext cast in casts)
            {
                cast.Interrupt();
                cast.Clear();
            }
            
            _interruptConditions.Dispose();
            _interruptConditions = null;
        }

        /// <summary>
        /// Handle the destruction of this character
        /// </summary>
        protected virtual void Destruction()
        {
            DestructionEffects.Run(this, Context);
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
        /// <summary>
        /// This property stores the Context of this Character.
        /// </summary>
        public IContext Context { get; private set; }

        /// <summary>
        /// Gets the <see cref="Attributes.Attribute"/> belonging to this Character with the given tag
        /// </summary>
        /// <param name="tag">Tag of the attribute</param>
        /// <returns>Attribute with the given tag</returns>
        public virtual Attribute GetAttribute(string tag)
        {
            return _attributes.GetAttribute(tag);
        }

        /// <summary>
        /// Gets the <see cref="Stat"/> belonging to this Character with the given tag
        /// </summary>
        /// <param name="tag">Tag of the stat</param>
        /// <returns>Stat with the given tag</returns>
        public virtual Stat GetStat(string tag)
        {
            return _attributes.GetStat(tag);
        }

        /// <summary>
        /// Returns a read-only collection of the casts this Character is casting
        /// </summary>
        public IList<ICastContext> Casts
        {
            get
            {
                return _casting.AsReadOnly();
            }
        }

        /// <summary>
        /// This property determines whether this Character is casting currently.
        /// </summary>
        public bool HasCast
        {
            get { return _casting.Count > 0; }
        }

        /// <summary>
        /// Determines whether the given <see cref="Spell"/> can be casted by this Character.
        /// </summary>
        /// <param name="spell"></param>
        /// <returns>Boolean representing whether this Character can cast the given Spell or not.</returns>
        public virtual bool CanCast(Spell spell)
        {
            return !HasCast && !IsSuspended;
        }

        /// <summary>
        /// Add the given <see cref="Cast"/> context to this character.
        /// </summary>
        /// <param name="cast">A <see cref="Cast"/> context.</param>
        public void AddCast(ICastContext cast)
        {
            if (CanCast(cast.Spell))
            {
                _casting.Add(cast);
            }
        }

        /// <summary>
        /// Removes the given <see cref="Cast"/> context from this character.
        /// </summary>
        /// <param name="cast"></param>
        public void ClearCast(ICastContext cast)
        {
            if (_casting != null)
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

        public void AttachBuff(IBuff buff)
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
        public IList<IBuff> Buffs
        {
            get
            {
                return _regularBuffs.Buffs;
            }
        }

        /// <summary>
        /// If a buff with the given type exists on this Character, 
        /// it will return the correct instance on the Character, otherwise it will return null.
        /// </summary>
        /// <returns>The buff instance being carried by this Character.</returns>
        /// <param name="buff">Example of the Buff to find. Only types should match.</param>
        public IBuff GetBuff(IBuff buff)
        {
            return _regularBuffs.GetBuff(buff);
        }

        /// <summary>
        /// This method returns the first instance of a buff type attached to this character.
        /// </summary>
        /// <typeparam name="T">Type of the buff</typeparam>
        /// <returns>The instance of the buff</returns>
        public T GetBuff<T>() where T : class, IBuff
        {
            return _regularBuffs.GetBuff<T>();
        }

        /// <summary>
        /// This method returns the collection of a buff type instances attached to this character.
        /// </summary>
        /// <typeparam name="T">Type of the collection</typeparam>
        /// <returns>All instances of the buff</returns>
        public T[] GetBuffs<T>() where T : class, IBuff
        {
            return _regularBuffs.GetBuffs<T>();
        }

        /// <summary>
        /// This method returns the first instance of a hidden buff type attached to this character.
        /// </summary>
        /// <typeparam name="T">Type of the buff</typeparam>
        /// <returns>The instance of the buff</returns>
        public T GetHidden<T>() where T : class, IBuff 
        {
            return _hiddenBuffs.GetBuff<T>();
        }

        /// <summary>
        /// This method returns the collection of a hidden buff type instances attached to this character.
        /// </summary>
        /// <typeparam name="T">Type of the buff</typeparam>
        /// <returns>The instance of the buff</returns>
        public T[] GetHiddens<T>() where T : class, IBuff
        {
            return _hiddenBuffs.GetBuffs<T>();
        }

        public IBuff GetHidden(IBuff hidden)
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

        public ConditionCollection<ICastContext> InterruptConditions
        {
            get
            {
                return _interruptConditions;
            }
        }

        void OnBuffAttached(IBuff buff)
        {
            //Messenger<Character, IBuff>.Broadcast("Character.BuffAttached", this, buff);
            BuffAttached(this, buff);
        }

        void OnBuffDetached(IBuff buff)
        {
            //Messenger<Character, IBuff>.Broadcast("Character.BuffDetached", this, buff);
            BuffDetached(this, buff);
        }

        void OnStatManipulated(Stat stat, float change)
        {
            Logger.Debug("Character::OnStatManipulated");

            //Messenger<Character, Stat, float>.Broadcast("Character.StatManipulated", this, stat, change);
            StatManipulated(this, stat, change);
        }

        void OnCharacterDestruction()
        {
            //Messenger<Character>.Broadcast("Character.CharacterDestroyed", this);
            CharacterDestroyed(this);
        }

        /// <summary>
        /// This event is raised after the Character component is destroyed
        /// </summary>
        public event CharacterDel CharacterDestroyed = delegate { };

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

        /// <summary>
        /// This event is raised when this Character is hit by a projectile.
        /// Handlers of this event may invalidate the event.
        /// </summary>
        public event HitValidateDelegate ProjectileHit = delegate(IHitContext hit, out bool result) { result = true; };

        /// <summary>
        /// This method validates a projectile hit on this Character.
        /// </summary>
        /// <returns>Whether the hit was valid or not.</returns>
        public bool ValidateHit(IHitContext hit)
        {
            foreach (Delegate del in ProjectileHit.GetInvocationList())
            {
                bool validation;
                HitValidateDelegate validator = del as HitValidateDelegate;
                validator(hit, out validation);
                if (!validation)
                    return false;
            }
            return true;
        }
    }
}
