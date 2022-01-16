using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;

namespace ScooterControlService.LogicControl.Domain
{
    public record BatteryLevel : QuantityWrapper<double>
    {
        private const double EmptyPercentage = 0.0;
        private const double FullPercentage = 100.0;

        private BatteryLevel(double value) : base(value)
        {
        }

        protected override void Validate(double value)
        {
            if (value is < EmptyPercentage or > FullPercentage)
            {
                throw new ArgumentException("Fraction must be between 0 and 1, inclusive.");
            }
        }

        public double AsPercentage => Value;

        public static BatteryLevel Empty { get; } = FromPercentage(EmptyPercentage);

        public static BatteryLevel Full { get; } = FromPercentage(FullPercentage);

        public static BatteryLevel FromPercentage(double percentage) => new(percentage);

        public static BatteryLevel FromFraction(double fraction) => FromPercentage(fraction * FullPercentage);
    }
}
