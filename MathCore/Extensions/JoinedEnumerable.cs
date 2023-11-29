#nullable enable
using System.Collections;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq;

public class JoinedEnumerable<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> _Source;

    public bool IsOuter { get; set; }

    public JoinedEnumerable(IEnumerable<T> source) => _Source = source;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _Source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _Source.GetEnumerator();
}

/// <remarks>
/// <code>
///     var left_outer_join = 
///         from item_a in ListA.Outer()
///         join item_b in ListB
///             on item_a.Key equals item_b.Key
///         select new (item_a, item_b);
///
///     var right_outer_join = 
///         from item_a in ListA.Inner()
///         join item_b in ListB.Outer()
///             on item_a.Key equals item_b.Key
///         select new (item_a, item_b);
///
///     var full_outer_join = 
///         from item_a in ListA.Outer()
///         join item_b in ListB.Outer()
///             on item_a.Key equals item_b.Key
///         select new (item_a, item_b);
/// </code>
/// <seealso url="https://habr.com/ru/sandbox/39626/"/>
/// </remarks>
public static class JoinedEnumerable
{
    public static JoinedEnumerable<T> Inner<T>(this IEnumerable<T> source) => Wrap(source, false);

    public static JoinedEnumerable<T> Outer<T>(this IEnumerable<T> source) => Wrap(source, true);

    public static JoinedEnumerable<T> Wrap<T>(IEnumerable<T> source, bool IsOuter)
    {
        var joined_source = source as JoinedEnumerable<T> ?? new JoinedEnumerable<T>(source);
        joined_source.IsOuter = IsOuter;
        return joined_source;
    }

    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
        this JoinedEnumerable<TOuter> outer,
        IEnumerable<TInner> inner,
        Func<TOuter, TKey> OuterKeySelector,
        Func<TInner, TKey> InnerKeySelector,
        Func<TOuter, TInner, TResult> ResultSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        if (outer is null) throw new ArgumentNullException(nameof(outer));
        if (inner is null) throw new ArgumentNullException(nameof(inner));
        if (OuterKeySelector is null) throw new ArgumentNullException(nameof(OuterKeySelector));
        if (InnerKeySelector is null) throw new ArgumentNullException(nameof(InnerKeySelector));
        if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

        var left_outer  = outer.IsOuter;
        var right_outer = inner is JoinedEnumerable<TInner> { IsOuter: true } inners;

        if (left_outer && right_outer)
            return FullOuterJoin(outer, inner, OuterKeySelector, InnerKeySelector, ResultSelector, comparer);

        if (left_outer)
            return LeftOuterJoin(outer, inner, OuterKeySelector, InnerKeySelector, ResultSelector, comparer);

        return right_outer
            ? RightOuterJoin(outer, inner, OuterKeySelector, InnerKeySelector, ResultSelector, comparer)
            : Enumerable.Join(outer, inner, OuterKeySelector, InnerKeySelector, ResultSelector, comparer);
    }

    public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
        this IEnumerable<TOuter> outer,
        IEnumerable<TInner> inner,
        Func<TOuter, TKey> OuterKeySelector,
        Func<TInner, TKey> InnerKeySelector,
        Func<TOuter, TInner, TResult> ResultSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        var inner_lookup = inner.ToLookup(InnerKeySelector, comparer);

        foreach (var outer_item in outer)
            foreach (var inner_item in inner_lookup[OuterKeySelector(outer_item)].DefaultIfEmpty())
                yield return ResultSelector(outer_item, inner_item);
    }

    public static IEnumerable<TResult> RightOuterJoin<TOuter, TInner, TKey, TResult>(
        this IEnumerable<TOuter> outer,
        IEnumerable<TInner> inner,
        Func<TOuter, TKey> OuterKeySelector,
        Func<TInner, TKey> InnerKeySelector,
        Func<TOuter, TInner, TResult> ResultSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        var outer_lookup = outer.ToLookup(OuterKeySelector, comparer);

        foreach (var inner_item in inner)
            foreach (var outer_item in outer_lookup[InnerKeySelector(inner_item)].DefaultIfEmpty())
                yield return ResultSelector(outer_item, inner_item);
    }

    public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(
        this IEnumerable<TOuter> outer,
        IEnumerable<TInner> inner,
        Func<TOuter, TKey> OuterKeySelector,
        Func<TInner, TKey> InnerKeySelector,
        Func<TOuter, TInner, TResult> ResultSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        var outer_lookup = outer.ToLookup(OuterKeySelector, comparer);
        var inner_lookup = inner.ToLookup(InnerKeySelector, comparer);

        foreach (var inner_grouping in inner_lookup)
            if (!outer_lookup.Contains(inner_grouping.Key))
                foreach (var inner_item in inner_grouping)
                    yield return ResultSelector(default, inner_item);

        foreach (var outer_grouping in outer_lookup)
            foreach (var inner_item in inner_lookup[outer_grouping.Key].DefaultIfEmpty())
                foreach (var outer_item in outer_grouping)
                    yield return ResultSelector(outer_item, inner_item);
    }
}