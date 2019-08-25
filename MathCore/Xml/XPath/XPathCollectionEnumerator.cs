using System.Collections;

namespace System.Xml.XPath
{
    internal sealed class XPathCollectionEnumerator : IEnumerator
    {
        #region Fields

        private readonly IDictionaryEnumerator _HashEnum;

        #endregion

        #region Properties

        public object Current => ((DictionaryEntry) _HashEnum.Current).Value;

        #endregion

        #region Constructors

        public XPathCollectionEnumerator(Hashtable xpathes)
        {
            _HashEnum = xpathes.GetEnumerator();
        }

        #endregion

        #region Interfaces

        public bool MoveNext() => _HashEnum.MoveNext();

        public void Reset() => _HashEnum.Reset();

        #endregion
    }
}