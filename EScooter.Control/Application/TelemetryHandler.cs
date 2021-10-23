using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
using System;
using System.Threading.Tasks;

namespace EScooter.Control.Application
{
    public class TelemetryHandler
    {
        private readonly IIotHubRegistryManager _iotHub;

        public TelemetryHandler(IIotHubRegistryManager iotHub)
        {
            _iotHub = iotHub;
        }

        [Function("UpdateOnNewTelemetry")]
        public async Task UpdateOnNewTelemetry([ServiceBusTrigger("%TopicName%", "%TelemetrySub%", Connection = "ServiceBusConnectionString")] string json, FunctionContext context)
        {
            var scooter = FromTelemetryJson(json);
            await _iotHub.SubmitScooterStatus(scooter);
        }

        private Scooter FromTelemetryJson(string telemetryJson)
        {
            var telemetryReceived = JsonConvert.DeserializeObject<ScooterTelemetryDto>(telemetryJson);
            return new Scooter(telemetryReceived.Id, telemetryReceived.Tag.Locked, telemetryReceived.Tag.Status with { BatteryLevel = BatteryLevel.FromFraction(Fraction.FromPercentage(telemetryReceived.BatteryLevel)), IsInStandby = telemetryReceived.Standby });
        }
    }
}
