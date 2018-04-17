using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Aspose.Words;
using Aspose.Words.Saving;
using HtmlAgilityPack;
using SPOC.Exam;
using SPOC.Category;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.QuestionBank;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SPOC.Common.File;
using SPOC.Core;
using SPOC.Lib;
using Html2TextUtil = SPOC.Common.Helper.htmlparser.Html2TextUtil;

namespace SPOC.ExamPaper
{
    /*这些都是原来旧考试系统代码的移植代码，代码构成有点糟糕，先保证能运行再考虑重构，也许永远不会有重构的机会
     * by leente 2016/09/24
     * */
    /// <summary>
    /// 试卷导入hepler类
    /// 
    /// </summary>
    public class ImportExamPaperHelper:ImportQuestionBaseHelper
    {
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;

        private readonly Dictionary<string, string> _paperField = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _paperNodeField = new Dictionary<string, string>();
        private readonly Guid _paperFolderUid;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="folderUid"></param>
        /// <param name="paperFolderUid"></param>
        /// <param name="iNvFolderRep"></param>
        /// <param name="iExamQuestionTypeRep"></param>
        /// <param name="iUnitOfWorkManager"></param>
        /// <param name="iExamPaperRep"></param>
        /// <param name="iExamPaperNodeRep"></param>
        /// <param name="iExamPaperNodeQuestionRep"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iQuestionStandardCodeRep"></param>
        public ImportExamPaperHelper(string rootPath, Guid folderUid, Guid paperFolderUid, IRepository<NvFolder, Guid> iNvFolderRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IUnitOfWorkManager iUnitOfWorkManager, IRepository<Exam.ExamPaper, Guid> iExamPaperRep, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<Label, Guid> iLabelRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep,string newMoocApiUrl, string  language)
            : base(rootPath, folderUid, iNvFolderRep, iExamQuestionRep, iExamQuestionTypeRep, iUnitOfWorkManager, iQuestionStandardCodeRep, iLabelRep, iQuestionLabelRep, newMoocApiUrl, language)
        {
            _iExamPaperRep = iExamPaperRep;
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _paperFolderUid = paperFolderUid;
        }
        /// <summary>
        /// 从word文件中导入
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="errMsg">out 错误信息</param>
        public void ImportFormWord(Stream stream, out string errMsg)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var awd = new Document(stream);
            var fileName = "ImportFromWord";

            var filePath = Path.Combine(_savePath, fileName + ".html");
            
            var opt = new HtmlSaveOptions();
            opt.SaveFormat = SaveFormat.Html;
            opt.Encoding = Encoding.UTF8;
            awd.Save(filePath, opt);

            //修改html里的内容，只保留body内的数据，不包含body
            var document = new HtmlDocument();
            document.Load(filePath, Encoding.UTF8);
            var bodyDom = document.DocumentNode.Descendants("body").FirstOrDefault();
            if (bodyDom == null)
            {
                throw new UserFriendlyException("载入数据异常");
            }

           

            //var divDom = bodyDom.FirstChild;
            //divDom.RemoveChild(divDom.FirstChild);//去水印

            var innerHtml = bodyDom.InnerHtml;
            innerHtml = innerHtml.Replace(" text-align:justify;", "").Replace("&#xa0;", "");
            
            File.WriteAllText(filePath, innerHtml, Encoding.UTF8);
            
            var txtFilePath = Path.Combine(_savePath, fileName + ".txt");
            string errorMessage = Html2TextUtil.ConvertHtml2TxtFile(filePath, txtFilePath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new UserFriendlyException("载入数据异常");
            }

            string content;
            try
            {
                content = File.ReadAllText(txtFilePath, Encoding.UTF8);
                //content = HttpUtility.HtmlDecode(content);
                content = ReplaceImportData(content);
            }
            catch (Exception)
            {
                throw new UserFriendlyException("载入数据异常");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new UserFriendlyException("载入数据异常");
            }


