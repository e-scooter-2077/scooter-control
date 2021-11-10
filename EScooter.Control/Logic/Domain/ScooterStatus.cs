using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace ScooterControlService.LogicControl.Domain
{
    public interface IScooterConfiguration
    {
        Speed PowerSavingMaxSpeed { get; }

        BatteryLevel PowerSavingThreshold { get; }

        Speed DesiredMaxSpeed { get; }

        Duration UpdateFrequency { get; }
    }

    public interface IScooterReportedStatus
    {
        bool IsInStandby { get; }
    }

    public record ScooterStatus(
        Speed PowerSavingMaxSpeed,
        BatteryLevel PowerSavingThreshold,
        Speed DesiredMaxSpeed,
        bool IsInStandby,
        BatteryLevel BatteryLevel,
        Duration UpdateFrequency)
        : IScooterConfiguration,
        IScooterReportedStatus;
}
