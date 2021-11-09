﻿using EScooter.Control.Logic.Domain;
using ScooterControlService.LogicControl.Domain;
using System;
using System.Threading.Tasks;

namespace EScooter.Control.Web
{
    public interface IIotHubRegistryManager
    {
        public Task<IScooterBuilder> FetchScooter(Guid id);

        public Task SubmitScooterStatus(Scooter scooter);
    }
}
