using ScooterControlService.LogicControl.Domain;
using System;

namespace EScooter.Control.Application
{
    public record ScooterTag(bool Locked, ScooterStatus Status) : IScooterTag;

    public record TagDto(InnerTagDto Tags); // TODO lettera minuscola?

    public record InnerTagDto(ScooterTag Control);
}
