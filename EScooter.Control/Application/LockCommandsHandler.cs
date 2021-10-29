using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using System.Threading.Tasks;

namespace EScooter.Control.Application
{
    /// <summary>
    /// A class containing a function to handle lock/unlock command.
    /// </summary>
    public class LockCommandsHandler
    {
        private readonly IIotHubRegistryManager _iotHub;

        /// <summary>
        /// Constructor for LockCommandsHandler.
        /// </summary>
        /// <param name="iotHub">The manager of IotHub.</param>
        public LockCommandsHandler(IIotHubRegistryManager iotHub)
        {
            _iotHub = iotHub;
        }

        /// <summary>
        /// A function that when receiving the lock command, locks the scooter and updates it on IotHub.
        /// </summary>
        /// <param name="json">The json containing the command data.</param>
        /// <param name="context">The context of the function.</param>
        /// <returns><see cref="Task"/>representing the asynchronous operation.</returns>
        [Function("LockScooter")]
        public async Task LockScooter([ServiceBusTrigger("%TopicName%", "%LockSub%", Connection = "ServiceBusConnectionString")] string json, FunctionContext context)
        {
            var scooter = await FromCommandJson(json);
            scooter.Lock();
            await _iotHub.SubmitScooterStatus(scooter);
        }

        /// <summary>
        /// A function that when receiving the unlock command, unlocks the scooter and updates it on IotHub.
        /// </summary>
        /// <param name="json">The json containing the command data.</param>
        /// <param name="context">The context of the function.</param>
        /// <returns><see cref="Task"/>representing the asynchronous operation.</returns>
        [Function("UnlockScooter")]
        public async Task UnlockScooter([ServiceBusTrigger("%TopicName%", "%UnlockSub%", Connection = "ServiceBusConnectionString")] string json, FunctionContext context)
        {
            var scooter = await FromCommandJson(json);
            scooter.Unlock();
            await _iotHub.SubmitScooterStatus(scooter);
        }

        private async Task<Scooter> FromCommandJson(string json)
        {
            var idWrapper = JsonConvert.DeserializeObject<ScooterCommandDto>(json);
            return await _iotHub.FetchScooter(idWrapper.Id);
        }
    }
}
