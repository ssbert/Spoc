using Castle.Core.Logging;
using SPOC.Exam;
using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using SPOC.Common.File;

namespace SPOC.Common.Exam
{
    /// <summary>
    /// 合成考试试卷的帮助类
    /// </summary>
    public class XlstHepler
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        public static ILogger Logger { get; set; }
        public XlstHepler()
        {
            Logger = new NullLogger();
        }
        /// <summary>
        /// 合成试卷内容（根据examUid_paperUid作为缓存key）
        /// </summary>
        /// <param name="examUid">考试编号</param>
        /// <param name="paperUid">试卷编号</param>
        /// <param name="xslFileUri"></param>
        /// <param name="paperXml"></param>
        /// <returns></returns>
        public static string Convert2XHtml(string examUid, string paperUid, string xslFileUri, string paperXml, ExamExam examInfo, ExamGrade gradeInfo)
        {
            XmlSecureResolver resolver = new XmlSecureResolver(new XmlUrlResolver(), "http://serverName/data/");
            System.Xml.Xsl.XslCompiledTransform trans = new System.Xml.Xsl.XslCompiledTransform();
            string xsltFile = xslFileUri;
            using (StreamReader rdr = new StreamReader(xsltFile))
            {
                using (XmlReader xmlRdr = XmlReader.Create(rdr))
                {
                    //设置考试模板的属性值
                    XmlDocument style = new XmlDocument();
                    style.Load(xsltFile);
                    XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(style.NameTable);
                    xmlnsManager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

                    //赋参数
                    SetXMLNodeAttribute("exam_do_mode_code", "'" + examInfo.examDoModeCode + "'", style, xmlnsManager);
                    SetXMLNodeAttribute("exam_uid", "'" + examInfo.Id + "'", style, xmlnsManager);
                    SetXMLNodeAttribute("exam_grade_uid", "'" + gradeInfo.Id + "'", style, xmlnsManager);
                    SetXMLNodeAttribute("exam_is_only_upload_file", "", style, xmlnsManager);
                    SetXMLNodeAttribute("is_need_limited_time", "'" + examInfo.isNeedLimitedTime + "'", style, xmlnsManager);      
                    SetXMLNodeAttribute("file_server_web_root_path", "", style, xmlnsManager);

                    //将赋值后的xmldoc转成xmlReader对象，加载到xslt编译器中
                    XmlReader reader = XmlDocument2XmlReader(style);

                    trans.Load(reader, XsltSettings.TrustedXslt, resolver);
                }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(paperXml);
            if (examInfo.isMixOrder == "Y") //打乱题序
            {
                var paperNodeList = xmlDoc.DocumentElement.SelectNodes("exam_paper_nodes/exam_paper_node");
                for (var i = 0; i < paperNodeList.Count; i++)
                {
                    var paperNode = paperNodeList[i];
                    var examPaperNodeQuestionsNode = paperNode.SelectSingleNode("exam_paper_node_questions");
                    var paperNodeQuestionNodeList =
                        paperNode.SelectNodes("exam_paper_node_questions/exam_paper_node_question");
                    var paperNodeQuestionNum = paperNodeQuestionNodeList.Count;

                    //打乱选项的顺序
                    for (var j = 0; j < paperNodeQuestionNum; j++)
                    {
                        var tempNode = paperNodeQuestionNodeList[j];
                        var question_base_type_code = tempNode.SelectSingleNode("question_base_type_code").Value;
                        //如果是单选或多选，则拿出两个选项来打乱
                        if (question_base_type_code == "single" || question_base_type_code == "multi" ||
                            question_base_type_code == "eva_single" || question_base_type_code == "eva_multi")
                        {
                            var selectAnswersNode = tempNode.SelectSingleNode("select_answers");
                            var selectAnswerNodeList = tempNode.SelectNodes("select_answers/select_answer");
                            var selectAnswerNum = selectAnswerNodeList.Count;
                            var randomNum1 = 0;
                            var randomNum2 = 0;
                            if (selectAnswerNum > 1) //最少两个选项才要做
                            {
                                do
                                {
                                    randomNum1 = GetRandomNum(0, selectAnswerNum - 1);
                                    randomNum2 = GetRandomNum(0, selectAnswerNum - 1);
                                } while (randomNum1 == randomNum2);
                                var tempNode1 = selectAnswerNodeList[randomNum1];
                                var tempNode2 = selectAnswerNodeList[randomNum2];
                                selectAnswersNode.RemoveChild(tempNode1);
                                selectAnswersNode.AppendChild(tempNode1);
                                selectAnswersNode.RemoveChild(tempNode2);
                                selectAnswersNode.AppendChild(tempNode2);
                            }
                        }
                        else if (question_base_type_code == "compose") //如果是组合题,打乱其下面的子试题选项
                        {
                            var composeChildNode = tempNode.SelectSingleNode("sub_exam_paper_node_questions");
                            var composeChildNodeList =
                                tempNode.SelectNodes("sub_exam_paper_node_questions/exam_paper_node_question");
                            var composeChildNodeNum = composeChildNodeList.Count;
                            for (var c = 0; c < composeChildNodeNum; c++)
                            {
                                var tempChildNode = composeChildNodeList[c];
                                question_base_type_code =
                                    tempChildNode.SelectSingleNode("question_base_type_code").Value;
                                //如果是单选或多选，则拿出两个选项来打乱
                                if (question_base_type_code == "single" || question_base_type_code == "multi" ||
                                    question_base_type_code == "eva_single" || question_base_type_code == "eva_multi")
                                {
                                    var selectAnswersNode = tempChildNode.SelectSingleNode("select_answers");
                                    var selectAnswerNodeList =
                                        tempChildNode.SelectNodes("select_answers/select_answer");
                                    var selectAnswerNum = selectAnswerNodeList.Count;

                                    var randomNum1 = 0;
                                    var randomNum2 = 0;
                                    if (selectAnswerNum > 1) //最少两个选项才要做
                                    {
                                        do
                                        {
                                            randomNum1 = GetRandomNum(0, selectAnswerNum - 1);
                                            randomNum2 = GetRandomNum(0, selectAnswerNum - 1);
                                        } while (randomNum1 == randomNum2);
                                        var tempNode1 = selectAnswerNodeList[randomNum1];
                                        var tempNode2 = selectAnswerNodeList[randomNum2];
                                        selectAnswersNode.RemoveChild(tempNode1);
                                        selectAnswersNode.AppendChild(tempNode1);
                                        selectAnswersNode.RemoveChild(tempNode2);
                                        selectAnswersNode.AppendChild(tempNode2);
                                    }
                                }
                            }
                        }
                    }

                    //用随机拿出两个删掉再加到后面的做法来打乱试题顺序
                    for (var j = 2; j < paperNodeQuestionNum; j++) //少掉2次
                    {
                        var randomNum1 = 0;
                        var randomNum2 = 0;
                        do
                        {
                            randomNum1 = GetRandomNum(0, paperNodeQuestionNum - 1);
                            randomNum2 = GetRandomNum(0, paperNodeQuestionNum - 1);
                        } while (randomNum1 == randomNum2);
                        var tempNode1 = paperNodeQuestionNodeList[randomNum1];
                        var tempNode2 = paperNodeQuestionNodeList[randomNum2];
                        examPaperNodeQuestionsNode.RemoveChild(tempNode1);
                        examPaperNodeQuestionsNode.AppendChild(tempNode1);
                        examPaperNodeQuestionsNode.RemoveChild(tempNode2);
                        examPaperNodeQuestionsNode.AppendChild(tempNode2);
                    }
                }
            }
            
            XmlReader xmlinput = XmlDocument2XmlReader(xmlDoc);//创建xmlreader
            StringWriter sw = new StringWriter();//输出字符串
            try
            {
                trans.Transform(xmlinput, null, sw);
            }
            catch (Exception ex)
            {
                // Logger.Error("合成试卷失败：" + ex.Message);
               // return string.Empty;
            }
            string xhtml = sw.ToString();

            sw.Close();
            sw.Flush();
            sw.Dispose();

            return xhtml;
        }

        /// <summary>
        /// 修改xmlDocument的节点属性值
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        /// <param name="xmlnsManager"></param>
        private static void SetXMLNodeAttribute(string attributeName, string value, XmlDocument style, XmlNamespaceManager xmlnsManager)
        {
            XmlNode node = style.SelectSingleNode("//xsl:param[@name='" + attributeName + "']", xmlnsManager);
            if (node == null) return;
            XmlElement xe = (XmlElement)node;
            if (attributeName == "exam_grade_uid")
            {
                xe.SetAttribute("select", value);
            }
            else if (attributeName == "file_server_web_root_path")
            {
                xe.SetAttribute("select", "'" + AppConfiguration.FileServerFileWebPathRoot + "'");
            }
            else if (attributeName == "exam_is_only_upload_file")
            {
             
                xe.SetAttribute("select", "'N'");
                
            }
            else
            {
                xe.SetAttribute("select", value);
            }
        }

        /// <summary>
        /// XmlDocument转xmlReader
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private static XmlReader XmlDocument2XmlReader(XmlDocument xmlDoc)
        {
            MemoryStream s = new MemoryStream();
            xmlDoc.Save(s);
            s.Seek(0, SeekOrigin.Begin);//一定要指定流的开头，否则无内容
            XmlReader input = XmlReader.Create(s);
            return input;
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int GetRandomNum(int min, int max)
        {
            var Range = max - min;
            var Rand = new Random();
            return min + Rand.Next(Range);
        }
    }
}
