namespace Quark.Attribute
{
    public class Stat : Attribute
    {
        public Stat(string Tag, string Name, AttributeBag Bag)
            : base(Tag, Name, Bag)
        {
        }

        public float Maximum
        {
            get
            {
                return base.Value;
            }
        }

        float LostValue = 0;

        public override float Value
        {
            get
            {
                return base.Value - this.LostValue;
            }
        }

        public void Manipulate(float Amount)
        {
            this.LostValue += Amount;
            this.LostValue = System.Math.Max(0, this.LostValue);
            this.LostValue = System.Math.Min(this.Maximum, this.LostValue);
        }

        public override string ToString()
        {
            return string.Format("[Stat {0}: {1}/{2}]", Name, Value, Maximum);
        }
    }
}