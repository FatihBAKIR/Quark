using System.Collections.Generic;
using UnityEngine;

namespace Quark
{
    public class Character : MonoBehaviour
    {
        AttributeBag Attributes;
        List<CastData> casting;
        BuffContainer Buffs;
        //TODO: items

        public void Start()
        {
            this.Attributes = new AttributeBag(this);
            this.Buffs = new BuffContainer(this);
            this.casting = new List<CastData>();
        }

        public Character()
        {
            Quark.Logger.Debug("Character::ctor");
        }

        public Character(Character obj)
        {
            Quark.Logger.Debug("Character::cctor");
        }

        public Attribute GetAttribute(string Tag)
        {
            return Attributes.GetAttribute(Tag);
        }

        public Stat GetStat(string Tag)
        {
            return Attributes.GetStat(Tag);
        }

        public CastData[] GetCasts
        {
            get
            {
                return casting.ToArray();
            }
        }

        public bool CanCast
        {
            get
            {
                return casting.Count == 0;
            }
        }

        public void AddCast(CastData cd)
        {
            if (this.CanCast)
                this.casting.Add(cd);
        }

        public void ClearCast(CastData cd)
        {
            this.casting.Remove(cd);
        }

        public void AttachBuff(Buff buff)
        {
            this.Buffs.AttachBuff(buff);
        }

        public void ApplyBases(Dictionary<string, double> Bases)
        {
            this.Attributes.ApplyBases(Bases);
        }

        public Attribute[] GetAttributes
        {
            get{
                return this.Attributes.GetAttributes();
            }
        }

        void Update()
        {
            foreach (CastData cast in casting)
            {
                //Debug.Log(cast.CastPercentage);
            }
            ///Run buffs, spell casts etc.
        }

        protected virtual void OnGUI()
        {
        }
    }
}