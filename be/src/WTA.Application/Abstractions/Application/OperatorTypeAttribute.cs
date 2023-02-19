namespace WTA.Application.Abstractions.Application;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class OperatorTypeAttribute : Attribute
{
    public OperatorTypeAttribute(OperatorType operatorType, int order = 0)
    {
        this.OperatorType = operatorType;
        this.Order = order;
    }

    public OperatorType OperatorType { get; }
    public int Order { get; }
}
