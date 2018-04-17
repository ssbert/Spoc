jQuery.noConflict();
//定义需要用到的参数
var controlObject = null;
var commandReuqestUrl = "ExamRequestSmartGetCommand.aspx";
var examSysOption = null;
var examUid = null;
var examArrangeUid = null;
var examUserUid = null;
var examInfo = null;
var userExamTimes = null; //考生参加考试次数
var userExamObject = null;
var nosubmitGradeUid = "";
var localServerUrl = new GetExamUrl().GetUrl();
//考试时间相关
var userUsedTime = 0;
var userAllowAotuSaveTime = 0;
var currentQuestionUid = "";
var isQuestionLock = false;
var questionExamTime = 0;
var questionUserAnswerTime = 0;
var hasSubmitPaperError = false;
var isRaceEnd = false;
var isInPauseStatus = false;
var isAllowContextMenu = false;
var isAllowSelect = false;
var isAllowSystemKey = false;
var SmartReuqestCommand = null;

//代码编辑器组
require.config({ paths: { 'vs': '../monaco-editor/min/vs' }});
var codeEditors = [];
var codeEditorDic = {};
var programAnswerDic = {};

var oX, oY, oLeft, oTop, oWidth, oHeight; //存储原始移动前的位置
var divClone, oDiv;   //克隆的节点和原始节点
var oTime;
function webApi(url) {
    return "/api/services/app/" + url;
}
//clone拖移的节点
function setMove(obj) {
    if (event.button == 1 || event.button == 0) {
        if (oTime) {
            clearTimeout(oTime);
            divClone.parentNode.removeChild(divClone);
        }
        oDiv = obj.parentNode;
        divClone = oDiv.cloneNode(true);
        divClone.style.filter = "Alpha(opacity=50)";
        divClone.childNodes[1].setAttribute("onmousemove", function () { startMove(this) });
        divClone.childNodes[1].setAttribute("onmouseup", function () { endMove() });
        oX = parseInt(event.clientX);
        oY = parseInt(event.clientY);
        oLeft = parseInt(divClone.style.left);
        oTop = parseInt(divClone.style.top);
        document.body.appendChild(divClone);
        divClone.childNodes[1].setCapture();
        eventType = "move";
    }
}

//拖移
function startMove(obj) {
    if (eventType == "move" && event.button == 1) {
        var moveDiv = obj.parentNode;
        moveDiv.style.left = (oLeft + event.clientX - oX) + "px";
        moveDiv.style.top = (oTop + event.clientY - oY) + "px";
    }
}

//拖移结束调用动画
function endMove() {
    if (eventType == "move") {
        divClone.childNodes[1].releaseCapture();
        move(parseInt(divClone.style.left), parseInt(divClone.style.top));
        eventType = "";
    }
}

//移动的动画
function move(aimLeft, aimTop) {
    var nowLeft = parseInt(oDiv.style.left);
    var nowTop = parseInt(oDiv.style.top);
    var moveSize = 30;
    if (nowLeft > aimLeft + moveSize || nowLeft < aimLeft - moveSize || nowTop > aimTop + moveSize || nowTop < aimTop - moveSize) {
        oDiv.style.left = aimLeft > nowLeft + moveSize ? (nowLeft + moveSize) + "px" : aimLeft < nowLeft - moveSize ? (nowLeft - moveSize) + "px" : nowLeft + "px";
        oDiv.style.top = aimTop > nowTop + moveSize ? (nowTop + moveSize) + "px" : aimTop < nowTop - moveSize ? (nowTop - moveSize) + "px" : nowTop + "px";
        oTime = setTimeout("move(" + aimLeft + ", " + aimTop + ")", 1);
    }
    else {
        oDiv.style.left = divClone.style.left;
        oDiv.style.top = divClone.style.top;
        divClone.parentNode.removeChild(divClone);
        divClone == null;
    }
}

function loadXMLDoc() {
    try { //Internet Explorer

        var xmlDoc = new ActiveXObject("Microsoft.XMLDOM");

        xmlDoc.async = false;

        return xmlDoc;
    }
    catch (e) {
        try { //Firefox, Mozilla, Opera, etc.

            xmlDoc = document.implementation.createDocument("", "", null);
        }
        catch (e) {
            alert(e.message)
        }
    }
    try {
        xmlDoc.async = false;
        xmlDoc.load(dname);
        return (xmlDoc);
    }
    catch (e) {
        alert(e.message)
    }
    return (null);
}

function showLogMessage(type, message) {
    if (type == "completed") {
        $("completedMessage").innerHTML += "<dd>• " + message + "</dd>";
    }
    else {
        $("ongoingMessage").innerHTML = "<dd>• " + message + "</dd>";
    }
}

function checkReturnValue(returnValue) {
    if (returnValue == null)
        return false;

    if (returnValue.hasError != null) {
        ExamScriptManage.paperViewUtil.onRequestError("", returnValue.message);
        return false;
    }
    return true;
}

function getRequstParam() {
    //====获取考试安排的基本信息==========
    examUid = ExamScriptManage.Environment.getRequstParam("examUid");
    examArrangeUid = ExamScriptManage.Environment.getRequstParam("examArrangeUid");
    examUserUid = ExamScriptManage.Environment.getRequstParam("userUid");
    if (examUid == null || examUid.length == 0 || examUserUid.length == 0) {
        ExamScriptManage.paperViewUtil.onRequestError("", "");
        return;
    }
    else {
        //执行下一步操作
        ExamScriptManage.paperViewUtil.runNextStep();
    }
}

function loadExamInfo() {
    var examPaperUrl = "../../fileroot/CacheFile/Exam/" + examUid + "/exam_info.html";
    $j.ajax({
        type: "GET",
        url: examPaperUrl,
        dataType: "jsonp",
        jsonp: "callback",
        jsonpCallback: "ExamInfoCallBack",
        success: function (data) {
            completedLoadEaxmInfo(data);
        },
        error: function (data) {
            CompletedLoadErrorEaxmInfo();
        }
    });
}

