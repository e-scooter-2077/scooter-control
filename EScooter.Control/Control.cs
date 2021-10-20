using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterControlService.Domain;
using ScooterControlService.Domain.Values;
using System;
using System.Threading.Tasks;

namespace EScooter.Control
{
    public record EScooterTelemetryReceived(
        Guid Id,
        int BatteryLevel,
        double Speed,
        double Latitude,
        double Longitude,
        bool Locked,
        bool Standby);

    public record ControlTags(
        double PowerSavingThreshold,
        double MaxSpeedOnPowerSaving,
        double DesiredMaxSpeed);

    public record EScooterTags(ControlTags Control);

    public record EScooterDesiredDto(
            string UpdateFrequency,
            double MaxSpeed,
            int StandbyThreshold);

    public record EScooterCommand(
        ControlTags Control);

    public static class Control
    {
        private static readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("HubRegistryConnectionString"));
        private static readonly EventHubProducerClient _producerClient = new(
                                                                            Environment.GetEnvironmentVariable("EventHubConnectionString"),
                                                                            Environment.GetEnvironmentVariable("EventHubName"));

        [Function("UpdateOnNewTelemetry")]
        public static async Task UpdateOnNewTelemetry([ServiceBusTrigger("%TopicName%", "%TelemetrySub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateOnNewTelemetry));
            var telemetryReceived = JsonConvert.DeserializeObject<EScooterTelemetryReceived>(myQueueItem);
            await ApplyPolicyOnTelemetry(telemetryReceived, logger);
            logger.LogInformation($"policy applied on {telemetryReceived.Id}");
        }

        private static async Task ApplyPolicyOnTelemetry(EScooterTelemetryReceived telemetryReceived, ILogger logger)
        {
            var twin = await _registryManager.GetTwinAsync(telemetryReceived.Id.ToString());

            var tags = JsonConvert.DeserializeObject<EScooterTags>(twin.Tags.ToJson()).Control;
            var desiredDto = JsonConvert.DeserializeObject<EScooterDesiredDto>(twin.Properties.Desired.ToJson());

            var scooter = FromDto(tags, desiredDto, telemetryReceived);



            if (false)
            {
                await PhysicalControl.UpdateReportedProperties(telemetryReceived.Id, desiredDto);
                logger.LogInformation($"Scooter {telemetryReceived.Id} modified Reported prop to : {desiredDto}");
            }
        }

        private static Scooter FromDto(ControlTags scooterTag, EScooterDesiredDto dto, EScooterTelemetryReceived telemetryReceived)
        {
            return new Scooter(
                telemetryReceived.Id,
                telemetryReceived.Locked,
                new ScooterStatus(
                    PowerSavingMaxSpeed: Speed.FromMetersPerSecond(scooterTag.MaxSpeedOnPowerSaving),
                    PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(scooterTag.PowerSavingThreshold)),
                    DesiredMaxSpeed: Speed.FromMetersPerSecond(scooterTag.DesiredMaxSpeed),
                    IsInStandby: telemetryReceived.Standby,
                    BatteryLevel: BatteryLevel.FromFraction(Fraction.FromPercentage(telemetryReceived.BatteryLevel))));
        }
    }
}
