using Quark.Buffs;
using Quark.Contexts;
using Quark.Spells;
using Quark.Utilities;

namespace Quark
{
    public class Item : Identifiable, ITagged
    {
        public bool IsEquipped
        {
            get { return Carrier.EquippedItem(this); }
        }

        public int MaxStacks = 1;
        public int CurrentStacks = 1;

        public string Name
        {
            get { return GetType().Name; }
        }

        protected bool IsActive
        {
            get { return ActiveSpell != null; }
        }

        protected Character Carrier;

        public void SetCarrier(Character carrier)
        {
            Carrier = carrier;
        }

        protected IContext Context
        {
            get { return Carrier.Context; }
        }

        /*public virtual void OnGrab()
        {
            GrabEffects.Run(Carrier, Context);
        }

        public virtual void OnStack()
        {
            StackEffects.Run(Carrier, Context);
        }

        public virtual void OnEquip()
        {
            EquipEffects.Run(Carrier, Context);
        }

        public virtual void OnActive()
        {
            if (!IsActive)
                return;
            
            Cast.PrepareCast(Carrier, ActiveSpell);
        }

        public virtual void OnUnequip()
        {
            UnequipEffects.Run(Carrier, Context);
        }

        public virtual void OnDrop()
        {
            DropEffects.Run(Carrier, Context);
        }

        protected virtual Condition GrabCondition { get { return new Condition(); } }
        protected virtual Condition EquipCondition { get { return new Condition(); } }

        public bool CanGrab()
        {
            GrabCondition.SetContext(Context);
            return GrabCondition.Check();
        }

        public bool CanEquip()
        {
            EquipCondition.SetContext(Context);
            return EquipCondition.Check();
        }

        protected virtual EffectCollection GrabEffects { get { return new EffectCollection(); } }
        protected virtual EffectCollection StackEffects { get { return new EffectCollection(); } }
        protected virtual EffectCollection EquipEffects { get { return new EffectCollection(); } }
        protected virtual EffectCollection UnequipEffects { get { return new EffectCollection(); } }
        protected virtual EffectCollection DropEffects { get { return new EffectCollection(); } }*/
        protected virtual Spell ActiveSpell { get { return null; } }

        public virtual string Identifier
        {
            get { return MakeID(this, Carrier); }
        }

        public static string MakeID(Item item, Character carrier)
        {
            return item.Name + "@" + (item.Carrier != null ? item.Carrier.Identifier : "Anon");
        }

        #region Tagging
        public StaticTags Tags { get; protected set; }

        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }
}