function CompletedLoadErrorEaxmInfo() {
    $j.ajax({
        type: "GET",
        url: webApi("ExamExam/GetExamInfo?examUid=" + examUid),
        dataType: "json",
        contentType: 'application/json',
        success: function (data) {
            examInfo = data.result;
            userAllowAotuSaveTime = Math.round(Math.random() * 300) + examInfo.autoSaveSecond;
            //执行下一步操作
            ExamScriptManage.paperViewUtil.runNextStep();
        },
        error: function (data) {
            //服务端返回错误的数据格式
            if (data.status == 200) {
                var examInfo = JSON.parse(data.responseText);
                examInfo = examInfo.Result;
                userAllowAotuSaveTime = Math.round(Math.random() * 300) + examInfo.autoSaveSecond;
                //执行下一步操作
                ExamScriptManage.paperViewUtil.runNextStep();
            }

        }
    });

}

function completedLoadEaxmInfo(returnValue) {
    if (!checkReturnValue(returnValue))
        return;
    examInfo = returnValue;
    userAllowAotuSaveTime = Math.round(Math.random() * 300) + examInfo.autoSaveSecond;
    //执行下一步操作
    ExamScriptManage.paperViewUtil.runNextStep();
}

function loadUserExamData() {

    $j.ajax({
        type: "GET",
        url: webApi("/ExamExam/GetUserExamData?examUid=" + examUid + "&userUid=" + examUserUid + "&examArrangeUid=" + examArrangeUid),
        dataType: "json",
        contentType: 'application/json',
        success: function (data) {
            // var jsonData = JSON.parse(data.result);
            var jsonData = UtilPassJsonResult(data.result);
            completedLoadUserExamData(jsonData);
        },
        error: function (xmlHttpRequest) {
            ExamScriptManage.paperViewUtil.onRequestError(xmlHttpRequest.status, xmlHttpRequest.statusText);
        }
    });
}

function completedLoadUserExamData(returnValue) {
    if (!checkReturnValue(returnValue))
        return;
    this.examSysOption = returnValue.sysSetting;
    this.userExamTimes = returnValue.userExamTimes;
    this.nosubmitGradeUid = returnValue.nosubmitGradeUid;

    //执行下一步操作
    ExamScriptManage.paperViewUtil.runNextStep();
}

function checkUserExamData() {
    //===检查提示用户次数==========
    var examAllowTimes = examInfo.maxExamNum;
    var userTimes = userExamTimes.exam;
    if (examInfo.isExamination == "Y") {
        examAllowTimes = examInfo.examinationCount;
        userTimes = userExamTimes.reexam;
    }

    //没有未提交的试卷时才进行验证
    if (IsNullOrEmpty(this.nosubmitGradeUid)) {
        if (examAllowTimes > 0 && userTimes >= examAllowTimes) {
            ExamScriptManage.paperViewUtil.onRequestError("", "你已经超过了考试最大允许次数,不能参加考试。");
            return;
        }
    }
    if (examSysOption.IsCheckExamTimesBeforeExam) {
        if (examAllowTimes > 0 && userTimes < examAllowTimes) {
            if (!window.confirm("此项考试只能考" + examAllowTimes + "次，您还剩余" + (examAllowTimes - userTimes) + "次的考试机会，是否参加考试？")) {
                var SmartReuqestCommand = new newvSmartReuqestCommand(localServerUrl);
                SmartReuqestCommand.CloseWindow();
            } else {
                //ExamScriptManage.Request.requestUrl("../../Assets/exam/js/saveAnswerBySmart.js", "GET", "", "JS", null, true, null, ExamScriptManage.paperViewUtil.onRequestError);
                completedCheckUserExamData();
            }
        } else {
            //ExamScriptManage.Request.requestUrl("../../Assets/exam/js/saveAnswerBySmart.js", "GET", "", "JS", null, true, null, ExamScriptManage.paperViewUtil.onRequestError);
            completedCheckUserExamData();
        }
    } else {
        //ExamScriptManage.Request.requestUrl("../../Assets/exam/js/saveAnswerBySmart.js", "GET", "", "JS", null, true, null, ExamScriptManage.paperViewUtil.onRequestError);
        completedCheckUserExamData();
    }
    //===检查全屏要求========【考试闭卷_isOpenBook】  【考评模块启用增强型防作弊机制IsLockKeybordWhenCloseExam】
    //加载答案操作脚本【考评管理设置启用个性化设置 EnableClientIndividuation】
    //加载Xctivex操作的脚本文件
    //ExamScriptManage.Request.requestUrl("../../Assets/exam/js/saveAnswerBySmart.js", "GET", "", "JS", null, true, null, ExamScriptManage.paperViewUtil.onRequestError);
    //completedCheckUserExamData();
}

function completedCheckUserExamData() {
    ExamScriptManage.paperViewUtil.runNextStep();
}

function initAttendExam() {

    $j.ajax({
        type: "GET",
        url: webApi("/ExamExam/InitAttendExam?examUid=" + examUid + "&userUid=" + examUserUid + "&examArrangeUid=" + examArrangeUid),
        dataType: "json",
        contentType: 'application/json',
        success: function (data) {
            // var jsonData = JSON.parse(data.result);
            var jsonData = UtilPassJsonResult(data.result);
            jsonData.ExamGrade.id = jsonData.ExamGrade.Id;
            window.allowPasteCode = jsonData.AllowPasteCode;
            completedInitAttendExam(jsonData);
        },
        error: function (xmlHttpRequest) {
            ExamScriptManage.paperViewUtil.onRequestError(xmlHttpRequest.status, xmlHttpRequest.statusText);
        }
    });

}

