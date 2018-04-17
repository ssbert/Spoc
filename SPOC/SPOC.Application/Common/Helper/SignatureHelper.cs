using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPOC.Common.Helper
{
    /// <summary>
    /// 接口签名类
    /// </summary>
    public class SignatureHelper
    {
        /// <summary>
        /// 签名秘钥
        /// </summary>
        private const string PrivateKey = "EB86F9BA59911A3229843644851562409E5FE43328D2DFEB480B86BC04334DB817238A4DBB68F265FAB63DB1663D265C3E75C9BF41E7426C7DB7233F0D5FCCFA";//app和服务器端共享一个key 去生成签名

        /// <summary>
        /// 计算参数签名
        /// </summary>
        /// <param name="parameters">请求参数集，所有参数必须已转换为字符串类型</param>
        /// <returns>签名</returns>
        public static string GetSignature(IDictionary<string, string> parameters)
        {
            // 先将参数以其参数名的字典序升序进行排序
            //IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            //移动端为降序排列 统一改为降序
            parameters = parameters.OrderByDescending(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
            IEnumerator<KeyValuePair<string, string>> iterator = parameters.GetEnumerator();

            // 遍历排序后的字典，将所有参数按"value"格式拼接在一起
            var basestring = new StringBuilder();
            while (iterator.MoveNext())
            {
                //string key = iterator.Current.Key;
                string value = iterator.Current.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    basestring.Append(value);
                }
            }
            basestring.Append(PrivateKey);

            // 使用SHA1对待签名串求签
            string res = basestring.ToString().Trim(',');
            System.Security.Cryptography.SHA1 encryptRes = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] byteArr = Encoding.UTF8.GetBytes(res);
            byteArr = encryptRes.ComputeHash(byteArr);
            string hashData = BitConverter.ToString(byteArr);
            hashData = hashData.Replace("-", "");

            return hashData;
        }
    }
}
