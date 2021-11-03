using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
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
            context.GetLogger("UpdateOnNewTelemetry").Log(logLevel: LogLevel.Warning, e.Data.ToString());

            var telemetryDto = JsonConvert.DeserializeObject<ScooterTelemetryDto>(e.Data.ToString());
            context.GetLogger("UpdateOnNewTelemetry").Log(logLevel: LogLevel.Warning, telemetryDto.ToString());
            var oldScooter = await _iotHub.FetchScooter(telemetryDto.SystemProperties.Id);

            var newStatus = oldScooter.Status with
            {
                BatteryLevel = BatteryLevel.FromFraction(
                    Fraction.FromPercentage(telemetryDto.Body.BatteryLevel))
            };

            context.GetLogger("UpdateOnNewTelemetry").Log(logLevel: LogLevel.Warning, "created new status with " + newStatus.BatteryLevel.AsFraction);
            if (!newStatus.Equals(oldScooter.Status))
            {
                context.GetLogger("UpdateOnNewTelemetry").Log(logLevel: LogLevel.Warning, "update");
                var scooter = new Scooter(oldScooter.Id, oldScooter.Locked, newStatus);
                await _iotHub.SubmitScooterStatus(scooter);
            }
        }
    }
}