function completedInitAttendExam(returnValue) {
    if (!checkReturnValue(returnValue))
        return;
    userExamObject = returnValue;
    ExamScriptManage.paperViewUtil.runNextStep();
}

function initPage() {
    ExamScriptManage.paperViewUtil.stepName = "getRequstParam";
    ExamScriptManage.paperViewUtil.runStep();
}

function initExamPaper() {

    //显示试卷信息
    if (examSysOption.IsShowPhotoWhenExamine) {
        //$("divUserPhoto").style.background = "url(" + userExamObject.UserInfo.PhotoFile + ") no-repeat";
        $("imgUserPhoto").src = userExamObject.UserInfo.PhotoFile;
    }
    else {
        $("divUserPhoto").style.display = "none";
    }
    $("labUserName").innerHTML = userExamObject.UserInfo.UserName;
    $("labUserCode").innerHTML = userExamObject.UserInfo.UserCode;
    if (userExamObject.ExamGrade.lastUpdateTime == undefined) {
        $("spanExamTime").innerHTML = "00:00:00";
    }
    else {
        $("spanExamTime").innerHTML = ""; //userExamObject.ExamGrade.lastUpdateTime;
    }
    $("divExamName").innerHTML = examInfo.examName;
    var paperQuestionInfo = "共" + userExamObject.ExamPaper.questionNum + "题";
    if (userExamObject.ExamPaper.isShowScore) {
        paperQuestionInfo += "共" + userExamObject.ExamPaper.totalScore + "分";
    }
    paperQuestionInfo += " 计时" + (Math.round((userExamObject.ExamGrade.allowExamTime / 60) * 100) / 100) + "分钟";
    $("paperQuestion").innerHTML = paperQuestionInfo;

    if (examInfo.examClassCode == "race") {
        $("divGateNumPanel").style.display = "";
        $("spanGateNum").innerHTML = userExamObject.ExamGrade.passGateNum + 1;
    }
    if (examInfo.isOpenBook == "N") {
        $("liSaveAndClose").style.display = "none";
    }
    //生成试卷时的提示信息
    if (examSysOption.AlterMessageWhenCreatingPaper != "") {
        ExamScriptManage.MessageBoxManager.create("提示", examSysOption.AlterMessageWhenCreatingPaper, true, false, null);
    }

    //生成试题界面
    ExamScriptManage.paperViewUtil.examGrade = userExamObject.ExamGrade;
    ExamScriptManage.paperViewUtil.examPaper = "" + userExamObject.ExamPaper + "";
    ExamScriptManage.paperViewUtil.paperXml = userExamObject.ExamPaper.paperXml;
    ExamScriptManage.paperViewUtil.examDoModeCode = examInfo.examDoModeCode;
    ExamScriptManage.paperViewUtil.paperTemplateText = userExamObject.ExamPaper.paperExtend01;
    ExamScriptManage.paperViewUtil.examInfo = examInfo;
    ExamScriptManage.paperViewUtil.isMixOrder = examInfo.isMixOrder;
    ExamScriptManage.paperViewUtil.examSysSetting = examSysOption;
    ExamScriptManage.paperViewUtil.paperTemplatePath = userExamObject.PaperTemplate;
    ExamScriptManage.paperViewUtil.questionJudgePolicy = userExamObject.QuestionJudgePolicy;
    ExamScriptManage.paperViewUtil.paperViewPanel = $("divPaperContent");
    ExamScriptManage.paperViewUtil.paperNavigatorPanel = $("divNavigatorPanel");
    SmartReuqestCommand = new newvSmartReuqestCommand();
    SmartReuqestCommand.SetlocalhostServerUrl(localServerUrl);
    ExamScriptManage.paperViewUtil.saveAsnwerObj = new saveAsnwerByControl();
    ExamScriptManage.paperViewUtil.saveAsnwerObj.controlObject = SmartReuqestCommand;
    ExamScriptManage.paperViewUtil.saveAsnwerObj.userExamAnswerPath = examSysOption.ExamAnswerSavePath;
    ExamScriptManage.paperViewUtil.saveAsnwerObj.examUserUid = examUserUid;
    var returnMessage = ExamScriptManage.paperViewUtil.LoadPaperView();
    if (returnMessage != null && returnMessage.length > 0) {
        ExamScriptManage.paperViewUtil.onRequestError("", returnMessage);
        return;
    }

    ExamScriptManage.paperViewUtil.SetUserAnswer();

    ExamScriptManage.paperViewUtil.LockExamWindow();
    if (examInfo.isOpenBook == "Y" && examSysOption.QuestionProtectionLevel == "low") {
        isAllowContextMenu = false;
        isAllowSelect = true;
        isAllowSystemKey = true;
    }
    //获取答卷
    //var examGradeUid = userExamObject.ExamGrade.id;
    var examUserUid = userExamObject.ExamGrade.userUid;
    //var examUid = userExamObject.ExamGrade.examUid;
    //var paperUid = userExamObject.ExamPaper.paperUid;
    //var urlTemplate = '[{ "userUid": "' + examUserUid + '", "examUid":"' + examUid + '" ,"examArrangeUid":"' + examArrangeUid + '" ,"examGradeUid":"' + examGradeUid + '" , "examPaperUid":"' + paperUid + '","recoverFromPartner": "false","recoverFromBox": "true"}]';
    //$j.ajax({
    //    type: "GET",
    //    url: localServerUrl + "/EXAMREQUEST.NEWV",
    //    data: "MethodName=ReadExamCache2&RequstArgs=" + urlTemplate,
    //    dataType: "jsonp",
    //    jsonp: "callback",
    //    jsonpCallback: "callback",
    //    success: function (data) {
    //    },
    //    error: function (data) {

    //    }
    //});


    //启动定时器
    userUsedTime = userExamObject.ExamGrade.examTime;
    ExamScriptManage.Timer.addEventListener("showExamTime", 1, showExamTime, true);
    if (examInfo.isRealTimeControll == "Y") {
        ExamScriptManage.Timer.addEventListener("GetCommandList", examSysOption.ControllerRefreshSecond, GetCommandList, true);
    }
    else {
        ExamScriptManage.Timer.addEventListener("UpdateExamTime", 200, UpdateExamTime, true);
    }
    //显示试卷页面
    $("maintop").style.display = "none";
    $("main").style.display = "";
}
////////////////返回试卷的具体内容////////////
//返回试卷的具体内容
function callback(data) {
    var localUserExamAnswer = "";
    try {
        if (data[0].isSuccess == "true") {
            var localUserExamAnswer = decodeURIComponent(data[0].fileContent);
        }
    }
    catch (ex)
    { }
    if (userExamObject == null)
        return;
    var userAnswer = userExamObject.ExamGrade.userAnswer;
    if (localUserExamAnswer != "") userAnswer = localUserExamAnswer;
    //加载考生答案
    //加载考生答案
    var xmlDoc = loadXMLDoc();
    xmlDoc.async = false;
    if (userAnswer != "" && userAnswer != null) {
        xmlDoc.loadXML(userAnswer);
    }
    if (xmlDoc.documentElement != null && xmlDoc.documentElement.selectSingleNode("current_paper_node_index")) {
        this.currentPaperNodeIndex = xmlDoc.documentElement.selectSingleNode("current_paper_node_index").text;
    }

    if (xmlDoc.documentElement != null && xmlDoc.documentElement.selectSingleNode("current_question_index") != null) {
        this.currentQuestionIndex = xmlDoc.documentElement.selectSingleNode("current_question_index").text;
    }
    if ($("trPaperNode_" + this.currentPaperNodeIndex) != null) $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
    if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
    if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";

    if (xmlDoc.documentElement != null) {
        var objNodeList = xmlDoc.documentElement.selectNodes("exam_answers/exam_answer");
        for (var i = 0; i < objNodeList.length; i++) {
            var examAnswerNode = objNodeList[i];
            var question_uid = examAnswerNode.selectSingleNode("question_uid").text;
            var answer_text = examAnswerNode.selectSingleNode("answer_text").text;
            var answer_time = 0;
            if (examAnswerNode.selectSingleNode("answer_time") != null) answer_time = eval(examAnswerNode.selectSingleNode("answer_time").text);
            var isSetBookmark = "N";
            if (examAnswerNode.selectSingleNode("is_set_bookmark") != null) isSetBookmark = examAnswerNode.selectSingleNode("is_set_bookmark").text;
            var isReadQuestion = "N";
            if (examAnswerNode.selectSingleNode("is_read") != null) isReadQuestion = examAnswerNode.selectSingleNode("is_read").text;

            if ($("hidQuestionBaseTypeCode_" + question_uid) == null) {
                //找不到该题则忽略
                continue;
            }
            var question_base_type_code = $("hidQuestionBaseTypeCode_" + question_uid).value;

            //赋上基本信息
            $("hidUserAnswerTime_" + question_uid).value = answer_time;
            if (isSetBookmark == "Y")
                this.SetQuestionBookmarkStatus(question_uid, isSetBookmark);
            if ($("hidIsRead_" + question_uid) != null)
                $("hidIsRead_" + question_uid).value = isReadQuestion;

            //赋上答案信息
            var arrAnswerOption = null;
            if (question_base_type_code == "single" || question_base_type_code == "multi" || question_base_type_code == "judge" || question_base_type_code == "eva_single" || question_base_type_code == "eva_multi") {
                arrAnswerOption = $name("Answer_" + question_uid);
                for (var j = 0; j < arrAnswerOption.length; j++) {
                    try {
                        if (("|" + answer_text + "|").indexOf("|" + arrAnswerOption[j].value + "|") > -1)
                            arrAnswerOption[j].checked = true;
                        else
                            arrAnswerOption[j].checked = false;
                    } catch (e) { }
                }
            }
            else if (question_base_type_code == "fill")		//填空题
            {
                var arrOneAnswer = answer_text.split("|");
                if (arrOneAnswer.length > 1) {
                    arrAnswerOption = $name("Answer_" + question_uid);
                    for (var j = 0; j < arrOneAnswer.length; j++) {
                        try {
                            arrOneAnswer[j] = arrOneAnswer[j].replace("&Vertical;", "|");
                            if (arrAnswerOption.length > j) {
                                arrAnswerOption[j].value = arrOneAnswer[j];
                                this.CheckFillQuestionAnswerLength(arrAnswerOption[j]);
                            }
                        } catch (e) { }
                    }
                }
                else {
                    try {
                        answer_text = answer_text.replace("&Vertical;", "|");
                        $("Answer_" + question_uid).value = answer_text;
                    } catch (e) { }
                }
            }
            else if (question_base_type_code == "judge_correct") {
                var checkBoxValue = answer_text;
                if (answer_text == "Y") {
                    checkBoxValue = "Y";
                    $("Answer_" + question_uid).value = "";
                }
                else {
                    checkBoxValue = "N";
                    answer_text = answer_text.replace("&Vertical;", "|");
                    $("Answer_" + question_uid).value = answer_text;
                }
                for (var j = 0; j < $name("JudgeCorrect_" + question_uid).length; j++) {
                    try {
                        if (("|" + checkBoxValue + "|").indexOf("|" + $name("JudgeCorrect_" + question_uid)[j].value + "|") > -1)
                            $name("JudgeCorrect_" + question_uid)[j].checked = true;
                        else
                            $name("JudgeCorrect_" + question_uid)[j].checked = false;
                    } catch (e) { }
                }
            }
            else {
                try {
                    answer_text = answer_text.replace("&Vertical;", "|");
                    $("Answer_" + question_uid).value = answer_text;
                } catch (e) { }
            }
        }
    }
    //检查考生答案
    var noAnswerQuestionCount = this.GetNoAnswerQuestionCount();
    if (this.examInfo.examDoModeCode == "question") {
        //转到第一题
        if (!isDoSaveUserAnswer) {
            this.GoToQuestion(1, 1, 1);
        }
    }

}

