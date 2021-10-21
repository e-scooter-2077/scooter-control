using ScooterControlService.LogicControl.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Control.Web
{
    public interface IIotHubRegistryManager
    {
        public Task<Scooter> FetchScooter(Guid id);

        public Task SubmitScooterStatus(Scooter scooter);
    }
}
