using WPFTest.Services.Interfaces;

namespace WPFTest.Services
{
    class DataService : IDataService
    {
        private readonly DataValue _Source = new (1, "Source");

        public IEnumerable<DataValue> GetData(int count) => Enumerable
           .Range(1, 100)
           .Select(i => _Source with { Index = i, Value = $"Value {i}" });
    }
}
