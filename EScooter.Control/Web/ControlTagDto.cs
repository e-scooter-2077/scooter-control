namespace EScooter.Control.Application
{
    public record ControlTagDto(
        bool? Locked,
        double? PowerSavingMaxSpeed,
        double? PowerSavingThreshold,
        double? DesiredMaxSpeed,
        bool? IsInStandby,
        double? BatteryLevel,
        string UpdateFrequency);

    public record TagDto(ControlTagDto Control);
}
