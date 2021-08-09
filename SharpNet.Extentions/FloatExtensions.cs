using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Extentions
{
    public static class FloatExtensions
    {
        public static string ToMoneyString(this float money, int pos = 2)
        {
            string retStr = Math.Round(money, pos).ToString();

            return retStr;
        }

        /// <summary>
        /// 소수점아래 자르기
        /// </summary>
        /// <param name="orginal"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float Truncate(this float orginal, int length = 2)
        {
            int k = 1;
            for (int i = 0; i < length; i++)
                k = k * 10;

            double val = Math.Truncate(orginal * k) / k;
            return val.ConvertTo<float>();
        }

        /// <summary>
        /// 소수점아래 자리수를 구한다.
        /// </summary>
        /// <param name="moeny"></param>
        /// <param name="strPercent"></param>
        /// <returns></returns>
        public static int GetNumberCount(this float money)
        {
            try
            {
                int nCount = money.ToString().Length - money.ToString().IndexOf(".") - 1;
                return nCount;
            }
            catch
            {
                return 0;
            }

        }
    }
}
