using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Threading;
using Abp.UI;
using Castle.Core.Logging;
using Newtonsoft.Json;
using SPOC.Category;
using SPOC.Common;
using SPOC.Exam;
using SPOC.Common.Cookie;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.Common.Http;
using SPOC.Core;
using SPOC.Lib;
using SPOC.Lib.Dto;
using SPOC.QuestionBank.Const;
using DateTimeUtil = newv.common.DateTimeUtil;

namespace SPOC.QuestionBank
{

    public class ImportQuestionBaseHelper
    {
        protected readonly string _savePath;
        protected readonly string _newMoocApiUrl;
        protected readonly string _language;
        protected readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        protected readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        protected readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        protected readonly IRepository<QuestionStandardCode, Guid> _iQuestionStandardCodeRep;
        protected readonly IRepository<Label, Guid> _iLabelRep;
        protected readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        protected readonly Guid _folderUid;
        protected readonly IUnitOfWorkManager _iUnitOfWorkManager;
        protected readonly ILogger _iLogger;
        private const string Pattern = @"(?isx)
                      <({0})\b[^>]*>                  #开始标记“<tag...>”
                          (?>                         #分组构造，用来限定量词“*”修饰范围
                              <\1[^>]*>  (?<Open>)    #命名捕获组，遇到开始标记，入栈，Open计数加1
                          |                           #分支结构
                              </\1>  (?<-Open>)       #狭义平衡组，遇到结束标记，出栈，Open计数减1
                          |                           #分支结构
                              (?:(?!</?\1\b).)*       #右侧不为开始或结束标记的任意字符
                          )*                          #以上子串出现0次或任意多次
                          (?(Open)(?!))               #判断是否还有'OPEN'，有则说明不配对，什么都不匹配
                      </\1>                           #结束标记“</tag>”
                     ";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="folderUid"></param>
        /// <param name="iNvFolderRep"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iExamQuestionTypeRep"></param>
        /// <param name="iUnitOfWorkManager"></param>
        /// <exception cref="UserFriendlyException"></exception>
        public ImportQuestionBaseHelper(string rootPath, Guid folderUid, IRepository<NvFolder, Guid> iNvFolderRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IUnitOfWorkManager iUnitOfWorkManager, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<Label, Guid> iLabelRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep,string newMoocApiUrl,string language)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            _savePath = Path.Combine(rootPath, "temp", cookie.Id.ToString());
            _folderUid = folderUid;
            _language = language;
            try
            {
                if (!Directory.Exists(_savePath))
                {
                    Directory.CreateDirectory(_savePath);
                }
            }
            catch (Exception)
            {
                throw new UserFriendlyException("无法创建临时存储文件夹");
            }
            _newMoocApiUrl = newMoocApiUrl;
            _iNvFolderRep = iNvFolderRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iQuestionStandardCodeRep = iQuestionStandardCodeRep;
            _iLabelRep = iLabelRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iLogger = new NullLogger(); 
        }

        /// <summary>
        /// 去html化
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ReplaceHtml(string html)
        {
            return Regex.Replace(html, string.Format(Pattern, Regex.Escape("td")), Mymatch);
        }

        private string Mymatch(Match m)
        {
            return m.Value.Replace("<br/>", "");
        }

