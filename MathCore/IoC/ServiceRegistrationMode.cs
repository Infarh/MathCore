using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.IoC
{
    /// <summary>Режим регистрации сервиса</summary>
    public enum ServiceRegistrationMode : byte
    {
        /// <summary>Режим регистрации единого объекта для всех вызовов</summary>
        Singleton,
        /// <summary>Режим регистрации, при котором для каждого вызова будет создан новый экземпляр сервиса</summary>
        SingleCall,
        /// <summary>Режим регистрации, при котором для каждого потока будет создан единый экземпляр сервиса</summary>
        SingleThread
    }
}
