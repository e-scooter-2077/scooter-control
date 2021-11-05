using ScooterControlService.LogicControl.Domain.Values;
using System;

namespace ScooterControlService.LogicControl.Domain
{
    public record Scooter
    {
        public Scooter(Guid id, bool locked, ScooterStatus status)
        {
            Id = id;
            Locked = locked;
            Status = status;
        }

        public Speed MaxSpeed => Speed.Min(Status.DesiredMaxSpeed, MaxSpeedByPowerMode);

        public Guid Id { get; }

        public bool Locked { get; private set; }

        public ScooterStatus Status { get; }

        private Speed MaxSpeedByPowerMode
        {
            get
            {
                if (Status.BatteryLevel.AsFraction <= Status.PowerSavingThreshold)
                {
                    return Status.PowerSavingMaxSpeed;
                }

                return Status.DesiredMaxSpeed;
            }
        }

        public void Lock() => Locked = true;

        public void Unlock() => Locked = false;

        public override string ToString()
        {
            return $"Scooter[{Id}] {{\nLocked = {Locked};\nMaxSpeed = {MaxSpeed};\nStatus = {Status};\n}}";
        }
    }

    public static class ScooterFactory
    {
        public static Scooter CreateScooter(Guid id) => new(
            id: id,
            locked: true,
            status: new ScooterStatus(
                PowerSavingThreshold: BatteryLevel.FromFraction(Fraction.FromPercentage(30)),
                PowerSavingMaxSpeed: Speed.FromKilometersPerHour(20),
                BatteryLevel: BatteryLevel.Full(),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(30),
                IsInStandby: false));
    }
}
