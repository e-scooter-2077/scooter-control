using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;

namespace ScooterControlService.LogicControl.Domain.Values
{
    public record Fraction : QuantityWrapper<double>
    {
        public int Base100ValueRounded => (int)Math.Round(Base100Value);

        public double Base100Value => Base1Value * 100;

        public double Base1Value => Value;

        public static Fraction FromPercentage(double percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException("Percentage must be between 0 and 100, inclusive.");
            }
            return new(percentage / 100);
        }

        protected override void Validate(double fraction)
        {
            if (fraction is < 0 or > 1)
            {
                throw new ArgumentException("Fraction must be between 0 and 1, inclusive.");
            }
        }

        public Fraction(double fraction) : base(fraction)
        {
        }
    }
}
