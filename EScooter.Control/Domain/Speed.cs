using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;
using System.Linq;

namespace ScooterControlService.Domain
{
    public record Speed : QuantityWrapper<double>
    {
        public double MetersPerSecond => Value;

        public double KilometersPerHour => Value * 3.6;

        private Speed(double metersPerSecond) : base(metersPerSecond)
        {
        }

        protected override void Validate(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Speed value can't be negative.", nameof(value));
            }
        }

        public static Speed FromMetersPerSecond(double mps) => new(mps);

        public static Speed FromKilometersPerHour(double kph) => new(kph / 3.6);

        public static Speed Min(params Speed[] speeds)
        {
            return speeds.Min();
        }

        public static Speed Max(params Speed[] speeds)
        {
            return speeds.Max();
        }
    }
}
