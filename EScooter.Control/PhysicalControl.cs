using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
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
        private static readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("HubRegistryConnectionString"));

        [Function("UpdateReportedProperties")]
        public static async Task UpdateReportedProperties([ServiceBusTrigger("%TopicName%", "%ModifySub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateReportedProperties));
            logger.LogInformation("Received something");
            var message = JsonConvert.DeserializeObject<EScooterDesiredReceived>(myQueueItem);
            var desiredDto = new EScooterDesiredDto(message.UpdateFrequency, message.MaxSpeed, message.StandbyThreshold);
            var twin = await _registryManager.GetTwinAsync(message.Id.ToString());
            var patch = CreateDesiredPatch(desiredDto);
            await UpdateTwin(twin, patch);
            logger.LogInformation($"Properties updated {myQueueItem}");
        }

        [Function("LockScooter")]
        public static async Task LockScooter([ServiceBusTrigger("%TopicName%", "%LockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(LockScooter));
            var message = JsonConvert.DeserializeObject<EScooter>(myQueueItem);
            await ModifyLockedProp(message.Id.ToString(), true);
            logger.LogInformation($"Scooter {message.Id} locked");
        }

        [Function("UnlockScooter")]
        public static async Task UnlockScooter([ServiceBusTrigger("%TopicName%", "%UnlockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UnlockScooter));
            var message = JsonConvert.DeserializeObject<EScooter>(myQueueItem);
            await ModifyLockedProp(message.Id.ToString(), false);
            logger.LogInformation($"Scooter {message.Id} unlocked");
        }

        private static async Task ModifyLockedProp(string id, bool locked)
        {
            var twin = await _registryManager.GetTwinAsync(id);
            var patch = CreateDesiredPatch(new EScooterCommand(locked));
            await UpdateTwin(twin, patch);
        }

        private record Properties(object Desired);

        private record Patch(Properties Properties);

        private static string CreateDesiredPatch(object obj)
        {
            return JsonConvert.SerializeObject(new Patch(new Properties(obj)));
        }

        private static async Task UpdateTwin(Twin twin, string patch)
        {
            await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }
    }
}