        /// <summary>
        /// 把多行内容里的回车替换掉。多行内容是以单独的一行{开头
        /// 如：
        /// {
        /// 题干第一行
        /// 题干第二行
        /// }
        /// 答案：{
        /// 答案第一行
        /// 答案第二行
        /// }
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string EnterCharEconde(string content)
        {
            var bracketsCharBegin = "$BracketCharBegin$";
            var bracketsCharEnd = "$BracketCharEnd$";
            var bracketBeginFlag = "$BracketBegin$";
            var bracketEndFlag = "$BracketEnd$";

            content = content.Replace("\\{", bracketsCharBegin);
            content = content.Replace("\\}", bracketsCharEnd);
            content = content.Replace("{\r\n", bracketBeginFlag);
            content = content.Replace("\r\n}", bracketEndFlag);
            content = content.Replace("}", bracketEndFlag);
            //中文的括号也是同样的
            content = content.Replace("\\｛", bracketsCharBegin);
            content = content.Replace("\\｝", bracketsCharEnd);
            content = content.Replace("｛\r\n", bracketBeginFlag);
            content = content.Replace("\r\n｝", bracketEndFlag);
            content = content.Replace("｝", bracketEndFlag);
            var sNewContent = string.Empty;
            var sLeftContent = content; //剩余的
            //找到开头
            var nBeginPos = sLeftContent.IndexOf(bracketBeginFlag, StringComparison.Ordinal);
            //找到结尾
            var nEndPos = sLeftContent.IndexOf(bracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            while (nBeginPos > -1 && nEndPos > -1)
            {
                //把前部分保存起来
                sNewContent = sNewContent + sLeftContent.Substring(0, nBeginPos + bracketBeginFlag.Length);
                //中间内容部分
                var middleContent = sLeftContent.Substring(nBeginPos + bracketBeginFlag.Length,
                    nEndPos + bracketEndFlag.Length - nBeginPos - bracketBeginFlag.Length); //中间部分内容
                //中间内容部分的回车换掉
                middleContent = middleContent.Replace("\r\n", "<br/>");
                //把转换后的内容保存起来
                sNewContent = sNewContent + middleContent;
                //把前部分去掉
                sLeftContent = sLeftContent.Substring(nEndPos + bracketEndFlag.Length);

                //找到开头
                nBeginPos = sLeftContent.IndexOf(bracketBeginFlag, StringComparison.Ordinal);
                //找到结尾
                nEndPos = sLeftContent.IndexOf(bracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            }
            //把后面的接上去
            sNewContent = sNewContent + sLeftContent;
            return sNewContent;
        }


        /// <summary>
        /// 获取一个字典，key是属性名，value是属性中文名
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> GetTableFieldList()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {"id", (GetFieldTitle("id"))},
                {"questionCode", (GetFieldTitle("questionCode"))},
                {"questionText", (GetFieldTitle("questionText"))},
                {"selectAnswer", (GetFieldTitle("selectAnswer"))},
                {"standardAnswer", (GetFieldTitle("standardAnswer"))},
                {"examTime", (GetFieldTitle("examTime"))},
                {"questionAnalysis", (GetFieldTitle("questionAnalysis"))},
                {"outdatedDate", (GetFieldTitle("outdatedDate"))},
                {"isAnswerByHtml", (GetFieldTitle("isAnswerByHtml"))},
                {"listOrder", (GetFieldTitle("listOrder"))},
                {"creator", (GetFieldTitle("creator"))},
                {"createTime", (GetFieldTitle("createTime"))},
                {"modifier", (GetFieldTitle("modifier"))},
                {"modifyTime", (GetFieldTitle("modifyTime"))},
                {"hardGrade", (GetFieldTitle("hardGrade"))},
                {"score", (GetFieldTitle("score"))},
                {"eachOptionScore", (GetFieldTitle("eachOptionScore"))},
                {"folderUid", GetFieldTitle("folderUid")},
                {"questionTypeUid", GetFieldTitle("questionTypeUid")},
                {"operateTypeCode", GetFieldTitle("operateTypeCode")},
                {"questionStatusCode", GetFieldTitle("questionStatusCode")},
                {"questionKnowledge", GetFieldTitle("questionKnowledge")},
                {"param", GetFieldTitle("param")},
                {"inputParam", GetFieldTitle("inputParam")},
                {"multiTest", GetFieldTitle("multiTest")},
                {"preinstallCode", GetFieldTitle("preinstallCode") },
                {"standardCode", GetFieldTitle("standardCode") },
                {"label", GetFieldTitle("label") },
                {"auxiliaryLabel", GetFieldTitle("auxiliaryLabel") }
            };

            return dictionary;
        }

        private string GetFieldTitle(string fieldName)
        {
            switch (fieldName)
            {
                case "id":
                    return "系统编号";
                case "folderUid":
                    return "试题分类";
                case "parentQuestionUid":
                    return "上级试题系统编号";
                case "questionTypeUid":
                    return "题型";
                case "source":
                    return "试题来源";
                case "questionBaseTypeCode":
                    return "基本题型编号";
                case "operateTypeCode":
                    return "操作题类型";
                case "examTime":
                    return "答题时间";
                case "questionText":
                    return "试题内容";
                case "hardGrade":
                    return "难度";
                case "score":
                    return "分数";
                case "selectAnswer":
                    return "可选答案";
                case "selectAnswerScore":
                    return "可选答案分数";
                case "eachOptionScore":
                    return "每选项得扣分";
                case "answerNum":
                    return "可选答案个数";
                case "standardAnswer":
                    return "答案";
                case "questionAnalysis":
                    return "试题分析";
                case "outdatedDate":
                    return "过期日期";
                case "hasFile":
                    return "是否有文件";
                case "questionStatusCode":
                    return "试题状态";
                case "isAnswerByHtml":
                    return "是否用超文本答题";
                case "isOnlyUploadFile":
                    return "只允许上传附件";
                case "listOrder":
                    return "试题排序号";
                case "creator":
                    return "创建者";
                case "createTime":
                    return "创建时间";
                case "modifier":
                    return "修改者";
                case "modifyTime":
                    return "修改时间";
                case "approveUserUid":
                    return "审批者ID";
                case "approveUserName":
                    return "审批者名称";
                case "approveTime":
                    return "审批时间";
                case "questionKnowledge":
                    return "所属知识点";
                case "questionCode":
                    return "试题编号";
                case "param":
                    return "程序参数";
                case "inputParam":
                    return "输入流参数";
                case "multiTest":
                    return "多次测试";
                case "preinstallCode":
                    return "预设代码";
                case "standardCode":
                    return "参考代码";
                case "label":
                    return "主标签";
                case "auxiliaryLabel":
                    return "辅标签";
         
                default:
                    return "";
            }
        }



