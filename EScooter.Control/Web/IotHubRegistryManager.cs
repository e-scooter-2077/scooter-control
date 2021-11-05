using EScooter.Control.Application;
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

        public async Task<Scooter> FetchScooter(Guid id)
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
                Reported = new UploadReportedDto(scooter.Locked)
            });
            await _registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }

        private Scooter FromTwin(Twin scooterTwin)
        {
            var scooterTag = JsonConvert.DeserializeObject<TagDto>(scooterTwin.Tags.ToJson());
            return new Scooter(
                id: new Guid(scooterTwin.DeviceId),
                locked: scooterTag.Control?.Locked ?? true,
                status: new ScooterStatus(
                    PowerSavingMaxSpeed: Speed.FromKilometersPerHour(scooterTag.Control?.PowerSavingMaxSpeed ?? 30),
                    PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(scooterTag.Control?.PowerSavingThreshold ?? 20)),
                    DesiredMaxSpeed: Speed.FromKilometersPerHour(scooterTag.Control?.DesiredMaxSpeed ?? 30),
                    IsInStandby: scooterTag.Control?.IsInStandby ?? false,
                    BatteryLevel: BatteryLevel.FromFraction(Fraction.FromPercentage(scooterTag.Control?.BatteryLevel ?? 100))));
        }
    }
}
