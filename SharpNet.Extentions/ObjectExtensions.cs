using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class ObjectExtensions
{
    private static readonly HashSet<string> _booleanValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true", "1", "on", "yes", "y" };

    public static bool In<T>(this T value, IEnumerable<T> values)
    {
        if (values.IsNull())
            throw new ArgumentNullException("values");

        return values.Contains(value);
    }

    public static bool In<T>(this T value, params T[] collection)
    {
        return collection.ToList().Contains(value);
    }


    public static T ConvertTo<T>(this object value)
    {
        return value.ConvertTo<T>(default(T));
    }

    public static T ConvertTo<T>(this object value, T defaultValue)
    {
        return value.ConvertTo<T>(defaultValue, true);
    }

    public static T ConvertTo<T>(this object value, T defaultValue, bool ignoreException)
    {
        if (value.ToString().IsEmptyOrWhiteSpace())
            return defaultValue;

        if (ignoreException)
        {
            try
            {
                return value.Convert<T>(defaultValue);
            }
            catch
            {
                if (typeof(T).Equals(typeof(string)) && defaultValue.IsNull())
                    return (T)((object)string.Empty);
                else
                    return defaultValue;
            }
        }
        return value.Convert<T>(defaultValue);
    }

    private static T Convert<T>(this object value, T defaultValue)
    {
        var targetType = typeof(T);

        if (value.IsNull())
        {
            if (targetType.Equals(typeof(string)))
            {
                if (defaultValue.IsNull())
                    return (T)(object)string.Empty;
            }
        }
        else
        {
            if (targetType.Equals(typeof(bool)))
            {
                return (T)(object)_booleanValues.Contains(value.ToString().ToLower());
            }
            else if (targetType.Equals(typeof(decimal)))
            {
                decimal deValue;
                if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out deValue))
                    return (T)(object)deValue;

                return default(T);
            }

            if (value.GetType() == targetType)
                return (T)value;

            var converter = TypeDescriptor.GetConverter(value);
            if (converter.IsNotNull())
            {
                if (converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(value, targetType);
            }

            converter = TypeDescriptor.GetConverter(targetType);
            if (converter.IsNotNull())
            {
                if (converter.CanConvertFrom(value.GetType()))
                    return (T)converter.ConvertFrom(value);
            }
        }

        return defaultValue;
    }

    //[Obsolete("ConvertTo<T>() 함수 사용하시기 바랍니다.")]
    //static public DateTime ToDate(this object objVal)
    //{
    //    DateTime retDate = default(DateTime);

    //    try
    //    {
    //        retDate = objVal.ConvertTo<DateTime>();
    //    }
    //    catch { }

    //    return retDate;
    //}

    public static bool IsNull(this object target)
    {
        return IsNull<object>(target);
    }

    public static bool IsNull<T>(this T target)
    {
        if (ReferenceEquals(target, DBNull.Value))
            return true;

        return ReferenceEquals(target, null);
    } 
        
    public static bool IsNotNull(this object target)
    {
        return IsNull(target) == false;
    }

    /// <summary>
    /// 날자형을 yyyy-MM-dd 식의 일반문자렬로 돌린다.
    /// </summary>
    /// <param name="dateTime">날자형 변수</param>
    /// <returns>변환된 문자렬</returns>
    static public string ToDateString(this object dateTime)
    {
        DateTime date = dateTime.ConvertTo<DateTime>();

        return date.ToString("MM/dd/yyyy");
    }

    public static void InvokeMethod(this object obj, string methodName, params object[] args)
    {
        if (obj != null)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(obj, args);
        }
    }

    public static T InvokeMethod<T>(this object obj, string methodName, params object[] args)
    {
        if (obj != null)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName);
            if (method != null)
                return (T)method.Invoke(obj, args);
        }
        return default(T);
    }

    #region XML
    public static bool SaveXMLToFile(this object obj, string fileName)
    {
        try
        {
            using (StreamWriter wr = new StreamWriter(fileName, true,Encoding.UTF8))
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                xs.Serialize(wr, obj);
            }
        }
        catch
        {
            return false;
        }

        return true;
       
    }

    public static T GetXMLFromFile<T>(this string fileName)
    {
        try
        {
            using (var reader = new StreamReader(fileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T t = (T)xs.Deserialize(reader);

                return t;
            }
        }
        catch 
        {
            return default(T);
        }
    }

    #endregion

}
