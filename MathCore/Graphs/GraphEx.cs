using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Graphs;

/// <summary>Методы-расширения, позволяющие рассматривать любой объект как граф</summary>
public static class GraphEx
{
    //public static void Test()
    //{
    //    //        var M = new[,]
    //    //        { /* 0  1  2  3  4  5  6  7  8  9 10 11 12 13 14*/
    //    // /*  0 */   {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /*  1 */   {1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /*  2 */   {1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /*  3 */   {0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0},
    //    // /*  4 */   {0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0},
    //    // /*  5 */   {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0},
    //    // /*  6 */   {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
    //    // /*  7 */   {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /*  8 */   {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /*  9 */   {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /* 10 */   {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /* 11 */   {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /* 12 */   {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /* 13 */   {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    //    // /* 14 */   {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    //    //        };

    //    // var P = new[,]
    //    // {
    //    //    {false, true, true, true, true, true, false, false},
    //    //    {false, false, true, false, true, false, false, false},
    //    //    {false, false, false, true, false, false, false, false},
    //    //    {false, false, false, false, true, false, false, false},
    //    //    {false, false, false, false, false, true, false, false},
    //    //    {false, false, true, false, false, false, true, true},
    //    //    {false, false, false, false, false, true, false, true},
    //    //    {false, false, false, false, false, true, true, false}
    //    // };

    //    // var dir = new DirectoryInfo("c:\\");
    //    // var root = dir.AsGraphNode(d => d.GetDirectories(), (from, to) => dir.GetFilesCount());

    //    // var root1 = new { M, i = 0 }
    //    //    .AsGraphNode(r => Enumerable.Range(0, r.M.GetLength(1)).Select(i => new { M, i }),
    //    //        (from, to) => from.M[to.i, from.i]);


    //    //var root2 = root1.AsGraphNode(node => node.Links.Where(link => link.Weight == 1)
    //    //    .Select(link => link.Node),
    //    //    (from, to) => to.Value.i - from.Value.i);
    //    //var R = root2.GetWaveRoute();

    //    //typeof (List<int>).AsGraphNode
    //    //    (
    //    //        type => type.GetProperties(BindingFlags.Public|BindingFlags.Instance)
    //    //            .Select(p => p.PropertyType),
    //    //        (From, to) => From.Name
    //    //    );
    //}

    /// <summary>Преобразование к типу вершины графа</summary>
    /// <typeparam name="TValue">Тап значения вершины</typeparam>
    /// <typeparam name="TWeight">Тип веса связи</typeparam>
    /// <param name="value">Значение вершины</param>
    /// <param name="GetChilds">Метод выделения дочерних узлов</param>
    /// <param name="GetWeight">Метод определения веса связи между узлами</param>
    /// <param name="Buffered">Буферизация узлов и связей</param>
    /// <returns>Узел графа</returns>
    /// <example>
    /// <code>
    /// var M = new[,]
    ///        { /* 0  1  2  3  4  5  6  7  8  9 10 11 12 13 14*/
    /// /*  0 */   {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /*  1 */   {1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /*  2 */   {1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /*  3 */   {0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0},
    /// /*  4 */   {0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0},
    /// /*  5 */   {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0},
    /// /*  6 */   {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
    /// /*  7 */   {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /*  8 */   {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /*  9 */   {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /* 10 */   {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /* 11 */   {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /* 12 */   {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /* 13 */   {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    /// /* 14 */   {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
    ///        };
    /// var node = new { M, i = 0 };
    /// var root = node.AsGraphNode(r => Enumerable.Range(0, r.M.GetLength(1)).Select(i => new { M, i }),
    ///              (from, to) => from.M[to.i, from.i]);
    /// var tree = root.AsGraphNode(root => root.Links.Where(link => link.Weight == 1)
    ///                                               .Select(link => link.Node),
    ///                             (from, to) => to.Value.i - from.Value.i);
    /// var rout = tree.GetWaveRoute();
    /// 
    /// </code>
    /// </example>
    [NotNull]
    public static IGraphNode<TValue, TWeight> AsGraphNode<TValue, TWeight>(
        this TValue value,
        Func<TValue, IEnumerable<TValue>> GetChilds,
        Func<TValue, TValue, TWeight> GetWeight,
        bool Buffered = false) 
        => new LambdaGraphNode<TValue, TWeight>(value, GetChilds, GetWeight, Buffered);

    /// <summary>Преобразование к виду графа</summary>
    /// <typeparam name="TValue">Тип преобразуемого объекта</typeparam>
    /// <param name="value">Преобразуемый объект</param>
    /// <param name="GetChilds">Метод извлечения дочерних узлов из каждого узла графа</param>
    /// <param name="Buffered">Флаг необходимости проведения буферизации</param>
    /// <returns>Узел графа</returns>
    [NotNull]
    public static IGraphNode<TValue> AsGraphNode<TValue>(
        this TValue value, 
        Func<TValue, IEnumerable<TValue>> GetChilds, 
        bool Buffered = false) 
        => new LambdaGraphNode<TValue>(value, GetChilds, Buffered);

