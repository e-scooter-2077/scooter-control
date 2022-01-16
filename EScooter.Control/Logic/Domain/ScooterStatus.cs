using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace ScooterControlService.LogicControl.Domain
{
    public record ScooterStatus(
        Speed PowerSavingMaxSpeed,
        BatteryLevel PowerSavingThreshold,
        Speed DesiredMaxSpeed,
        bool IsInStandby,
        BatteryLevel BatteryLevel,
        Duration UpdateFrequency);
}
