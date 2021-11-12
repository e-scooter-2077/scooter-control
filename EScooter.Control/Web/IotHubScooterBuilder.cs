using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.Control.Application;
using EScooter.Control.Logic;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
using System;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.Control.Web
{
    public class IotHubScooterBuilder : IScooterBuilder
    {
        private Option<Guid> _id = None;
        private Option<bool> _locked = None;
        private Option<double> _powerSavingMaxSpeed = None;
        private Option<double> _powerSavingThreshold = None;
        private Option<double> _desiredMaxSpeed = None;
        private Option<bool> _isInStandby = None;
        private Option<double> _batteryLevel = None;
        private Option<string> _updateFrequency = None;

        public Scooter Build() =>
            CanBuild() ?
                BuildScooter(
                    _id.Value,
                    _locked.Value,
                    _powerSavingMaxSpeed.Value,
                    _powerSavingThreshold.Value,
                    _desiredMaxSpeed.Value,
                    _isInStandby.Value,
                    _batteryLevel.Value,
                    _updateFrequency.Value) :
            throw new InvalidOperationException();

        public Scooter BuildWithDefaults() =>
            _id.IsPresent ?
                BuildScooter(
                    _id.Value,
                    _locked.OrElse(true),
                    _powerSavingMaxSpeed.OrElse(4),
                    _powerSavingThreshold.OrElse(20),
                    _desiredMaxSpeed.OrElse(8.3),
                    _isInStandby.OrElse(false),
                    _batteryLevel.OrElse(100),
                    _updateFrequency.OrElse("00:00:30")) :
            throw new InvalidOperationException();

        private Scooter BuildScooter(
            Guid id,
            bool locked,
            double powerSavingMaxSpeed,
            double powerSavingThreshold,
            double desiredMaxSpeed,
            bool isInStandby,
            double batteryLevel,
            string updateFrequency) =>
                new Scooter(
                    id,
                    locked,
                    new ScooterStatus(
                        PowerSavingMaxSpeed: Speed.FromMetersPerSecond(powerSavingMaxSpeed),
                        PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(powerSavingThreshold)),
                        DesiredMaxSpeed: Speed.FromMetersPerSecond(desiredMaxSpeed),
                        IsInStandby: isInStandby,
                        BatteryLevel: BatteryLevel.FromFraction(Fraction.FromPercentage(batteryLevel)),
                        UpdateFrequency: Duration.Parse(updateFrequency)));

        public bool CanBuild() =>
            _id.IsPresent &&
            _locked.IsPresent &&
            _powerSavingMaxSpeed.IsPresent &&
            _powerSavingThreshold.IsPresent &&
            _desiredMaxSpeed.IsPresent &&
            _isInStandby.IsPresent &&
            _batteryLevel.IsPresent &&
            _updateFrequency.IsPresent;

        public void SetDeviceId(Guid id) => _id = id;

        public void SetLocked(bool? locked) => _locked = locked.AsOption();

        public void SetPowerSavingMaxSpeed(double? powerSavingMaxSpeed) => _powerSavingMaxSpeed = powerSavingMaxSpeed.AsOption();

        public void SetPowerSavingThreshold(double? powerSavingThreshold) => _powerSavingThreshold = powerSavingThreshold.AsOption();

        public void SetDesiredMaxSpeed(double? desiredMaxSpeed) => _desiredMaxSpeed = desiredMaxSpeed.AsOption();

        public void SetIsInStandby(bool? isInStandby) => _isInStandby = isInStandby.AsOption();

        public void SetBatteryLevel(double? batteryLevel) => _batteryLevel = batteryLevel.AsOption();

        public void SetUpdateFrequency(string updateFrequency) => _updateFrequency = updateFrequency.AsOption();

        public static IotHubScooterBuilder FromTwin(Twin scooterTwin)
        {
            var scooterTag = JsonConvert.DeserializeObject<TagDto>(scooterTwin.Tags.ToJson());
            var builder = new IotHubScooterBuilder();
            builder.SetDeviceId(new Guid(scooterTwin.DeviceId));
            builder.SetLocked(scooterTag.Control?.Locked);
            builder.SetPowerSavingMaxSpeed(scooterTag.Control?.PowerSavingMaxSpeed);
            builder.SetPowerSavingThreshold(scooterTag.Control?.PowerSavingThreshold);
            builder.SetDesiredMaxSpeed(scooterTag.Control?.DesiredMaxSpeed);
            builder.SetIsInStandby(scooterTag.Control?.IsInStandby);
            builder.SetBatteryLevel(scooterTag.Control?.BatteryLevel);
            builder.SetUpdateFrequency(scooterTag.Control?.UpdateFrequency);
            return builder;
        }
    }
}