    /// <summary>Метод перебора вершин графа путём обхода "в глубину". Обход на основе стека дочерних узлов.</summary>
    /// <typeparam name="TValue">Тип узлов графа</typeparam>
    /// <typeparam name="TWeight">Тип связи узлов графа</typeparam>
    /// <param name="Node">Начальный узел графа</param>
    /// <returns>Последовательность узлов графа</returns>
    public static IEnumerable<IGraphNode<TValue, TWeight>> BypassInDepth<TValue, TWeight>(this IGraphNode<TValue, TWeight> Node)
    {
        var stack = new Stack<IGraphNode<TValue, TWeight>>();
        stack.Push(Node);
        var visited = new HashSet<IGraphNode<TValue, TWeight>>();
        do
        {
            do Node = stack.Pop(); while(visited.Contains(Node));
            yield return Node;
            visited.Add(Node);
            Node.Links.Select(l => l.Node)
               .Where(node => !stack.Contains(node) && !visited.Contains(node))
               .ToArray()
               .GetReversed()
               .Foreach(stack.Push);
        } while(stack.Count != 0);
    }

    /// <summary>Метод перебора вершин графа путём обхода "в глубину". Обход на основе стека дочерних узлов.</summary>
    /// <typeparam name="TValue">Тип узлов графа</typeparam>
    /// <param name="Node">Начальный узел графа</param>
    /// <returns>Последовательность узлов графа</returns>
    public static IEnumerable<IGraphNode<TValue>> BypassInDepth<TValue>(this IGraphNode<TValue> Node)
    {
        var stack = new Stack<IGraphNode<TValue>>();
        stack.Push(Node);
        var visited = new HashSet<IGraphNode<TValue>>();
        do
        {
            do Node = stack.Pop(); while(visited.Contains(Node));
            yield return Node;
            visited.Add(Node);
            Node.Where(node => !stack.Contains(node) && !visited.Contains(node))
               .ToArray()
               .GetReversed()
               .Foreach(stack.Push);
        } while(stack.Count != 0);
    }

    /// <summary>Метод перебора вершин графа путём обхода "в ширину". Обход на основе очереди дочерних узлов.</summary>
    /// <typeparam name="TValue">Тип узлов графа</typeparam>
    /// <typeparam name="TWeight">Тип связи узлов графа</typeparam>
    /// <param name="Node">Начальный узел графа</param>
    /// <returns>Последовательность узлов графа</returns>
    public static IEnumerable<IGraphNode<TValue, TWeight>> BypassInWidth<TValue, TWeight>(this IGraphNode<TValue, TWeight> Node)
    {
        var queue = new Queue<IGraphNode<TValue, TWeight>>();
        queue.Enqueue(Node);
        var visited = new HashSet<IGraphNode<TValue, TWeight>>();
        do
        {
            Node = queue.Dequeue();
            yield return Node;
            visited.Add(Node);
            Node.Links.Select(l => l.Node)
               .Where(node => !queue.Contains(node) && !visited.Contains(node))
               .Foreach(queue.Enqueue);
        } while(queue.Count != 0);
    }

    /// <summary>Метод перебора вершин графа путём обхода "в ширину". Обход на основе очереди дочерних узлов.</summary>
    /// <typeparam name="TValue">Тип узлов графа</typeparam>
    /// <param name="Node">Начальный узел графа</param>
    /// <param name="TakeRoot">Перечислять корень дерева?</param>
    /// <returns>Последовательность узлов графа</returns>
    public static IEnumerable<IGraphNode<TValue>> BypassInWidth<TValue>(this IGraphNode<TValue> Node, bool TakeRoot = true)
    {
        var queue   = new Queue<IGraphNode<TValue>>();
        var visited = new HashSet<IGraphNode<TValue>>();
        if (TakeRoot) queue.Enqueue(Node);
        else Node.Where(node => !queue.Contains(node)).Foreach(queue.Enqueue);
        do
        {
            Node = queue.Dequeue();
            yield return Node;
            visited.Add(Node);
            Node.Where(node => !queue.Contains(node) && !visited.Contains(node)).Foreach(queue.Enqueue);
        } while(queue.Count != 0);
    }

