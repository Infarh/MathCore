using System.Collections;

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal sealed class XPathCollectionEnumerator(Hashtable XPatches) : IEnumerator
{
    #region Fields

    private readonly IDictionaryEnumerator _HashEnum = XPatches.GetEnumerator();

    #endregion

    #region Properties

    public object Current => ((DictionaryEntry) _HashEnum.Current).Value;

    #endregion

    #region Interfaces

    public bool MoveNext() => _HashEnum.MoveNext();

    public void Reset() => _HashEnum.Reset();

    #endregion
}