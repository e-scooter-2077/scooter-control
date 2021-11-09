using EasyDesk.Tools.Options;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.Control.Logic.Domain
{
    public class ScooterBuilder : IScooterBuilder
    {
        private Option<Guid> _id = None;
        private Option<bool> _locked = None;
        private Option<double> _powerSavingMaxSpeed = None;
        private Option<double> _powerSavingThreshold = None;
        private Option<double> _desiredMaxSpeed = None;
        private Option<bool> _isInStandby = None;
        private Option<double> _batteryLevel = None;

        public Scooter Build() => CanBuild() ?
            BuildScooter(
                        _id.Value,
                        _locked.Value,
                        _powerSavingMaxSpeed.Value,
                        _powerSavingThreshold.Value,
                        _desiredMaxSpeed.Value,
                        _isInStandby.Value,
                        _batteryLevel.Value) :
            throw new InvalidOperationException();

        public Scooter BuildWithDefaults() =>
            _id.IsPresent ?
            BuildScooter(
                        _id.Value,
                        _locked.OrElse(true),
                        _powerSavingMaxSpeed.OrElse(30),
                        _powerSavingThreshold.OrElse(20),
                        _desiredMaxSpeed.OrElse(30),
                        _isInStandby.OrElse(false),
                        _batteryLevel.OrElse(100)) :
            throw new InvalidOperationException();

        private Scooter BuildScooter(
                                    Guid id,
                                    bool locked,
                                    double powerSavingMaxSpeed,
                                    double powerSavingThreshold,
                                    double desiredMaxSpeed,
                                    bool isInStandby,
                                    double batteryLevel) =>
            new Scooter(
                        id,
                        locked,
                        new ScooterStatus(
                                        PowerSavingMaxSpeed: Speed.FromKilometersPerHour(powerSavingMaxSpeed),
                                        PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(powerSavingThreshold)),
                                        DesiredMaxSpeed: Speed.FromKilometersPerHour(desiredMaxSpeed),
                                        IsInStandby: isInStandby,
                                        BatteryLevel: BatteryLevel.FromFraction(Fraction.FromPercentage(batteryLevel))));

        public bool CanBuild() =>
            _id.IsPresent &&
            _locked.IsPresent &&
            _powerSavingMaxSpeed.IsPresent &&
            _powerSavingThreshold.IsPresent &&
            _desiredMaxSpeed.IsPresent &&
            _isInStandby.IsPresent &&
            _batteryLevel.IsPresent;

        public void SetDeviceId(Guid id) => _id = id;

        public void SetLocked(bool? locked) => _locked = locked.AsOption();

        public void SetPowerSavingMaxSpeed(double? powerSavingMaxSpeed) => _powerSavingMaxSpeed = powerSavingMaxSpeed.AsOption();

        public void SetPowerSavingThreshold(double? powerSavingThreshold) => _powerSavingThreshold = powerSavingThreshold.AsOption();

        public void SetDesiredMaxSpeed(double? desiredMaxSpeed) => _desiredMaxSpeed = desiredMaxSpeed.AsOption();

        public void SetIsInStandby(bool? isInStandby) => _isInStandby = isInStandby.AsOption();

        public void SetBatteryLevel(double? batteryLevel) => _batteryLevel = batteryLevel.AsOption();
    }
}
