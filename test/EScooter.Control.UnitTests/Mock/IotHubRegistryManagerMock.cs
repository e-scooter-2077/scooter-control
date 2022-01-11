using EScooter.Control.Logic;
using EScooter.Control.Web;
using ScooterControlService.LogicControl.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Control.UnitTests.Mock
{
    public class IotHubRegistryManagerMock : IIotHubRegistryManager
    {
        private readonly Action<Scooter> _assertions;

        public IScooterBuilder ScooterBuilderMock { get; } = new IotHubScooterBuilder();

        public IotHubRegistryManagerMock(Action<Scooter> assertions)
        {
            _assertions = assertions;
        }

        public Task<IScooterBuilder> FetchScooterBuilder(Guid id)
        {
            ScooterBuilderMock.SetDeviceId(id);
            return Task.FromResult(ScooterBuilderMock);
        }

        public Task SubmitScooterStatus(Scooter scooter)
        {
            _assertions(scooter);
            return Task.CompletedTask;
        }
    }
}
