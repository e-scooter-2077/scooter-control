using EScooter.Control.Web;
using ScooterControlService.LogicControl.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EScooter.Control.UnitTests.Web
{
    public class IotHubScooterBuilderTests
    {
        private IotHubScooterBuilder Sut() => new IotHubScooterBuilder();

        [Fact]
        public void IotHubScooterBuilder_CanBuild_ShouldReturnFalseUntilComplete()
        {
            var sut = Sut();
            sut.CanBuild().ShouldBeFalse();
            sut.SetBatteryLevel(55);
            sut.CanBuild().ShouldBeFalse();
            sut.SetDesiredMaxSpeed(60);
            sut.CanBuild().ShouldBeFalse();
            sut.SetDeviceId(Guid.Empty);
            sut.CanBuild().ShouldBeFalse();
            sut.SetIsInStandby(true);
            sut.CanBuild().ShouldBeFalse();
            sut.SetLocked(true);
            sut.CanBuild().ShouldBeFalse();
            sut.SetPowerSavingMaxSpeed(60);
            sut.CanBuild().ShouldBeFalse();
            sut.SetPowerSavingThreshold(60);
            sut.CanBuild().ShouldBeFalse();
            sut.SetUpdateFrequency("00:00:10");
            sut.CanBuild().ShouldBeTrue();
        }

        [Fact]
        public void IotHubScooterBuilder_ShouldThrowException_OnBuild_WhileNotReady()
        {
            var sut = Sut();
            sut.CanBuild().ShouldBeFalse();
            Should.Throw<InvalidOperationException>(() => sut.Build());
        }

        [Fact]
        public void IotHubScooterBuilder_ShouldThrowException_OnBuildWithDefaults_WithoutId()
        {
            var sut = Sut();
            Should.Throw<InvalidOperationException>(() => sut.BuildWithDefaults());
        }

        [Fact]
        public void IotHubScooterBuilder_ShouldBuildWithDefaults()
        {
            var sut = Sut();
            sut.SetDeviceId(Guid.Empty);
            var res = sut.BuildWithDefaults();
            res.ShouldBeOfType<Scooter>();
            res.ShouldNotBeNull();
        }
    }
}
