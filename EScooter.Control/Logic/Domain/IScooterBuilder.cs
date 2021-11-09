using ScooterControlService.LogicControl.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Control.Logic.Domain
{
    public interface IScooterBuilder : IBuilder<Scooter>
    {
        public void SetDeviceId(Guid id);

        public void SetLocked(bool? locked);

        public void SetPowerSavingMaxSpeed(double? powerSavingMaxSpeed);

        public void SetPowerSavingThreshold(double? powerSavingThreshold);

        public void SetDesiredMaxSpeed(double? desiredMaxSpeed);

        public void SetIsInStandby(bool? isInStandby);

        public void SetBatteryLevel(double? batteryLevel);
    }
}
