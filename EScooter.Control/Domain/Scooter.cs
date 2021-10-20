using EasyDesk.CleanArchitecture.Domain.Metamodel;
using ScooterControlService.Domain.Values;
using System;

namespace ScooterControlService.Domain
{
    public class Scooter : Entity
    {
        public Guid Id { get; }

        public bool Locked { get; private set; }

        public ScooterStatus Status { get; private set; }

        public Scooter(Guid id, bool locked, ScooterStatus status)
        {
            Id = id;
            Locked = locked;
            Status = status;
        }

        public Speed MaxSpeed => Speed.Min(Status.DesiredMaxSpeed, MaxSpeedByPowerMode);

        public void Lock() => Locked = true;

        public void Unlock() => Locked = false;

        public void SetBatteryLevel(BatteryLevel level)
        {
            Status = Status with { BatteryLevel = level };
        }

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

        public void SetDesiredMaxSpeed(Speed speed)
        {
            Status = Status with { DesiredMaxSpeed = speed };
        }

        public void SetStandby(bool isInStandby)
        {
            Status = Status with { IsInStandby = isInStandby };
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
