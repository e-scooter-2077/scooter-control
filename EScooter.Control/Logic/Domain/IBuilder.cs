using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Control.Logic.Domain
{
    public interface IBuilder<T>
    {
        public bool CanBuild();

        public T BuildWithDefaults();

        public T Build();
    }
}
