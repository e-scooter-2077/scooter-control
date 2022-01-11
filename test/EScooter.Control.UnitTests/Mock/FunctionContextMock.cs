using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Control.UnitTests.Mock
{
    public class FunctionContextMock : FunctionContext
    {
        public override string InvocationId => string.Empty;

        public override string FunctionId => string.Empty;

        public override TraceContext TraceContext => null;

        public override BindingContext BindingContext => null;

        public override IServiceProvider InstanceServices { get => null; set => throw new NotImplementedException(); }

        public override FunctionDefinition FunctionDefinition => null;

        public override IDictionary<object, object> Items { get => null; set => throw new NotImplementedException(); }

        public override IInvocationFeatures Features => null;
    }
}
