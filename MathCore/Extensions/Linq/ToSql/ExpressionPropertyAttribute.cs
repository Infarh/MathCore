
namespace System.Linq.ToSQL
{
    public class ExpressionPropertyAttribute : Attribute
    {
        public string SubPropertyName { get; private set; }

        public string PropertyName { get; private set; }

        public ExpressionPropertyAttribute(string PropertyName, string SubPropertyName = null)
        {
            this.PropertyName = PropertyName;
            this.SubPropertyName = SubPropertyName;
        }
    }
}
