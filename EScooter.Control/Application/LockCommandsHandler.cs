using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using System.Threading.Tasks;

namespace EScooter.Control.Application
{
    public class LockCommandsHandler
    {
        private readonly IIotHubRegistryManager _iotHub;

        public LockCommandsHandler(IIotHubRegistryManager iotHub)
        {
            _iotHub = iotHub;
        }

        [Function("LockScooter")]
        public async Task LockScooter([ServiceBusTrigger("%TopicName%", "%LockSub%", Connection = "ServiceBusConnectionString")] string json, FunctionContext context)
        {
            var scooter = await FromCommandJson(json);
            scooter.Lock();
            await _iotHub.SubmitScooterStatus(scooter);
        }

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
