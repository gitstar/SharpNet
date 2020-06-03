using System;
using System.Reflection;

public static class EnumExtension
{
    public static string ToEnumString(this Enum value)
    {
        Type type = value.GetType();

        string returnValue = string.Empty;


        FieldInfo fi = type.GetField(value.ToString());
        StringValue[] attrs =
           fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
        if (attrs.Length > 0)
            returnValue = attrs[0].Value;

        return returnValue;
    }
}

public class StringValue : Attribute
{
    private string _value;
    public StringValue(string value)
    {
        this._value = value;
    }

    public string Value
    {
        get
        {
            return _value;
        }
    }
}