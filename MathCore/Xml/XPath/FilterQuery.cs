//------------------------------------------------------------------------------
// <copyright file="FilterQuery.cs" company="Microsoft">
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    // Filter query
    // PathExpr ::= LocationPath
    //              | FilterExpr
    //              | FilterExpr '/' RelativeLocationPath
    //              | FilterExpr '//'RelativeLocationPath
    //
    // FilterExpr ::= PrimaryExpr | FilterExpr Predicate

    // PrimaryExpr    ::=    VariableReference
    //    | '(' Expr ')'
    //    | Literal
    //    | Number
    //    | FunctionCall
    //

    internal class FilterQuery : BaseAxisQuery
    {
        #region Fields

        private readonly Query _Axis;
        private readonly Query _Predicate;
        private int _MatchCount;

        #endregion

        #region Properties

        internal BaseAxisQuery Axis => (BaseAxisQuery)_Axis;

        #endregion

        #region Constructors

        internal FilterQuery(Query AxisQuery, Query predicate)
        {
            QueryInput = ((BaseAxisQuery)AxisQuery).QueryInput;
            _Predicate = predicate;
            _Axis = AxisQuery;
        }

        #endregion

        #region Methods

        //
        // predicate could be result in two results type
        // 1). Number: position query
        // 2). Boolean
        //
        internal override bool MatchNode(XPathReader reader)
        {
            if(!_Axis.MatchNode(reader)) return false;
            var ret = false;
            if(reader.NodeType != XmlNodeType.EndElement && reader.ReadMethod != ReadMethods.MoveToElement)
            {
                ++_MatchCount;
                //
                // send position information down to the predicates
                //
                _Predicate.PositionCount = _MatchCount;
            }

            var obj = _Predicate.GetValue(reader);
            if(obj is bool && Convert.ToBoolean(obj))
                ret = true;
            else if(obj is double && Convert.ToDouble(obj).Equals(_MatchCount))
                ret = true; //we need to know how many this axis has been evaluated
            else if(obj != null && !(obj is double || obj is bool))
                ret = true; //object is node set
            return ret;
        }

        //
        // The filter query value should the selected
        // node value
        // for example:
        //
        // <e><e1 a='1' b='2'><e2 a='3'/><e2 a='1/></e1></e>
        //
        // /e/e1[e2[@a='1'] = 1]
        //
        /// <exception cref="XPathReaderException">Can't get value</exception>
        internal override object GetValue(XPathReader reader) => throw new XPathReaderException("Can't get value");

        #endregion
    }
}