            errMsg = string.Empty;
            //=======1. 初始化数据 ==========
            int currPaperNodeIndex = 0;
            int currPaperNodeQuestionIndex = 0;
            bool hasChild = false;
            int childNodeQuesitonIndex = 0;
            Exam.ExamPaper paperRow = null;
            ExamPaperNode currPaperNodeRow = null;
            List<Guid> updatedPaperNodeUids = new List<Guid>();

            //公共属性
            Hashtable publicProperty = new Hashtable();
            publicProperty.Clear();
            //var questionFolderRootUid =
            //    _iNvFolderRep.FirstOrDefault(a => string.IsNullOrEmpty(a.parentUid) && a.folderTypeCode == "question_bank")
            //        .id;

            //publicProperty.Add("parentUid", questionFolderRootUid.ToString());//试题分类
            var preQuestionUid = Guid.Empty;
            //组合题总分
            decimal composeQuestionScore = 0;
            //当前试题的ID
            var questionUid = Guid.Empty;
            var paperUid = Guid.Empty;
            var questionMUid = Guid.Empty;
            try
            {
                string sContentList = ReplaceHtml(content);
                //加载试卷字段
                LoadPaperField();
                //加载试卷大题字段
                LoadPaperNodeField();

                //对内容里的回车进行编码
                sContentList = EnterCharEconde(sContentList);
                sContentList = sContentList + "\r\n\r\n";
                int i = 0;
                int nPos = sContentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                while (nPos > -1)
                {
                    try
                    {
                        //=============2. 取公共属性 ================
                        var sOneContent = sContentList.Substring(0, nPos);
                        if (sOneContent != "" && sOneContent.StartsWith("[") && (sOneContent.IndexOf("]:", StringComparison.Ordinal) >= 0 || sOneContent.IndexOf("]：", StringComparison.Ordinal) >= 0))
                        {
                            #region 公共属性
                            sOneContent = sOneContent.Replace("：", ":");
                            string sPropertyName = sOneContent.Substring(1, sOneContent.IndexOf("]:", StringComparison.Ordinal) - 1);
                            string sPropertyValue = sOneContent.Substring(sOneContent.IndexOf("]:", StringComparison.Ordinal) + 2);
                            if (sPropertyName == ("试卷信息"))
                            {
                                #region 试卷信息
                                sOneContent = sPropertyValue.Trim();
                                paperRow = ExamImportAndExportUtil.ConvertTextToPaperRow(sOneContent, _paperField, cookie.Id, _iExamPaperRep);
                                paperRow.subjectUid = Guid.Empty;
                                paperRow.departmentUid = Guid.Empty;
                                paperRow.folderUid = _paperFolderUid;
                                paperRow.totalScore = 0;
                                paperRow.questionNum = 0;
                                paperRow.courseUid = Guid.Empty;
                                if (paperRow.Id == Guid.Empty)
                                {
                                    paperRow.Id = Guid.NewGuid();
                                    _iExamPaperRep.Insert(paperRow);
                                }
                                else
                                {
                                    _iExamPaperRep.Update(paperRow);
                                }
                                paperUid = paperRow.Id;
                                _iUnitOfWorkManager.Current.SaveChanges();
                                #endregion
                            }
                            else if (sPropertyName == ("试卷大题信息"))
                            {
                                #region 试卷大题信息
                                if (paperRow == null)
                                {
                                    errMsg = errMsg + ("[错误：]") + ("在试卷大题前未发试卷或信息") + Environment.NewLine;
                                    break;
                                }
                                if (currPaperNodeRow != null)
                                {
                                    //如果当前试卷大题不为空,保存信息
                                    //_iExamPaperNodeRep.Update(currPaperNodeRow);
                                    //_iUnitOfWorkManager.Current.SaveChanges();

                                    paperRow.totalScore += currPaperNodeRow.totalScore;
                                    paperRow.questionNum += currPaperNodeRow.questionNum;
                                    currPaperNodeQuestionIndex = 0;
                                    childNodeQuesitonIndex = 0;
                                }

                                //开始新的大题信息
                                sOneContent = sPropertyValue.Trim();
                                currPaperNodeRow = ExamImportAndExportUtil.ConvertTextToPaperNodeRow(sOneContent, _paperNodeField, paperRow.Id, null, _iExamPaperNodeRep, _iExamQuestionTypeRep);
                                currPaperNodeRow.listOrder = currPaperNodeIndex;
                                currPaperNodeRow.paperUid = paperRow.Id;
                                currPaperNodeRow.totalScore = 0;
                                currPaperNodeRow.questionNum = 0;
                                if (currPaperNodeRow.Id == Guid.Empty)
                                {
                                    currPaperNodeRow.Id = Guid.NewGuid();
                                    _iExamPaperNodeRep.Insert(currPaperNodeRow);
                                }
                                else
                                {
                                    _iExamPaperNodeRep.Update(currPaperNodeRow);
                                }

                                //清除大题下的试题;
                                if (_iExamPaperNodeQuestionRep.GetAll().Any(a => a.paperNodeUid == currPaperNodeRow.Id))
                                {
                                    var row = currPaperNodeRow;
                                    _iExamPaperNodeQuestionRep.Delete(a => a.paperNodeUid == row.Id);
                                }

                                _iUnitOfWorkManager.Current.SaveChanges();

                                updatedPaperNodeUids.Add(currPaperNodeRow.Id);
                                currPaperNodeIndex += 1;
                                #endregion
                            }
                            else if (sPropertyName == ("父知识点"))
                            {
                                if (publicProperty.ContainsKey("parentUid"))
                                {
                                    publicProperty["parentUid"] = GetFolderUidByPath("paper", publicProperty["parentUid"].ToString(), sPropertyValue, cookie.Id, true, null);
                                }
                                else
                                {
                                    publicProperty.Add("parentUid", GetFolderUidByPath("paper", publicProperty["parentUid"].ToString(), sPropertyValue, cookie.Id, true, null));
                                }
                            }

                            else if (sPropertyName == "试题分类")        ////试题目录
                            {
                                if (publicProperty.ContainsKey("folderName"))
                                {
                                    publicProperty["folderName"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("folderName", sPropertyValue);
                                }
                            }
                            else if (sPropertyValue != "" && sPropertyName == "过期日期") ////过期时间
                            {
                                if (publicProperty.ContainsKey("outdatedDate"))
                                {
                                    publicProperty["outdatedDate"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("outdatedDate", sPropertyValue);
                                }
                            }
                            else if (sPropertyValue != "" && sPropertyName == "试题状态")   //状态
                            {
                                if (publicProperty.ContainsKey("questionStatusCode"))
                                {
                                    publicProperty["questionStatusCode"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("questionStatusCode", sPropertyValue);
                                }
                            }
                            else if (sPropertyName == "题型")  //题型
                            {
                                if (publicProperty.ContainsKey("questionTypeUid"))
                                {
                                    publicProperty["questionTypeUid"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("questionTypeUid", sPropertyValue);
                                }
                            }
                            else if (sPropertyName == ("开始子试题"))  //开始子试题
                            {
                                composeQuestionScore = 0;
                                questionMUid = questionUid;
                                if (publicProperty.ContainsKey("parentQuestionUid"))
                                {
                                    publicProperty["parentQuestionUid"] = preQuestionUid;
                                }
                                else
                                {
                                    publicProperty.Add("parentQuestionUid", preQuestionUid);
                                }
                                childNodeQuesitonIndex = 0;
                                hasChild = true;
                            }
                            else if (sPropertyName == ("结束子试题"))  //结束子试题
                            {
                                if (publicProperty.ContainsKey("parentQuestionUid"))
                                {
                                    publicProperty["parentQuestionUid"] = "";
                                }
                                else
                                {
                                    publicProperty.Add("parentQuestionUid", "");
                                }
                                childNodeQuesitonIndex = 0;
                                hasChild = false;
                                //更新父试题paper_question_score
                                if (preQuestionUid != Guid.Empty || paperUid != Guid.Empty)
                                {
                                    var uid = paperUid;
                                    var mUid = questionMUid;
                                    var examPaperNodeQuestionRow = _iExamPaperNodeQuestionRep.FirstOrDefault(a => a.paperUid == uid && a.questionUid == mUid);
                                    if (examPaperNodeQuestionRow != null && examPaperNodeQuestionRow.paperQuestionScore != composeQuestionScore)
                                    {
                                        examPaperNodeQuestionRow.paperQuestionScore = composeQuestionScore;
                                        _iExamPaperNodeQuestionRep.Update(examPaperNodeQuestionRow);
                                        _iUnitOfWorkManager.Current.SaveChanges();
                                    }
                                }
                                composeQuestionScore = 0;
                            }
                            //else if (sPropertyName == ("父试题分类"))  //父试题分类
                            //{
                            //    string newParentFolderUid = "";
                            //    if (string.IsNullOrEmpty(sPropertyValue))
                            //    {
                            //        newParentFolderUid = "3daa769b-5549-4eb5-92ec-4f70292c50ac";
                            //    }
                            //    else
                            //    {
                            //        var folderRep = new NvFolderRepository();
                            //        newParentFolderUid = folderRep.GetFolderUidByPath("question", null, sPropertyValue, CookieHelper.GetCookie().fullName, true, null);
                            //    }
                            //    publicProperty["parent_uid"] = newParentFolderUid;
                            //}
                            else
                            {
                                if (publicProperty.ContainsKey(sPropertyName))
                                {
                                    publicProperty[sPropertyName] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add(sPropertyName, sPropertyValue);
                                }
                            }
                            #endregion
                        }
                        else if (sOneContent != "" && !sOneContent.StartsWith("//"))
                        {
                            Dictionary<string, string> questionField = new Dictionary<string, string>();
                            Dictionary<string, string> tempField = GetTableFieldList();
                            questionField.Clear();
                            foreach (string key in tempField.Keys)
                            {
                                questionField.Add(key, tempField[key]);
                            }
                            var retValue = ConvertTextToQuestion(sOneContent, questionField, publicProperty, cookie.Id);
                            if (retValue.HasError)
                            {
                                errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1)) + retValue.Message + Environment.NewLine;
                            }
                            else
                            {
                                ExamQuestion quRow = (ExamQuestion)retValue.GetValue("question");
                                preQuestionUid = quRow.Id;
                                //更新试卷信息及试卷大题信息
                                ExamPaperNodeQuestion nodeQuestonRow = new ExamPaperNodeQuestion();
                                if (hasChild)
                                {
                                    nodeQuestonRow.listOrder = childNodeQuesitonIndex;
                                    childNodeQuesitonIndex += 1;
                                }
                                else
                                {
                                    nodeQuestonRow.listOrder = currPaperNodeQuestionIndex;
                                    currPaperNodeQuestionIndex += 1;
                                }
                                composeQuestionScore += quRow.score;
                                nodeQuestonRow.paperNodeUid = currPaperNodeRow.Id;
                                nodeQuestonRow.paperQuestionScore = quRow.score;
                                nodeQuestonRow.paperUid = paperRow.Id;
                                nodeQuestonRow.questionUid = quRow.Id;
                                //检验一份试卷中是否有相同的题目
                                var dt = _iExamPaperNodeQuestionRep.GetAll().Where(
                                    a => a.paperNodeUid.Equals(currPaperNodeRow.Id) &&
                                         a.questionUid.Equals(quRow.Id)).ToList();

                                if (dt.Count > 0)
                                {
                                    errMsg = errMsg + ("[错误：]第") + (i + 1) + ("题导入失败:该大题中已存在与该题相同的题目") + Environment.NewLine;
                                }
                                else
                                {
                                    nodeQuestonRow.Id = Guid.NewGuid();
                                    _iExamPaperNodeQuestionRep.Insert(nodeQuestonRow);
                                    _iUnitOfWorkManager.Current.SaveChanges();
                                }

                                if (quRow.questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                                {
                                    #region 如果不为组合题
                                    currPaperNodeRow.totalScore += quRow.score;
                                    currPaperNodeRow.questionNum += 1;
                                    _iExamPaperNodeRep.Update(currPaperNodeRow);
                                    _iUnitOfWorkManager.Current.SaveChanges();
                                    #endregion
                                }
                                //如果是从Word导入,则处理图片
                                if (retValue.HasError == false)
                                {
                                    string sContentText = quRow.questionText;
                                    string selectAnswer = quRow.selectAnswer;
                                    string questionAnslysis = quRow.questionAnalysis ?? "";
                                    string standardAnswer = quRow.standardAnswer;
                                    string sSourceFilePath = FilePathUtil.GetOppositeUserTempPath(cookie.Id.ToString());
                                    bool hasCopyContentFile = false;
                                    var retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref sContentText, sSourceFilePath);
                                    if (retValueCopyFile.HasError)
                                    {
                                        if (retValueCopyFile.Message != "")
                                            errMsg = errMsg + string.Format(("[错误：]第{0}题导入试题内容成功，但处理文件失败:"), (i + 1)) + retValueCopyFile.Message + Environment.NewLine;
                                    }
                                    else
                                    {
                                        hasCopyContentFile = true;
                                    }
                                    bool hasCopySelectAnswerFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref selectAnswer, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopySelectAnswerFile = true;
                                    bool hasCopyStandardAnswerFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref standardAnswer, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopyStandardAnswerFile = true;
                                    bool hasCopyQuestionAnslysisFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref questionAnslysis, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopyQuestionAnslysisFile = true;

                                    if (hasCopyContentFile || hasCopySelectAnswerFile || hasCopyStandardAnswerFile || hasCopyQuestionAnslysisFile)
                                    {
                                        quRow.questionText = sContentText;
                                        quRow.selectAnswer = selectAnswer;
                                        quRow.standardAnswer = standardAnswer;
                                        quRow.questionAnalysis = questionAnslysis;

                                        _iExamQuestionRep.Update(quRow);
                                        _iUnitOfWorkManager.Current.SaveChanges();
                                    }
                                }
                                else
                                {
                                    errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1)) + retValue.Message + Environment.NewLine;
                                }
                            }
                            i = i + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1)) + ex.Message + Environment.NewLine;
                    }
                    sContentList = sContentList.Substring(nPos + 4);
                    nPos = sContentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                }

