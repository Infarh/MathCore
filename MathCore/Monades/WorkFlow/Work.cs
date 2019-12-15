using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Monades.WorkFlow
{
    public abstract class Work
    {
        #region Поля

        [CanBeNull] protected readonly Work _BaseWork;

        #endregion

        #region Свойства

        [NotNull, ItemNotNull]
        public IEnumerable<Work> SubWorks
        {
            get
            {
                var work = _BaseWork;
                while (work != null)
                {
                    yield return work;
                    work = work._BaseWork;
                }
            }
        }

        #endregion

       
    }

    public abstract class Work<T> : Work
    {
       
    }

    public class Work<T, TResult> : Work<TResult>
    {
       
    }
}