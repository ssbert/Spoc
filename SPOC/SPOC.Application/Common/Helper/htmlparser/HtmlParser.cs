using System;
using System.Collections;
using System.IO;
using System.Text;

namespace SPOC.Common.Helper.htmlparser
{
    public class HtmlParser
    {
        // Constructors
        public HtmlParser()
        {
            this._htmlElementList = new HtmlElementList();
        }


        // Methods
        private bool EndWith(string sStr, string sSubStr)
        {
            int startIndex = sStr.Length - sSubStr.Length;
            if ((startIndex >= 0) && (sStr.Substring(startIndex) == sSubStr))
            {
                return true;
            }
            return false;
        }

        private bool StartWith(string sStr, string sSubStr)
        {
            if ((sStr.Length >= sSubStr.Length) && (sStr.Substring(0, sSubStr.Length) == sSubStr))
            {
                return true;
            }
            return false;
        }

        private void CutLeadingTrailingChar(ref string sStr)
        {
            int startIndex = 0;
            while ((startIndex < sStr.Length) && ((((sStr[startIndex] == ' ') || (sStr[startIndex] == '\r')) || (sStr[startIndex] == '\n')) || (sStr[startIndex] == '\t')))
            {
                startIndex++;
            }
            sStr = sStr.Substring(startIndex);
            startIndex = sStr.Length - 1;
            while ((startIndex >= 0) && ((((sStr[startIndex] == ' ') || (sStr[startIndex] == '\r')) || (sStr[startIndex] == '\n')) || (sStr[startIndex] == '\t')))
            {
                startIndex--;
            }
            sStr = sStr.Substring(0, startIndex + 1);

            //sStr = sStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sStr"></param>
        /// <param name="isClearEmptyChar">是否清除空字符</param>
        private void CutLeadingTrailingChar(ref string sStr, bool isClearEmptyChar)
        {
            if (isClearEmptyChar)
            {
                CutLeadingTrailingChar(ref sStr);
                return;
            }
            int emptyType = 0;  //0:没空格；1：前空格；2：后空格；3：前后空格
            if (sStr == "\r\n")
            {
                sStr = " ";
            }
            else if (sStr.Length > 2)
            {
                if (sStr.Substring(0, 2) == "\r\n")
                {
                    if (sStr.Length >= 4)
                    {
                        if (sStr.Substring(2, 2) != "\r\n")
                            emptyType = 1;
                    }
                    else
                        emptyType = 1;
                }
                if (sStr.Substring(sStr.Length - 2, 2) == "\r\n")
                {
                    if (sStr.Length >= 4)
                    {
                        if (sStr.Substring(sStr.Length - 4, 2) != "\r\n")
                        {
                            if (emptyType == 1)
                                emptyType = 3;
                            else
                                emptyType = 2;
                        }
                    }
                    else
                    {
                        if (emptyType == 1)
                            emptyType = 3;
                        else
                            emptyType = 2;
                    }
                }
            }
            int startIndex = 0;
            while ((startIndex < sStr.Length) && (((sStr[startIndex] == '\r') || (sStr[startIndex] == '\n')) || (sStr[startIndex] == '\t')))
            {
                startIndex++;
            }
            if (startIndex > 0)
            {
                sStr = sStr.Substring(startIndex);
            }
            startIndex = sStr.Length - 1;
            while ((startIndex >= 0) && (((sStr[startIndex] == '\r') || (sStr[startIndex] == '\n')) || (sStr[startIndex] == '\t')))
            {
                startIndex--;
            }
            if (startIndex < sStr.Length - 1)
            {
                sStr = sStr.Substring(0, startIndex + 1);
            }
            switch (emptyType)
            {
                case 1:
                    sStr = " " + sStr;
                    break;
                case 2:
                    sStr = sStr + " ";
                    break;
                case 3:
                    sStr = " " + sStr + " ";
                    break;
                default:
                    break;
            }
        }

        private void ParseCommentThenAdd(string sTagBuffer)
        {
            string sValue = sTagBuffer.Substring(4);
            if (sValue.Length >= 3)
            {
                sValue = sValue.Substring(0, sValue.Length - 3);
            }
            this._htmlElementList.AddHtmlElement(HtmlElementType.Comment, sTagBuffer, sValue);
        }

        private void ParseDocTypeThenAdd(string sTagBuffer)
        {
            string sValue = sTagBuffer.Substring(sTagBuffer.Length);
            if (sValue.Length >= 2)
            {
                sValue = sValue.Substring(0, sValue.Length - 2);
            }
            this._htmlElementList.AddHtmlElement(HtmlElementType.DocType, sTagBuffer, sValue);
        }

        private void ParseEndTagThenAdd(string sTagBuffer)
        {
            string sValue = sTagBuffer.Substring(2);
            if (sValue.Length >= 1)
            {
                sValue = sValue.Substring(0, sValue.Length - 1);
            }
            sValue = sValue.ToLower();
            this._htmlElementList.AddHtmlElement(HtmlElementType.End, sTagBuffer, sValue);
        }

        private void ParseStartTagThenAdd(string sTagBuffer)
        {
            sTagBuffer = sTagBuffer.Substring(1);
            if (sTagBuffer.Length >= 1)
            {
                sTagBuffer = sTagBuffer.Substring(0, sTagBuffer.Length - 1);
            }
            int startIndex = 0;
            while ((startIndex < sTagBuffer.Length) && ((((sTagBuffer[startIndex] == ' ') || (sTagBuffer[startIndex] == '\r')) || (sTagBuffer[startIndex] == '\n')) || (sTagBuffer[startIndex] == '\t')))
            {
                startIndex++;
            }
            string arg0 = "";
            while ((startIndex < sTagBuffer.Length) && (sTagBuffer[startIndex] != ' '))
            {
                arg0 = arg0 + sTagBuffer[startIndex];
                startIndex++;
            }
            arg0 = arg0.ToLower();
            while ((startIndex < sTagBuffer.Length) && ((((sTagBuffer[startIndex] == ' ') || (sTagBuffer[startIndex] == '\r')) || (sTagBuffer[startIndex] == '\n')) || (sTagBuffer[startIndex] == '\t')))
            {
                startIndex++;
            }
            string sTagAttributes = sTagBuffer.Substring(startIndex);
            this._htmlElementList.AddHtmlElement(HtmlElementType.Start, sTagBuffer, arg0, sTagAttributes);
        }

        public void Parse(string sContent)
        {
            this._htmlElementList.Clear();
            CutLeadingTrailingChar(ref sContent, true);
            if (sContent != "")
            {
                string sStr = "";
                string arg0 = "";
                bool flag1 = false;
                HtmlTagType type1 = HtmlTagType.None;
                ParserSkipType type2 = ParserSkipType.SkipNone;
                int num1 = 1;
                int num2 = 1;
                int num3 = 0;
                try
                {
                    for (num3 = 0; num3 < sContent.Length; num3++)
                    {
                        ParserSkipType type3;
                        char arg1 = sContent[num3];
                        char chr2 = arg1;
                        if (chr2 != '\n')
                        {
                            if (chr2 != '\r')
                            {
                                goto Label_0075;
                            }
                            goto Label_007D;
                        }
                        num1++;
                        num2 = 1;
                        goto Label_007D;
                    Label_0075:
                        num2++;
                    Label_007D:
                        type3 = type2;
                        switch (type3)
                        {
                            case ParserSkipType.SkipNone:
                                {
                                    chr2 = arg1;
                                    switch (chr2)
                                    {
                                        case '<':
                                            {
                                                break;
                                            }
                                        case '=':
                                            {
                                                goto Label_0337;
                                            }
                                        case '>':
                                            {
                                                goto Label_022B;
                                            }
                                        default:
                                            {
                                                goto Label_0337;
                                            }
                                    }
                                }
                                break;
                            case ParserSkipType.SkipScript:
                                {
                                    arg0 = arg0 + arg1;
                                    if (this.EndWith(arg0.ToUpper(), "</SCRIPT>"))
                                    {
                                        type2 = ParserSkipType.SkipNone;
                                        num3 -= 9;
                                        num2 -= 9;
                                        arg0 = arg0.Substring(0, arg0.Length - 9);
                                        this._htmlElementList.AddHtmlElement(HtmlElementType.Comment, arg0, arg0);
                                        arg0 = "";
                                    }
                                    continue;
                                }
                                break;
                            case ParserSkipType.SkipStyle:
                                {
                                    arg0 = arg0 + arg1;
                                    if (this.EndWith(arg0.ToUpper(), "</STYLE>"))
                                    {
                                        type2 = ParserSkipType.SkipNone;
                                        num3 -= 8;
                                        num2 -= 8;
                                        arg0 = arg0.Substring(0, arg0.Length - 8);
                                    }
                                    continue;
                                }
                                break;
                            default:
                                {
                                    continue;
                                }
                        }
                        HtmlTagType type4 = type1;
                        switch (type4)
                        {
                            case HtmlTagType.Comment:
                                {
                                    sStr = sStr + arg1;
                                    continue;
                                }
                                break;
                            case HtmlTagType.DocType:
                                {
                                    throw new HtmlParserException("Meet '<' while TAG_DOCTYPE", sContent, num3, num1, num2);
                                }
                                break;
                            case HtmlTagType.End:
                                {
                                    throw new HtmlParserException("Meet '<' while TAG_END", sContent, num3, num1, num2);
                                }
                            case HtmlTagType.Start:
                                {
                                    throw new HtmlParserException("Meet '<' while TAG_START", sContent, num3, num1, num2);
                                }
                                break;
                            default:
                                {
                                    if (!flag1)
                                    {
                                        flag1 = true;
                                        sStr = sStr + arg1;
                                        this.CutLeadingTrailingChar(ref arg0, false);
                                        if (arg0 != "")
                                        {
                                            this._htmlElementList.AddHtmlElement(HtmlElementType.Text, arg0, arg0);
                                        }
                                        arg0 = "";
                                    }
                                    continue;
                                }
                        }
                    Label_022B:
                        type4 = type1;
                        switch (type4)
                        {
                            case HtmlTagType.Comment:
                                {
                                    if (!this.EndWith(sStr, "--"))
                                    {
                                        sStr = sStr + arg1;
                                        continue;
                                    }
                                    type1 = HtmlTagType.None;
                                    sStr = sStr + arg1;
                                    this.ParseCommentThenAdd(sStr);
                                    sStr = "";
                                    continue;
                                }
                                break;
                            case HtmlTagType.DocType:
                                {
                                    type1 = HtmlTagType.None;
                                    sStr = sStr + arg1;
                                    this.ParseDocTypeThenAdd(sStr);
                                    sStr = "";
                                    continue;
                                }
                                break;
                            case HtmlTagType.End:
                                {
                                    type1 = HtmlTagType.None;
                                    sStr = sStr + arg1;
                                    this.ParseEndTagThenAdd(sStr);
                                    sStr = "";
                                    continue;
                                }
                                break;
                            case HtmlTagType.Start:
                                {
                                    type1 = HtmlTagType.None;
                                    sStr = sStr + arg1;
                                    if (this.StartWith(sStr.ToUpper(), "<SCRIPT"))
                                    {
                                        type2 = ParserSkipType.SkipScript;
                                    }
                                    if (this.StartWith(sStr.ToUpper(), "<STYLE"))
                                    {
                                        type2 = ParserSkipType.SkipStyle;
                                    }
                                    this.ParseStartTagThenAdd(sStr);
                                    sStr = "";
                                    continue;
                                }
                            default:
                                {
                                    continue;
                                }
                        }
                    Label_0337:
                        if (!flag1)
                        {
                            type4 = type1;
                            switch (type4)
                            {
                                case HtmlTagType.Comment:
                                    {
                                        sStr = sStr + arg1;
                                        continue;
                                    }
                                case HtmlTagType.DocType:
                                case HtmlTagType.End:
                                case HtmlTagType.Start:
                                    {
                                        goto Label_03BD;
                                    }
                                default:
                                    {
                                        goto Label_03EF;
                                    }
                            }
                        }
                        flag1 = false;
                        chr2 = arg1;
                        if (chr2 != '!')
                        {
                            if (chr2 == '/')
                            {
                                type1 = HtmlTagType.End;
                                goto Label_037D;
                            }
                        }
                        else
                        {
                            if (sContent[(num3 + 1)] == '-')
                            {
                                type1 = HtmlTagType.Comment;
                                goto Label_037D;
                            }
                            type1 = HtmlTagType.DocType;
                            goto Label_037D;
                        }
                        type1 = HtmlTagType.Start;
                    Label_037D:
                        sStr = sStr + arg1;
                        continue;
                    Label_03BD:
                        chr2 = arg1;
                        if ((chr2 == '\n') || (chr2 == '\r'))
                        {
                            sStr = sStr + " ";
                            continue;
                        }
                        sStr = sStr + arg1;
                        continue;
                    Label_03EF:
                        arg0 = arg0 + arg1;
                    }
                    if ((sStr != "") && !this.EndWith(sStr, ">"))
                    {
                        throw new HtmlParserException("HTML never closed", sContent, sContent.Length, num1, num2);
                    }
                    this.CutLeadingTrailingChar(ref arg0);
                    if (arg0 != "")
                    {
                        this._htmlElementList.AddHtmlElement(HtmlElementType.Text, arg0, arg0);
                    }
                }
                catch (Exception exception1)
                {
                    Console.Write(exception1.Message + num3.ToString());
                }
            }
        }

        public void ParseFile(string sFileName)
        {
            StreamReader reader1 = new StreamReader(System.IO.File.OpenRead(sFileName), Encoding.GetEncoding("gb2312"));
            string sContent = reader1.ReadToEnd();
            reader1.Close();
            this.Parse(sContent);
        }

        public int GetElementCount()
        {
            return this._htmlElementList.Count;
        }

        public HtmlElement GetElement(int nIndex)
        {
            return (HtmlElement)this._htmlElementList[nIndex];
        }

        public Hashtable ParseAttributes(string sTagAttributes)
        {
            Hashtable hashtable1 = new Hashtable();
            int num1 = 0;
            while (num1 < sTagAttributes.Length)
            {
                string arg0;
                char arg1 = sTagAttributes[num1];
                char chr2 = arg1;
                switch (chr2)
                {
                    case '\t':
                    case '\n':
                    case '\r':
                        {
                            break;
                        }
                    case '\v':
                    case '\f':
                        {
                            goto Label_004B;
                        }
                    default:
                        {
                            if (chr2 != ' ')
                            {
                                goto Label_004B;
                            }
                            break;
                        }
                }
                num1++;
                continue;
            Label_004B:
                arg0 = "";
                string value = "";
                while (num1 < sTagAttributes.Length)
                {
                    arg1 = sTagAttributes[num1];
                    if ((arg1 == '=') || (arg1 == ' '))
                    {
                        break;
                    }
                    arg0 = arg0 + arg1;
                    num1++;
                }
                chr2 = arg1;
                if (chr2 != '=')
                {
                    goto Label_01BF;
                }
                num1++;
                if (num1 >= sTagAttributes.Length)
                {
                    return hashtable1;
                }
                arg1 = sTagAttributes[num1];
                chr2 = arg1;
                if (chr2 != '"')
                {
                    if (chr2 == '\'')
                    {
                        num1++;
                        goto Label_0167;
                    }
                    goto Label_01AC;
                }
                num1++;
                while (num1 < sTagAttributes.Length)
                {
                    arg1 = sTagAttributes[num1];
                    if (arg1 == '"')
                    {
                        break;
                    }
                    value = value + arg1;
                    num1++;
                }
                num1++;
                goto Label_01C7;
            Label_0139:
                arg1 = sTagAttributes[num1];
                if (arg1 == '\'')
                {
                    goto Label_0176;
                }
                value = value + arg1;
                num1++;
            Label_0167:
                if (num1 < sTagAttributes.Length)
                {
                    goto Label_0139;
                }
            Label_0176:
                num1++;
                goto Label_01C7;
            Label_017E:
                arg1 = sTagAttributes[num1];
                if (arg1 == ' ')
                {
                    goto Label_01C7;
                }
                value = value + arg1;
                num1++;
            Label_01AC:
                if (num1 < sTagAttributes.Length)
                {
                    goto Label_017E;
                }
                goto Label_01C7;
            Label_01BF:
                value = "";
            Label_01C7:
                hashtable1.Add(arg0, value);
            }
            return hashtable1;
        }


        // Instance Fields
        private HtmlElementList _htmlElementList;
    }
}
