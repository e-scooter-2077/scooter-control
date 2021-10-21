using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
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

        public Task SubmitScooterStatus(Scooter scooter) => throw new NotImplementedException();

        private Scooter FromTwin(Twin scooterTwin)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
