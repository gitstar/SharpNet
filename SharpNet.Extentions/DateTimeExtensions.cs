using System;

public static class DateTimeExtensions
{
    /// <summary>
    /// 날짜형을 년-월-일 시:분:초 형식의 문자열로 변환한다.
    /// </summary>
    /// <param name="dateVal">날짜정보</param>
    /// <returns>날짜형식 문자열을 반환한다. </returns>
    static public string ToShortString(this DateTime dateVal)
    {
        if (dateVal.Year < 1900)
            return "";

        return dateVal.ToString("yyyy-MM-dd HH:mm:ss");
      //  return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
    }

    static public string ToShortString(this DateTime? dateVal)
    {
        if (dateVal == null)
            return "1900-01-01";

        return dateVal.ConvertTo<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");
        //  return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
    }

    /// <summary>
    /// 날짜형을 년-월-일 시:분:초 형식의 문자열로 변환한다.
    /// </summary>
    /// <param name="dateVal"> 날짜정보</param>
    /// <returns>날짜형 자료를 리턴한다.  </returns>
    static public string ToLongString(this DateTime dateVal)
    {
        if (dateVal.Year < 1900)
            return "";

        //return dateVal.ToString("yyyy-MM-dd HH:mm:ss.fff");

        return dateVal.ToString("MM/dd/yyyy HH:mm:ss.fff");
    }

    /// <summary>
    /// 날자형을 yyyy-MM-dd 식의 일반문자렬로 돌린다.
    /// </summary>
    /// <param name="dateTime">날자형 변수</param>
    /// <returns>변환된 문자렬</returns>
    static public string ToDateString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }
    

    /// <summary>
    /// 두 날자를 비교하여 크기 판정
    /// </summary>
    /// <param name="dateTime1"> 첫번째 날자</param>
    /// <param name="dateTime2"> 두번째 날자 </param>
    /// <returns> a>b => 1, a < b => -1, a=b=> 0의 세가지 상태를 돌린다.</returns>
    static public int DateTimeCompare(this DateTime dateTime1, DateTime dateTime2)
    {
        try
        {
            Int64 nDateTime1 = 0;
            Int64 nDateTime2 = 0;

            nDateTime1 = Convert.ToInt64(dateTime1.ToStaticString());
            nDateTime2 = Convert.ToInt64(dateTime2.ToStaticString());

            if (nDateTime1 > nDateTime2)
                return 1;
            else if (nDateTime1 < nDateTime2)
                return -1;
            else if (nDateTime1 == nDateTime2)
                return 0;

            return -1;
        }
        catch (Exception)
        { return -1; }
    }

    /// <summary>
    /// 날자 차이 계산하는 함수.
    /// </summary>
    /// <param name="DateTime1"></param>
    /// <param name="DateTime2"></param>
    /// <returns></returns>
    public static string DateDiff(this DateTime DateTime1, DateTime DateTime2)
    {
        string dateDiff = null;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();

        dateDiff = ts.Days.ToString();

        //  dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";

        return dateDiff;
    }
    /// <summary>
    /// 날자형을 yyMMddHHmmss 식의 일반문자렬로 돌린다.
    /// </summary>
    /// <param name="dateTime">날자형 변수</param>
    /// <returns>변환된 문자렬</returns>
    static private string ToStaticString(this DateTime dateTime)
    {
        return dateTime.ToString("yyMMddHHmmss");
    }
}