    /// <summary>Метод перебора вершин графа путём обхода "в ширину". Обход на основе очереди дочерних узлов с указанием функции хеширования.</summary>
    /// <typeparam name="TValue">Тип узлов графа</typeparam>
    /// <param name="Node">Начальный узел графа</param>
    /// <param name="hash">Функция хеширования элементов</param>
    /// <param name="TakeRoot">Перечислять корень дерева?</param>
    /// <returns>Последовательность узлов графа</returns>
    public static IEnumerable<IGraphNode<TValue>> BypassInWidth<TValue>(
        this IGraphNode<TValue> Node,
        Func<IGraphNode<TValue>, int> hash,
        bool TakeRoot = true)
    {
        var queue   = new Queue<IGraphNode<TValue>>();
        var visited = new HashSet<int>();
        if(TakeRoot) queue.Enqueue(Node);
        else Node.Where(node => !queue.Contains(node)).Foreach(queue.Enqueue);
        do
        {
            Node = queue.Dequeue();
            yield return Node;
            visited.Add(hash(Node));
            Node.Where(node => !queue.Contains(node) && !visited.Contains(hash(node)))
               .Foreach(queue.Enqueue);
        } while(queue.Count != 0);
    }

    /// <summary>Метод поиска пути в графе путём обхода вершин "в глубину"</summary>
    /// <typeparam name="TValue">Тип вершины графа</typeparam>
    /// <typeparam name="TWeight">Тип связи вершин графа</typeparam>
    /// <param name="RootNode">Начальный элемент поиска пути</param>
    /// <param name="FindPredicate">Метод определения окончания поиска, как успешного</param>
    /// <returns>Маршрут в графе</returns>
    [NotNull]
    public static GraphRoute<TValue, TWeight> FindRouteInDepth<TValue, TWeight>(
        this IGraphNode<TValue, TWeight> RootNode,
        [NotNull] Predicate<TValue> FindPredicate)
    {
        var stack = new Stack<IGraphNode<TValue, TWeight>>();
        var node  = RootNode;
        stack.Push(node);
        var visited     = new HashSet<IGraphNode<TValue, TWeight>>();
        var route_stack = new Stack<IGraphNode<TValue, TWeight>>();
        do
        {
            do node = stack.Pop(); while(visited.Contains(node));
            route_stack.Push(node);

            if(FindPredicate(node.Value))
                return new(route_stack.Reverse());

            visited.Add(node);
            var next = node.Links.Select(l => l.Node)
               .Where(n => !stack.Contains(n) && !visited.Contains(n))
               .ToArray()
               .GetReversed();

            if(next.Length == 0)
                while(route_stack.Count > 0
                      && route_stack.Peek().Links.All(l => visited.Contains(l.Node)))
                    route_stack.Pop();
            next.Foreach(stack.Push);
        } while(stack.Count != 0);

        return new([]);
    }

    /// <summary>Метод поиска всех путей из указанной вершины до всех доступных вершин графа методом фронта волны</summary>
    /// <typeparam name="TValue">Тип вершины графа</typeparam>
    /// <typeparam name="TWeight">Тип связи вершин графа</typeparam>
    /// <param name="Root">Начальный элемент поиска пути</param>
    /// <returns>Массив найденных путей</returns>
    [NotNull]
    public static GraphRoute<TValue, TWeight>[] GetWaveRoute<TValue, TWeight>(this IGraphNode<TValue, TWeight> Root)
    {
        var visited = new HashSet<IGraphNode<TValue, TWeight>>();
        var queue   = new Queue<Stack<IGraphNode<TValue, TWeight>>>();
        var stack   = new Stack<IGraphNode<TValue, TWeight>>();
        stack.Push(Root);
        visited.Add(Root);
        queue.Enqueue(stack);
        var result = new List<GraphRoute<TValue, TWeight>>();
        do
        {
            var route_stack = queue.Dequeue();
            var wave = route_stack
               .Peek()
               .Links
               .Select(l => l.Node)
               .Where(n => !visited.Contains(n))
               .ToArray();
            if(wave.Length == 0)
            {
                result.Add(new(route_stack.ToArray().GetReversed()));
                continue;
            }
            foreach(var node in wave)
            {
                visited.Add(node);
                var r = new Stack<IGraphNode<TValue, TWeight>>(route_stack.Reverse());
                r.Push(node);
                queue.Enqueue(r);
            }
        } while(queue.Count > 0);
        return [.. result];
    }

    [NotImplemented]
    internal static IGraphNode<TValue> GetRouteAStar<TValue>(this IGraphNode<TValue> Root,
        Func<IGraphNode<TValue>, int> hash,
        Func<IGraphNode<TValue>, IGraphNode<TValue>, double> step_length,
        Func<IGraphNode<TValue>, double> length)
    {
        var visited = new HashSet<int>();
        var queue   = new Queue<IGraphNode<TValue>>();
        Root.Childs.Foreach(queue.Enqueue);


        throw new NotImplementedException();
    }
}