        /// <summary>
        /// 把本文内容转化为ExamQuestionRow对像并保存在数据中
        /// </summary>
        /// <param name="sOneContent"></param>
        /// <param name="questionField"></param>
        /// <param name="publicProperty"></param>
        /// <param name="operationUid">操作者用户ID</param>
        /// <returns></returns>
        protected  ReturnValue ConvertTextToQuestion(string sOneContent, Dictionary<string, string> questionField,
            Hashtable publicProperty, Guid operationUid)
        {
            var reValue = new ReturnValue(false, "");

            if (string.IsNullOrWhiteSpace(sOneContent))
                return reValue;
            var properties = GetContentProperty(sOneContent, questionField);

            //把其它的默认属性加上去
            foreach (string htKey in publicProperty.Keys)
            {
                if (properties.ContainsKey(htKey) == false)
                {
                    properties.Add(htKey, publicProperty[htKey].ToString());
                }
            }
            bool isNew;
            //取当前属性的QuestionUid,然后创建ExamQuestionRow对像,如果没有新添加.
            ExamQuestion quRow;
            var qUid = Guid.Empty;
            var questionType = "";
            var cFolder = "";
            var cFolderUid = Guid.Empty;
            foreach (var pkey in properties.Keys)
            {
                if (pkey == questionField["id"])
                {
                    qUid = Guid.Parse(properties[pkey]);
                }
                else if (pkey == questionField["questionTypeUid"])
                {
                    questionType = properties[pkey];
                }
                else if (pkey == questionField["folderUid"])
                {
                    cFolder = properties[pkey];
                }
            }
            if (qUid == Guid.Empty)
            {
                quRow = new ExamQuestion { Id = Guid.NewGuid() };
                isNew = true;
            }
            else
            {
                quRow = _iExamQuestionRep.Get(qUid);
                isNew = false;
                if (quRow == null)
                {
                    quRow = new ExamQuestion { Id = qUid };
                    isNew = true;
                }
            }
            //取本试题的题型
            if (string.IsNullOrEmpty(questionType) && publicProperty.ContainsKey("questionTypeUid"))
                questionType = ConvertUtil.ToString(publicProperty["questionTypeUid"]);

            var qtRow = _iExamQuestionTypeRep.FirstOrDefault(a => a.questionTypeName.Equals(questionType));
            if (qtRow != null)
            {
                quRow.questionTypeUid = qtRow.Id;
                quRow.questionBaseTypeCode = qtRow.questionBaseTypeCode;
            }
            //当前目录
            if (string.IsNullOrEmpty(cFolder) && publicProperty.ContainsKey("folderName"))
            {
                cFolder = ConvertUtil.ToString(publicProperty["folderName"]).Trim();
            }
            if (string.IsNullOrEmpty(cFolder))
            {
                cFolderUid = _folderUid;
            }
            else
            {
                var parentUid = _folderUid;
                var dt = _iNvFolderRep.GetAll().Where(a => a.folderName == cFolder && a.Id == parentUid).ToList();

                if (dt.Count > 0)
                {
                    cFolderUid = parentUid;
                }
                else
                {
                    var parentUidGuid = Guid.Empty;
                    if (parentUid != Guid.Empty)
                    {
                        parentUidGuid = parentUid;
                    }
                    cFolderUid = GetFolderUid("question_bank", parentUidGuid, cFolder, operationUid, true);
                }
            }
            //设置ExamQuestionRow对像的属性
            foreach (string pkey in properties.Keys)
            {
                if (pkey == questionField["id"])
                {
                    //不做处理
                }
                else if (pkey == questionField["questionTypeUid"])
                {
                    //题型(前面已赋值)
                }
                else
                {
                    string fieldTitle;
                    if (pkey == questionField["operateTypeCode"])
                    {
                        //操作题类型
                        fieldTitle = properties[pkey];
                        quRow.operateTypeCode = fieldTitle;
                    }
                    else if (pkey == questionField["questionStatusCode"])
                    {
                        //状态

                        quRow.questionStatusCode = "normal";

                        fieldTitle = properties[pkey];
                        switch (fieldTitle)
                        {
                            case "正常":
                                fieldTitle = "normal";
                                break;
                            case "已过期":
                                fieldTitle = "outdated";
                                break;
                            case "禁用":
                                fieldTitle = "disabled";
                                break;
                            default:
                                fieldTitle = "normal";
                                break;
                        }

                        quRow.questionStatusCode = fieldTitle;

                    }
                    else if (pkey == questionField["selectAnswer"])
                    {
                        //备选答案
                        var strSelectAnswer = properties[pkey];
                        var strSelectAnswerScore = "";
                        double questionScore = 0;
                        if (qtRow.questionBaseTypeCode == QuestionTypeConst.EvaluationSingle ||
                            qtRow.questionBaseTypeCode == QuestionTypeConst.EvaluationMulti ||
                            qtRow.questionBaseTypeCode == QuestionTypeConst.Answer ||
                            qtRow.questionBaseTypeCode == QuestionTypeConst.Program)
                        {
                            var arrSelectAnswer = properties[pkey].Split("|".ToCharArray());
                            strSelectAnswer = "";
                            foreach (string selectAnswer in arrSelectAnswer)
                            {
                                var startIndex = selectAnswer.LastIndexOf('(');
                                var endIndex = selectAnswer.LastIndexOf(')');
                                if (startIndex < 0 || startIndex >= endIndex)
                                {
                                    strSelectAnswerScore = strSelectAnswerScore + "0" + "|";
                                    strSelectAnswer = strSelectAnswer + selectAnswer + "|";
                                }
                                else
                                {
                                    double score =
                                        ConvertUtil.ToFloat(
                                            selectAnswer.Substring(startIndex + 1, endIndex - startIndex - 1)
                                                .Trim()
                                                .Replace("%", ""), 0);
                                    strSelectAnswerScore = strSelectAnswerScore + score + "|";
                                    strSelectAnswer = strSelectAnswer + selectAnswer.Substring(0, startIndex).TrimEnd() +
                                                      "|";
                                    if (qtRow.questionBaseTypeCode == QuestionTypeConst.EvaluationSingle)
                                    {
                                        if (questionScore < score) questionScore = score;
                                    }
                                    else
                                    {
                                        questionScore += score;
                                    }
                                }
                            }
                            strSelectAnswer = strSelectAnswer.Trim("|".ToCharArray());
                            strSelectAnswerScore = strSelectAnswerScore.Trim("|".ToCharArray());
                            quRow.answerNum = arrSelectAnswer.Length;

                            if (qtRow.questionBaseTypeCode == QuestionTypeConst.EvaluationSingle ||
                                qtRow.questionBaseTypeCode == QuestionTypeConst.EvaluationMulti)
                                quRow.score = (decimal)questionScore;
                        }
                        else if (qtRow.questionBaseTypeCode == QuestionTypeConst.Single ||
                                 qtRow.questionBaseTypeCode == QuestionTypeConst.Multi)
                        {
                            int asCount = properties[pkey].Split("|".ToCharArray()).Length;
                            quRow.answerNum = asCount;
                        }

                        quRow.selectAnswer = strSelectAnswer;
                        quRow.selectAnswerScore = strSelectAnswerScore;
                    }
                    else if (pkey == questionField["standardAnswer"])
                    {
                        //标准答案
                        string sdAnswer = properties[pkey];
                        sdAnswer = GetStandardAnswerText(quRow.questionBaseTypeCode, StringUtil.HtmlDecode(sdAnswer) );
                        //如果是多选题，对答案进行排序 lopping 2011-06-29
                        if (quRow.questionBaseTypeCode == QuestionTypeConst.Multi)
                        {
                            sdAnswer = QuestionUtil.OrderAnswerNumbers(sdAnswer);
                        }
                        quRow.standardAnswer = sdAnswer;
                    }
                    else if (pkey == questionField["examTime"])
                    {
                        //考试时间
                        if (!string.IsNullOrEmpty(properties[pkey]))
                        {
                            try
                            {
                                quRow.examTime = Convert.ToInt32(properties[pkey]);
                            }
                            catch (Exception)
                            {
                                quRow.examTime = 0;
                            }

                        }
                    }
                    else if (questionField.ContainsKey("score") && pkey == questionField["score"])
                    {
                        //分数
                        if (qtRow.questionBaseTypeCode != QuestionTypeConst.EvaluationSingle &&
                            qtRow.questionBaseTypeCode != QuestionTypeConst.EvaluationMulti)
                            quRow.score = ConvertUtil.ToDecimal(properties[pkey], 0);
                    }
                    else if (pkey == questionField["listOrder"])
                    {
                        //顺序号
                        quRow.listOrder = ConvertUtil.ToInt(properties[pkey], 0);
                        //quRow.QuestionCode = properties[pkey];
                    }
                    else if (pkey == questionField["questionCode"])
                    {
                        //编号
                        quRow.questionCode = properties[pkey];
                    }
                    else if (pkey == questionField["outdatedDate"])
                    {
                        //过期时间
                        if (!string.IsNullOrEmpty(properties[pkey]))
                            quRow.outdatedDate = (int)DateTimeUtil.ConvertToUnixTime(properties[pkey]);
                        else
                            quRow.outdatedDate = 0;
                    }
                    else if (pkey == questionField["hardGrade"])
                    {
                        //试题难度

                        quRow.hardGrade = properties[pkey];
                    }
                    else if (pkey == questionField["folderUid"])
                    {
                        //在前面已处理,这里不要做处理
                    }
                    else if (pkey == questionField["createTime"] || pkey == questionField["creator"])
                    {
                        if (isNew)
                        {
                            quRow = SetPropertity(GetDictionaryKeyByTitle(questionField, pkey), properties[pkey], quRow);
                        }
                    }
                    else if (pkey == questionField["eachOptionScore"])
                    {
                        //倒扣分数
                        quRow.eachOptionScore = ConvertUtil.ToDecimal(properties[pkey], 0);
                    }
                    else if (pkey == questionField["param"])
                    {
                        //程序参数
                        quRow.param = properties[pkey];
                    }
                    else if (pkey == questionField["inputParam"])
                    {
                        //输入流参数
                        quRow.InputParam = properties[pkey];
                    }
                    else if (pkey == questionField["multiTest"])
                    {
                        //多次测试
                        quRow.MultiTest = properties[pkey] == "是";
                    }
                    else if (pkey == questionField["preinstallCode"])
                    {
                        //预设代码
                        quRow.PreinstallCode = StringUtil.HtmlDecode(properties[pkey]);
                    }
                    //else if (pkey == questionField["isAnswerByHtml"])
                    //{
                    //    //倒扣分数
                    //    string isAnswerByHtml = properties[pkey].ToUpper();
                    //    quRow.isAnswerByHtml = isAnswerByHtml;
                    //}

                    else
                    {
                        string field = GetDictionaryKeyByTitle(questionField, pkey);

                        var exValue = properties[pkey];
                        if (!string.IsNullOrEmpty(field))
                            quRow = SetPropertity(field, exValue, quRow);
                    }
                }
            }
           


            if (publicProperty.ContainsKey("parentQuestionUid"))
            {
                string parentUid = ConvertUtil.ToString(publicProperty["parentQuestionUid"]);
                if (!string.IsNullOrEmpty(parentUid))
                {
                    quRow.parentQuestionUid = Guid.Parse(parentUid);
                }
            }

            quRow.folderUid = cFolderUid == Guid.Empty ? _folderUid : cFolderUid;
            //if (quRow.folderUid == Guid.Empty)
            //    quRow.folderUid = new Guid(publicProperty["parentUid"].ToString());

            quRow.modifier = quRow.modifier == Guid.Empty ? operationUid : quRow.modifier;
            quRow.modifyTime = DateTime.Now;
            //状态


            if (string.IsNullOrEmpty(quRow.questionStatusCode))
            {
                quRow.questionStatusCode = "normal";
            }
           

            //操作题类型
            if (quRow.questionBaseTypeCode == QuestionTypeConst.Operate && string.IsNullOrEmpty(quRow.operateTypeCode))
            {
                quRow.operateTypeCode = OperateTypeConst.Word;
            }
            if (quRow.creatorUid == Guid.Empty)
                quRow.creatorUid = operationUid;
            if (quRow.createTime == DateTime.MinValue)
                quRow.createTime = DateTime.Now;
            if (quRow.listOrder == 0)
            {
                quRow.listOrder = _iExamQuestionRep.Count() == 0
                    ? 0
                    : _iExamQuestionRep.GetAll().Max(a => a.listOrder) + 1;
            }
            //编号为空时
            if (string.IsNullOrEmpty(quRow.questionCode))
            {
                quRow.questionCode = CreateNewCode();
            }
            //if (string.IsNullOrEmpty(quRow.isAnswerByHtml))
            //    quRow.isAnswerByHtml = "N";
            var questionInput = new QuestionToCloudDto
            {
                ExamQuestion = quRow,
                QuestionStandardCodes = _iQuestionStandardCodeRep.GetAllList(a => a.isDefault == 0 && a.questionId.Equals(quRow.Id)),
                QuestionLabels = new List<QuestionLabel>()
            };
            //参考代码
            if (properties.Keys.Any(a => a.Equals("参考代码")))
            {
                var standardCode = StringUtil.HtmlDecode(properties["参考代码"]);
                _iQuestionStandardCodeRep.Delete(a => a.isDefault == 1 && a.questionId.Equals(quRow.Id));
                if (!string.IsNullOrWhiteSpace(standardCode))
                {
                    var standardCodeModel = new QuestionStandardCode
                    {
                        Id = Guid.NewGuid(),
                        questionId = quRow.Id,
                        code = standardCode,
                        isDefault = 1,
                        modifyTime = DateTime.Now,
                        type = "normal"
                    };
                    _iQuestionStandardCodeRep.Insert(standardCodeModel);
                    questionInput.QuestionStandardCodes.Add(standardCodeModel);
                }
            }
            //标签信息
            if (properties.Keys.Any(a => a.Equals("主标签")))
            {
                var labels = properties["主标签"].Split(',');
                var labelList = _iLabelRep.GetAll().Where(l => labels.Contains(l.title)).Select(l => l.Id).ToList();
                labelList.ForEach(label =>
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        questionId = quRow.Id,
                        questionType = "normal",
                        labelId = label,
                        labelType = 1
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                });
            }
            if (properties.Keys.Any(a => a.Equals("辅标签")))
            {
                var labels = properties["辅标签"].Split(',');
                var labelList = _iLabelRep.GetAll().Where(l => labels.Contains(l.title)).Select(l => l.Id).ToList();
                labelList.ForEach(label =>
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        questionId = quRow.Id,
                        questionType = "normal",
                        labelId = label,
                        labelType = 0
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                   
                });
            }
            if(quRow.questionBaseTypeCode.Contains("program"))
            {
                quRow.language = _language;
            }
            var success = CompileQuestionPublish(questionInput);
            if (!success)
            {
                reValue.Message = "试题云端存储保存失败!请联系管理员";
                reValue.HasError = true;
                return reValue;
            }
            //插入前先检查是否存在相同试题
            var row = quRow;
            var sameQuestions = _iExamQuestionRep.GetAll()
                    .Where(a => a.questionTypeUid == row.questionTypeUid 
                        && a.folderUid == row.folderUid 
                        && row.questionText == a.questionText 
                        && row.selectAnswer == a.selectAnswer 
                        && row.standardAnswer == a.standardAnswer
                        && row.creatorUid == a.creatorUid
                        ).WhereIf(row.parentQuestionUid != Guid.Empty, a=>a.parentQuestionUid == row.parentQuestionUid);
            if (!sameQuestions.Any()) //相同分类下不存在相同试题则导入
            {
                quRow.departmentUid = Guid.Empty;
                quRow.subjectUid = Guid.Empty;
                quRow.questionPureText = QuestionUtil.RemoveHtmlTag(quRow.questionText);
                _iExamQuestionRep.Insert(quRow);
                _iUnitOfWorkManager.Current.SaveChanges();
            }
            else
            {
                var score = quRow.score;
                quRow = sameQuestions.FirstOrDefault();
                if (score != quRow.score)
                {
                    quRow.score = score;
                    _iExamQuestionRep.Update(quRow);
                    _iUnitOfWorkManager.Current.SaveChanges();
                }
            }
            if (reValue.HasError)
            {
                return reValue;
            }
     
            //更新父试题分数
            if (quRow.parentQuestionUid != Guid.Empty)
            {
                var parentQuestion = _iExamQuestionRep.Get(quRow.parentQuestionUid);
                var score =
                    _iExamQuestionRep.GetAll()
                        .Where(a => a.parentQuestionUid == quRow.parentQuestionUid)
                        .Sum(a => a.score);
                parentQuestion.score = score;
                _iExamQuestionRep.UpdateAsync(parentQuestion);
            }

            if ((quRow.parentQuestionUid != Guid.Empty ||
                quRow.questionBaseTypeCode != QuestionTypeConst.Compose) &&
                string.IsNullOrEmpty(quRow.standardAnswer))
            {
                _iExamQuestionRep.Delete(quRow.Id);
                reValue.Message = "试题没有答案";
                reValue.HasError = true;
                return reValue;
            }
            reValue.PutValue("question", quRow);

            return reValue;
        }
        /// <summary>
        /// 编译题发布新课云
        /// </summary>
        /// <returns></returns>
        private  bool CompileQuestionPublish(QuestionToCloudDto question)
        {

            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(question));
            var apiContent = System.Web.HttpUtility.UrlEncode(jsonValue.ToString(),
                Encoding.UTF8);
            var url = _newMoocApiUrl + "/compile/question?sign=";
            var result = AsyncHelper.RunSync(()=> HttpHelper.PostResponseSerializeData<ApiResponseResult<dynamic>>(url, apiContent));
            return result.IsSuccess;
        }
        /// <summary>
        /// 把文本转化为属性列表
        /// </summary>
        /// <param name="sOneContent"></param>
        /// <param name="questionField"></param>
        /// <returns></returns>
        protected Dictionary<string, string> GetContentProperty(string sOneContent, Dictionary<string, string> questionField)
        {
            var reg = new Regex("^\r\n", RegexOptions.Multiline);

            if (reg.IsMatch(sOneContent))
            {
                sOneContent = reg.Replace(sOneContent, "");
            }


            var reg2 = new Regex("<p.*?>((.|\r|\n)+?)<\\/p>", RegexOptions.Multiline);
            if (reg2.IsMatch(sOneContent))
            {
                var matchs = reg2.Matches(sOneContent);
                var pReg = new Regex("<p.*?>");
                foreach (Match match in matchs)
                {
                    var str = pReg.Replace(match.Value, "").Replace("</p>", "");
                    sOneContent = sOneContent.Replace(match.Value, str);
                }
            }

            char[] arrSplit = { '\r', '\n' };
            var arrOneContent = sOneContent.Split(arrSplit);
            var properties = new Dictionary<string, string>();
            //=========1. 先取得题干信息==============
            var sReturn = arrOneContent[0];
            if (sReturn.IndexOf(".", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf(".", StringComparison.Ordinal))))
            {
                sReturn = sReturn.Substring(sReturn.IndexOf(".", StringComparison.Ordinal) + 1);
            }
            else if (sReturn.IndexOf("。", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf("。", StringComparison.Ordinal))))
            {
                sReturn = sReturn.Substring(sReturn.IndexOf("。", StringComparison.Ordinal) + 1);
            }
            else if (sReturn.IndexOf("．", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf("．", StringComparison.Ordinal))))
            {
                sReturn = sReturn.Substring(sReturn.IndexOf("．", StringComparison.Ordinal) + 1);
            }
            else if (sReturn.IndexOf(" ", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf(" ", StringComparison.Ordinal))))
            {
                sReturn = sReturn.Substring(sReturn.IndexOf(" ", StringComparison.Ordinal) + 1);
            }
            //对属性内容进行回车反编码
            sReturn = EnterCharDecode(sReturn);
            properties.Add(questionField["questionText"], sReturn);
            //=======2. 取试题序号 ===============
            sReturn = arrOneContent[0];
            if (sReturn.IndexOf(".", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf(".", StringComparison.Ordinal))))
            {
                sReturn = ConvertUtil.ToInt(sReturn.Substring(0, sReturn.IndexOf(".", StringComparison.Ordinal))).ToString();
            }
            else if (sReturn.IndexOf("。", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf("。", StringComparison.Ordinal))))
            {
                sReturn = ConvertUtil.ToInt(sReturn.Substring(0, sReturn.IndexOf("。", StringComparison.Ordinal))).ToString();
            }
            else if (sReturn.IndexOf("．", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf("．", StringComparison.Ordinal))))
            {
                sReturn = ConvertUtil.ToInt(sReturn.Substring(0, sReturn.IndexOf("．", StringComparison.Ordinal))).ToString();
            }
            //else if (sOneContent.IndexOf("、", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("、", StringComparison.Ordinal))))
            //{
            //    sOneContent = sOneContent.Substring(sOneContent.IndexOf("、", StringComparison.Ordinal) + 1);
            //}
            else if (sReturn.IndexOf(" ", StringComparison.Ordinal) > -1 && StringUtil.IsNumber(sReturn.Substring(0, sReturn.IndexOf(" ", StringComparison.Ordinal))))
            {
                sReturn = ConvertUtil.ToInt(sReturn.Substring(0, sReturn.IndexOf(" ", StringComparison.Ordinal))).ToString();
            }
            else
                sReturn = "0";
            properties.Add("序号", sReturn);

            //=======3. 备选答案 ===========
            sReturn = "";
            for (int i = 0; i < arrOneContent.Length; i++)
            {
                arrOneContent[i] = arrOneContent[i].Trim();
                if (arrOneContent[i].Length > 2 && !arrOneContent[i].StartsWith("//"))
                {
                    string sPrefix = arrOneContent[i].Substring(0, 1);
                    int ascii = sPrefix.ToCharArray()[0];
                    if (ascii >= 65 && ascii < 97)		//A-Z
                    {
                        if (arrOneContent[i].Substring(1, 1) == "." || arrOneContent[i].Substring(1, 1) == "。" || arrOneContent[i].Substring(1, 1) == "．" || arrOneContent[i].Substring(1, 1) == "、" || arrOneContent[i].Substring(1, 1) == " ")
                        {
                            sReturn = sReturn + arrOneContent[i].Substring(2).Trim().Replace('|', '︱') + "|";
                        }
                    }
                }
            }
            if (sReturn != "") sReturn = sReturn.Substring(0, sReturn.Length - "|".Length);
            //对属性内容进行回车反编码
            sReturn = EnterCharDecode(sReturn);
            properties.Add(questionField["selectAnswer"], sReturn);
            //========4. 取其它属性 ===============
            for (var i = 1; i < arrOneContent.Length; i++)
            {
                arrOneContent[i] = arrOneContent[i].Replace("：", ":");
                var index = arrOneContent[i].IndexOf(':');
                if (index > 0)
                {
                    var propertyName = arrOneContent[i].Substring(0, index);
                    if (questionField.ContainsValue(propertyName))
                    {
                        sReturn = arrOneContent[i].Substring(index + 1);
                        sReturn = sReturn.Trim();
                        //对属性内容进行回车反编码
                        sReturn = EnterCharDecode(sReturn);
                        //要检查是否有，防止重复写属性
                        if (!properties.ContainsKey(propertyName)) properties.Add(propertyName, sReturn);
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// 把内容里的回车反编码,并把标致去掉
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        private string EnterCharDecode(string sContent)
        {
            var sBracketsCharBegin = "$BracketCharBegin$";
            var sBracketsCharEnd = "$BracketCharEnd$";
            var sBracketBeginFlag = "$BracketBegin$";
            var sBracketEndFlag = "$BracketEnd$";
            var sEnterFlag = "$Enter$";
            var sNewContent = sContent;

            sNewContent = sNewContent.Replace(sBracketsCharBegin, "{");
            sNewContent = sNewContent.Replace(sBracketsCharEnd, "}");
            sNewContent = sNewContent.Replace(sEnterFlag, "\r\n");


            sNewContent = sNewContent.Replace(sBracketBeginFlag, "");
            sNewContent = sNewContent.Replace(sBracketEndFlag, "");
            return sNewContent;
        }

        /// <summary>
        /// 返回试题答案
        /// </summary>
        /// <param name="questionBaseTypeCode"></param>
        /// <param name="sAnswer"></param>
        /// <returns></returns>
        private string GetStandardAnswerText(string questionBaseTypeCode, string sAnswer)
        {
            sAnswer = sAnswer.TrimEnd();
            if ((questionBaseTypeCode == QuestionTypeConst.Judge || questionBaseTypeCode == QuestionTypeConst.JudgeCorrect) && (sAnswer == "正确" || sAnswer == "Y"))
                return "Y";
            else if ((questionBaseTypeCode == QuestionTypeConst.Judge || questionBaseTypeCode == QuestionTypeConst.JudgeCorrect) && (sAnswer == "错误" || sAnswer == "N"))
                return "N";
            else if (questionBaseTypeCode == QuestionTypeConst.Single || questionBaseTypeCode == QuestionTypeConst.Multi || questionBaseTypeCode == QuestionTypeConst.EvaluationMulti || questionBaseTypeCode == QuestionTypeConst.EvaluationSingle)
            {
                string errorMessage;
                sAnswer = QuestionUtil.FormatSelectQuestionAnswer(sAnswer, out errorMessage);
                return sAnswer;
            }
            else
            {
                return sAnswer;
            }
        }

        /// <summary>
        /// 通过字段名进行赋值
        /// </summary>
        private ExamQuestion SetPropertity(string fieldName, object propertityValue, ExamQuestion eq)
        {
            switch (fieldName)
            {
                case "id":
                    eq.Id = new Guid(propertityValue.ToString());
                    break;
                case "folderUid":
                    eq.folderUid = new Guid(propertityValue.ToString());
                    break;
                case "parentQuestionUid":
                    eq.parentQuestionUid = Guid.Parse(propertityValue.ToString());
                    break;
                //case "QuestionTypeUid":
                //    eq.QuestionTypeUid = (string)propertityValue;
                //    break;
                //case "source":
                //    eq.source = (string)propertityValue;
                //    break;
                case "questionBaseTypeCode":
                    eq.questionBaseTypeCode = (string)propertityValue;
                    break;
                case "operateTypeCode":
                    eq.operateTypeCode = (string)propertityValue;
                    break;
                case "examTime":
                    eq.examTime = Convert.ToInt32(propertityValue);
                    break;
                case "questionText":
                    eq.questionText = (string)propertityValue;
                    break;
                case "hardGrade":
                    eq.hardGrade = (string)propertityValue;
                    break;
                case "score":
                    eq.score = (decimal)propertityValue;
                    break;
                case "selectAnswer":
                    eq.selectAnswer = (string)propertityValue;
                    break;
                case "selectAnswerScore":
                    eq.selectAnswerScore = (string)propertityValue;
                    break;
                case "eachOptionScore":
                    eq.eachOptionScore = (decimal)propertityValue;
                    break;
                case "answerNum":
                    eq.answerNum = (int)propertityValue;
                    break;
                case "standardAnswer":
                    eq.standardAnswer = (string)propertityValue;
                    break;
                case "questionAnalysis":
                    eq.questionAnalysis = (string)propertityValue;
                    break;
                case "outdatedDate":
                    eq.outdatedDate = (int)propertityValue;
                    break;
                //case "hasFile":
                //    eq.hasFile = (string)propertityValue;
                //    break;
                case "questionStatusCode":
                    eq.questionStatusCode = (string)propertityValue;
                    break;
                //case "isAnswerByHtml":
                //    eq.isAnswerByHtml = (string)propertityValue;
                //    break;
                //case "isOnlyUploadFile":
                //    eq.isOnlyUploadFile = (string)propertityValue;
                //    break;
                case "listOrder":
                    eq.listOrder = (int)propertityValue;
                    break;
                case "creatorUid":
                    eq.creatorUid = new Guid(propertityValue.ToString());
                    break;
                case "createTime":
                    eq.createTime = DateTimeUtil.ToTime(propertityValue.ToString());
                    break;
                case "modifier":
                    eq.modifier = new Guid(propertityValue.ToString());
                    break;
                case "modifyTime":
                    eq.modifyTime = DateTimeUtil.ToTime(propertityValue.ToString());
                    break;

                //case "approveUserUid":
                //    eq.approveUserUid = (string)propertityValue;
                //    break;
                //case "approveUserName":
                //    eq.approveUserName = (string)propertityValue;
                //    break;
                //case "approveTime":
                //    eq.approveTime = (int)DateTimeUtil.ConvertToUnixTime(propertityValue.ToString());
                //    break;
                case "questionCode":
                    eq.questionCode = (string)propertityValue;
                    break;
            }
            return eq;
        }

        private string GetDictionaryKeyByTitle(Dictionary<string, string> dictionary, string title)
        {
            foreach (string key in dictionary.Keys)
            {
                if (title == dictionary[key])
                    return key;
            }
            return "";
        }

        /// <summary>
        /// 文本内容清理，去掉无用标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string ReplaceImportData(string content)
        {
            content = content.Replace("<b> </b>", "");
            content = content.Replace("<u> </u>", "");
            content = content.Replace("<i> </i>", "");
            content = content.Replace("<s> </s>", "");
            content = content.Replace("<sub> </sub>", "");
            content = content.Replace("<sup> </sup>", "");
            content = content.Replace("<b>{</b>", "{");
            content = content.Replace("<b>}<\b>", "}");
            content = content.Replace("<b>{ </b>", "{");
            content = content.Replace("<b>} <\b>", "}");
            content = content.Replace("\r\n \r\n", "\r\n\r\n").Replace("\r\n \r\n", "\r\n\r\n");
            return content;
        }
    
        private string CreateNewCode()
        {
            var code = "Q000001";
            var entity =
                _iExamQuestionRep.GetAll()
                    .Where(a => !a.isCustomCode)
                    .OrderByDescending(a => a.createTime)
                    .FirstOrDefault();
            if (entity != null)
            {
                code = entity.questionCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamQuestionRep.GetAll().Any(a => a.questionCode == code));
            }
            return code;
        }

        private string CreateFolderNewCode(string folderTypeCode)
        {
            var code = "F000001";
            var entity = _iNvFolderRep.GetAll().Where(a => !a.isCustomCode && a.folderTypeCode == folderTypeCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.folderCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iNvFolderRep.GetAll().Any(a => a.folderCode == code));
            }
            return code;
        }
        /// <summary>
        /// 根据条件获取folderUid
        /// </summary>
        /// <param name="folderTypeCode">分类类型编码</param>
        /// <param name="parentGuid">父分类名称</param>
        /// <param name="folderName">分类名称</param>
        /// <param name="creatorUid">创建者id</param>
        /// <param name="createWhenNotFound">当没找到时是否创建新的分类</param>
        /// <returns></returns>
        private Guid GetFolderUid(string folderTypeCode, Guid parentGuid, string folderName, Guid creatorUid,
            bool createWhenNotFound)
        {
            var folder = _iNvFolderRep.GetAll().FirstOrDefault(a => a.folderName == folderName && a.folderTypeCode == folderTypeCode && a.parentUid == parentGuid);
            if (folder != null)
            {
                return folder.Id;
            }

            if (!createWhenNotFound)
            {
                return Guid.Empty;
            }
            var parentFolder = _iNvFolderRep.GetAll().FirstOrDefault(a => a.Id == parentGuid);


            folder = new NvFolder()
            {
                Id = Guid.NewGuid(),
                folderName = folderName,
                creatorUid = creatorUid,
                createTime = DateTime.Now,
                hasChild = "N",
                folderTypeCode = folderTypeCode,
                remarks = string.Empty,
                isCustomCode = false,
                folderCode = CreateFolderNewCode(folderTypeCode)
            };

            if (parentFolder == null)
            {
                parentFolder = _iNvFolderRep.GetAll().FirstOrDefault(a => a.folderTypeCode == folderTypeCode && a.parentUid == Guid.Empty);
                if (parentFolder == null)
                {
                    throw new Exception("无效的folderTypeCode:" + folderTypeCode);
                }
            }

            var listOrderQueryable = _iNvFolderRep.GetAll().Where(a => a.parentUid == parentFolder.Id).Select(a => a.listOrder);
            folder.listOrder = listOrderQueryable.Any() ? listOrderQueryable.Max() + 1 : 0;
            folder.parentUid = parentFolder.Id;
            folder.folderLevel = parentFolder.folderLevel + 1;
            folder.fullPath = parentFolder.fullPath + "," + folder.Id;
            folder.isCustomCode = false;
            _iNvFolderRep.Insert(folder);
            _iUnitOfWorkManager.Current.SaveChanges();
            return folder.Id;
        }

        private Guid GetFolderUid(string folderTypeCode, string parentPath, Guid creatorUid, bool createWhenNotFound)
        {
            Guid parentUid = Guid.Empty;
            var parentNames = parentPath.Split('/');
            foreach (var parentName in parentNames)
            {
                parentUid = GetFolderUid(folderTypeCode, parentUid, parentName, creatorUid, createWhenNotFound);
            }
            return parentUid;
        }
    }
}