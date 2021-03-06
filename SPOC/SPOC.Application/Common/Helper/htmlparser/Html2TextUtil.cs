using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SPOC.Common.Helper.htmlparser
{
	public class Html2TextUtil
	{
		// Constructors
		public Html2TextUtil ()
		{
		}
		
		
		// Methods
		public static string ConvertHtml2TxtFile (string sourceFileName, string distFileName)
		{
			StreamWriter writer1 = null;
			try
			{
				Encoding encoding1 = EncodingGetterClass.GetEncoding(sourceFileName);
				StreamReader reader1 = new StreamReader(System.IO.File.OpenRead(sourceFileName), encoding1);
				writer1 = new StreamWriter(System.IO.File.Create(distFileName), encoding1);
				string sContent = reader1.ReadToEnd();
				reader1.Close();
				HtmlParser parser1 = new HtmlParser();
				parser1.Parse(sContent);
                ContextStack elementPStack = new ContextStack(); //用于记录对标签“P”的过滤信息
				long num1 = parser1.GetElementCount();
				bool flag1 = false;
				for (int nIndex = 0;nIndex < num1; nIndex++)
				{
					string format;
					long arg0;
					HtmlElementType type1 = parser1.GetElement(nIndex).ElementType;
					string sElementValue = parser1.GetElement(nIndex).Value;
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "STYLE"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "SCRIPT"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "TITLE"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "STYLE"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "SCRIPT"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "TITLE"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.Text) && !flag1)
					{
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "&amp;", "&");
                        sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "&nbsp;", "&nbsp;&nbsp;");
                        sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "&nbsp;", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\r\n", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\n", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\r", " ");
                        
						arg0 = sElementValue.Length;
						writer1.Write(sElementValue);
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "P"))
					{
                        if (elementPStack.Current == null)
                        {
                            writer1.Write("\r\n", 2);
                        }
                        else
                        {
                            bool isFilterP = (bool)elementPStack.Pop();
                            if (isFilterP)
                            {
                                writer1.Write("\r\n", 2);
                                writer1.Write("</p>", 4);
                            }
                            else
                            {
                                writer1.Write("\r\n", 2);
                            }
                        }
					}
					if (type1 == HtmlElementType.Start)
					{
						string sTagAttributes;
						//long num3;
                        string num3 = "";
						Hashtable hashtable1;
						if (sElementValue.ToUpper() == "BR")
						{
							writer1.Write("\r\n", 2);
						}
						if (sElementValue.ToUpper() == "IMG")
						{
							sTagAttributes = parser1.GetElement(nIndex).TagAttributes;
							hashtable1 = parser1.ParseAttributes(sTagAttributes);
							string text4 = hashtable1["src"].ToString();
							num3 = hashtable1["width"].ToString();
							string num4 =hashtable1["height"].ToString();
							format = string.Concat(new string[]{"<img src=\"", text4, "\" width=", num3.ToString(), " height=", num4.ToString(), ">"});
							arg0 = format.Length;
							writer1.Write(format, arg0);
						}
                        if (sElementValue.ToUpper() == "P")
                        {
                            sTagAttributes = parser1.GetElement(nIndex).TagAttributes;
                            hashtable1 = parser1.ParseAttributes(sTagAttributes);
                            StringBuilder attBuilder = new StringBuilder();
                            format = "";
                            foreach(string attKey in hashtable1.Keys)
                            {
                                string[] styleItems = null;
                                if (attKey.ToLower().Equals("style"))
                                {
                                    StringBuilder styleAtt = new StringBuilder();
                                    string text4 = hashtable1["style"].ToString();
                                    if (text4.Contains("mso-char-indent-count") || text4.Contains("text-align") || text4.Contains("text-indent"))
                                    {
                                        styleItems = text4.Split(';');
                                        for (int si = 0; si < styleItems.Length; si++)
                                        {
                                            int styleIndex = styleItems[si].IndexOf("mso-char-indent-count");
                                            string numValue = null;
                                            if (styleIndex >= 0)
                                            {
                                                numValue = styleItems[si].Substring(styleIndex + "mso-char-indent-count".Length + 1).Trim();
                                                double num = newv.common.ConvertUtil.ToFloat(numValue);
                                                if (num > 0)
                                                    format = new string(' ', (int)(num * 4));
                                            }
                                            else if (styleItems[si].IndexOf("text-align") >= 0)
                                            {
                                                styleIndex = styleItems[si].IndexOf("text-align");
                                                numValue = styleItems[si].Substring(styleIndex + "text-align".Length + 1).Trim();
                                                if (!string.IsNullOrEmpty(numValue) &&　numValue.ToLower() != "left")
                                                {
                                                    styleAtt.Append(styleItems[si] + "; ");
                                                }
                                            }
                                            else if (styleItems[si].IndexOf("text-indent") >= 0 && !text4.Contains("mso-char-indent-count"))
                                            {
                                                styleIndex = styleItems[si].IndexOf("text-indent");
                                                numValue = styleItems[si].Substring(styleIndex + "text-indent".Length + 1).Trim();
                                                //当缩进为负数时不进行处理
                                                if (!numValue.Contains("-"))
                                                    styleAtt.Append(styleItems[si] + "; ");
                                            }
                                        }
                                    }

                                    if(styleAtt.Length>0)
                                        attBuilder.Append("style=\"" + styleAtt.ToString() + "\" ");
                                }
                                else if (attKey.ToLower().Equals("align"))
                                {
                                    string attValue = hashtable1[attKey].ToString().Trim();
                                    if (!string.IsNullOrEmpty(attValue) && attValue.ToLower() != "left")
                                    {
                                        attBuilder.Append("align=\"" + attValue + "\" ");
                                    }
                                }
                            }

                            if (attBuilder.Length > 0)
                            {
                                attBuilder.Insert(0, "<p ");
                                attBuilder.Append(">");
                                arg0 = attBuilder.Length;
                                writer1.Write(attBuilder.ToString(), arg0);

                                elementPStack.Push(true);
                            }
                            else
                            {
                                elementPStack.Push(false);
                            }

                            if (!string.IsNullOrEmpty(format))
                            {
                                arg0 = format.Length;
                                writer1.Write(format, arg0);
                            }
                        }
						if (sElementValue.ToUpper() == "TABLE")
						{
							format = "<table  border=\"1\" bordercolor=\"#000000\" style='border-collapse:collapse'>";
							arg0 = format.Length;
							writer1.Write(format, arg0);
						}
						if (sElementValue.ToUpper() == "TR")
						{
							format = "<" + sElementValue + ">";
							arg0 = format.Length;
							writer1.Write(format, arg0);
						}
						if (sElementValue.ToUpper() == "TD")
						{
							sTagAttributes = parser1.GetElement(nIndex).TagAttributes;
							hashtable1 = parser1.ParseAttributes(sTagAttributes);
                            if (hashtable1.ContainsKey("width"))
                            {
                                num3 = hashtable1["width"].ToString();
                                format = "<td width=" + num3.ToString();
                            }
                            else
                            {
                                num3 = "";
                                format = "<td ";
                            }
                            long num5;
                            num5 = Convert.ToInt32(hashtable1["rowspan"]);
                            if (num5 != 0)
							{
                                format = format + " rowspan=" + num5.ToString();
							}
                            num5 = Convert.ToInt32(hashtable1["colspan"]);
                            if (num5 != 0)
							{
                                format = format + " colspan=" + num5.ToString();
							}
							format = format + " valigh=top>";
							arg0 = format.Length;
							writer1.Write(format, arg0);
						}
						if (((((sElementValue.ToUpper() == "B") || (sElementValue.ToUpper() == "I")) || ((sElementValue.ToUpper() == "U") || (sElementValue.ToUpper() == "S"))) || (sElementValue.ToUpper() == "SUP")) || (sElementValue.ToUpper() == "SUB"))
						{
							format = "<" + sElementValue + ">";
							arg0 = format.Length;
							writer1.Write(format, arg0);
						}
					}
					if ((type1 == HtmlElementType.End) && (((((sElementValue.ToUpper() == "TABLE") || (sElementValue.ToUpper() == "TR")) || ((sElementValue.ToUpper() == "TD") || (sElementValue.ToUpper() == "B"))) || (((sElementValue.ToUpper() == "I") || (sElementValue.ToUpper() == "U")) || ((sElementValue.ToUpper() == "S") || (sElementValue.ToUpper() == "SUP")))) || (sElementValue.ToUpper() == "SUB")))
					{
						format = "</" + sElementValue + ">";
						arg0 = format.Length;
						writer1.Write(format, arg0);
					}
				}
				writer1.Close();
			}
			catch (Exception exception1)
			{
				if (writer1 != null)
				{
					writer1.Close();
				}
				return exception1.Message;
			}
			return "";
		}
		
		public static string ReplaceAllSubStr (string sElementValue, string sSrcString, string sToStrign)
		{
			return sElementValue = Regex.Replace(sElementValue, sSrcString, sToStrign, RegexOptions.IgnoreCase);
		}
		
		public static string ConvertHtml2Txt (string htmlText)
		{
			StringBuilder builder1 = new StringBuilder();
			try
			{
                string sContent = htmlText;
				HtmlParser parser1 = new HtmlParser();
				parser1.Parse(sContent);
				long num1 = parser1.GetElementCount();
				bool flag1 = false;
				for (int nIndex = 0;nIndex < num1; nIndex++)
				{
					string value;
					long num5;
					HtmlElementType type1 = parser1.GetElement(nIndex).ElementType;
					string sElementValue = parser1.GetElement(nIndex).Value;
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "STYLE"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "SCRIPT"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.Start) && (sElementValue.ToUpper() == "TITLE"))
					{
						flag1 = true;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "STYLE"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "SCRIPT"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "TITLE"))
					{
						flag1 = false;
					}
					if ((type1 == HtmlElementType.Text) && !flag1)
					{
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "&amp;", "&");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "&NBSP;", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\r\n", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\n", " ");
						sElementValue = Html2TextUtil.ReplaceAllSubStr(sElementValue, "\r", " ");
						num5 = sElementValue.Length;
						builder1.Append(sElementValue);
					}
					if ((type1 == HtmlElementType.End) && (sElementValue.ToUpper() == "P"))
					{
						builder1.Append("\r\n");
					}
					if (type1 == HtmlElementType.Start)
					{
						string sTagAttributes;
						long num3;
						Hashtable hashtable1;
						if (sElementValue.ToUpper() == "BR")
						{
							builder1.Append("\r\n");
						}
						if (sElementValue.ToUpper() == "IMG")
						{
							sTagAttributes = parser1.GetElement(nIndex).TagAttributes;
							hashtable1 = parser1.ParseAttributes(sTagAttributes);
							string text4 = hashtable1["src"].ToString();
							num3 = Convert.ToInt32(hashtable1["width"]);
							long num4 = Convert.ToInt32(hashtable1["height"]);
							value = string.Concat(new string[]{"<img src=\"", text4, "\" width=", num3.ToString(), " height=", num4.ToString(), ">"});
							num5 = value.Length;
							builder1.Append(value);
						}
						if (sElementValue.ToUpper() == "TABLE")
						{
							value = "<table  border=\"1\" bordercolor=\"#000000\" style='border-collapse:collapse'>";
							num5 = value.Length;
							builder1.Append(value);
						}
						if (sElementValue.ToUpper() == "TR")
						{
							value = "<" + sElementValue + ">";
							num5 = value.Length;
							builder1.Append(value);
						}
						if (sElementValue.ToUpper() == "TD")
						{
							sTagAttributes = parser1.GetElement(nIndex).TagAttributes;
							hashtable1 = parser1.ParseAttributes(sTagAttributes);
							num3 = Convert.ToInt32(hashtable1["width"]);
							value = "<td width=" + num3.ToString();
							num3 = Convert.ToInt32(hashtable1["rowspan"]);
							if (num3 != 0)
							{
								value = value + " rowspan=" + num3.ToString();
							}
							num3 = Convert.ToInt32(hashtable1["colspan"]);
							if (num3 != 0)
							{
								value = value + " colspan=" + num3.ToString();
							}
							value = value + " valigh=top>";
							num5 = value.Length;
							builder1.Append(value);
						}
						if (((((sElementValue.ToUpper() == "B") || (sElementValue.ToUpper() == "I")) || ((sElementValue.ToUpper() == "U") || (sElementValue.ToUpper() == "S"))) || (sElementValue.ToUpper() == "SUP")) || (sElementValue.ToUpper() == "SUB"))
						{
							value = "<" + sElementValue + ">";
							num5 = value.Length;
							builder1.Append(value);
						}
					}
					if ((type1 == HtmlElementType.End) && (((((sElementValue.ToUpper() == "TABLE") || (sElementValue.ToUpper() == "TR")) || ((sElementValue.ToUpper() == "TD") || (sElementValue.ToUpper() == "B"))) || (((sElementValue.ToUpper() == "I") || (sElementValue.ToUpper() == "U")) || ((sElementValue.ToUpper() == "S") || (sElementValue.ToUpper() == "SUP")))) || (sElementValue.ToUpper() == "SUB")))
					{
						value = "</" + sElementValue + ">";
						num5 = value.Length;
						builder1.Append(value);
					}
				}
			}
			catch (Exception exception1)
			{
				return exception1.Message;
			}
			return builder1.ToString();
		}
		
	}
}
