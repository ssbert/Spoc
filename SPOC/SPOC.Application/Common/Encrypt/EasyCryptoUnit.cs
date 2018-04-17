using System;

namespace SPOC.Common.Encrypt
{
    public class EasyCryptoUnit
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密结果</returns>
        public static string Encode(string str)
        {
            //生成随机数
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int randomNum = random.Next(10);
            string encoded = randomNum.ToString();
            for (int i = 0; i < str.Length; i++)
            {
                int asc = str[i] + randomNum;
                string strAsc = asc.ToString("x4");
                encoded += strAsc;
            }
            return encoded;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            //取得随机数,第一位为随机数
            int randomNum = int.Parse(str.Substring(0, 1));
            string decoded = "";
            for (int i = 1; i < str.Length; i += 4)
            {
                int charAsc = Convert.ToInt32(str.Substring(i, 4), 16);
                decoded += (char)(charAsc - randomNum);
            }

            return decoded;
        }
    }
}