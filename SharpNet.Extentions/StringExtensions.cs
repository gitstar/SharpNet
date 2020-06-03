using System;
using System.Text;

public static class StringExtensions
{   

    /// <summary>
    ///  금액문자렬을  , 을 포함한 소수점 2자리 반올림 문자열형으로 변환한다.
    ///  
    /// </summary>
    /// <param name="money"> 소수점자리 포함한 금액</param>
    /// <returns>소수점 2자리형식으로 표현한 금액 문자열을 리턴한다.  </returns>
    public static string ToMoneyStringView(this string strMoney, int pos = 2, int viewPos = 2)
    {
        decimal money = strMoney.ToMoney(pos);

        string typeStr = money.ToMoneyStringView(viewPos);

        return typeStr;
    }

    /// <summary>
    /// 금액을 문자열형으로부터 옹근수형으로 변환한다.
    /// </summary>
    /// <param name="str">소수점이 포함된 금액표현 문자열.</param>
    /// <returns>소수점자리수를 곱한 옹근수형식으로 금액을 리턴한다.     </returns>
    public static decimal ToMoney(this string str, int pos = 2)
    {
        decimal money = 0;

        try
        {
            money =Math.Round( Convert.ToDecimal(str), pos);
        }
        catch { }

        return money;
    }

    /// <summary>
    /// 금액을 문자열형으로부터 지정된 자리수만큼 버린다.
    /// </summary>
    /// <param name="str">소수점이 포함된 금액표현 문자열.</param>
    /// <returns>소수점자리수를 곱한 옹근수형식으로 금액을 리턴한다.     </returns>
    public static decimal ToTruncateMoney(this string str, int pos = 2)
    {
        decimal money = 0;
        int unit = 1;
        for (int i = 0; i < pos; i++)
        {
            unit = unit * 10;
        }

        try
        {
            money = Math.Truncate(Convert.ToDecimal(str) * unit) / unit;
        }
        catch { }

        return money;
    }

    public static string  ToDateString(this string str)
    {
        DateTime retDate = default(DateTime);

        try
        {
            retDate = str.ConvertTo<DateTime>();
        }
        catch { }

        return retDate.ToDateString();
    }

    public static bool IsNumeric(this string s)
    {
        float output;
        return float.TryParse(s, out output);
    }

    public static bool IsDate(this string s)
    {
        DateTime dt;
        return DateTime.TryParse(s, out dt);
    }

    public static string ToCodeString(this string str)
    {
        string strCode = string.Empty;

        if (str.Length > 4)
            strCode = str.Substring(0, 4);
        else
            strCode = str;

        return strCode;
    }

    public static bool IsEmptyOrWhiteSpace(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNotEmptyOrWhiteSpace(this string value)
    {
        return (value.IsEmptyOrWhiteSpace() == false);
    }

    public static bool IsDigit(this string value)
    {
        foreach(char i in value)
            if (i < '0' || i > '9')
                return false;

        return true;
    } 

    /// <summary>
    /// 문자렬을 kr로 변환
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] GetByteEUCKR(this string value)
    {
        int euckrCodepage = 51949;
        return Encoding.GetEncoding(euckrCodepage).GetBytes(value);
    }

    public static byte[] GetByteDefault(this string value)
    {
        return Encoding.Default.GetBytes(value);
    }

    public static byte[] GetByteUtf8(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }
}
