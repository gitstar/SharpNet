using System;
using System.Collections.Generic;


public static class TypeExtension
{
    //You can't insert or update complex types. Lets filter them out.
    public static bool IsSimpleType(this Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        type = underlyingType ?? type;
        var simpleTypes = new List<Type>
                            {
                                typeof(byte),
                                typeof(sbyte),
                                typeof(short),
                                typeof(ushort),
                                typeof(int),
                                typeof(uint),
                                typeof(long),
                                typeof(ulong),
                                typeof(float),
                                typeof(double),
                                typeof(decimal),
                                typeof(bool),
                                typeof(string),
                                typeof(char),
                                typeof(Guid),
                                typeof(DateTime),
                                typeof(DateTimeOffset),
                                typeof(byte[])
                            };
        return simpleTypes.Contains(type) || type.IsEnum;
    }
}

