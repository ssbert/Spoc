using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SPOC.Common.Encrypt
{
    /// <summary>
    /// DES加密/解密类。
    /// </summary>
    public class DESEncrypt
    {
        public DESEncrypt()
        {
        }

        #region ========加密======== 

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "NewvSoft");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密======== 


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "NewvSoft");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string text, string sKey)
        {
            try
            {


                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int len;
                len = text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                des.Key =
                    ASCIIEncoding.ASCII.GetBytes(
                        System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                            .Substring(0, 8));
                des.IV =
                    ASCIIEncoding.ASCII.GetBytes(
                        System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                            .Substring(0, 8));
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());

            }
            catch
            {
                return text;
            }
        }

        #endregion


    }
    public class sha1Encrypt
    {
        /// <summary>
        /// 计算hash字符值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getSHA1Value(string str)
        {
            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;
            SHA1CryptoServiceProvider oSHA1Hasher = new SHA1CryptoServiceProvider();
            arrbytHashValue = oSHA1Hasher.ComputeHash(Encoding.Unicode.GetBytes(str));
            strHashData = System.BitConverter.ToString(arrbytHashValue);
            strHashData = strHashData.Replace("-", "");
            strResult = strHashData;
            return strResult;
        }

        #region 移动设备加解密方法

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AirEncode(string str)
        {
            string a_z = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random objRandom = new Random();
            double ran = Math.Ceiling(objRandom.NextDouble() * 10);
            string temp = a_z.Length + "@#$" + a_z.Length * ran;
            for (int j = 0; j < str.Length; j++)
            {
                string temp2 = "%" + str.Substring(j, 1);
                for (int i = 0; i < a_z.Length; i++)
                {
                    if (str.Substring(j, 1) == a_z.Substring(i, 1))
                    {
                        temp2 = "%" + ((i + 1) * ran);
                        break;
                    }
                }
                temp += temp2;
            }
            return temp;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AirDecode(string str)
        {
            string a_z = "abcdefghijklmnopqrstuvwxyz0123456789";
            string[] arr = str.Split('%');
            string[] arr1 = arr[0].Split(new string[] { "@#$" }, StringSplitOptions.None);
            double ran = int.Parse(arr1[1]) / int.Parse(arr1[0]);
            string temp = "";
            int outresult = 0;
            for (int i = 1; i < arr.Length; i++)
            {
                string temp2 = arr[i];
                int result = int.TryParse(temp2, out outresult) ? int.Parse(arr[i]) : 0;
                if (result > 0)
                {
                    temp2 = a_z.Substring(Convert.ToInt32(result / ran - 1), 1);
                }
                temp += temp2;
            }
            return temp;
        }



        #endregion



        #region RSA 加密解密

        #endregion
    }


    public class RESEncript
    {

        private static RSAParameters rsap = new RSAParameters()
        {
            Modulus = Convert.FromBase64String(@"sB6+4rtO2sYeIZ8kJGGM647PIm+dJkwvSPNWcQ01D2cwPjIGV2c41h39FjYuzgAKzrIFjSvuBpG4y/PFEHuN+  
                                         LackSt6MU7qcbs7lzub8V97XZ5fddPaq/GWXo9mrIMMFDMW7z88WrukLGTvwkqySPBemc22rjua1uTR3azae7U="),
            Exponent = Convert.FromBase64String(@"AQAB"),
            P = Convert.FromBase64String(@"8yUCFVCufr3z2LDAwHaUO4r3na3WZqhAb3J7aXv/rj9UEXQWwZoG8IbUzV2fUhMXjnFXyrRSqywWdpxeE6oLWw=="),
            Q = Convert.FromBase64String(@"uW6NlpzkBl4Do7K4RUDCsZ9uiVqnU0cbm7JVuygWJts+pu1ho5s0auUekQy5al6p4xifjWIcCsLvPxsLuWISLw=="),
            DP = Convert.FromBase64String(@"rDsf0ad4I3E8hNcXgn28nLzgj8Hu6ILwOcGXZ+4c+/oB++cGo5cOqVxo6xwRWhsKCa2B6aV4FaZCNzymazl9lw=="),
            DQ = Convert.FromBase64String(@"dVVT+FKMIs9IZEPJP+DrkTM94WHgcNyUxp9Aii2iXrHqYfvhBYJG18Dk54lypbECtLU2+GJ1NgYFFxxI/ePldw=="),
            InverseQ = Convert.FromBase64String(@"z8qRY0+yyfZFNFPMtlTumpYyCXUbK+GpWnFp2hOyTABya/h7g4DCRE6iO9UZKgW4paB5K75mJwdBgVib5NgFiQ=="),
            D = Convert.FromBase64String(@"W1ZWoLeLWaJNlho2YDfHIZLakX1Y/reb/jVUqySyU96sAlVnPITn0QOUcaR/+Y3EDRX+EwypUPbZ48v0c2vgYDHwIb  
                                   rIbsEyN+vHoUNJ319R5kUZ8Wlfw/w6/6BSclqbWQ8OdSj1cKwx/EEJh4iipqJ8HBTsmoT0anQHP/jdybE=")
        };

        public static string GetEncriptKey()
        {

            return RESEncript.rsap.Exponent.ToHexEncode();
        }

        public static string GetEncriptModulusKey()
        {

            return RESEncript.rsap.Modulus.ToHexEncode();
        }

        public static RSAParameters GetRSAParameters()
        {

            return rsap;
        }

        public static string RSAEncrypt(string publicKey, string content)
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);// RSACryptoServiceProvider();  

            rsa.ImportParameters(rsap);//导入公钥  

            byte[] result = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(result);
        }

        public static string RSADecrypt(string privateKey, string content)
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            rsa.ImportParameters(rsap);

            var result = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// 对数据进行解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        public static string UserLoginRSADecrypt(string content, bool DoOAEPPadding, System.Web.Mvc.Controller ctl)
        {
            try
            {
                byte[] DataToDecrypt = content.HexDecode();
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.ImportParameters(rsap);


                // return   RSA.Decrypt(DataToDecrypt, DoOAEPPadding).GetString();
                return ctl.Server.UrlDecode(RSA.Decrypt(DataToDecrypt, DoOAEPPadding).GetString());

            }

            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

    }

    public static class ByteExt
    {

        public static string ToHexEncode(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
        public static byte[] HexDecode(this string me)
        {
            string s = me.Length % 2 == 1 ? "0" + me : me;
            byte[] data = new byte[s.Length / 2];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = byte.Parse(s.Substring(i + i, 2), NumberStyles.HexNumber);
            }
            return data;
        }
        public static string GetString(this byte[] content, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;
            return encoding.GetString(content);
        }
    }
}