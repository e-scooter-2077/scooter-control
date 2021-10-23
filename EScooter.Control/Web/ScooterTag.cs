using ScooterControlService.LogicControl.Domain;
using System;

namespace EScooter.Control.Application
{
    public record ScooterTag(bool Locked, ScooterStatus Status) : IScooterTag;
}
