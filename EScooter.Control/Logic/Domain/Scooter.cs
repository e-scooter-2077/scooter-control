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
                if (Status.BatteryLevel <= Status.PowerSavingThreshold)
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
}
