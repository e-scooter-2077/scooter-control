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

    public record EScooterDesiredDto(
         string UpdateFrequency,
         double MaxSpeed,
         int StandbyThreshold);

    public static class Control
    {
        private static readonly int _powerSaveMode = 50;
        private static readonly EventHubProducerClient _producerClient = new EventHubProducerClient(
                                                                                Environment.GetEnvironmentVariable("EventHubConnectionString"),
                                                                                Environment.GetEnvironmentVariable("EventHubName"));

        [Function("UpdateOnNewTelemetry")]
        public static async Task UpdateOnNewTelemetry([ServiceBusTrigger("%TopicName%", "%TelemetrySub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateOnNewTelemetry));
            var telemetryReceived = JsonConvert.DeserializeObject<EScooterTelemetryReceived>(myQueueItem);
            await ApplyPolicyOnTelemetry(telemetryReceived);
            logger.LogInformation($"policy applied on {telemetryReceived.Id}");
        }

        private static async Task ApplyPolicyOnTelemetry(EScooterTelemetryReceived telemetryReceived)
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
        }
    }
}
