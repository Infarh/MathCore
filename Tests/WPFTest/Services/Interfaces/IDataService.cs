using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTest.Services.Interfaces
{
    record DataValue(int Index, string Value);

    interface IDataService
    {
        IEnumerable<DataValue> GetData(int count);
    }

}
