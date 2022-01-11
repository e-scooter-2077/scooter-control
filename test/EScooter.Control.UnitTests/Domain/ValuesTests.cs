using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EScooter.Control.UnitTests.Domain
{
    public class ValuesTests
    {
        private double _percentageValue = 57.7;
        private double _fractionValue = 0.3624876;
        private double _tolerance = 0.000001;
        private double _doubleValue = 17.567;

        [Fact]
        public void Fraction_FromPercentage()
        {
            Fraction.FromPercentage(_percentageValue).Base100Value.ShouldBe(_percentageValue, _tolerance);
        }

        [Fact]
        public void Fraction_Constructor()
        {
            new Fraction(_fractionValue).Base1Value.ShouldBe(_fractionValue, _tolerance);
        }

        [Fact]
        public void Fraction_Base1ValueConversion()
        {
            Fraction.FromPercentage(_percentageValue).Base1Value.ShouldBe(_percentageValue / 100, _tolerance);
        }

        [Fact]
        public void Fraction_Base100ValueConversion()
        {
            new Fraction(_fractionValue).Base100Value.ShouldBe(_fractionValue * 100, _tolerance);
        }

        [Fact]
        public void BatteryLevel_FromFraction()
        {
            var f = Fraction.FromPercentage(_percentageValue);
            BatteryLevel.FromFraction(f).AsFraction.ShouldBe(f);
        }

        [Fact]
        public void BatteryLevel_Full()
        {
            BatteryLevel.Full().ShouldBe(BatteryLevel.FromFraction(Fraction.FromPercentage(100)));
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
