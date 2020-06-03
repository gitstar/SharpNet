using System;

public static class Int32Extensions
{
    /// <summary>
    /// CodeType 값을 4자리 문자렬로 변환한다.
    /// </summary>
    /// <param name="codetype">CodeType 값</param>
    /// <returns>문자렬 코드값</returns>
    public static string ToCodeString(this Enum codetype)
    {
        object obj = Convert.ChangeType(codetype, codetype.GetTypeCode());
        return string.Format("{0:D4}", (int)obj);
    }

   public static string ToCodeString(this int codeval)
    {
        return string.Format("{0:D4}", codeval);
    }

    /// <summary>
    /// 1자리 옹근수 인덱스 에 따른 게임이름 얻기.
    /// </summary>
    /// <param name="gameIndex"></param>
    /// <returns></returns>
    public static string ToGameName(this int gameIndex)
    {
        string gameName = string.Empty;
        switch (gameIndex)
        {
            case 1:
                gameName = "바다이야기";
                break;
            case 2:
                gameName = "오션";
                break;
            case 3:
                gameName = "손오공";
                break;
            case 4:
                gameName = "알라딘";
                break;
            default:
                gameName = "새게임";
                break;

        }

        return gameName;
    }
}
