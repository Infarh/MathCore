namespace WPFTest.Services.Interfaces
{
    record DataValue(int Index, string Value);

    interface IDataService
    {
        IEnumerable<DataValue> GetData(int count);
    }

}
