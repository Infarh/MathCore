using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.IoC
{
    public interface IServiceRegistrations
    {
        ServiceRegistration this[Type ServiceType] { get; }
    }
}