                //全部导入,保存最后一个大题信息及试卷的信息
                //_iExamPaperNodeRep.Update(currPaperNodeRow);
                //_iUnitOfWorkManager.Current.SaveChanges();
                if (currPaperNodeRow.totalScore != 0 && currPaperNodeRow.questionNum != 0)
                {
                    paperRow.totalScore += currPaperNodeRow.totalScore;
                    paperRow.questionNum += currPaperNodeRow.questionNum;
                    _iExamPaperRep.Update(paperRow);
                    _iUnitOfWorkManager.Current.SaveChanges();
                }
                //db.SaveChanges();
                //删除试卷下面多余的大题

                List<ExamPaperNode> dtPaperNode = _iExamPaperNodeRep.GetAll().Where(a => a.paperUid == paperRow.Id).ToList();
                var hasDelete = false;
                foreach (ExamPaperNode paperNodeRow in dtPaperNode)
                {
                    var dPaperNodeUid = paperNodeRow.Id;
                    if (!updatedPaperNodeUids.Contains(dPaperNodeUid))
                    {
                        hasDelete = true;
                        _iExamPaperNodeRep.Delete(paperNodeRow);
                        if (_iExamPaperNodeQuestionRep.GetAll().Any(a => a.paperNodeUid == dPaperNodeUid))
                        {
                            _iExamPaperNodeQuestionRep.Delete(a => a.paperNodeUid == dPaperNodeUid);
                        }
                    }
                }
                if (hasDelete)
                {
                    _iUnitOfWorkManager.Current.SaveChanges();
                }
                //errMsg = errMsg + ("[提示：]导入试卷完毕");
            }
            catch (Exception ex)
            {
                errMsg = errMsg + ex.Message;
            }
        }

        
        private void LoadPaperField()
        {
            _paperField.Clear();
            _paperField.Add("id", "试卷ID");
            _paperField.Add("paperName", "试卷名称");
            _paperField.Add("paperCode", "试卷编号");
            _paperField.Add("planTotalScore", "计划分数");
            _paperField.Add("isShowScore", "是否显示分数");
            _paperField.Add("isSingleAsMulti", "单选变为不定项");
            _paperField.Add("paperClassCode", "试卷类型");
        }

        private void LoadPaperNodeField()
        {
            _paperNodeField.Clear();
            _paperNodeField.Add("id", "试卷大题ID");
            _paperNodeField.Add("paperNodeName", "试卷大题名称");
            _paperNodeField.Add("questionScore", "每题分数");
            _paperNodeField.Add("planQuestionNum", "计划题目数");
            _paperNodeField.Add("questionTypeUid", "题型");
            _paperNodeField.Add("paperNodeDesc", "试卷大题说明");
        }

        /// <summary>
        /// 获取分类id 
        /// </summary>
        /// <param name="folderTypeCode"></param>
        /// <param name="parentUidStr"></param>
        /// <param name="path"></param>
        /// <param name="creatorUid"></param>
        /// <param name="isCreateWhenNotFound"></param>
        /// <param name="folderRow"></param>
        /// <returns></returns>
        public Guid GetFolderUidByPath(string folderTypeCode, string parentUidStr, string path, Guid creatorUid, bool isCreateWhenNotFound, NvFolder folderRow)
        {
            var parentUid = string.IsNullOrEmpty(parentUidStr)? Guid.Empty : new Guid(parentUidStr);
            //处理相关数据
            path = path.Trim('/').Replace("\\", "/");
            string[] arrPaths = path.Split('/');


            var firstOrDefault = _iNvFolderRep
                    .GetAll()
                    .FirstOrDefault(a => a.parentUid == Guid.Empty && a.folderTypeCode == folderTypeCode);
            var folderId = Guid.Empty;
            if (firstOrDefault != null)
            {
                folderId = firstOrDefault.Id;
            }
            if (parentUid == Guid.Empty)
            {
                parentUid = folderId;
            }
            var rootFolderUid = folderId;
            var rootName = folderTypeCode.Equals("question_bank") ? "试题分类" : "试卷分类";

            int startIndex = 0;
            var path0 = Regex.Replace(arrPaths[0], @"^\s|\s&", "");
            if (path0 == rootName && parentUid == rootFolderUid)//如果选择的目标节点不是根节点，则应从0开始循环新增 
                startIndex = 1;
            //查找Folder
            for (int i = startIndex; i < arrPaths.Length; i++)
            {
                var foldername = arrPaths[i];
                //var folder = db.NvFolder.FirstOrDefault(a => a.folder_type_code.Equals(folderTypeCode) &&
                //    a.parent_uid.Equals(parentUid) && a.folder_name.Equals(foldername));
                var folder = (from e in _iNvFolderRep.GetAll()
                              where e.folderTypeCode.Equals(folderTypeCode) && e.parentUid == parentUid && e.folderName == foldername
                              select new
                              {
                                  e.Id
                              }).FirstOrDefault();

                if (folder != null)
                {
                    parentUid = folder.Id;
                }
                else
                {
                    if (isCreateWhenNotFound)
                    {
                        var uid = parentUid;
                        NvFolder parentRow = _iNvFolderRep.FirstOrDefault(a => a.Id == uid);
                        NvFolder nfrRow = new NvFolder
                        {
                            createTime = DateTime.Now,
                            creatorUid = creatorUid,
                            folderLevel = parentRow.folderLevel + 1,
                            folderName = arrPaths[i],
                            folderTypeCode = parentRow.folderTypeCode,
                            Id = Guid.NewGuid()
                        };
                        nfrRow.fullPath = parentRow.fullPath.Trim(',') + "," + nfrRow.Id;
                        nfrRow.hasChild = "N";
                        //nfrRow.isActive = "Y";
                        nfrRow.listOrder = 0;
                        nfrRow.parentUid = parentRow.Id;
                        nfrRow.remarks = "";

                        nfrRow.listOrder = _iNvFolderRep.GetAll().Where(a => a.folderTypeCode == folderTypeCode).Max(a => a.listOrder) + 1;
                        
                        //用于导入分类----------------
                        if (folderRow != null)
                        {
                            nfrRow.folderCode = folderRow.folderCode;
                            nfrRow.remarks = folderRow.remarks;
                            if (folderRow.listOrder.ToString().Trim() != "")
                            {
                                nfrRow.listOrder = folderRow.listOrder;
                            }
                        }
                        //用于导入分类----------------
                        _iNvFolderRep.Insert(nfrRow);

                        parentRow.hasChild = "Y";
                        _iNvFolderRep.Update(parentRow);

                        _iUnitOfWorkManager.Current.SaveChanges();
                        parentUid = nfrRow.Id;
                        UpdateChildFolderPath(nfrRow.Id);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return parentUid;
        }

        /// <summary>
        /// 更新子分类的路径(包括ID和全名)
        /// </summary>
        /// <param name="folderUid"></param>
        /// <returns></returns>
        public ReturnValue UpdateChildFolderPath(Guid folderUid)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            NvFolder nvFolderRow = _iNvFolderRep.FirstOrDefault(a => a.Id == folderUid);

            if (nvFolderRow == null)
            {
                retValue.HasError = true;
                retValue.Message = ("找不到记录。");
                return retValue;
            }
            //string sql = string.Empty;
            //sql = "SELECT folder_uid,folder_name FROM nv_folder WHERE parent_uid='" + folderUid + "'";
            var dtChildFolder = _iNvFolderRep.GetAll().Where(a => a.parentUid == folderUid).ToList();

            //更新是否有子节点
            string hasChild = "N";
            if (dtChildFolder.Count > 0)
            {
                hasChild = "Y";
            }
            //sql = "UPDATE nv_folder SET has_child='" + hasChild + "' WHERE folder_uid='" + folderUid + "'";
            //db.Database.ExecuteSqlCommand(sql);

            _iNvFolderRep.Update(folderUid, f => { f.hasChild = hasChild; });
            _iUnitOfWorkManager.Current.SaveChanges();

            foreach (var t in dtChildFolder)
            {
                var childFolderUid = t.Id;
                //string childFolderName = dtChildFolder[i].folderName;
                //sql = "UPDATE nv_folder SET folder_level=" + (nvFolderRow.folder_level + 1) + ",full_folder_uid=" + StringUtil.QuotedToDBStr(nvFolderRow.full_folder_uid + "," + childFolderUid) + ",full_folder_name=" + StringUtil.QuotedToDBStr(nvFolderRow.full_folder_name + "/" + childFolderName) + " WHERE folder_uid='" + childFolderUid + "'";
                //db.Database.ExecuteSqlCommand(sql);
                _iNvFolderRep.Update(childFolderUid, f =>
                {
                    f.folderLevel = nvFolderRow.folderLevel + 1;
                    f.fullPath = nvFolderRow.fullPath + "," + childFolderUid;
                });
                retValue = UpdateChildFolderPath(childFolderUid);
                if (retValue.HasError)
                {
                    return retValue;
                }
            }
            _iUnitOfWorkManager.Current.SaveChanges();
            return retValue;
        }
        
    }
}