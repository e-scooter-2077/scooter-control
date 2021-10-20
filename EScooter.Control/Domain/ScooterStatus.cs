namespace ScooterControlService.Domain
{
    public interface IScooterConfiguration
    {
        Speed PowerSavingMaxSpeed { get; }

        BatteryLevel PowerSavingThreshold { get; }

        Speed DesiredMaxSpeed { get; }
    }

    public interface IScooterReportedStatus
    {
        Speed DesiredMaxSpeed { get; }

        bool IsInStandby { get; }
    }

    public record ScooterStatus(
        Speed PowerSavingMaxSpeed,
        BatteryLevel PowerSavingThreshold,
        Speed DesiredMaxSpeed,
        bool IsInStandby,
        BatteryLevel BatteryLevel)
        : IScooterConfiguration,
        IScooterReportedStatus;
}
