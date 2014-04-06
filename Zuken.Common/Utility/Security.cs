using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;

namespace Zuken.Common.Utility
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public class Security
    {
        public static string EncryptText(string text)
        {
            string str = "0e3f96f8-daf1-42d2-b55b-8d335f0a1af5";
            string strPassword = "{" + str + "}";
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(strPassword, new byte[]
			{
				73,
				118,
				97,
				110,
				32,
				77,
				101,
				100,
				118,
				101,
				100,
				101,
				118
			});
            MemoryStream memoryStream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = passwordDeriveBytes.GetBytes(32);
            rijndael.IV = passwordDeriveBytes.GetBytes(16);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            byte[] inArray = memoryStream.ToArray();
            return Convert.ToBase64String(inArray);
        }
        public static string DecryptText(string text)
        {
            string str = "0e3f96f8-daf1-42d2-b55b-8d335f0a1af5";
            string strPassword = "{" + str + "}";
            byte[] array = Convert.FromBase64String(text);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(strPassword, new byte[]
			{
				73,
				118,
				97,
				110,
				32,
				77,
				101,
				100,
				118,
				101,
				100,
				101,
				118
			});
            MemoryStream memoryStream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = passwordDeriveBytes.GetBytes(32);
            rijndael.IV = passwordDeriveBytes.GetBytes(16);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.Close();
            byte[] bytes = memoryStream.ToArray();
            return Encoding.Unicode.GetString(bytes);
        }
        public static string ThreeDesEncryptHEX(string input)
        {
            string result;
            if (string.IsNullOrEmpty(input))
            {
                result = string.Empty;
            }
            else
            {
                string text = "";
                TripleDES tripleDES = TripleDES.Create();
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.PKCS7;
                byte[] rgbKey = new byte[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					1,
					2,
					3,
					4,
					5,
					6,
					1,
					2,
					3,
					4,
					5,
					6,
					1,
					2,
					3,
					4,
					5,
					6
				};
                byte[] rgbIV = new byte[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					1,
					2
				};
                ICryptoTransform transform = tripleDES.CreateEncryptor(rgbKey, rgbIV);
                byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(input);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                byte[] array = memoryStream.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    text += array[i].ToString("x").PadLeft(2, '0');
                }
                result = text;
            }
            return result;
        }
        public static string ThreeDesDecryptHEX(string input)
        {
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Mode = CipherMode.CBC;
            tripleDES.Padding = PaddingMode.PKCS7;
            byte[] rgbKey = new byte[]
			{
				1,
				2,
				3,
				4,
				5,
				6,
				1,
				2,
				3,
				4,
				5,
				6,
				1,
				2,
				3,
				4,
				5,
				6,
				1,
				2,
				3,
				4,
				5,
				6
			};
            byte[] rgbIV = new byte[]
			{
				1,
				2,
				3,
				4,
				5,
				6,
				1,
				2
			};
            ICryptoTransform transform = tripleDES.CreateDecryptor(rgbKey, rgbIV);
            if (input.Length <= 1)
            {
                throw new Exception("encrypted HEX string is too short!");
            }
            byte[] array = new byte[input.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
            }
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            return Encoding.GetEncoding("GB2312").GetString(memoryStream.ToArray());
        }
    }
}
