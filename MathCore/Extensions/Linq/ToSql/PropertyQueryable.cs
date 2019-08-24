//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;

//namespace System.Linq.ToSQL
//{
//    public class PropertyQueryable<T> : IQueryable<T>
//    {
//        private readonly IQueryable<T> _Queryable;

//        public IQueryProvider Provider { get; private set; }

//        public Expression Expression { get { return _Queryable.Expression; } }

//        public Type ElementType { get { return typeof(T); } }

//        public PropertyQueryable(IQueryable<T> queryable)
//        {
//            _Queryable = queryable;
//            Provider = new PropertyProvider(queryable.Provider);
//        }

//        public IEnumerator<T> GetEnumerator() { return _Queryable.GetEnumerator(); }

//        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
//    }
//}