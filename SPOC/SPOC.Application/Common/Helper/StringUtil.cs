using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SPOC.Common.Helper
{
    /// <summary>
    /// StringUtil 的摘要说明。
    /// </summary>
    public class StringUtil
    {
        public StringUtil()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        //========== 注释：NumberToBigNumber 只能支持到百位，用函数FloatToHZString替代 =====

        #region NumberToBigNumber

        /// <summary>
        /// 小写数字转换成大写的数字如1转换为一,2转换成二
        /// </summary>
        /// <param name="n">小写数字</param>
        /// <returns>大写数字字符串</returns>
        public static string NumberToBigNumber(int n)
        {
            if (n == 0) return "零";
            if (n == 10) return "十";
            if (n == 100) return "一百";

            string[] numberStr = new string[] {"", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十"};
            if (n > 0 && n < 10)
                return numberStr[n];

            if (n > 10 && n < 20)
            {
                int outInt;
                int divResult;
                divResult = Math.DivRem(n, 10, out outInt);
                return "十" + numberStr[outInt];
            }

            if (n >= 20 && n < 100)
            {
                int outInt;
                int divResult;
                divResult = Math.DivRem(n, 10, out outInt);
                return numberStr[divResult] + "十" + numberStr[outInt];
            }

            if (n > 100 && n < 999)
            {
                int outInt;
                int divResult;
                divResult = Math.DivRem(n, 100, out outInt);
                return numberStr[divResult] + "百" + NumberToBigNumber(outInt);
            }
            return "";
        }

        #endregion NumberToBigNumber

        /// <summary>
        /// 在字符串的两边加上单引号'
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>格式化的字符串</returns>
        /// <remarks>该函数做两件事：
        /// 1. 将字符串内的单引号变为两个单引号；
        /// 2. 前后加上单引号；
        /// </remarks>
        public static string QuotedToStr(string str)
        {
            return "'" + str.Replace("'", "''") + "'";
        }

        /// <summary>
        /// 对字符串格式化为符合SQL语句的带引号的字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>格式化的字符串</returns>
        /// <remarks>该函数做两件事：
        /// 1. 将字符串内的单引号变为两个单引号；
        /// 2. 前后加上单引号；
        /// </remarks>
        public static string QuotedToDBStr(string str)
        {
            string returnStr = "'" + str.Replace("'", "''") + "'";
            //returnStr = returnStr.Replace("[", "[[]");
            return returnStr;
        }

        /// <summary>
        /// 对用逗号分隔的字符串格式化为带引号的字符串(用在in语句中)
        /// </summary>
        /// <param name="str">原始字符串(逗号分隔)</param>
        /// <returns>格式化的字符串</returns>
        /// <remarks>该函数做两件事：
        /// 1. 将字符串内的单引号变为两个单引号；
        /// 2. 前后加上单引号；
        /// </remarks>
        public static string QuotedToStrs(string str)
        {
            string newStrs = "";
            string[] arrStrs = str.Split(',');
            for (int i = 0; i < arrStrs.Length; i++)
            {
                newStrs = newStrs + "," + StringUtil.QuotedToStr(arrStrs[i]);
            }
            newStrs = newStrs.Trim(',');
            return newStrs;
        }

        /// <summary>
        /// 功能描述：将字符串转换成参数。
        /// </summary>
        /// <param name="str">需转换的字符串</param>
        /// <returns></returns>
        public static string paramChg(string str)
        {
            str = "@" + str;
            return str;
        }

        /// <summary>
        /// 当没有Html标记时把回车换成br
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string ReplaceEnter2BrWhenNoHtml(string sText)
        {
            if (sText.IndexOf("<") == -1 || sText.IndexOf(">") == -1)
            {
                sText = sText.Replace("\r\n", "<br>");
            }
            return sText;
        }

        /// <summary>
        /// 功能描述：从字符串中的尾部删除指定的字符串。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="removedString">移除字符串</param>
        /// <returns>留下的字符串</returns>
        public static string Remove(string sourceString, string removedString)
        {
            try
            {
                if (sourceString.IndexOf(removedString) == -1)
                    throw new Exception("原字符串中不包含移除字符串！");
                string result = sourceString;
                int LengthOfsourceString = sourceString.Length;
                int LengthOfremovedString = removedString.Length;
                int startIndex = LengthOfsourceString - LengthOfremovedString;
                string sourceStringSub = sourceString.Substring(startIndex);
                if (sourceStringSub.ToUpper() == removedString.ToUpper())
                {
                    result = sourceString.Remove(startIndex, LengthOfremovedString);
                }
                return result;
            }
            catch
            {
                return sourceString;
            }

        }

        /// <summary>
        /// 功能描述：从字符串中的指定位置删除指定的字符串。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="removedString">移除字符串</param>
        /// <returns>留下的字符串</returns>
        public static string StrRemove(string sourceString, string removedString)
        {
            try
            {
                if (sourceString.IndexOf(removedString) == -1)
                    throw new Exception("原字符串中不包含移除字符串！");
                string result = sourceString;
                int LengthOfremovedString = removedString.Length;
                if (sourceString.IndexOf(removedString) > 0)
                {
                    int startIndex = sourceString.IndexOf(removedString);
                    result = sourceString.Remove(startIndex, LengthOfremovedString);
                }
                return result;
            }
            catch
            {
                return sourceString;
            }

        }

        /// <summary>
        /// 功能描述：获取拆分符右边的字符串。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>右边的字符串</returns>
        public static string RightSplit(string sourceString, char splitChar)
        {
            string result = null;
            string[] tempStr = sourceString.Split(splitChar);
            if (tempStr.Length > 0)
            {
                result = tempStr[tempStr.Length - 1].ToString();
            }
            return result;

        }

        /// <summary>
        /// 功能描述：获取拆分符左边的字符串。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>左边的字符串</returns>
        public static string LeftSplit(string sourceString, char splitChar)
        {
            string result = null;
            string[] tempStr = sourceString.Split(splitChar);
            if (tempStr.Length > 0)
            {
                result = tempStr[0].ToString();
            }
            return result;

        }

        /// <summary>
        /// 功能描述：去掉最后一个逗号后面的字符串。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>左边的字符串</returns>
        public static string DelLsatComma(string sourceString)
        {
            if (sourceString.IndexOf(",") == -1)
            {
                return sourceString;
            }
            return sourceString.Substring(0, sourceString.LastIndexOf(","));

        }

        /// <summary>
        /// 进行解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(string url)
        {
            string obj = url;
            obj = System.Web.HttpUtility.UrlDecode(obj);
            return obj;
        }

        /// <summary>
        /// 功能描述：删除不可见字符。
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    strBuilder.Append(sourceString[i].ToString());
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 功能描述：获取数组元素的合并字符串。
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <returns></returns>
        public static string GetArrayString(string[] stringArray)
        {
            string totalString = null;
            for (int i = 0; i < stringArray.Length; i++)
            {
                totalString = totalString + stringArray[i];
            }
            return totalString;
        }

        /// <summary>
        /// 功能描述：分割数组，并以splitStr相连。
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <returns></returns>
        public static string GetArrayStringJoinStr(string[] stringArray, string splitStr)
        {
            string totalString = "";
            int len = stringArray.Length;
            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrEmpty(stringArray[i]))
                {
                    totalString = totalString + stringArray[i] + (i == len - 1 ? "" : splitStr);
                }
            }
            return totalString;
        }

        /// <summary>
        /// 功能描述：获取某一字符串在字符串数组中出现的次数。
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="findString">某一字符串</param>
        /// <returns></returns>
        public static int GetStringCount(string[] stringArray, string findString)
        {
            int count = 0;
            string totalString = GetArrayString(stringArray);
            int findStringLength = findString.Length;
            string subString = totalString;
            while (subString.IndexOf(findString) >= 0)
            {
                subString = subString.Substring(subString.IndexOf(findString) + findStringLength);
                count += 1;
            }
            return count;
        }

        /// <summary>
        /// 功能描述：获取某一字符串在字符串中出现的次数。
        /// </summary>
        /// <param name="sourceString">字符串</param>
        /// <param name="findString">某一字符串</param>
        /// <returns></returns>
        public static int GetStringCount(string sourceString, string findString)
        {
            int count = 0;
            int findStringLength = findString.Length;
            string subString = sourceString;
            while (subString.IndexOf(findString) >= 0)
            {
                subString = subString.Substring(subString.IndexOf(findString) + findStringLength);
                count += 1;
            }
            return count;
        }

        /// <summary>
        /// 功能描述：截取从startString开始到原字符串结尾的所有字符。
        /// </summary>
        /// <param name="sourceString">字符串</param>
        /// <param name="startString">某一字符串</param>
        /// <returns></returns>
        public static string GetSubstring(string sourceString, string startString)
        {
            int startIndex = sourceString.IndexOf(startString);
            if (startIndex > 0)
                return sourceString.Substring(startIndex);
            return sourceString;
        }

        /// <summary>
        /// 功能描述：按字节数取出字符串的长度。
        /// </summary>
        /// <param name="sourceString">要计算的字符串</param>
        /// <returns>字符串的字节数</returns>
        public static int GetByteCount(string sourceString)
        {
            int itnCharCount = 0;
            for (int i = 0; i < sourceString.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(sourceString.Substring(i, 1)) == 3)
                {
                    itnCharCount = itnCharCount + 2;
                }
                else
                {
                    itnCharCount = itnCharCount + 1;
                }
            }
            return itnCharCount;
        }

        /// <summary>
        /// 功能描述：按字节数要在字符串的位置。
        /// </summary>
        /// <param name="intIns">字符串的位置</param>
        /// <param name="strTmp">要计算的字符串</param>
        /// <returns>字节的位置</returns>
        public static int GetByteIndex(int intIns, string strTmp)
        {
            int intReIns = 0;
            if (strTmp.Trim() == "")
            {
                return intIns;
            }
            for (int i = 0; i < strTmp.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(strTmp.Substring(i, 1)) == 3)
                {
                    intReIns = intReIns + 2;
                }
                else
                {
                    intReIns = intReIns + 1;
                }
                if (intReIns >= intIns)
                {
                    intReIns = i + 1;
                    break;
                }
            }
            return intReIns;
        }

        /// <summary>
        /// 返回路径分割符号"\"最后一个字符串。
        /// </summary>
        /// <param name="sourceStr">原字符串</param>
        /// <param name="splitChar">分割符号</param>
        /// <returns></returns>
        public static string getLastStr(string sourceStr, char splitChar)
        {
            string[] strArr = sourceStr.Split(splitChar);
            string lastStr = strArr[strArr.Length - 1];
            if (lastStr == "")
                return strArr[strArr.Length - 2];
            return lastStr;
        }

        /// <summary>
        /// 获取路径最后一个分隔符"\"左边的全部字符串
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="splitChar">分隔符</param>
        /// <returns></returns>
        public static string getLeftStr(string sourceStr, char splitChar)
        {
            string[] strArr = sourceStr.Split(splitChar);
            int length = sourceStr.Length - getLastStr(sourceStr, '\\').Length;
            string leftStr = sourceStr.Substring(0, length);
            return leftStr;
        }

        /// <summary>
        /// 提交支付信息到支付平台转义特殊字符 逗号替代
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSymbols(string str)
        {
            str = str.Replace("+", ",");
            str = str.Replace("/", ",");
            str = str.Replace("?", ",");
            str = str.Replace("%", ",");
            str = str.Replace("#", ",");
            str = str.Replace("&", ",");
            str = str.Replace("=", ",");
            return str;
        }
        /// <summary>
        /// 转UNICODE字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UniCode(string str)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    //将中文字符转为10进制整数，然后转为16进制unicode字符
                    outStr += "//u" + ((int)str[i]).ToString("x");
                }
            }

            return outStr;
        }
        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2) r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            return r;
        }
        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGb2312(string str)
        {
            string r = "";
            MatchCollection mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] bts = new byte[2];
            foreach (Match m in mc)
            {
                bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                r += Encoding.Unicode.GetString(bts);
            }
            return r;
        }

        public static string SqlEncode(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }

        /// <summary>
        /// 解码字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            str = str.Replace("<br>", "\n");
            //str = str.Replace("&gt;", ">");
            //str = str.Replace("&lt;", "<");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&quot;", "\"");
            return str;
        }

        /// <summary>
        /// 文本域的html编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(string str)
        {
            str = str.Replace("'", "''");
            str = str.Replace("\"", "&quot;");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br>");
            str = str.Replace("//", "／／");
            str = str.Replace("http", "ｈｔｔｐ");
            str = str.Replace("js", "ｊｓ");
            str = str.Replace("gif", "ｇｉｆ");
            str = str.Replace("com", "ｃｏｍ");
            str = str.Replace(".", "．");
            return str;
        }
        public static string CodeEncode(string str)
        {
            str = str.Replace("'", "''");
            str = str.Replace("\"", "&quot;");
           // str = str.Replace(" ", "&nbsp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br>");
            str = str.Replace("//", "／／");
            str = str.Replace("http", "ｈｔｔｐ");
            str = str.Replace("js", "ｊｓ");
            str = str.Replace("gif", "ｇｉｆ");
            str = str.Replace("com", "ｃｏｍ");
            str = str.Replace(".", "．");
            return str;
        }
        /// <summary>
        /// 文本域的html解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(string str)
        {
            str = str.Replace("&gt;", ">");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("''", "'");
            str = str.Replace("／／", "//");
            str = str.Replace("ｈｔｔｐ", "http");
            str = str.Replace("ｊｓ", "js");
            str = str.Replace("ｇｉｆ", "gif");
            str = str.Replace("ｃｏｍ", "com");
            str = str.Replace("．", ".");
            str = str.Replace("<br>", "\n");
            str = str.Replace("<br/>", "\n");
            return str;
        }

        /// <summary>
        /// 出去空格，并见对尖括号内的空格加入。
        /// </summary>
        /// <param name="contStr"></param>
        /// <returns></returns>
        public static string ReplaceStr(string contStr)
        {
            contStr = System.Text.RegularExpressions.Regex.Replace(contStr, "\\s", "&nbsp;");
            int y, z;
            z = 0;
            int i = 0;
            do
            {
                y = contStr.IndexOf("<", z);
                if (y >= 0)
                {
                    z = contStr.IndexOf(">", y);
                    if (z >= 0)
                    {
                        i += contStr.Substring(y, z - y + 1).Replace("&nbsp;", " ").Length + 4; //统计超级链接标签占用的字符数
                        string tStr1, tStr2;
                        tStr1 = contStr.Substring(0, z + 1);
                        tStr2 = contStr.Substring(z + 1);
                        contStr =
                            tStr1.Replace(tStr1.Substring(y, z - y + 1),
                                tStr1.Substring(y, z - y + 1).Replace("&nbsp;", " ")) + tStr2;
                    }
                    else
                    {

                        z = y + 1;
                        if (z > contStr.Length - 1)
                            break;
                    }
                }
            } while (y >= 0 && z <= contStr.Length - 1);
            return contStr;
        }

        ///   <summary> 
        ///   将指定字符串按指定长度进行截取并加上指定的后缀
        ///   </summary> 
        ///   <param   name= "oldStr "> 需要截断的字符串 </param> 
        ///   <param   name= "maxLength "> 字符串的最大长度 </param> 
        ///   <param   name= "endWith "> 超过长度的后缀 </param> 
        ///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns> 
        public static string StringTruncat(string oldStr, int maxLength, string endWith)
        {
            //判断原字符串是否为空
            if (string.IsNullOrEmpty(oldStr))
                return oldStr;


            //返回字符串的长度必须大于1
            if (maxLength < 1)
                throw new Exception("返回的字符串长度必须大于[0] ");


            //判断原字符串是否大于最大长度
            if (oldStr.Length > maxLength)
            {
                //截取原字符串
                string strTmp = oldStr.Substring(0, maxLength);


                //判断后缀是否为空
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }

            return oldStr;
        }

        /// <summary>
        /// 去html
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(string Htmlstring) //去除HTML标记
        {
            if (string.IsNullOrEmpty(Htmlstring))
                return "";

            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        /// <summary>
        /// 取后缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExtName(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = "";
            }
            string extName = str;
            if (str.IndexOf('.') > -1)
            {
                int index = str.LastIndexOf('.');
                extName = str.Substring(index + 1, str.Length - index - 1).ToLower();
            }
            return extName;
        }

        /// <summary>
        /// 文件名称编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeFileName(string str)
        {
            string[] str1 = str.Split('/');
            if (str1.Length > 0)
            {
                str1[str1.Length - 1] = System.Web.HttpUtility.UrlEncode(str1[str1.Length - 1]);
                string str2 = string.Empty;
                for (int i = 0; i < str1.Length; i++)
                {
                    str2 += "/" + str1[i];
                }
                return str2.TrimStart('/');
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 取文件名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetFileName(string str)
        {
            string[] str1 = str.Split('/');
            if (str1.Length > 0)
            {
                return str1[str1.Length - 1];
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 分隔字符串为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="sSplitStr">分隔符</param>
        /// <returns>经分隔的字符串数组</returns>
        /// <remarks>.net自带的split方法只能用一个字符分隔</remarks>
        public static string[] Split(string str, string sSplitStr)
        {
            int nPos;
            string[] arrStr;
            ArrayList arrList = new ArrayList();
            nPos = -1;
            nPos = str.IndexOf(sSplitStr);
            while (nPos >= 0)
            {
                arrList.Add(str.Substring(0, nPos));
                str = str.Substring(nPos + sSplitStr.Length);
                nPos = str.IndexOf(sSplitStr);
            }
            arrList.Add(str);
            arrStr = new string[arrList.Count];
            arrStr = (string[]) arrList.ToArray(Type.GetType("System.String"));
            arrList = null;
            return arrStr;
        }

        /// <summary>
        /// 数组字符串转List
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> GetArrayToList(string str)
        {
            string str1 = str;
            str1 = str1.Replace("\\", "/");
            str1 = str1.Replace("|", "/");
            str1 = str1.Replace(",", "/");
            str1 = str1.Trim('/');
            List<string> list = str1.Split('/').ToList();
            return list;
        }

        /// <summary>
        /// 判断一个字符串是否为数字串
        /// </summary>
        /// <param name="s">待判断的字符串</param>
        /// <returns>是否数字串</returns>
        public static bool IsNumber(string s)
        {
            if (s != null)
                return Regex.IsMatch(s.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 通过自动编号规则获取下一个编号
        /// </summary>
        /// <param name="maxCode"></param>
        /// <returns></returns>
        public static string GetNextCodeByAuto(string maxCode)
        {
            string stringPart = "";
            string numberPart = maxCode;
            for (int i = maxCode.Length - 1; i >= 0; i--)
            {
                int asciiNumber = (int) maxCode.Substring(i, 1).ToCharArray()[0];
                if (asciiNumber < 48 || asciiNumber > 58)
                {
                    stringPart = maxCode.Substring(0, i + 1);
                    numberPart = maxCode.Substring(i + 1);
                    break;
                }
            }
            string newCode = "";
            if (string.IsNullOrEmpty(numberPart)) //如果得到的课程体系编号不包含数字,则返回00000
                numberPart = "00000";
            if (numberPart != "")
                newCode = stringPart + (ToDecimal(numberPart, 0) + 1).ToString("0").PadLeft(numberPart.Length, '0');
            else
                newCode = "";
            return newCode;
        }

        public static Decimal ToDecimal(object objDecimal, Decimal defaultValue)
        {
            if (objDecimal == null)
                return defaultValue;
            try
            {
                return Convert.ToDecimal(objDecimal);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 查询Html中的文本内容，然后把空格替换为Html中的空格标记
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string ReplaceSpaces2Html(string sText)
        {
            return ReplaceSpaces2Html(sText, false);
        }

        /// <summary>
        /// 查询Html中的文本内容，然后把空格替换为Html中的空格标记
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="isForWord"></param>
        /// <returns></returns>
        public static string ReplaceSpaces2Html(string sText, bool isForWord)
        {
            string result = "";
            try
            {
                if (sText.Contains(">") && sText.Contains("<"))
                {
                    StringBuilder strBuilder = new StringBuilder();
                    int startIndex = 0;
                    string str = null;
                    Regex regex = new Regex(@"<[^>]*>");
                    Match prMatch = null;
                    MatchCollection matchCollection = regex.Matches(sText);
                    foreach (Match match in matchCollection)
                    {
                        if (!match.Success)
                            continue;

                        if (startIndex < match.Index)
                        {
                            str = sText.Substring(startIndex, match.Index - startIndex);
                            if (prMatch != null)
                            {
                                string prMatchValue = prMatch.Value.TrimStart().TrimStart('<').TrimStart();
                                if (prMatchValue.StartsWith("table") || prMatchValue.StartsWith("tr") ||
                                    prMatchValue.StartsWith("/td") || prMatchValue.StartsWith("/tr"))
                                {
                                    strBuilder.Append(str);
                                }
                                else
                                {
                                    if (!isForWord)
                                        strBuilder.Append(ReplaceTextSpaces2Html(str));
                                    else
                                        strBuilder.Append(ReplaceTextSpaces2HtmlForWord(str));
                                }
                            }
                            else
                            {
                                if (!isForWord)
                                    strBuilder.Append(ReplaceTextSpaces2Html(str));
                                else
                                    strBuilder.Append(ReplaceTextSpaces2HtmlForWord(str));
                            }
                            strBuilder.Append(match.Value);
                        }
                        else
                        {
                            strBuilder.Append(match.Value);
                        }
                        startIndex = match.Index + match.Value.Length;
                        prMatch = match;
                    }

                    if (startIndex <= sText.Length - 1)
                    {
                        str = sText.Substring(startIndex);
                        if (!isForWord)
                            strBuilder.Append(ReplaceTextSpaces2Html(str));
                        else
                            strBuilder.Append(ReplaceTextSpaces2HtmlForWord(str));
                    }
                    result = strBuilder.ToString();
                }
                else
                {
                    if (!isForWord)
                        result = ReplaceTextSpaces2Html(sText);
                    else
                        result = ReplaceTextSpaces2HtmlForWord(sText);
                }
            }
            catch
            {
                result = sText;
            }

            return result;
        }

        /// <summary>
        /// 把普通文本格式的空格转为Html格式的空格,制表符以四个空格计算
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        private static string ReplaceTextSpaces2Html(string sText)
        {
            if (sText.IndexOf("<") == -1 || sText.IndexOf(">") == -1)
            {
                sText = sText.Replace("\r\n", "<br>");
                sText = sText.Replace("\n\r", "<br>");
                sText = sText.Replace("\r", "<br>");
                sText = sText.Replace("\n", "<br>");
                sText = sText.Replace("\t", " &nbsp;&nbsp;&nbsp;"); //第一个空格不进行转换
                sText = sText.Replace("　", " &nbsp;&nbsp;&nbsp;");
                //处理空格,如果有1个空格不进行处理, 连续的多个空格第一个不进行转换
                //sText = sText.Replace(" ", "&nbsp;");
                Regex regex = new Regex("\\s{2}");
                MatchEvaluator evaluator = new MatchEvaluator(ReplaceSpaceString);
                sText = regex.Replace(sText, evaluator);
            }
            return sText;
        }

        private static string ReplaceTextSpaces2HtmlForWord(string sText)
        {
            if (sText.IndexOf("<") == -1 || sText.IndexOf(">") == -1)
            {
                sText = sText.Replace("\r\n", "<br>");
                sText = sText.Replace("\n\r", "<br>");
                sText = sText.Replace("\r", "<br>");
                sText = sText.Replace("\n", "<br>");
                sText = sText.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;"); //第一个空格不进行转换
                sText = sText.Replace("　", "&nbsp;&nbsp;&nbsp;&nbsp;");

                sText = sText.Replace(" ", "&nbsp;");
                sText = sText.Replace("&nbsp;&nbsp;", "&nbsp;");
            }
            return sText;
        }

        /// <summary>
        /// 获取字符串中连续空格的替换字符串, 只用于方法 ReplaceSpaces2Html 的委托调用
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string ReplaceSpaceString(Match match)
        {
            if (!match.Success)
            {
                return match.Value;
            }

            int length = match.Value.Length;
            return " " + RepeatChar("&nbsp;", length - 1);
        }

        /// <summary>
        /// 得到一个字符重复多遍后生成的字符串
        /// </summary>
        /// <param name="c">字符</param>
        /// <param name="count">重复次数</param>
        /// <returns>结果字符串</returns>
        public static string RepeatChar(char c, int count)
        {
            string str = "";
            /*
			for(int i = 0; i < count; i++)
			{
				str += c.ToString();
			}
             * */
            str = new string(c, count);
            return str;


        }

        /// <summary>
        /// 得到一个字符串重复多遍后生成的字符串
        /// </summary>
        /// <param name="c">字符串</param>
        /// <param name="count">重复次数</param>
        /// <returns>结果字符串</returns>
        public static string RepeatChar(string c, int count)
        {
            /*
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str += c;
            }
            * */
            int len = c.Length*count;
            StringBuilder sb = new StringBuilder(len, len);
            for (int i = 0; i < count; i++)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字符转ASCII码：
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int ChrToAsc(string character)
        {
            if (character.Length == 1)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                int intAsciiCode = (int) asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }

        }

        /// <summary>
        /// 把数组转为字符串
        /// </summary>
        /// <param name="arr">字符串数组</param>
        /// <param name="spChar">分隔字符串</param>
        /// <returns>字符串</returns>
        public static string ArrayToDbString(string[] arr, string spChar)
        {
            if (arr.Length > 0)
            {
                StringBuilder sb = new StringBuilder("'" + arr[0] + "'");
                for (int i = 1; i < arr.Length; i++)
                {
                    sb.Append(spChar + "'" + arr[i].Replace("'", "''") + "'");
                }
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// ASCII码转字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string AscToChr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                byte[] byteArray = new byte[] {(byte) asciiCode};
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

      

    }

    public static class StringOpt
    {
        public static string TrimEmptyChar(this string obj)
        {
            string str = string.Empty;
            if (string.IsNullOrWhiteSpace(obj))
            {
                return str;
            }
            else
            {
                str = obj.Trim();
                return str;
            }
        }

        public static bool IsGuid(this string obj)
        {
            Guid gid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(obj) && Guid.TryParse(obj, out gid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Guid TryParseGuid(this string obj)
        {
            Guid gid = Guid.Empty;
            if (string.IsNullOrWhiteSpace(obj))
            {
               return gid;
            }

            Guid.TryParse(obj, out gid);
            return gid;
        }

        public static DateTime TryParseDateTime(this string obj)
        {
            DateTime dt = DateTime.Now;
            if (string.IsNullOrWhiteSpace(obj))
            {
                return dt;
            }
            DateTime.TryParse(obj, out dt);
            return dt;
        }

        public static int TryParseInt(this string obj)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(obj))
            {
                return i;
            }
            int.TryParse(obj,out i);
            return i;
        }
        
    }
}