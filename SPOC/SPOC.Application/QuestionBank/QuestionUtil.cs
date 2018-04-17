using System;
using System.Text.RegularExpressions;

namespace SPOC.QuestionBank
{
    public class QuestionUtil
    {
        /// <summary>
        /// 将对象转换为整型
        /// </summary>
        /// <param name="objInt">对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的整数</returns>
        public static int ToInt(object objInt, int defaultValue)
        {
            if (objInt == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(Convert.ToDecimal(objInt));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        /// <summary>
        /// 将对象转换为整型
        /// </summary>
        /// <param name="objInt">对象</param>
        /// <returns>得到的整数。缺省为0</returns>
        public static int ToInt(object objInt)
        {
            return ToInt(objInt, 0);
        }
        public static string DecodeSpliter(string str)
        {
            if (str == null) return "";
            return str.Replace("&Vertical;", "|");
        }
        /// <summary>
        /// 将0,1,2,3等数字转换成A,B,C,D等字母
        /// </summary>
        /// <param name="sNumbers"></param>
        /// <returns></returns>
        public static string AnswerNumbersToChars(string sNumbers)
        {
            var arrNumber = sNumbers.Split("|".ToCharArray());
            var answerChars = string.Empty;
            foreach (var number in arrNumber)
            {
                if (ToInt(number, -1) >= 0)
                    answerChars = answerChars + ((char)(65 + ToInt(number, 0))) + "|";	//只是显示用
            }
            if (answerChars.Length > 0) answerChars = answerChars.Substring(0, answerChars.Length - 1);
            return answerChars;
        }

        /// <summary>
        /// 格式化选择题答案
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static string FormatSelectQuestionAnswer(string answer, out string errorMessage)
        {
            errorMessage = "";
            string spliter = "|";
            answer = answer.Trim();
            answer = answer.Replace("，", spliter);
            answer = answer.Replace(",", spliter);
            answer = answer.Replace("、", spliter);
            answer = answer.Replace("︱", spliter);
            answer = answer.Replace("｜", spliter);
            string[] arrChar = answer.Split(spliter.ToCharArray());
            foreach (var c in arrChar)
            {
                if (c == "") continue;
                if (c.Length <= 1 && char.Parse(c) >= 65 && char.Parse(c) <= 91) continue;
                errorMessage = ("选择题答案格式不对。");
                return "";
            }
            answer = answer.Trim("|".ToCharArray());
            return answer;
        }

        /// <summary>
        /// 格式化多选题的标准答案格式
        /// </summary>
        /// <param name="standerAnswer"></param>
        /// <returns></returns>
        public static string FormatStanderdAnswerForMulti(string standerAnswer)
        {
            char[] arrStanserAnswer = standerAnswer.ToUpper().ToCharArray();
            string newStanderAnswer = "";
            foreach (char stanserAnswer in arrStanserAnswer)
            {
                int charCode = stanserAnswer;
                if (charCode >= 65 && charCode <= 90)
                {
                    newStanderAnswer += stanserAnswer + "|";
                }
            }
            newStanderAnswer = newStanderAnswer.Trim("|".ToCharArray());
            return newStanderAnswer;
        }

        /// <summary>
        /// 获取选择题的答案的显示格式(当打乱顺序时)
        /// </summary>
        /// <param name="selectAnswer"></param>
        /// <param name="arrOriginalSelectAnswerIndex"></param>
        /// <returns></returns>
        public static string GetSelectAnswerView(string selectAnswer, int[] arrOriginalSelectAnswerIndex)
        {
            string answerView = "";
            string thisNumber = "";
            //如果打乱了顺序，则显示的答案名称不是数据库里的名称，而是打乱后的名称
            if (arrOriginalSelectAnswerIndex != null && arrOriginalSelectAnswerIndex.Length > 0)
            {
                selectAnswer = selectAnswer.Replace(" ", "");  //防止有空格
                string[] arrSelectAnswer = selectAnswer.Split("|".ToCharArray());
                selectAnswer = "";
                foreach (string selectAnswerItem in arrSelectAnswer)
                {
                    if (selectAnswerItem == "") continue;
                    int nAnswerNum = ToInt(selectAnswerItem, -1);
                    if (nAnswerNum == -1)
                    {
                        if (selectAnswerItem != "") nAnswerNum = char.Parse(selectAnswerItem) - 64;
                    }
                    nAnswerNum = nAnswerNum - 1;    //因为数组的顺序号是从0开始，而上面的是从1开始，所以要-1
                    if (nAnswerNum >= 0)
                    {
                        for (int j = 0; j < arrOriginalSelectAnswerIndex.Length; j++)
                        {
                            //找出真正的答案在显示的答案中的顺序
                            if (nAnswerNum == arrOriginalSelectAnswerIndex[j])
                            {
                                thisNumber = j.ToString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        thisNumber = "-1";
                    }
                    selectAnswer = selectAnswer + thisNumber + "|";
                }
                if (selectAnswer != "") selectAnswer = selectAnswer.Substring(0, selectAnswer.Length - 1);
                //排序
                selectAnswer = OrderAnswerNumbers(selectAnswer);
                arrSelectAnswer = selectAnswer.Split("|".ToCharArray());

                foreach (string selectAnswerItem in arrSelectAnswer)
                {
                    int nAnswerNum = ToInt(selectAnswerItem, -1);
                    if (nAnswerNum >= 0)
                    {
                        answerView = answerView + (char)(65 + nAnswerNum) + ",";	//只是显示用
                    }
                    else
                    {
                        answerView = answerView + " " + ",";	//只是显示用
                    }
                }

                if (answerView.Length > 0) answerView = answerView.Substring(0, answerView.Length - 1);
            }
            else
            {
                answerView = selectAnswer.Replace("|", ",");
            }


            return answerView;
        }

        public static string AnswerCharsToNumbers(string sNumbers)
        {
            string sTemp = "";
            //第一种方式：A,B,C
            for (int i = 0; i < sNumbers.Length; i++)
            {
                int nAnswerNum = char.Parse(sNumbers.Substring(i, 1)) - 65;
                if (nAnswerNum >= 0 && nAnswerNum < 26)
                    sTemp = sTemp + nAnswerNum + "|";
            }
            if (sTemp.Length > 0) sTemp = sTemp.Substring(0, sTemp.Length - 1);

            //第二种方式1,2,3
            if (sTemp == "")
            {
                sNumbers = sNumbers.Replace("，", ",");
                sNumbers = sNumbers.Replace(";", ",");
                sNumbers = sNumbers.Replace("；", ",");
                string[] arrChars = sNumbers.Split(',');
                foreach (string c in arrChars)
                {
                    int nNum = ToInt(c) - 1;
                    if (nNum >= 0 && nNum < 12)
                    {
                        sTemp = sTemp + nNum.ToString() + "|";
                    }
                }
                if (sTemp.Length > 0) sTemp = sTemp.Substring(0, sTemp.Length - 1);
            }
            return sTemp;

        }

        //对答案进行排序，例如D|A|C要排成A|C|D,这样才能与正确答案进行比较
        public static string OrderAnswerNumbers(string sNumbers)
        {
            string sTemp;

            var arrTemp = sNumbers.Split("|".ToCharArray());
            for (int i = 0; i < arrTemp.Length - 1; i++)
            {
                if (arrTemp[i].Length != 1) continue;
                for (int j = i + 1; j < arrTemp.Length; j++)
                {
                    if (arrTemp[j].Length != 1) continue;
                    if (arrTemp[i][0] > arrTemp[j][0])
                    {
                        sTemp = arrTemp[j];
                        arrTemp[j] = arrTemp[i];
                        arrTemp[i] = sTemp;
                    }
                }
            }

            //组织数据
            sTemp = "";
            foreach (string temp in arrTemp)
            {
                sTemp = sTemp + temp + "|";
            }
            sTemp = sTemp.Trim("|".ToCharArray());
            return sTemp;
        }


        /// <summary>
        /// 新的替换填空题文本框的方法,三个或以上连在一起的下划线代表一个填空
        /// </summary>
        /// <param name="content"></param>
        /// <param name="questionUid"></param>
        /// <param name="contentAnswer"></param>
        /// <returns></returns>
        public static string ReplaceFillInContent(string content, string questionUid, string contentAnswer)
        {
            int fillInBoxCount = 0;
            return ReplaceFillInContent(content, questionUid, contentAnswer, ref fillInBoxCount);
        }



        #region 练习试卷

        /// <summary>
        /// 练习试卷
        /// </summary>
        /// <param name="content"></param>
        /// <param name="questionUid"></param>
        /// <param name="contentAnswer"></param>
        /// <returns></returns>
        public static string ReplaceFillInContentExercise(string content, string questionUid, string contentAnswer)
        {
            int fillInBoxCount = 0;
            return ReplaceFillInContentExercise(content, questionUid, contentAnswer, ref fillInBoxCount);
        }

        public static string ReplaceFillInContentExercise(string content, string questionUid, string contentAnswer, ref int fillInBoxCount)
        {
            if (fillInBoxCount < 0) throw new ArgumentOutOfRangeException("fillInBoxCount");
            string[] answer = contentAnswer.Split("|".ToCharArray());
            var lowLinePos = content.IndexOf("_", StringComparison.Ordinal);
            fillInBoxCount = 0;
            int preLowLinePos = -1;		//上一个下划线位置
            int lowLineCount = 0;
            string oneAnswer;
            string oneInputBoxString;
            while (lowLinePos > -1)
            {
                //如果前一个下划线不是前一个字符
                if (lowLinePos != preLowLinePos + 1)
                {
                    //如果下划线个数大于3
                    if (lowLineCount >= 3)
                    {
                        //找到一个填空题
                        oneAnswer = answer.Length > fillInBoxCount ? answer[fillInBoxCount] : "";
                        oneAnswer = DecodeSpliter(oneAnswer);
                        oneInputBoxString = "<input type='text'  onmouseout=\"jscomCheckedFillQuestionAnswer('Answer" + questionUid + "')\" id='Answer" + questionUid + "' name='Answer" + questionUid + "'  maxlength='1950' class='commoninput' style='BACKGROUND-COLOR: #ffff66;width=" + (lowLineCount * 30).ToString() + "' value='" + oneAnswer + "' onfocus='try{Controller_onfocus(this)}catch(e){}'>";
                        content = content.Substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + content.Substring(preLowLinePos + 1);
                        //重设原来那个下划线的位置
                        lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
                        fillInBoxCount = fillInBoxCount + 1;
                    }

                    //重新开始
                    lowLineCount = 1;
                }
                else
                {
                    lowLineCount = lowLineCount + 1;

                }
                preLowLinePos = lowLinePos;
                lowLinePos = content.IndexOf("_", lowLinePos + 1, StringComparison.Ordinal);
            }
            if (lowLineCount >= 3)
            {
                //找到一个填空题
                oneAnswer = answer.Length > fillInBoxCount ? answer[fillInBoxCount] : "";
                oneAnswer = DecodeSpliter(oneAnswer);
                oneInputBoxString = "<input type='text' onmouseout=\"jscomCheckedFillQuestionAnswer('Answer" + questionUid + "')\" id='Answer" + questionUid + "' name='Answer" + questionUid + "'  maxlength='1950' class='LineText' style='BACKGROUND-COLOR: #ffff66;width=" + (lowLineCount * 30) + "' value=\"" + oneAnswer + "\" onfocus='try{Controller_onfocus(this)}catch(e){}'>";
                content = content.Substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + content.Substring(preLowLinePos + 1);
                //重设原来那个下划线的位置
                fillInBoxCount = fillInBoxCount + 1;
            }
            return content;
        }
        #endregion

        /// <summary>
        /// 得到填空题的填空个数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int GetFillInBoxCount(string content)
        {
            int fillInBoxCount = 0;
            ReplaceFillInContent(content, "questionUid", "", ref fillInBoxCount);
            return fillInBoxCount;
        }

        public static string ReplaceFillInContent(string content, string questionUid, string contentAnswer, ref int fillInBoxCount)
        {
            if (fillInBoxCount < 0) throw new ArgumentOutOfRangeException("fillInBoxCount");
            fillInBoxCount = 0;
            string[] answer = contentAnswer.Split("|".ToCharArray());
            int lowLinePos = content.IndexOf("_", StringComparison.Ordinal);
            int preLowLinePos = -1;		//上一个下划线位置
            int lowLineCount = 0;
            string oneAnswer;
            string oneInputBoxString;
            while (lowLinePos > -1)
            {
                //如果前一个下划线不是前一个字符
                if (lowLinePos != preLowLinePos + 1)
                {
                    //如果下划线个数大于3
                    if (lowLineCount >= 3)
                    {
                        //找到一个填空题
                        oneAnswer = answer.Length > fillInBoxCount ? answer[fillInBoxCount] : "";
                        oneAnswer = DecodeSpliter(oneAnswer);
                        oneInputBoxString = "<input type='text' id='Answer" + questionUid + "' name='Answer" + questionUid + "'  maxlength='1950' class='commoninput' style='BACKGROUND-COLOR: #ffff66;width=" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onfocus='try{Controller_onfocus(this)}catch(e){}'>";
                        content = content.Substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + content.Substring(preLowLinePos + 1);
                        //重设原来那个下划线的位置
                        lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
                        fillInBoxCount = fillInBoxCount + 1;
                    }

                    //重新开始
                    lowLineCount = 1;
                }
                else
                {
                    lowLineCount = lowLineCount + 1;

                }
                preLowLinePos = lowLinePos;
                lowLinePos = content.IndexOf("_", lowLinePos + 1, StringComparison.Ordinal);
            }
            if (lowLineCount >= 3)
            {
                //找到一个填空题
                oneAnswer = answer.Length > fillInBoxCount ? answer[fillInBoxCount] : "";
                oneAnswer = DecodeSpliter(oneAnswer);
                oneInputBoxString = "<input type='text' id='Answer" + questionUid + "' name='Answer" + questionUid + "'  maxlength='1950' class='LineText' style='BACKGROUND-COLOR: #ffff66;width=" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onfocus='try{Controller_onfocus(this)}catch(e){}'>";
                content = content.Substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + content.Substring(preLowLinePos + 1);
                //重设原来那个下划线的位置
                fillInBoxCount = fillInBoxCount + 1;
            }
            return content;
        }

        /// <summary>
        /// 删除文本中html标签
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(string text)
        {
            var reg = new Regex("<.+?>");
            return reg.Replace(text, "");
        }
    }
}