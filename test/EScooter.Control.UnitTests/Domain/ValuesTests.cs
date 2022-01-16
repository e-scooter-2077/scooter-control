using ScooterControlService.LogicControl.Domain;
using Shouldly;
using Xunit;

namespace EScooter.Control.UnitTests.Domain
{
    public class ValuesTests
    {
        private readonly double _percentageValue = 57.7;
        private readonly double _fractionValue = 0.3624876;
        private readonly double _tolerance = 0.000001;
        private readonly double _doubleValue = 17.567;

        [Fact]
        public void BatteryLevel_FromPercentage()
        {
            BatteryLevel.FromPercentage(_percentageValue).AsPercentage.ShouldBe(_percentageValue, _tolerance);
        }

        [Fact]
        public void BatteryLevel_Constructor()
        {
            BatteryLevel.FromFraction(_fractionValue).AsFraction.ShouldBe(_fractionValue, _tolerance);
        }

        [Fact]
        public void BatteryLevel_Base1ValueConversion()
        {
            BatteryLevel.FromPercentage(_percentageValue).AsFraction.ShouldBe(_percentageValue / 100, _tolerance);
        }

        [Fact]
        public void BatteryLevel_Base100ValueConversion()
        {
            BatteryLevel.FromFraction(_fractionValue).AsPercentage.ShouldBe(_fractionValue * 100, _tolerance);
        }

        [Fact]
        public void Speed_FromMetersPerSecond()
        {
            Speed.FromMetersPerSecond(_doubleValue).MetersPerSecond.ShouldBe(_doubleValue, _tolerance);
        }

        [Fact]
        public void Speed_FromKilometersPerHour()
        {
            Speed.FromKilometersPerHour(_doubleValue).KilometersPerHour.ShouldBe(_doubleValue, _tolerance);
        }

        [Fact]
        public void Speed_MetersPerSecondConversion()
        {
            Speed.FromKilometersPerHour(_doubleValue).MetersPerSecond.ShouldBe(_doubleValue / 3.6, _tolerance);
        }

        [Fact]
        public void Speed_KilometersPerHourConversion()
        {
            Speed.FromMetersPerSecond(_doubleValue).KilometersPerHour.ShouldBe(_doubleValue * 3.6, _tolerance);
        }

        [Fact]
        public void Speed_Min()
        {
            Speed.Min(Speed.FromMetersPerSecond(_doubleValue), Speed.FromMetersPerSecond(_doubleValue * 2)).ShouldBe(Speed.FromMetersPerSecond(_doubleValue));
        }

        [Fact]
        public void Speed_Max()
        {
            Speed.Max(Speed.FromMetersPerSecond(_doubleValue), Speed.FromMetersPerSecond(_doubleValue * 2)).ShouldBe(Speed.FromMetersPerSecond(_doubleValue * 2));
        }
    }
}
