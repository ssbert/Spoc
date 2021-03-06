using System;
using System.IO;
using System.Text;

namespace SPOC.Common.Helper.htmlparser
{
	public class EncodingGetterClass
	{
		// Constructors
		public EncodingGetterClass ()
		{
		}
		
		
		// Methods
		public static Encoding GetEncoding (string fileName)
		{
			return EncodingGetterClass.GetEncoding(fileName, Encoding.Default);
		}
		
		public static Encoding GetEncoding (FileStream stream)
		{
			return EncodingGetterClass.GetEncoding(stream, Encoding.Default);
		}
		
		public static Encoding GetEncoding (string fileName, Encoding defaultEncoding)
		{
			FileStream stream = new FileStream(fileName, FileMode.Open);
			Encoding encoding1 = EncodingGetterClass.GetEncoding(stream, defaultEncoding);
			stream.Close();
			return encoding1;
		}
		
		public static Encoding GetEncoding (FileStream stream, Encoding defaultEncoding)
		{
			Encoding encoding1 = defaultEncoding;
			if ((stream != null) && (stream.Length >= 2))
			{
				byte num1 = 0;
				byte num2 = 0;
				byte num3 = 0;
				byte num4 = 0;
				long offset = stream.Seek((long) 0, SeekOrigin.Begin);
				stream.Seek((long) 0, SeekOrigin.Begin);
				int value = stream.ReadByte();
				num1 = Convert.ToByte(value);
				num2 = Convert.ToByte(stream.ReadByte());
				if (stream.Length >= 3)
				{
					num3 = Convert.ToByte(stream.ReadByte());
				}
				if (stream.Length >= 4)
				{
					num4 = Convert.ToByte(stream.ReadByte());
				}
				if ((num1 == 0xfe) && (num2 == 0xff))
				{
					encoding1 = Encoding.BigEndianUnicode;
				}
				if (((num1 == 0xff) && (num2 == 0xfe)) && (num3 != 0xff))
				{
					encoding1 = Encoding.Unicode;
				}
				if (((num1 == 0xef) && (num2 == 0xbb)) && (num3 == 0xbf))
				{
					encoding1 = Encoding.UTF8;
				}
				stream.Seek(offset, SeekOrigin.Begin);
			}
			return encoding1;
		}
		
	}
}
