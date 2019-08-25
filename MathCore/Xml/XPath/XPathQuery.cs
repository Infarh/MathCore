using System.Collections;

namespace System.Xml.XPath
{
    public class XPathQuery
    {
        public event EventHandler<EventArgs<string>> QueryMatch;

        protected virtual void OnMatch(string value) => QueryMatch?.Invoke(this, value);

        #region Fields

        private int[] _DepthLookup;
        private int _MatchCount;
        private int _MatchIndex;
        private bool _MatchState;
        private int _TreeDepth = -1;
        private string _Value;

        #endregion

        #region Properties

        internal int Key { set; get; }

        public string XPath { get; set; }

        /// use can store this compiled expression to query other documents
        public ArrayList GetXPathQueries { get; private set; }

        public bool MatchState => _MatchState;

        public string Value
        {
            get { return _Value; }
            private set
            {
                _Value = value;
                if(value != null) OnMatch(value);
            }
        }

        public object Tag { get; set; }

        #endregion

        #region Constructors

        private XPathQuery() { }

        // once the xpathexpression is constructed
        // the xpath is compiled into the query format
        public XPathQuery(string xpath)
        {
            XPath = xpath;
            Compile();
        }

        public XPathQuery(string xpath, int depth)
        {
            XPath = xpath;
            _TreeDepth = depth - 1;
            Compile();
        }

        #endregion

        #region Methods

        public XPathQuery Clone() => new XPathQuery
        {
            XPath = XPath,
            GetXPathQueries = GetXPathQueries,
            _DepthLookup = _DepthLookup,
            _TreeDepth = _TreeDepth
        };

        //
        // Compile the xpath
        //
        private void Compile()
        {
            GetXPathQueries = new ArrayList();
            var builder = new QueryBuilder();

            //
            // Need to set the query with current reader heigth;
            //
            builder.Build(XPath, GetXPathQueries, _TreeDepth);

            // Index is 0 based , but the count is 1 based
            // plus the null query we added.
            var lv_LookupLength = ((BaseAxisQuery)GetXPathQueries[GetXPathQueries.Count - 2]).Depth + 1;

            _DepthLookup = new int[lv_LookupLength];

            //exclude the null query
            for(var i = 0; i < GetXPathQueries.Count - 1; ++i)
                if(_DepthLookup[((BaseAxisQuery)GetXPathQueries[i]).Depth] == 0)
                    _DepthLookup[((BaseAxisQuery)GetXPathQueries[i]).Depth] = i;
        }

        public override string ToString() => XPath;

        //
        // report if the current query is matched
        //
        internal bool Match()
        {
#if DEBUG1
            Console.WriteLine("GetState: Query: {0}, matchState {1}", xpath, matchState);
#endif
            return _MatchState;
        }


        //
        // Look if the current query match what we have found
        // Two conditions to evaluate a match
        // 1). matchingIndex points to the end of query
        // 2). the depth of the tree matches the depth of the query
        private bool _IsElementValue;

        private void SetMatchState(XPathReader reader)
        {
            if(_MatchIndex < 1) return;

            var lv_QueryCount = GetXPathQueries.Count - 1; // take out the null query;
            var lv_QueryDepth = ((BaseAxisQuery)GetXPathQueries[_MatchIndex - 1]).Depth;

            if(_MatchCount == lv_QueryCount && lv_QueryDepth == reader.Depth)
            {
                _MatchState = true;
                if(reader.NodeType == XmlNodeType.Attribute)
                    Value = reader.Value;
                if(reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement)
                    _IsElementValue = true;
            }

#if DEBUG1
            Console.WriteLine("\nMatch query: {0}", xpath);
            Console.WriteLine("MatchCount {0}, query count{1}", matchCount, queryCount);
            Console.WriteLine("reader depth {0}, queryDepth{1}", matchCount, queryCount);
            Console.WriteLine("Name: {0}", reader.Name);
            Console.WriteLine("matchState {0}", matchState);
#endif
        }


        //
        //check if the current processing query is attribute query
        //
        internal bool IsAttributeQuery() => GetXPathQueries[_MatchIndex] is AttributeQuery;


        private bool _WaitingAttributes;
        //
        // Reset the matching index if the reader move to the
        // node depth less than the expected depth
        //
        private void ResetMatching(XPathReader reader)
        {
            if(_WaitingAttributes)
            {
                switch(reader.NodeType)
                {
                    case XmlNodeType.Text:
                        Value = reader.Value;
                        _WaitingAttributes = false;
                        break;
                    case XmlNodeType.EndElement:
                        _WaitingAttributes = false;
                        break;
                }
            }

            if(_MatchState)
            {
                if(_IsElementValue)
                {
                    if(reader.NodeType == XmlNodeType.Attribute)
                        _WaitingAttributes = true;
                    _IsElementValue = false;
                }
                else
                    Value = null;
                _MatchState = false;
            }

            // reset the matching index
            var count = GetXPathQueries.Count;

            if(reader.Depth < ((BaseAxisQuery)GetXPathQueries[_MatchIndex]).Depth)
                _MatchCount = (_MatchIndex = _DepthLookup[reader.Depth]) + 1;

            if(_MatchCount != count - 1 || _MatchIndex <= 0) return;
            _MatchCount--;
            _MatchIndex--;

#if DEBUG1
            Console.WriteLine("\nResetMatchingIndex: {0}\n", xpath);
            Console.WriteLine("matchIndex: {0}, matchCount {1}, compiledXPath.Count {2}", matchIndex, matchCount, count);
#endif
        }


        // In the query expression, we need to be
        // store the depth of tree that we have treversed
        // we shouldn't move the reader, if we move to the
        // an attribute, we need to move it back

        internal void Advance(XPathReader reader)
        {
            ResetMatching(reader);


            if((Query)GetXPathQueries[_MatchIndex] is DescendantQuery)
            {
                //look through the subtree for the node is
                //looking for
                if(!((Query)GetXPathQueries[_MatchIndex + 1]).MatchNode(reader)) return;
                //found the node that we were looking for
                _MatchIndex += 2;
                _MatchCount = _MatchIndex;

                //set the expected depth for the rest of query
                for(var i = _MatchCount; i < GetXPathQueries.Count - 1; i++)
                    ((BaseAxisQuery)GetXPathQueries[_MatchIndex]).Depth += reader.Depth - 1;
            }
            else
            {
                while(reader.Depth == ((BaseAxisQuery)GetXPathQueries[_MatchIndex]).Depth)
                {
                    if(((Query)GetXPathQueries[_MatchIndex]).MatchNode(reader))
                    {
                        _MatchIndex++;
                        _MatchCount = _MatchIndex;
                    }
                    else
                    {
                        //--matchIndex;
                        break;
                    }
                }

                SetMatchState(reader);
            }
        }

        internal void AdvanceUntil(XPathReader reader)
        {
            Advance(reader);

            if(GetXPathQueries[_MatchIndex] is AttributeQuery)
                reader.ProcessAttribute = reader.Depth + 1; // the attribute depth should be current element plus one
        }

        #endregion
    }
}