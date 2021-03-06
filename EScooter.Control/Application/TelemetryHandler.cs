using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using EventGridTriggerAttribute = Microsoft.Azure.Functions.Worker.EventGridTriggerAttribute;

namespace EScooter.Control.Application
{
    /// <summary>
    /// A class containing a function to handle device telemetry updates.
    /// </summary>
    public class TelemetryHandler
    {
        private readonly IIotHubRegistryManager _iotHub;

        /// <summary>
        /// Constructor for TelemetryHandler.
        /// </summary>
        /// <param name="iotHub">The manager of IotHub.</param>
        public TelemetryHandler(IIotHubRegistryManager iotHub)
        {
            _iotHub = iotHub;
        }

        /// <summary>
        /// A function that when receiving an input event, submits to IotHub the new scooter.
        /// </summary>
        /// <param name="e">The event containing the telemetry data.</param>
        /// <param name="context">The context of the function.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        [Function("UpdateOnNewTelemetry")]
        public async Task UpdateOnNewTelemetry([EventGridTrigger] EventGridEvent e, FunctionContext context)
        {
            var telemetryDto = JsonConvert.DeserializeObject<ScooterTelemetryDto>(e.Data.ToString());
            var builder = await _iotHub.FetchScooterBuilder(telemetryDto.SystemProperties.Id);
            var needsDefaults = !builder.CanBuild();
            var oldScooter = needsDefaults ? builder.BuildWithDefaults() : builder.Build();
            var newStatus = oldScooter.Status with
            {
                BatteryLevel = BatteryLevel.FromPercentage(telemetryDto.Body.BatteryLevel)
            };
            if (needsDefaults || !newStatus.Equals(oldScooter.Status))
            {
                var scooter = new Scooter(oldScooter.Id, oldScooter.Locked, newStatus);
                await _iotHub.SubmitScooterStatus(scooter);
            }
        }
    }
}
