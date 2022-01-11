using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
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
    public class ScooterTests
    {
        private Scooter Sut() => new Scooter(
            id: Guid.Empty,
            locked: false,
            status: new ScooterStatus(
                PowerSavingMaxSpeed: Speed.FromKilometersPerHour(15),
                PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(30)),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(30),
                IsInStandby: false,
                BatteryLevel: BatteryLevel.Full(),
                UpdateFrequency: Duration.Parse("00:00:10")));

        [Fact]
        public void Scooter_MaxSpeed_Tests()
        {
            var sut = Sut();
            sut.MaxSpeed.ShouldBe(sut.Status.DesiredMaxSpeed);
            sut = new Scooter(sut.Id, sut.Locked, sut.Status with
            {
                BatteryLevel = sut.Status.PowerSavingThreshold
            });
            sut.MaxSpeed.ShouldBe(sut.Status.PowerSavingMaxSpeed);
        }

        [Fact]
        public void Scooter_ShouldLock()
        {
            var sut = Sut();
            sut.Lock();
            sut.Locked.ShouldBeTrue();
        }

        [Fact]
        public void Scooter_ShouldUnlock()
        {
            var sut = Sut();
            sut.Unlock();
            sut.Locked.ShouldBeFalse();
        }
    }
}