this.GetNoAnswerQuestionCount = function () {
    var total = 0;
    if ($("hidQuestionUid") == null) return;

    var questionUidList = $name("hidQuestionUid");
    total = questionUidList.length;
    var noAnswerQuestionNo = "";
    var noAnswerQuestionCount = 0;
    for (var i = 0; i < total; i++) {
        var questionUid = questionUidList[i].value;
        var hasAnswer = this.CheckOneContent(i);

        if (hasAnswer == false)      //没做
        {
            noAnswerQuestionCount = noAnswerQuestionCount + 1;
        }

        //设置试题的已做没做状态
        this.SetQuestionAnswerStatus(questionUid, hasAnswer);

    }
    return noAnswerQuestionCount;
}

this.CheckOneContent = function (i) {
    var questionUid = $name("hidQuestionUid")[i].value;
    var questionBaseTypeCode = $("hidQuestionBaseTypeCode_" + questionUid).value;

    if (questionBaseTypeCode == "compose") {
        return true;
    }
    if ($("Answer_" + questionUid) == null)
        return true; 	//没有答案的,只是文章

    var slen = $name("Answer_" + questionUid).length;
    if (questionBaseTypeCode == "single" || questionBaseTypeCode == "multi" || questionBaseTypeCode == "judge" || questionBaseTypeCode == "eva_single" || questionBaseTypeCode == "eva_multi") {
        if (slen <= 1) {
            if ($("Answer_" + questionUid).checked == true)
                return true;
        } else if (questionBaseTypeCode == "program") {
            var editor = codeEditorDic[questionUid];
            if (editor) {
                var code = editor.getValue();
                return code != "" && code != null;
            } else {
                return false;
            }
        } else {
            for (var j = 0; j < slen; j++) {
                if ($name("Answer_" + questionUid)[j].checked == true)
                    return true
            }
        }
    }
    else {
        if (slen <= 1) {
            if ($("Answer_" + questionUid).value != "")
                return true;
        } else {
            for (var j = 0; j < slen; j++) {
                if ($name("Answer_" + questionUid)[j].value != "")
                    return true
            }
        }
    }
    return false;
}
////////////////***************////////////
function showExamTime() {
    userUsedTime = userUsedTime + 1;
    var examTime = userExamObject.ExamGrade.allowExamTime;
    //处理自动保存答卷
    if (examInfo.autoSaveSecond > 0) {
        if (userUsedTime % userAllowAotuSaveTime == 0) {
            doSaveUserAnswer(true);
        }
    }
    else {
        if (examInfo.autoSaveToServer == "Y" && userUsedTime % 300 == 0) {
            doSaveUserAnswer(true);
        }
        else {
            if (userUsedTime % 900 == 0) {
                var clientDate = new Date();
                var returnvale = ExamScriptManage.paperViewUtil.SaveAnswerToLocal();
                if (returnvale == "errorexption") {
                    $("spAutoSaveMessage").innerHTML = (clientDate.getHours() >= 10 ? clientDate.getHours() : "0" + clientDate.getHours()) + ":" + (clientDate.getMinutes() >= 10 ? clientDate.getMinutes() : "0" + clientDate.getMinutes()) + "保存存在异常，请联系管理员！";
                }
                $("spAutoSaveMessage").innerHTML = (clientDate.getHours() >= 10 ? clientDate.getHours() : "0" + clientDate.getHours()) + ":" + (clientDate.getMinutes() >= 10 ? clientDate.getMinutes() : "0" + clientDate.getMinutes()) + "系统自动将答卷保存到本地系统";
            }
        }
    }
    if (examTime > 0) {

        //已用时间
        var timeMinute = Math.floor(userUsedTime / 60);
        var timeSecond = userUsedTime - timeMinute * 60;
        //剩余时间
        var leftExamTime = examTime - userUsedTime;
        var leftTimeMinute = Math.floor(leftExamTime / 60);
        var leftTimeSecond = leftExamTime - leftTimeMinute * 60;
        if (userUsedTime > examTime) {
            //停止时钟并自动提交答卷
            ExamScriptManage.Timer.stop();
            submitPaper(true);
        }
        else {
            //计时时间显示，countdown|倒计时;usagetime|正计时 Lopping 2012-09-03
            if (examSysOption.ComputeExamTimeTypeCode == "countdown") {
                $("spanExamTime").innerHTML = leftTimeMinute + "分" + leftTimeSecond + "秒";
            }
            if (examSysOption.ComputeExamTimeTypeCode == "usagetime") {
                $("spanExamTime").innerHTML = timeMinute + "分" + timeSecond + "秒";
            }
        }

        //考试剩余时间提示
        /*
         if (eval(leftExamTime) == eval(examSysOption.AlertLeftTimeWhenExaming)) {
         var leftTimeView = leftTimeMinute + "分";
         if (leftTimeSecond > 0)
         leftTimeView = leftTimeView + leftTimeSecond + "秒";
         else
         leftTimeView = leftTimeView + "钟";

         ExamScriptManage.MessageBoxManager.create("提示", "时间只剩" + leftTimeView + ",请抓紧时间准备交卷!", true, false, null);
         }
         */
        //处理单个试题的情况
        if (isQuestionLock == false && questionExamTime > 0 && examInfo.examDoModeCode == "question") {
            questionUserAnswerTime = questionUserAnswerTime + 1;
            if (questionUserAnswerTime >= questionExamTime) {
                ExamScriptManage.paperViewUtil.GoToNextQuestion();
            }
            else {
                $("divQuestionTimePanel_" + currentQuestionUid).innerHTML = questionExamTime - questionUserAnswerTime;
            }
        }
    }
    //每5分钟发一次心跳
    if (userUsedTime % 300 === 0) {
        jQuery.get("/ExamTask/Heartbeat");
    }
}

