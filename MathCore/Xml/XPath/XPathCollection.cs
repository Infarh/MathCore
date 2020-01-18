//------------------------------------------------------------------------------
// <copyright file="XPathCollection.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    //
    // Enumerator
    //


    //-----------------------------------------------------------------------------------------
    // The class is the place to associate the xpath string expression
    // and it's compiled query expression


    //XPathCollection class
    public class XPathCollection : ICollection
    {
        #region Fields

        private readonly Hashtable _XPathes;

        private int _Key; // number of xpathes added into collection as keys
        private XPathReader _Reader;

        #endregion

        #region Properties

        internal XPathReader SetReader { set => _Reader = value; }

        internal int ProcessCount { get; set; } = 0;

        public XmlNamespaceManager NamespaceManager { set; get; }

        public XPathQuery this[int index] => (XPathQuery)_XPathes[index];

        public int Count => _XPathes.Count;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;


        public bool IsFixedSize => false;

        #endregion

        #region Constructors

        public XPathCollection() => _XPathes = new Hashtable();

        public XPathCollection(XmlNamespaceManager NsManager) : this() => NamespaceManager = NsManager;

        #endregion

        #region Methods

        //
        // Add matched query index into an array list
        //
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null" />.</exception>
        internal bool MatchesAny(ArrayList list, int depth)
        {

            if(list is null)
                throw new ArgumentNullException(nameof(list));

            list.Clear();

            var ret = false;
            foreach(var expr in this.Cast<XPathQuery>().Where(expr => expr.Match()))
            {
                list.Add(expr.Key);
                ret = true;
            }
            return ret;
        }

        //
        // If the current processing query is attribute
        // query Read will not Read, it will MoveToAttribute
        // instead.
        //
        internal bool CurrentContainAttributeQuery() => this.Cast<XPathQuery>().Any(expr => expr.IsAttributeQuery());

        //
        // walk through all the queries to see
        // if any query matches with the current
        // read
        //
        internal void Advance(XPathReader reader)
        {
            foreach(XPathQuery expr in this)
                expr.Advance(reader);
        }

        //
        // walk through all the queries to see
        // if any query matches with the current
        // read
        //
        internal void AdvanceUntil(XPathReader reader)
        {
            foreach(XPathQuery expr in this)
                expr.AdvanceUntil(reader);

            if(!CurrentContainAttributeQuery())
                reader.ProcessAttribute = -1;
        }

        //
        // Look throw all the query list
        // to see if any query has reach the end
        // in MatchAnyQuery, we need to move to
        // attribute as well we there is a query
        //
        //
        internal bool MatchAnyQuery() => this.Cast<XPathQuery>().Any(expr => expr.Match());

        //
        // check if a expression contains in the collection
        //
        public bool Contains(XPathQuery expr) => _XPathes.ContainsValue(expr);

        public bool Contains(string xpath) => _XPathes.Cast<XPathQuery>().Any(expr => expr.ToString() == xpath);

        public XPathQuery Add(string expression)
        {
            XPathQuery xpathexpr;

            if(_Reader is null)
                xpathexpr = new XPathQuery(expression);
            else
            {
                xpathexpr = new XPathQuery(expression, _Reader.Depth);
                if(_Reader.ReadState == ReadState.Interactive)
                    xpathexpr.Advance(_Reader);
            }

            xpathexpr.Key = _Key;

            _XPathes.Add(_Key++, xpathexpr);

            return xpathexpr;
        }

        public int Add(XPathQuery xpathexpr)
        {
            xpathexpr.Key = _Key;
            _XPathes.Add(_Key++, xpathexpr);
            return _Key - 1;
        }

        public void Clear() => _XPathes.Clear();

        public void Remove(XPathQuery xpathexpr) => _XPathes.Remove(xpathexpr.Key);

        public void Remove(string xpath) => _XPathes
            .Cast<XPathQuery>()
            .Where(xpathexpr => xpathexpr.ToString() == xpath)
            .Select(p => p.Key)
            .Foreach(Remove);

        public void Remove(int index) => _XPathes.Remove(index);

        #endregion

        #region Interfaces

        public void CopyTo(Array array, int index) { }

        //IEnumerable interface
        public IEnumerator GetEnumerator() => new XPathCollectionEnumerator(_XPathes);

        #endregion
    }
}