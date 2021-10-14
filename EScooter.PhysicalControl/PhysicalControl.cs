using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EScooter.PhysicalControl
{
    public record EScooter(Guid Id);

    public record EScooterCommand(bool Locked);

    public record EScooterDesiredReceived(
        Guid Id,
        string UpdateFrequency,
        double MaxSpeed,
        int StandbyThreshold);

    public record EScooterDesiredDto(
         string UpdateFrequency,
         double MaxSpeed,
         int StandbyThreshold);

    public static class PhysicalControl
    {
        private static RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("HubRegistryConnectionString"));

        [FunctionName("UpdateReportedProperties")]
        public static async Task UpdateReportedProperties([HttpTrigger("%TopicName%", "%ModifySub%", "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation("Received something");
            var message = JsonConvert.DeserializeObject<EScooterDesiredReceived>(myQueueItem);
            var desiredDto = new EScooterDesiredDto(message.UpdateFrequency, message.MaxSpeed, message.StandbyThreshold);
            var twin = await _registryManager.GetTwinAsync(message.Id.ToString());
            var patch = CreateDesiredPatch(desiredDto);
            await UpdateTwin(twin, patch);
            log.LogInformation($"Properties updated {myQueueItem}");
        }

        [FunctionName("LockScooter")]
        public static async Task LockScooter([ServiceBusTrigger("%TopicName%", "%LockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<EScooter>(myQueueItem);
            await ModifyLockedProp(message.Id.ToString(), true);
            log.LogInformation($"Scooter {message.Id} locked");
        }

        [FunctionName("UnlockScooter")]
        public static async Task UnlockScooter([ServiceBusTrigger("%TopicName%", "%UnlockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<EScooter>(myQueueItem);
            await ModifyLockedProp(message.Id.ToString(), false);
            log.LogInformation($"Scooter {message.Id} unlocked");
        }

        private static async Task ModifyLockedProp(string id, bool locked)
        {
            var twin = await _registryManager.GetTwinAsync(id);
            var patch = CreateDesiredPatch(new EScooterCommand(locked));
            await UpdateTwin(twin, patch);
        }

        private static string CreateDesiredPatch(object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            return
                @"{
                    properties: {
                        desired: " + jsonObj + @"
                    }
                }";
        }

        private static async Task UpdateTwin(Twin twin, string patch)
        {
            await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }
    }
}
