using System.Diagnostics.Contracts;

namespace MathCore
{
    /// <summary>Объект позволяет определять количество вложенных объектов</summary>
    [ContractClass(typeof(ContractClassICountable))]
    public interface ICountable
    {
        /// <summary>Число элементов</summary>
        int Count { get; }
    }

    [ContractClassFor(typeof(ICountable))]
    abstract class ContractClassICountable : ICountable
    {
        #region Implementation of ICountable

        /// <summary>Число элементов</summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return 0;
            }
        }

        #endregion
    }
}