var isGetCommandRunning = false;
function GetCommandList() {
    if (isGetCommandRunning == true)
        return;
    isGetCommandRunning = true;
    ExamScriptManage.Request.requestUrl(commandReuqestUrl, "GET", "command=GetCommandList&examUid=" + examInfo.examUid + "&userUid=" + examUserUid + "&examGradeUid=" + userExamObject.ExamGrade.id, "JSON", null, false, GetCommandListSuc, GetCommandListErr);
}

function GetCommandListErr(errorCode, errorMessage) {
    isGetCommandRunning = false;
}

function GetCommandListSuc(returnData) {
    if (returnData.hasError == true || returnData.hasFoundCommand == "false") {
        isGetCommandRunning = false;
        return;
    }

    if (returnData.commandName == "ForceToSubmit") {
        ForceToSubmit("来自考试管理员的强行提交答卷命令：" + returnData.commandPara);
    }
    else if (returnData.commandName == "SubmitUserExam") {
        ForceToSubmit("来自考试管理员的强行提交答卷命令：" + returnData.commandPara);
    }
    else if (returnData.commandName == "SendMessage") {
        ExamScriptManage.MessageBoxManager.create("来自管理员的消息", returnData.commandPara, true, false, null);
    }
    else if (returnData.commandName == "PauseExam") {
        PauseExam(returnData.commandPara);
    }
    else if (returnData.commandName == "EndPauseExam") {
        EndPauseExam(returnData.commandPara);
    }
    else if (returnData.commandName == "DelayTime") {
        DelayTime(returnData.commandPara);
    }

    isGetCommandRunning = false;
}

