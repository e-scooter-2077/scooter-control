using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EScooter.Control
{
    public record EScooterTelemetryReceived(
        Guid Id,
        int BatteryLevel,
        double Speed,
        double Latitude,
        double Longitude);

    public record EScooter(Guid Id);

    public record EScooterCommand(bool Locked);

    public record EScooterDesiredDto(
            string UpdateFrequency,
            double MaxSpeed,
            int StandbyThreshold);

    public static class Control
    {
        private static readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("HubRegistryConnectionString"));
        private static readonly int _powerSaveMode = 50;
        private static readonly int _powerSaveMaxSpeed = 15;
        private static readonly EventHubProducerClient _producerClient = new EventHubProducerClient(
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
            var twin = await _registryManager.GetTwinAsync(telemetryReceived.Id.ToString()); // TODO: vorrei fosse un option
            var desiredDto = JsonConvert.DeserializeObject<EScooterDesiredDto>(twin.Properties.Desired.ToJson());
            var modified = false;
            if (telemetryReceived.BatteryLevel <= _powerSaveMode)
            {
                modified = true;
                desiredDto = desiredDto with { MaxSpeed = _powerSaveMaxSpeed };
            }

            if (modified)
            {
                await PhysicalControl.UpdateReportedProperties(telemetryReceived.Id, desiredDto);
                logger.LogInformation($"Scooter {telemetryReceived.Id} modified Reported prop to : {desiredDto}");
            }
        }

        /*private static async Task ApplyPolicyOnTelemetry(EScooterTelemetryReceived telemetryReceived)
        {
            if (telemetryReceived.BatteryLevel <= _powerSaveMode)
            {
                // Create a batch of events

                // using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();
                try
                {
                    await _producerClient.SendAsync(new List<EventData> { new EventData(" ") });

                    // Console.WriteLine($"A batch of {numOfEvents} events has been published.");
                }
                finally
                {
                    await _producerClient.DisposeAsync();
                }
            }
        }*/
    }
}
