using Azure.Core.Amqp;
using Azure.Messaging.EventGrid;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.Control.Application;
using EScooter.Control.UnitTests.Mock;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EScooter.Control.UnitTests.Application
{
    public class TelemetryHandlerTests
    {
        private ScooterStatus _scooterStatus = new ScooterStatus(
            BatteryLevel: BatteryLevel.FromFraction(Fraction.FromPercentage(50)),
            DesiredMaxSpeed: Speed.FromKilometersPerHour(15),
            PowerSavingMaxSpeed: Speed.FromKilometersPerHour(5),
            IsInStandby: false,
            PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(60)),
            UpdateFrequency: Duration.FromTimeSpan(TimeSpan.Parse("00:00:10")));

        private Body _scooterTelemetryBodyFromStatus;

        public TelemetryHandlerTests()
        {
            _scooterTelemetryBodyFromStatus = new Body(
            BatteryLevel: _scooterStatus.BatteryLevel.AsFraction.Base100Value,
            Speed: 10,
            Latitude: 30,
            Longitude: 30);
        }

        [Fact]
        public async Task TelemetryHandler_ShouldWork_WithProperInput()
        {
            var iotHub = new IotHubRegistryManagerMock(s =>
            {
                s.Status.ShouldBe(_scooterStatus);
                s.Id.ShouldBe(Guid.Empty);
                s.MaxSpeed.ShouldBe(s.Status.PowerSavingMaxSpeed);
            });
            iotHub.ScooterBuilderMock.SetDesiredMaxSpeed(_scooterStatus.DesiredMaxSpeed.MetersPerSecond);
            iotHub.ScooterBuilderMock.SetIsInStandby(_scooterStatus.IsInStandby);
            iotHub.ScooterBuilderMock.SetPowerSavingMaxSpeed(_scooterStatus.PowerSavingMaxSpeed.MetersPerSecond);
            iotHub.ScooterBuilderMock.SetPowerSavingThreshold(_scooterStatus.PowerSavingThreshold.AsFraction.Base100Value);
            iotHub.ScooterBuilderMock.SetBatteryLevel(BatteryLevel.Full().AsFraction.Base100Value);
            iotHub.ScooterBuilderMock.SetUpdateFrequency(_scooterStatus.UpdateFrequency.ToString());
            var sut = new TelemetryHandler(iotHub);
            await sut.UpdateOnNewTelemetry(new EventGridEvent("testSubject", "EScooter.TelemetryUpdate", "1.0", new BinaryData(new ScooterTelemetryDto(new SystemProperties(Guid.Empty), _scooterTelemetryBodyFromStatus))), new FunctionContextMock());
        }
    }
}
