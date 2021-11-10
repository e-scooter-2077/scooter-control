using EScooter.Control.Logic;
using ScooterControlService.LogicControl.Domain;
using System;
using System.Threading.Tasks;

namespace EScooter.Control.Web
{
    public interface IIotHubRegistryManager
    {
        public Task<IScooterBuilder> FetchScooterBuilder(Guid id);

        public Task SubmitScooterStatus(Scooter scooter);
    }
}
