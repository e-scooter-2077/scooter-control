using ScooterControlService.LogicControl.Domain;
using System;

namespace EScooter.Control.Application
{
    public interface IScooterTag
    {
        bool Locked { get; }

        ScooterStatus Status { get; }
    }
}
