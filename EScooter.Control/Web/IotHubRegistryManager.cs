using EScooter.Control.Application;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using ScooterControlService.LogicControl.Domain;
using System;
using System.Threading.Tasks;

namespace EScooter.Control.Web
{
    public class IotHubRegistryManager : IIotHubRegistryManager
    {
        private readonly IotHubConfiguration _iotHubConfiguration;
        private readonly RegistryManager _registryManager;
        private readonly string _hostName;

        public IotHubRegistryManager(IotHubConfiguration iotHubConfiguration)
        {
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
            _hostName = _iotHubConfiguration.HostName;
        }

        public async Task<Scooter> FetchScooter(Guid id)
        {
            var twin = await _registryManager.GetTwinAsync(id.ToString());
            return FromTwin(twin);
        }

        public Task SubmitScooterStatus(Scooter scooter)
        {
            // var twin = await _registryManager.GetTwinAsync(scooter.Id.ToString());
            // var patch = JsonConvert.SerializeObject(new TagDto(new InnerTagDto(new ScooterTag(scooter.Status, scooter.Locked))));
            // await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
            //// TODO qui vanno aggiornati solo i tags?
            // throw new NotImplementedException();
            Console.WriteLine(scooter);
            return Task.CompletedTask;
        }

        private Scooter FromTwin(Twin scooterTwin)
        {
            var scooterTag = JsonConvert.DeserializeObject<ScooterTag>(scooterTwin.Tags.ToJson());

            return new Scooter(new Guid(scooterTwin.DeviceId), true, new ScooterStatus(Speed.FromKilometersPerHour(30), BatteryLevel.Full(), Speed.FromKilometersPerHour(30), false, BatteryLevel.Full()));

            // TODO: return true values
            return new Scooter(new Guid(scooterTwin.DeviceId), scooterTag.Locked, scooterTag.Status);
        }
    }
}
