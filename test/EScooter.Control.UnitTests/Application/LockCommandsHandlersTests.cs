using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EScooter.Control.Application;
using EScooter.Control.UnitTests.Mock;
using EScooter.Control.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace EScooter.Control.UnitTests.Application
{
    public class LockCommandsHandlersTests
    {
        [Fact]
        public async Task LockCommandTest()
        {
            var iotRM = new IotHubRegistryManagerMock(s =>
            {
                s.Locked.ShouldBeTrue();
            });
            iotRM.ScooterBuilderMock.SetLocked(false);
            var sut = new LockCommandsHandler(iotRM);
            await sut.LockScooter(JsonConvert.SerializeObject(new ScooterCommandDto(Guid.Empty)), new FunctionContextMock());
        }

        [Fact]
        public async Task UnlockCommandTest()
        {
            var iotRM = new IotHubRegistryManagerMock(s =>
            {
                s.Locked.ShouldBeFalse();
            });
            iotRM.ScooterBuilderMock.SetLocked(true);
            var sut = new LockCommandsHandler(iotRM);
            await sut.UnlockScooter(JsonConvert.SerializeObject(new ScooterCommandDto(Guid.Empty)), new FunctionContextMock());
        }
    }
}