var isUpdateExamTimeRunning = false;
function UpdateExamTime() {
    if (isUpdateExamTimeRunning == true)
        return;
    isUpdateExamTimeRunning = true;
    ExamScriptManage.Request.requestUrl("/ExamExam/InitAttendExam", "GET", "examUid=" + examInfo.examUid + "&examGradeUid=" + userExamObject.ExamGrade.id, "JSON", null, false, EndUpdateExamTime, EndUpdateExamTime);
}

function EndUpdateExamTime() {
    isUpdateExamTimeRunning = false;
}

function GetQuestionUserAnswerTime() {
    return questionUserAnswerTime;
}

function GetQuestionExamTime() {
    return questionExamTime;
}

function GetUserExamTime() {
    return userUsedTime;
}

function GetExamTime() {
    return userExamObject.ExamGrade.allowExamTime;
}

function GetOfficeDTFileWebPath() {
    return userExamObject.UserInfo.WebPath + userExamObject.UserInfo.OfficeDTFileWebPath;
}

function GetWebServerUrl() {
    return userExamObject.UserInfo.WebServerUrl;
}

function GetUserUid() {
    return userExamObject.ExamGrade.userUid;
}

function GetExamGradeUid() {
    return userExamObject.ExamGrade.id;
}

function SetRaceEnd() {
    isRaceEnd = true;
}

