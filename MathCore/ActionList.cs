using System;
using System.Collections.Generic;

namespace MathCore
{
    public class ActionList : List<Action>
    {
        public ActionList() { }
        public ActionList(int capacity) : base(capacity) { }
        public ActionList(IEnumerable<Action> collection) : base(collection) { }

        public void Invoke() => ForEach(a => a.Invoke());
    }
}