using ScooterControlService.LogicControl.Domain;

namespace EScooter.Control.Application
{
    public interface IScooterTag
    {
        bool Locked { get; }

        ScooterStatus Status { get; }
    }
}