function jscomNewOpenMaxWindow(url, target) {
    var tt, w, left, top, width, height;
    width = screen.width;
    height = screen.height - 60;
    left = 0;
    if (left < 0) { left = 0; }

    top = 0;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

function DisableExamButton() {
    $("lnkSaveAndClose").disabled = true;
    $("lnkSaveAsnwer").disabled = true;
    $("lnkSubmitPaper").disabled = true;
    $("lnkCheckQuestion").disabled = true;
}

function InitQuestionTimeWhenByQuestion(questionUid, qExamTime, userAnswerTime, isLock) {
    currentQuestionUid = questionUid;
    questionExamTime = qExamTime;
    questionUserAnswerTime = userAnswerTime;
    isQuestionLock = isLock;
}

function CloseMe(isSaveAnswerToLocal) {
    ClearWindowCloseEvent();
    if (isSaveAnswerToLocal)
        ExamScriptManage.paperViewUtil.SaveAnswerToLocal();
    //下面是smartgate的调用，改为window.close();
    //SmartReuqestCommand.CloseWindow();
    window.close();
    //var SmartReuqestCommand = new newvSmartReuqestCommand(localServerUrl);

}

function ClearWindowCloseEvent() {
    window.onbeforeunload = null;
}

function SetWindowCloseEvent() {
    if (examInfo.isOpenBook == "Y") {
        window.onbeforeunload = function () {
            SaveAnswerToServerAndClose(false);
        }
    }
    else {
        window.onbeforeunload = function () {
            submitPaper(false);
        }
    }
}
var isDoSaveUserAnswer = false;
function doSaveUserAnswer(isAutoSave) {
    //isAutoSave用于区分是否自动保存
    isDoSaveUserAnswer = true;
    var clientDate = new Date();

    ExamScriptManage.paperViewUtil.SaveAnswerToServer(true, function (returnData) {
        if (isAutoSave) {
            $("spAutoSaveMessage").innerHTML = (clientDate.getHours() >= 10 ? clientDate.getHours() : "0" + clientDate.getHours()) + ":" + (clientDate.getMinutes() >= 10 ? clientDate.getMinutes() : "0" + clientDate.getMinutes()) + "系统自动将答卷保存到服务器";
        } else {
            $("spAutoSaveMessage").innerHTML = (clientDate.getHours() >= 10 ? clientDate.getHours() : "0" + clientDate.getHours()) + ":" + (clientDate.getMinutes() >= 10 ? clientDate.getMinutes() : "0" + clientDate.getMinutes()) + "手动将答卷保存到服务器";
        }
    }, function (errorCode, errorMessage) {
        $("spAutoSaveMessage").innerHTML = (clientDate.getHours() >= 10 ? clientDate.getHours() : "0" + clientDate.getHours()) + ":" + (clientDate.getMinutes() >= 10 ? clientDate.getMinutes() : "0" + clientDate.getMinutes()) + "由于网络故障，导致本次保存答卷失败，请与网络管理员联系！";
    });
}

function doSaveUserAnswerAndClose() {
    var buttons = new Array();
    buttons[0] = { "title": "确定", "className": "Nsb_layer_btb", "script": "SaveAnswerToServerAndClose(true)" };
    buttons[1] = { "title": "取消", "className": "Nsb_layer_btg", "script": "ExamScriptManage.MessageBoxManager.closeAll()" };
    ExamScriptManage.MessageBoxManager.create("提示", "您并未完成此次考试，请珍惜机会，确认暂时离开吗？", true, false, buttons);
}

function SaveAnswerToServerAndClose(isAsyn) {
    submitOpenPaperQuestion();
    ExamScriptManage.paperViewUtil.SaveUserAnswerAndClose(isAsyn);
}

function doSubmitUserAnswer() {
    var buttons = new Array();
    buttons[0] = { "title": "确定", "className": "Nsb_layer_btb", "script": "submitPaper(true)" };
    buttons[1] = { "title": "取消", "className": "Nsb_layer_btg", "script": "ExamScriptManage.MessageBoxManager.closeAll()" };
    ExamScriptManage.MessageBoxManager.create("提示", "您确定要提交答卷吗？", true, false, buttons);
}

function SubmitTransmissionData(mum, isAsyn) {
    $("lnkSubmitPaper").disabled = true;
    ExamScriptManage.MessageBoxManager.showWaitSubmit("正在交卷", "正在交卷…" + mum + "秒计时");
    if (mum <= 0) {
        submitOpenPaperQuestion();
        var returnMessage = ExamScriptManage.paperViewUtil.SubmitPaper(isAsyn, "userUid=" + examUserUid);
    }
    else {
        mum--;
        setTimeout("SubmitTransmissionData(" + parseInt(mum) + "," + isAsyn + ")", 1000);
    }
}


//防止onload中再次提交
var hasSubmitPaper = false;
function submitPaper(isAsyn) {
    ExamScriptManage.MessageBoxManager.closeAll();
    var num = 3 + Math.round(Math.random() * 7);
    SubmitTransmissionData(parseInt(num), isAsyn);
}



function ForceToSubmit(Message) {
    if (hasSubmitPaper == true) return;
    hasSubmitPaper = true;
    var paraMessage = "isForceToSubmit=true&forceReasonMessage=" + escape(Message) + "&userUid=" + examUserUid;
    submitOpenPaperQuestion();
    var returnMessage = ExamScriptManage.paperViewUtil.SubmitPaper(true, paraMessage);
}

function submitOpenPaperQuestion() {
    //提交弹出层的题目
    if ($("divOperatePanel").style.display != "none") {
        try {
            document.frames["ifrOperate"].submitPaperQuestion();
        } catch (e) { }

        CloseOperateWindow();
    }
}

function PauseExam(Message) {
    isInPauseStatus = true;
    ExamScriptManage.Timer.pauseAllListener();
    ExamScriptManage.Timer.activeListener("GetCommandList");
    ExamScriptManage.paperViewUtil.LockPaper();
    ExamScriptManage.MessageBoxManager.create("考试暂停中", Message, false, true, null);
}

function EndPauseExam(Message) {
    isInPauseStatus = false;
    ExamScriptManage.paperViewUtil.UnLockPaper();
    ExamScriptManage.Timer.activeAllListener();
    ExamScriptManage.MessageBoxManager.closeAll();
    ExamScriptManage.MessageBoxManager.create("结束暂停", Message, true, false, null);
}

function DelayTime(time) {
    var examTime = userExamObject.ExamGrade.allowExamTime;
    var delayTimeStr = ToTimeStrFromSecondStr(time);
    examTime = examTime + eval(time);
    var alterDelayTime = ToMinuteStrFromSecondStr(examTime);

    userExamObject.ExamGrade.allowExamTime = examTime;
    var paperQuestionInfo = "(共" + userExamObject.ExamPaper.questionNum + "题";
    if (userExamObject.ExamPaper.isShowScore) {
        paperQuestionInfo += "共" + userExamObject.ExamPaper.totalScore + "分";
    }
    paperQuestionInfo += " 计时" + (userExamObject.ExamGrade.allowExamTime / 60) + "分钟)";
    $("paperQuestion").innerHTML = paperQuestionInfo;

    ExamScriptManage.MessageBoxManager.create("延长考试时间", "延长考试时间：" + delayTimeStr, true, false, null);
}

//转换时间
function ToTimeStrFromSecondStr(secondTime) {
    var secondNum = parseInt(secondTime);
    var hourNum = parseInt(secondNum / 3600);
    var minuteReal = parseInt(secondNum % 3600);
    var minuteNum = parseInt(minuteReal / 60);
    var secondReal = parseInt(minuteReal % 60);
    if (parseInt(hourNum) < 10)
        hourNum = "0" + hourNum;
    if (parseInt(minuteNum) < 10)
        minuteNum = "0" + minuteNum;
    if (parseInt(secondReal) < 10)
        secondReal = "0" + secondReal;
    return hourNum + ":" + minuteNum + ":" + secondReal
}

function ToMinuteStrFromSecondStr(secondTime) {
    var minuteNum = parseInt(secondTime / 60);
    var secondReal = parseInt(secondTime % 60);
    var minuteStr = minuteNum + "分钟";
    if (secondReal > 0)
        minuteStr += secondReal + "秒";

    return minuteStr;
}

function GetExamUserAnswerForOneContent(questionUid) {
    try {
        return $("Answer_" + questionUid).value;
    } catch (e) {
        return "";
    }
}

function SetExamUserAnswerForOneContent(questionUid, ExamUserAnswer) {
    try {
        $("Answer_" + questionUid).value = ExamUserAnswer;
        SetQuestionAnswerStatus(questionUid, true);
    } catch (e) {
    }
}

function SetJudgeScoreForOneQuestion(questionUid, judgeScore) {
    try {
        $("hidJudgeScore_" + questionUid).value = judgeScore;
    } catch (e) {
    }
}

function UtilPassJsonResult(data) {

    var returnData = data;
    if (typeof returnData !== "string" || !returnData) {
        return null;
    }

    if (/^[\],:{}\s]*$/.test(returnData.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
        .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
        .replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) {

        return window.JSON && window.JSON.parse ? window.JSON.parse(returnData) : (new Function("return " + returnData))();
    } else {

        return null;
    }

}
function CloseOperateWindow() {
    ExamScriptManage.paperViewUtil.CloseOperateWindow();
}

//打开语音题窗口
function OpenContractVoiceFile(examGradeUid, questionUid, examUid, paperQuestionExamTime) {
    ExamScriptManage.paperViewUtil.OpenContractVoiceFile(examGradeUid, questionUid, examUid, paperQuestionExamTime);
}
//打开操作题窗口
function OpenContractTextFile(examGradeUid, questionUid, examUid, paperQuestionExamTime) {
    ExamScriptManage.paperViewUtil.OpenContractTextFile(examGradeUid, questionUid, examUid, paperQuestionExamTime);
}
//打字题窗口
var msTypingContentText = "";
function TypingOpenWindow(examGradeUid, questionUid, questionExamTime, paperQuestionScore) {
    ExamScriptManage.paperViewUtil.TypingOpenWindow(examGradeUid, questionUid, questionExamTime, paperQuestionScore);
}

function TypingGetContent() {
    return msTypingContentText;
}

function TypingBeginTyping(questionUid) {
    $("btnBeginTyping_" + questionUid).disabled = true;
}

//操作题只上传附件
var OpenUploadFileQuestion = "";
function OpenUploadFile(questionUid) {
    OpenUploadFileQuestion = questionUid;
    ExamScriptManage.paperViewUtil.OpenUploadFile(questionUid);
}

function GetUploadFileHtml(fileHtml) {
    if (fileHtml != "" && $("Answer_" + OpenUploadFileQuestion) != null) {
        $("Answer_" + OpenUploadFileQuestion).value = fileHtml;
        ExamScriptManage.paperViewUtil.SetQuestionAnswerStatus(OpenUploadFileQuestion, true);
    }
}

var OpenHtmlControlName = "";
function OpenHtmlEditor(controlName, fileRootPath) {
    OpenHtmlControlName = controlName;
    ExamScriptManage.paperViewUtil.OpenHtmlEditor(controlName, fileRootPath);
}

//HTML编辑器的回调函数(要进行反编码)
function ReturnHtmlEditorText(text) {
    text = unescape(text);
    $(OpenHtmlControlName).value = text;
}

//HTML编辑器回来取值时调用的函数
function GetHtmlText() {
    return $(OpenHtmlControlName).value;
}

function doFontSizeChanged() {
    if ($("radSmallFont").checked) {
        $("divPaperContent").className = "Nsb_exam_content";
    }
    else if ($("radMiddleFont").checked) {
        $("divPaperContent").className = "Nsb_exam_content_middle";
    }
    else if ($("radBigFont").checked) {
        $("divPaperContent").className = "Nsb_exam_content_big";
    }
}

//随机数
function RndNum(n) {
    var rnd = "";
    for (var i = 0; i < n; i++)
        rnd += Math.floor(Math.random() * 10);
    return rnd;
}
//账号多处登录
var getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
};
var userId = getUrlParam("userUid");
var guid = getUrlParam("guid");
var url = "http://" + window.location.host;
var stop = setInterval(function () {
    $j.ajax({
        url: url + "/User/IsLogOut?userId=" + userId + "&guid=" + guid,
        dataType: "jsonp",
        jsonp: "callback",
        jsonpCallback: "isLogOut",
        success: function (data) {
            if (data.IsLogOut == "true") {
                submitPaper(true);
                ExamScriptManage.MessageBoxManager.create("提示", "您的账号已在其他地方登录，请重新登录！", true, false, null);
                clearInterval(stop);
            }
        }
    });
}, 1000000000);

function GetFilePath(question_text) {
    var str = question_text;
    var filepath;
    var data;
    var old;
    var start = str.indexOf('<vcastr><channel><item><source>');
    if (start <= -1) return question_text;
    var length = '<vcastr><channel><item><source>'.length;
    var end = str.indexOf('</source></item></channel></vcastr>');
    filepath = str.substring(start + length, end);
    old = '<vcastr><channel><item><source>' + filepath + '</source></item></channel></vcastr>';
    data = '<vcastr><channel><item><source>' + filepath + '?r=' + RndNum(6) + '</source></item></channel></vcastr>';
    str = str.replaceAll(old, data)
    return str;
}
String.prototype.replaceAll = function (reallyDo, replaceWith, ignoreCase) {
    if (!RegExp.prototype.isPrototypeOf(reallyDo)) {
        return this.replace(new RegExp(reallyDo, (ignoreCase ? "gi" : "g")), replaceWith);
    } else {
        return this.replace(reallyDo, replaceWith);
    }
};


jQuery(function () {
    initPage();
});
