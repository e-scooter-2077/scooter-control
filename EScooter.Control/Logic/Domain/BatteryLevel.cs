using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using ScooterControlService.LogicControl.Domain.Values;

namespace ScooterControlService.LogicControl.Domain
{
    public record BatteryLevel : ValueWrapper<Fraction>
    {
        private BatteryLevel(Fraction value) : base(value)
        {
        }

        protected override void Validate(Fraction value)
        {
        }

        public Fraction AsFraction => Value;

        public static BatteryLevel FromFraction(Fraction level) => new(level);

        public static BatteryLevel Full() => new(Fraction.FromPercentage(100));
    }
}
