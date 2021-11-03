using Newtonsoft.Json;
using System;

namespace EScooter.Control.Application
{
    public record ScooterTelemetryDto(
        SystemProperties SystemProperties,
        Body Body);


    public record SystemProperties([JsonProperty(PropertyName="iothub-connection-device-id")] Guid Id);

    public record Body(
        int BatteryLevel,
        double Speed,
        double Latitude,
        double Longitude);
}
