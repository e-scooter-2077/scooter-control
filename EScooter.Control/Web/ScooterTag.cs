using ScooterControlService.LogicControl.Domain;
using System;

namespace EScooter.Control.Application
{
    public record ScooterTag(
        ScooterStatus Status,
        bool Locked = true) : IScooterTag;

    public record TagDto(InnerTagDto Tags);

    public record InnerTagDto(ScooterTag Control);
}
