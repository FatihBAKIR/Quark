using System;
using Quark.Utilities;

namespace Quark.Attributes
{
    public class Stat : Attribute
    {
        public Stat(string tag, string name, AttributeCollection collection)
            : base(tag, name, collection)
        {
        }

        public float Maximum
        {
            get
            {
                return base.Value;
            }
        }

        float _lostValue;

        public override float Value
        {
            get
            {
                return base.Value - _lostValue;
            }
        }

        public float Rate
        {
            get { return Value / Maximum; }
        }

        public void Manipulate(float amount)
        {
            float temp = _lostValue;

            _lostValue -= amount;
            _lostValue = System.Math.Max(0, _lostValue);
            _lostValue = System.Math.Min(Maximum, _lostValue);

            OnManipulation(_lostValue - temp);
        }

        public void ClearEvents()
        {
            Manipulated = null;
            Manipulated = delegate { };
        }

        protected void OnManipulation(float amount)
        {
            Logger.Debug("Stat::OnManipulation");

            Messenger<Character, Stat, float>.Broadcast("Manipulated", Owner, this, amount);
            Manipulated(Owner, this, amount);
        }
        
        public event StatDel Manipulated = delegate { };
        
        public override string ToString()
        {
            return string.Format("[Stat {0}: {1}/{2}]", Name, Value, Maximum);
        }
    }
}