using ScooterControlService.LogicControl.Domain;
using System;

namespace EScooter.Control.Application
{
    public record ControlTagDto(
        bool? Locked,
        double? PowerSavingMaxSpeed,
        double? PowerSavingThreshold,
        double? DesiredMaxSpeed,
        bool? IsInStandby,
        double? BatteryLevel);

    public record TagDto(ControlTagDto Control);
}
