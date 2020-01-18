using System.Collections.Generic;
using System.Dynamic;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq
{
    [NotImplemented]
    internal class DynamicNumber : DynamicObject
    {
        // The inner dictionary to store field names and values.
        private readonly Dictionary<string, object> _Dictionary = new Dictionary<string, object>();

        // Get the property value.
        public override bool TryGetMember(GetMemberBinder binder, [CanBeNull] out object result) =>
            _Dictionary.TryGetValue(binder.Name, out result);

        // Set the property value.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _Dictionary[binder.Name] = value;
            return true;
        }

        // Perform the unary operation. 
        //public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        //{
        //    // The Textual property contains 
        //    // the name of the unary operation in addition 
        //    // to the textual representation of the number.
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
        //    // The Textual property contains the textual representation 
        //    // of two numbers, in addition to the name 
        //    // of the binary operation.
        //    string resultTextual = $"{dictionary["Textual"]} {binder.Operation} {((DynamicNumber)arg).dictionary["Textual"]}";

        //    int resultNumeric;

        //    // Checking what type of operation is being performed.
        //    switch(binder.Operation)
        //    {
        //        // Processing mathematical addition (a + b).
        //        case ExpressionType.Add:
        //            resultNumeric = (int)dictionary["Numeric"] + (int)((DynamicNumber)arg).dictionary["Numeric"];
        //            break;

        //        // Processing mathematical subtraction (a - b).
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