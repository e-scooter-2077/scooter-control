using EScooter.Control.Application;
using EScooter.Control.Logic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ScooterControlService.LogicControl.Domain;
using System;
using System.Threading.Tasks;

namespace EScooter.Control.Web
{
    public class IotHubRegistryManager : IIotHubRegistryManager
    {
        private readonly IotHubConfiguration _iotHubConfiguration;
        private readonly RegistryManager _registryManager;

        public IotHubRegistryManager(IotHubConfiguration iotHubConfiguration)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
        }

        public async Task<IScooterBuilder> FetchScooterBuilder(Guid id)
        {
            var twin = await _registryManager.GetTwinAsync(id.ToString());
            return IotHubScooterBuilder.FromTwin(twin);
        }

        public async Task SubmitScooterStatus(Scooter scooter)
        {
            var twin = await _registryManager.GetTwinAsync(scooter.Id.ToString());
            var patch = JsonConvert.SerializeObject(new
            {
                Tags = new TagDto(
                        new ControlTagDto(
                            PowerSavingMaxSpeed: scooter.Status.PowerSavingMaxSpeed.MetersPerSecond,
                            PowerSavingThreshold: scooter.Status.PowerSavingThreshold.AsFraction.Base100Value,
                            DesiredMaxSpeed: scooter.Status.DesiredMaxSpeed.MetersPerSecond,
                            IsInStandby: scooter.Status.IsInStandby,
                            BatteryLevel: scooter.Status.BatteryLevel.AsFraction.Base100Value,
                            Locked: scooter.Locked,
                            UpdateFrequency: scooter.Status.UpdateFrequency.ToString())),
                Desired = new UploadDesiredDto(scooter.Locked, scooter.Status.UpdateFrequency.ToString(), scooter.MaxSpeed.MetersPerSecond)
            });
            await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }
    }
}
