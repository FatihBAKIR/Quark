namespace Quark.Attribute
{
    public class Stat : Attribute
    {
        public Stat(string Tag, string Name, AttributeBag Bag)
            : base(Tag, Name, Bag)
        {
        }

        public double Maximum
        {
            get
            {
                return base.Value;
            }
        }

        double LostValue = 0;

        public override double Value
        {
            get
            {
                return base.Value - this.LostValue;
            }
        }

        public void Decrease(double Difference)
        {
            //OnChange(this, Difference);
            this.LostValue += Difference;
            this.LostValue = System.Math.Min(this.Maximum, this.LostValue);
        }

        public void Increase(double Difference)
        {
            //OnChange(this, Difference);
            this.LostValue -= Difference;
            this.LostValue = System.Math.Max(0, this.LostValue);
        }

        public override string ToString()
        {
            return string.Format("[Stat {0}: {1}/{2}]", Name, Value, Maximum);
        }
    }
}