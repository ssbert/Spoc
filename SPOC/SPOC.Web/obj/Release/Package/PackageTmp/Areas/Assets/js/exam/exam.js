var serviceUrl = "/api/services/app/";
var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';
//考试信息
var ExamInfoClass = function (examUid, taskId) {
    var self = {};
    self.param = {
        examClassCode: "exam",
        examTypeCode: "exam_normal",
        examTimeModule: "join_exam",
        examStatusCode: "normal"
    };
    self.param.examUid = examUid;
    self.param.taskId = taskId;
    function init() {
        if (!stringIsEmpty(examUid)) {
            $(function () {
                getData(examUid);
            });
        } else {
            $(function () {
                $("#dg-container").hide();
            });
        }
        var onExamPaperSelected = function (paper) {
            self.paper = paper;
            $("#paperName").textbox("setValue", paper.paperName);
            $("#paperUid").val(paper.id);
            $("#paperTypeCode").val(paper.paperTypeCode);
            if (paper.paperTypeCode === "random") {
                $("#bufferPaperNumRow").show();
            } else if (paper.paperTypeCode === "fix") {
                $("#bufferPaperNumRow").hide();
            }
            parent.$("#tabs").tabs("close", getTabIndex())
            .tabs("select", self.tabIndex);
        };
        var evtHandle = evtBus.addEvt("exam_paper_selected", onExamPaperSelected);
        self.tabIndex = getTabIndex();
        $(window).unload(function () {
            evtBus.removeEvt(evtHandle);
        });
    }

    function showExamPaperSelector() {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        var url = "/Exam/Component/ExamPaperSelector";
        var title = "选择试卷";
        parent.$("#tabs").tabs("add", {
            title: title,
            content: iframeHtml.format({ title: title, url: url }),
            closable: true,
            icon: "icon-ok"
        });
    }

    function saveData(param) {
        if (!$("#edit-form").form("validate")) {
            return;
        }
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        var url = serviceUrl + "ExamExam/";
        if (guidIsEmpty(param.id)) {
            url += "Create";
        } else {
            url += "Update";
        }

        VE.Mask("");
        nv.post(url, param, function (data) {
            VE.UnMask();
            if (data.success) {
                var newExam = stringIsEmpty(param.id) ? data.result : param;
                notification(newExam, $.extend({}, self.param));
                setFormData(newExam);
                $.messager.show({ title: "提示", msg: "操作成功！" });
                evtBus.dispatchEvt("exam_changed");
            } else {
                $.messager.alert("提示", data.error.message, "info");
            }
        });
    }

    function getData(examUid) {
        var url = serviceUrl + "ExamExam/Get?id=" + examUid;
        VE.Mask("");
        nv.get(url, function (data) {
            VE.UnMask();
            if (data.success) {
                setFormData(data.result);
            } else {
                $.messager("提示", data.error.message, "info");
            }
        });
    }

    function setFormData(data) {
        var funcText = data.isCustomCode ? "uncheck" : "check";
        $("#isCustomCode").switchbutton(funcText);
        $("#examCode").textbox("setValue", data.examCode);
        $("#paperName").textbox("setValue", data.paperName);
        $("#paperUid").val(data.paperUid);
        $("#examName").textbox("setValue", data.examName);
        $("#edit-form input[name='examDoModeCode'][value='" + data.examDoModeCode + "']").attr("checked", true);
        $("#beginTime").datetimebox("setValue", data.beginTime);
        $("#endTime").datetimebox("setValue", data.endTime);
        $("#examTime").numberspinner("setValue", data.examTime / 60);
        $("#maxExamNum").numberspinner("setValue", data.maxExamNum);
        //changeCheckbox("isAllowSeeGrade", data.isAllowSeeGrade === "Y");
        //$("#allowSeeGradeDays").numberspinner("setValue", data.allowSeeGradeDays);
        //changeCheckbox("isOpenBook", data.isOpenBook === "Y");
        //changeCheckbox("isAllowSeePaper", data.isAllowSeePaper === "Y");
        //changeCheckbox("isAllowSeeAnswer", data.isAllowSeeAnswer === "Y");
        //changeCheckbox("isAllowModifyUserAnswer", data.isAllowModifyUserAnswer === "Y");
        //changeCheckbox("isAllowObjectJudge", data.isAllowObjectJudge === "Y");
        //changeCheckbox("isAllowModifyObjectAnswer", data.isAllowModifyObjcetAnswer === "Y"),
        //changeCheckbox("isDisplayResult", data.isDisplayResult === "Y");
        $("#edit-form input[name='autoSaveToServer'][value='" + data.autoSaveToServer + "']").attr("checked", true);
        $("#edit-form input[name='isMixOrder'][value='" + data.isMixOrder + "']").attr("checked", true);
        $("#autoSaveSecond").numberspinner("setValue", data.autoSaveSecond / 60); 
        $("#edit-form input[name='passGradeType'][value='" + data.passGradeType + "']").attr("checked", true);
        $("#passGradeRate").numberspinner("setValue", data.passGradeRate);
        $("#passGradeScore").numberspinner("setValue", data.passGradeScore);
        $("#paperTypeCode").val(data.paperTypeCode);
        if (data.paperTypeCode === "random") {
            $("#bufferPaperNumRow").show();
            $("#bufferPaperNum").numberspinner("setValue", data.bufferPaperNum);
        } else if (data.paperTypeCode === "fix") {
            $("#bufferPaperNumRow").hide();
        }
        
        if (data.autoSaveToServer === "Y") {
            $("#autoSaveSecondRow").show();
        } else {
            $("#autoSaveSecondRow").hide();
        }
        self.param = data;
    }

    function getFormData() {
        var param = $.extend({}, self.param);
        param.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
        param.examCode = $("#examCode").textbox("getValue");
        param.paperName = $("#paperName").textbox("getValue");
        param.paperUid = $("#paperUid").val();
        param.examName = $("#examName").textbox("getValue");
        param.examDoModeCode = $("#edit-form input[name='examDoModeCode']:checked").val();
        param.examTime = $("#examTime").numberspinner("getValue") * 60;
        param.beginTime = $("#beginTime").datetimebox("getValue");
        param.endTime = $("#endTime").datetimebox("getValue");
        param.maxExamNum = $("#maxExamNum").numberspinner("getValue");
        //param.isAllowSeeGrade = getCheckboxCodeValue("isAllowSeeGrade");
        //param.allowSeeGradeDays = $("#allowSeeGradeDays").numberspinner("getValue");
        //param.isOpenBook = getCheckboxCodeValue("isOpenBook");
        //param.isAllowSeePaper = getCheckboxCodeValue("isAllowSeePaper");
        //param.isAllowSeeAnswer = getCheckboxCodeValue("isAllowSeeAnswer");
        //param.isAllowModifyUserAnswer = getCheckboxCodeValue("isAllowModifyUserAnswer");
        //param.isAllowObjectJudge = getCheckboxCodeValue("isAllowObjectJudge");
        //param.isAllowModifyObjectAnswer = getCheckboxCodeValue("isAllowModifyObjectAnswer");
        //param.isDisplayResult = getCheckboxCodeValue("isDisplayResult");
        param.autoSaveToServer = $("#edit-form input[name='autoSaveToServer']:checked").val();
        param.isMixOrder = $("#edit-form input[name='isMixOrder']:checked").val();
        if (param.autoSaveToServer === "Y") {
            param.autoSaveSecond = $("#autoSaveSecond").numberspinner("getValue") * 60;
        } else {
            param.autoSaveSecond = 0;
        }
        param.passGradeType = $("#edit-form input[name='passGradeType']:checked").val();
        param.passGradeRate = $("#passGradeRate").numberspinner("getValue");
        param.passGradeScore = $("#passGradeScore").numberspinner("getValue");
        param.jsNeedJudge = param.isAllowModifyObjectAnswer;
        param.paperTypeCode = $("#paperTypeCode").val();
        if (param.paperTypeCode === "random") {
            param.bufferPaperNum = $("#bufferPaperNum").numberspinner("getValue");
        } else {
            param.bufferPaperNum = null;
        }
        return param;
    }

    function onChangeExamCodeMode(checked) {
        var isCustomCode = !checked;
        $("#examCode").textbox("readonly", !isCustomCode)
        .textbox({ required: isCustomCode });
        if (!isCustomCode) {
            $("#examCode").textbox("setValue", stringIsEmpty(self.param.examCode) ? "":self.param.examCode);
        }
    }

    function changeCheckbox(id, checked) {
        if (checked) {
            $("#" + id).attr("checked", true);
        } else {
            $("#" + id).removeAttr("checked");
        }
    }

    function getCheckboxCodeValue(id) {
        if ($("#" + id).prop("checked")) {
            return "Y";
        }
        return "N";
    }

    function getTabIndex() {
        var tab = parent.$("#tabs").tabs("getSelected");
        return parent.$("#tabs").tabs("getTabIndex", tab);
    }

    function notification(newExam, oldExam) {
        var url = apiUrl + "ExamTask/Get?id=" + newExam.taskId;
        VE.Mask("");
        nv.get(url,
            function (data) {
                VE.UnMask();
                if (data.success) {
                    var classIds = [];
                    $.each(data.result.classes,
                        function(k, v) {
                            classIds.push(k);
                        });
                    if (classIds.length === 0) {
                        return;
                    }
                    if (guidIsEmpty(oldExam.id)) {
                        openAddExamNotification(data.result, classIds);
                    } else {
                        openEditExamNotification(data.result, classIds, newExam, oldExam);
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
    }

    function openAddExamNotification(task, classIds) {
        $.messager.confirm("", "需要发送考试任务考试变动通知吗？", function (y) {
            if (y) {
                var content = "考试任务《{0}》考试更新".format(task.title);
                content += getExamNotificationContent(task.exams);
                var notificationHelper = new NotificationHelper("tabs");
                notificationHelper.open("exam", classIds, content);
            }
        });
        
    }

    function openEditExamNotification(task, classIds, newExam, oldExam) {
        var content = "";
        if (newExam.beginTime !== oldExam.beginTime || newExam.endTime !== oldExam.endTime) {
            var beginTime = newExam.beginTime ? newExam.beginTime : "不限";
            var endTime = newExam.endTime ? newExam.endTime : "不限";
            content += "\n时间变更为：{0} 至 {1}".format(beginTime, endTime);
        }

        if (newExam.maxExamNum !== oldExam.maxExamNum) {
            var maxExamNum = newExam.maxExamNum && newExam.maxExamNum > 0 ? newExam.maxExamNum + "次" : "不限";
            content += "\n考试次数变更为：" + maxExamNum;
        }

        if (newExam.passGradeType !== oldExam.passGradeType) {
            if (newExam.passGradeType === "passGradeRate") {
                var passGradeRate = newExam.passGradeRate && newExam.passGradeRate > 0 ? newExam.passGradeRate + "%" : "不限";
                content += "\n通过条件变更为：" + passGradeRate;
            } else if (newExam.passGradeType === "passGradeScore") {
                var passGradeScore = newExam.passGradeScore && newExam.passGradeScore > 0 ? newExam.passGradeScore + "分" : "不限";
                content += "\n通过条件变更为：" + passGradeScore;
            }
        } else {
            if (newExam.passGradeType === "passGradeRate" && newExam.passGradeRate !== oldExam.passGradeRate) {
                var passGradeRate2 = newExam.passGradeRate && newExam.passGradeRate > 0 ? newExam.passGradeRate + "%" : "不限";
                content += "\n通过条件变更为：" + passGradeRate2;
            } else if (newExam.passGradeType === "passGradeScore" && newExam.passGradeScore !== oldExam.passGradeScore) {
                var passGradeScore2 = newExam.passGradeScore && newExam.passGradeScore > 0 ? newExam.passGradeScore + "分" : "不限";
                content += "\n通过条件变更为：" + passGradeScore2;
            }
        }

        if (newExam.examTime !== oldExam.examTime) {
            var examTime = newExam.examTime === 0 ? "不限" : (newExam.examTime / 60) + "分钟";
            content += "\n考试时长变更为：" + examTime;
        }

        if (stringIsEmpty(content)) {
            return;
        }

        content = "考试任务《{0}》中考试《{1}》".format(task.title, newExam.examName) + content;
        $.messager.confirm("", "需要发送考试任务考试变动通知吗？", function (y) {
            if (y) {
                var notificationHelper = new NotificationHelper("tabs");
                notificationHelper.open("exam", classIds, content);
            }
        });
        
    }

    function getExamNotificationContent(rows) {
        var content = "";
        $.each(rows, function (k, v) {
            var exam = {
                examName: v.examName,
                beginTime: v.beginTime ? v.beginTime : "不限",
                endTime: v.endTime ? v.endTime : "不限",
                examTime: v.examTime === 0 ? "不限" : (v.examTime / 60) + "分钟",
                maxExamNum: v.maxExamNum ? v.maxExamNum + "次" : "不限"
            }
            content += "\n考试：{examName}\n" +
                "开始时间：{beginTime}  考试结束时间：{endTime} \n" +
                "时长：{examTime}\n" +
                "次数：{maxExamNum}\n";
            content = content.format(exam);
            if (v.passGradeType === "passGradeRate") {
                var passGradeRate = v.passGradeRate && v.passGradeRate !== 0 ? v.passGradeRate + "%" : "不限";
                content += "通过率：" + passGradeRate;
            } else if (v.passGradeType === "passGradeScore") {
                var passGradeScore = v.passGradeScore && v.passGradeScore !== 0 ? v.passGradeScore + "分" : "不限";
                content += "通过分数：" + passGradeScore;
            }
        });
        return content;
    }

    init();
    return {
        onChangeExamCodeMode: onChangeExamCodeMode,
        save: function () {
            saveData(getFormData());
        },
        showExamPaperSelector: showExamPaperSelector
    }
};