using EScooter.Control.Application;
using EScooter.Control.Logic.Domain;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ScooterControlService.LogicControl.Domain;
using ScooterControlService.LogicControl.Domain.Values;
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
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
            _hostName = _iotHubConfiguration.HostName;
        }

        public async Task<IScooterBuilder> FetchScooter(Guid id)
        {
            var twin = await _registryManager.GetTwinAsync(id.ToString());
            return FromTwin(twin);
        }

        public async Task SubmitScooterStatus(Scooter scooter)
        {
            var twin = await _registryManager.GetTwinAsync(scooter.Id.ToString());
            var patch = JsonConvert.SerializeObject(new
            {
                Tags = new TagDto(
                        new ControlTagDto(
                            PowerSavingMaxSpeed: scooter.Status.PowerSavingMaxSpeed.KilometersPerHour,
                            PowerSavingThreshold: scooter.Status.PowerSavingThreshold.AsFraction.Base100Value,
                            DesiredMaxSpeed: scooter.Status.DesiredMaxSpeed.KilometersPerHour,
                            IsInStandby: scooter.Status.IsInStandby,
                            BatteryLevel: scooter.Status.BatteryLevel.AsFraction.Base100Value,
                            Locked: scooter.Locked)),
                Desired = new UploadDesiredDto(scooter.Locked, null, scooter.MaxSpeed.KilometersPerHour) // TODO insert string updateFrequency
            });
            await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }

        private IScooterBuilder FromTwin(Twin scooterTwin)
        {
            var scooterTag = JsonConvert.DeserializeObject<TagDto>(scooterTwin.Tags.ToJson());
            var builder = new ScooterBuilder();
            builder.SetDeviceId(new Guid(scooterTwin.DeviceId));
            builder.SetLocked(scooterTag.Control?.Locked);
            builder.SetPowerSavingMaxSpeed(scooterTag.Control?.PowerSavingMaxSpeed);
            builder.SetPowerSavingThreshold(scooterTag.Control?.PowerSavingThreshold);
            builder.SetDesiredMaxSpeed(scooterTag.Control?.DesiredMaxSpeed);
            builder.SetIsInStandby(scooterTag.Control?.IsInStandby);
            builder.SetBatteryLevel(scooterTag.Control?.BatteryLevel);
            return builder;
        }
    }
}
