using EScooter.Control;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public static class PhysicalControl
{
    private static readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("HubRegistryConnectionString"));

    /*[Function("UpdateReportedProperties")]
    public static async Task UpdateReportedProperties([ServiceBusTrigger("%TopicName%", "%ModifySub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(UpdateReportedProperties));
        var message = JsonConvert.DeserializeObject<EScooterDesiredReceived>(myQueueItem);
        var desiredDto = new EScooterDesiredDto(message.UpdateFrequency, message.MaxSpeed, message.StandbyThreshold);
        var twin = await _registryManager.GetTwinAsync(message.Id.ToString());
        await UpdateTwin(twin, CreateDesiredPatch(desiredDto));
        logger.LogInformation($"Properties updated {myQueueItem}");
    }

    [Function("LockScooter")]
    public static async Task LockScooter([ServiceBusTrigger("%TopicName%", "%LockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
    {
        await ModifyLockedProp(myQueueItem, true, context.GetLogger(nameof(LockScooter)));
    }

    [Function("UnlockScooter")]
    public static async Task UnlockScooter([ServiceBusTrigger("%TopicName%", "%UnlockSub%", Connection = "ServiceBusConnectionString")] string myQueueItem, FunctionContext context)
    {
        await ModifyLockedProp(myQueueItem, false, context.GetLogger(nameof(UnlockScooter)));
    }

    private static async Task ModifyLockedProp(string scooterJson, bool locked, ILogger logger)
    {
        var message = JsonConvert.DeserializeObject<EScooter>(scooterJson);
        var twin = await _registryManager.GetTwinAsync(message.Id.ToString());
        var patch = CreateDesiredPatch(new EScooterCommand(locked));
        await UpdateTwin(twin, patch);
        logger.LogInformation($"Scooter {message.Id} {(locked ? "locked" : "unlocked")}");
    }*/

    public static async Task UpdateReportedProperties(Guid id, EScooterDesiredDto desiredDto)
    {
        var twin = await _registryManager.GetTwinAsync(id.ToString());
        await UpdateTwin(twin, CreateDesiredPatch(desiredDto));
    }

    public static async Task ModifyLockedProp(Guid id, bool locked)
    {
        var twin = await _registryManager.GetTwinAsync(id.ToString());
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
