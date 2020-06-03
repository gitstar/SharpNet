using System;

public static class DecimalExtensions
{
    /// <summary>
    /// 금액을 옹근수형으로부터 소수점 2자리 반올림 문자열형으로 변환한다.
    /// </summary>
    /// <param name="money"></param>
    /// <returns>금액 문자렬</returns>
    public static string ToMoneyString(this decimal money, int pos = 2)
    {
        string retStr = Math.Round(money, pos).ToString();

        return retStr;
    }

    /// <summary>
    ///  decimal 금액을 옹근수형으로부터 ,을 포함한 소수점 2자리 반올림 문자열형으로 변환한다.
    ///  
    /// </summary>
    /// <param name="money"> 소수점자리 포함한 금액</param>
    /// <returns>소수점 2자리형식으로 표현한 금액 문자열을 리턴한다.  </returns>
    public static string ToMoneyStringView(this decimal money, int pos = 2)
    {
        string typeStr = "{0:###,###,###,###0"; //.00}";

        if (pos > 0)
        {
            typeStr += ".";
            for (int i = 0; i < pos; i++)
            {
                typeStr += "0";
            }
        }
        typeStr += "}";

        typeStr = string.Format(typeStr, money);

        if (typeStr == ".00")
            return "0";
        else
            return typeStr;
    }

    /// <summary>
    /// % 계산된 Deal 금액을 돌린다.
    /// </summary>
    /// <param name="moeny"></param>
    /// <param name="strPercent"></param>
    /// <returns></returns>
    public static decimal ToPercentDealMoney(this decimal moeny, string strPercent)
    {
        int mkPercent = Convert.ToInt32(strPercent);

        decimal pMoney = moeny * (100 - mkPercent) / 100;

        return pMoney;
    }


    /// <summary>
    /// % 계산된 금액을 돌린다.
    /// </summary>
    /// <param name="moeny"></param>
    /// <param name="strPercent"></param>
    /// <returns></returns>
    public static decimal ToPercentMoney(this decimal moeny, string strPercent)
    {
        int mkPercent = Convert.ToInt32(strPercent);

        decimal pMoney = moeny * mkPercent / 100;

        return pMoney;
    }

    /// <summary>
    /// 소수점자래 버린 금액을 구한다.
    /// </summary>
    /// <param name="moeny"></param>
    /// <param name="strPercent"></param>
    /// <returns></returns>
    public static decimal ToTruncateMoney(this decimal money, int pos = 2)
    {
        int unit = 1;
        for( int i= 0;i< pos; i++)
        {
            unit = unit * 10;
        }

        decimal pMoney = Math.Truncate(money * unit) / unit;
        return pMoney;
    }

    /// <summary>
    /// 소수점아래 자리수를 구한다.
    /// </summary>
    /// <param name="moeny"></param>
    /// <param name="strPercent"></param>
    /// <returns></returns>
    public static int GetNumberCount(this decimal money)
    {
        try
        {
            decimal me = money % 1;

            int nCount = me.ToString().Length - 2;
            return nCount;
        }
        catch 
        {
            return 0;
        }
       
    }
}
