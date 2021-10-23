using System;

namespace EScooter.Control.Application
{
    public record ScooterTelemetryDto(
        Guid Id,
        int BatteryLevel,
        double Speed,
        double Latitude,
        double Longitude,
        bool Standby,
        IScooterTag Tag);
}
