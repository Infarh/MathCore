using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace System.Xml.Linq
{
    public class DynamicXmlNode : DynamicObject
    {
        readonly XElement _Node;
        public DynamicXmlNode() { }
        public DynamicXmlNode(XElement node) { _Node = node; }
        public DynamicXmlNode(string name) { _Node = new XElement(name); }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var node = _Node.Element(binder.Name);
            if(node != null)
                node.SetValue(value);
            else
                _Node.Add(value.GetType() == typeof(DynamicXmlNode)
                    ? new XElement(binder.Name)
                    : new XElement(binder.Name, value));
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var lv_GetNode = _Node.Element(binder.Name);
            if(lv_GetNode != null)
            {
                result = new DynamicXmlNode(lv_GetNode);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if(binder.Type == typeof(string))
            {
                result = _Node.Value;
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var xtype = typeof(XElement);
            try
            {
                const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance;
                result = xtype.InvokeMember(binder.Name, flags, null, _Node, args);
                return true;
            } catch
            {
                result = null;
                return false;
            }
        }
    }

    public class DynamicNumber : DynamicObject
    {
        // The inner dictionary to store field names and values.
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        // Get the property value.
        public override bool TryGetMember(GetMemberBinder binder, out object result) =>
            dictionary.TryGetValue(binder.Name, out result);

        // Set the property value.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            return true;
        }

        // Perform the unary operation. 
        //public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        //{
        //    // The Textual property contains 
        //    // the name of the unary operation in addition 
        //    // to the textual representaion of the number.
        //    string resultTextual = $"{binder.Operation} {dictionary["Textual"]}";
        //    int resultNumeric;

        //    // Determining what type of operation is being performed.
        //    switch(binder.Operation)
        //    {
        //        case ExpressionType.Negate:
        //            resultNumeric = -(int)dictionary["Numeric"];
        //            break;
        //        default:
        //            // In case of any other unary operation,
        //            // print out the type of operation and return false,
        //            // which means that the language should determine 
        //            // what to do.
        //            // (Usually the language just throws an exception.)            
        //            Console.WriteLine("{0}: This unary operation is not implemented", binder.Operation);
        //            result = null;
        //            return false;
        //    }

        //    dynamic finalResult = new DynamicNumber();
        //    finalResult.Textual = resultTextual;
        //    finalResult.Numeric = resultNumeric;
        //    result = finalResult;
        //    return true;
        //}

        //public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        //{
        //    // The Textual property contains the textual representaion 
        //    // of two numbers, in addition to the name 
        //    // of the binary operation.
        //    string resultTextual = $"{dictionary["Textual"]} {binder.Operation} {((DynamicNumber)arg).dictionary["Textual"]}";

        //    int resultNumeric;

        //    // Checking what type of operation is being performed.
        //    switch(binder.Operation)
        //    {
        //        // Proccessing mathematical addition (a + b).
        //        case ExpressionType.Add:
        //            resultNumeric = (int)dictionary["Numeric"] + (int)((DynamicNumber)arg).dictionary["Numeric"];
        //            break;

        //        // Processing mathematical substraction (a - b).
        //        case ExpressionType.Subtract:
        //            resultNumeric = (int)dictionary["Numeric"] - (int)((DynamicNumber)arg).dictionary["Numeric"];
        //            break;

        //        // In case of any other binary operation,
        //        // print out the type of operation and return false,
        //        // which means that the language should determine 
        //        // what to do.
        //        // (Usually the language just throws an exception.)
        //        default:
        //            Console.WriteLine("{0}: This binary operation is not implemented", binder.Operation);
        //            result = null;
        //            return false;
        //    }

        //    dynamic finalResult = new DynamicNumber();
        //    finalResult.Textual = resultTextual;
        //    finalResult.Numeric = resultNumeric;
        //    result = finalResult;
        //    return true;
        //}
    }
}
