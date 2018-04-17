var ExamTaskEdit = (function() {
    function init(taskId) {
        var self = this;
        var task = { id: guidIsEmpty(taskId) ? emptyGuid : taskId };
        var classes = [];
        var tabHelper = new TabHelper("tabs");
        var tabIndex = tabHelper.getTabIndex();
        var hasChange = false;

        var evtHandle = evtBus.addEvt("exam_changed", function() {
            hasChange = true;
        });

        var evtHandle2 = evtBus.addEvt("tabs_tab_change", function(data) {
            if (!hasChange || data.index !== tabIndex) {
                return;
            }
            self.loadData();
        });

        $(window).unload(function () {
            evtBus.removeEvt(evtHandle);
            evtBus.removeEvt(evtHandle2);
        });

        this.onChangeCodeMode = function (checked) {
            var isCustomCode = !checked;
            $("#code").textbox("readonly", !isCustomCode)
                .textbox({ required: isCustomCode });
            if (!isCustomCode) {
                $("#code").textbox("setValue", stringIsEmpty(task.code) ? "" : task.code);
            }
        };

        this.save = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            if (!$("#taskForm").form("validate")) {
                return;
            }
            
            var param = getParam();
            var isCreate = guidIsEmpty(task.id);
            var url = apiUrl + "ExamTask/" + (isCreate ? "Create" : "Update");
            var oldTitle = task.title;
            VE.Mask("");
            nv.post(url, param, function(data) {
                VE.UnMask();
                if (data.success) {
                    if (isCreate) {
                        task.id = data.result.id;
                        $.messager.show({ title: "提示", msg: "创建成功!" });
                    } else {
                        $.messager.show({ title: "提示", msg: "保存成功!" });

                        self.loadData(function() {
                            if (oldTitle !== task.title && hasChange && classes.length > 0) {
                                $.messager.confirm("", "需要发送考试任务变动通知吗？", function () {
                                    openEditNotification(oldTitle);
                                });
                            }
                        });
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.editExam = function (id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建考试任务后再进行操作！", "info");
                return;
            }
            var url = "/Exam/Manage/Edit?taskId=" + task.id;
            var title = "新增考试";
            var icon = "icon-add";
            if (!guidIsEmpty(id)) {
                title = "编辑考试";
                icon = "icon-edit";
                url += "&id=" + id;
            } else {
                var examName = $("#title").textbox("getValue");
                url += "&examName=" + encodeURIComponent(examName);
            }
            tabHelper.openTab(title, url, icon, ["新增考试", "编辑考试"]);
        };

        this.setMainExam = function (id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = apiUrl + "ExamExam/SetExamTypeCodeNormal?id=" + id;
            $("#examDg").datagrid("loading");
            nv.get(url,
                function(data) {
                    $("#examDg").datagrid("loaded");
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: "设置成功!" });
                        self.loadData();
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                },
                function() {
                    $("#examDg").datagrid("loaded");
                });
        };

        this.delExam = function (id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建考试任务后再进行操作！", "info");
                return;
            }
            var ids = [];
            if (guidIsEmpty(id)) {
                var rows = $("#examDg").datagrid("getChecked");
                if (rows.length === 0) {
                    $.messager.alert("提示", "请先选择要删除的项", "info");
                    return;
                }
                $.each(rows,
                    function(k, v) {
                        ids.push(v.id);
                    });
            } else {
                ids.push(id);
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    var url = apiUrl + "ExamExam/Delete?ids=" + ids.join(",");
                    $("#examDg").datagrid("loading");
                    nv.get(url, function (data) {
                        $("#examDg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            self.loadData(function () {
                                if (classes.length === 0) {
                                    return;
                                }
                                $.messager.confirm("", "需要发送考试任务考试变动通知吗？", function (r) {
                                    if (r) {
                                        openExamChangeNotification();
                                    }
                                });
                            });
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        };

        this.showClassDialog = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建考试任务后再进行操作！", "info");
                return;
            }

            $("#classDialog").dialog("open");
        };

        this.publish = function () {
            
            if (!$("#classesForm").form("validate")) {
                return;
            }
            var param = { taskId: task.id, idList: $("#classIds").combotree("getValues") };
            var url = apiUrl + "ExamTask/Publish";
            $("#classDialog").dialog("close");
            $("#classDg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#classDg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "发布成功！" });
                    $.messager.confirm("", "需要发送考试通知吗？", function (y) {
                        if (y) {
                            openPublishNotification(param.idList);
                        }
                    });
                    self.loadData();
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.unpublish = function(id) {
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建考试任务后再进行操作！", "info");
                return;
            }
            var param = { taskId: task.id, idList: [] };
            if (guidIsEmpty(id)) {
                var rows = $("#classDg").datagrid("getChecked");
                if (rows.length === 0) {
                    $.messager.alert("提示", "请先选择要取消发布的项", "info");
                    return;
                }
                $.each(rows,
                    function (k, v) {
                        param.idList.push(v.id);
                    });
            } else {
                param.idList.push(id);
            }

            $.messager.confirm("取消发布确认", "确认进行取消发布操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }

                    var url = apiUrl + "ExamTask/Unpublish";
                    $("#classDg").datagrid("loading");
                    nv.post(url, param, function (data) {
                        $("#classDg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "取消发布成功！" });
                            self.loadData();
                            $.messager.confirm("", "需要发送取消考试任务通知吗？", function (y) {
                                if (y) {
                                    openUnpublishNotification(param.idList);
                                }
                            });
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        };

        this.loadData = function (callback) {
            var url = apiUrl + "ExamTask/Get?id=" + task.id;
            VE.Mask("");
            nv.get(url, function(data) {
                VE.UnMask();
                if (data.success) {
                    task = data.result;
                    setData();
                    $("#examDg").datagrid("loadData", task.exams);
                    classes = [];
                    $.each(task.classes, function(k, v) {
                        classes.push({ id: k, name: v });
                    });
                    $("#classDg").datagrid("loadData", classes);
                    if (callback) {
                        callback();
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
            loadCandidateClasses();
        };

        function loadCandidateClasses() {
            $("#classForm").form("clear");
            var url = apiUrl + "ExamTask/GetCandidateClasses?id=" + task.id;
            nv.get(url, function(data) {
                if (data.success) {
                    $.each(data.result, function(k, v) { v.text = v.name; });
                    $("#classIds").combotree("loadData", data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        function setData() {
            var funcText = task.isCustomCode ? "uncheck" : "check";
            $("#isCustomCode").switchbutton(funcText);
            $("#code").textbox("setValue", task.code);
            $("#title").textbox("setValue", task.title);
        }

        function getParam() {
            var param = {};
            param.id = task.id;
            param.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
            param.code = $("#code").textbox("getValue");
            param.title = $("#title").textbox("getValue");
            return param;
        }

        //发送发布通知
        function openPublishNotification(classIds) {
            var content = "考试任务《{0}》已发布".format(task.title);
            content += getExamNotificationContent();
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exam", classIds, content);
        }

        //发送取消发布通知
        function openUnpublishNotification(classIds) {
            var content = "已取消考试任务《{0}》的发布。".format(task.title);
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exam", classIds, content);
        }

        //发送任务信息变动通知
        function openEditNotification(oldTitle) {
            var content = "原考试任务《{0}》更名为《{1}》".format(oldTitle, task.title);
            var classIds = [];
            $.each(classes, function(k, v) {
                classIds.push(v.id);
            });
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exam", classIds, content);
        }

        //发送删除考试通知
        function openExamChangeNotification() {
            var content = "考试任务《{0}》考试更新".format(task.title);
            content += getExamNotificationContent();
            var classIds = [];
            $.each(classes, function (k, v) {
                classIds.push(v.id);
            });
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exam", classIds, content);
        }

        function getExamNotificationContent() {
            var rows = $("#examDg").datagrid("getRows");
            var content = "";
            var template = "\n考试：{examName}\n" +
                "开始时间：{beginTime}  考试结束时间：{endTime} \n" +
                "时长：{examTime}\n" +
                "次数：{maxExamNum}\n";
            $.each(rows, function (k, v) {
                var exam = {
                    examName: v.examName,
                    beginTime: !stringIsEmpty(v.beginTime) ? v.beginTime : "不限",
                    endTime: !stringIsEmpty(v.endTime) ? v.endTime : "不限",
                    examTime: v.examTime === 0 ? "不限" : (v.examTime / 60) + "分钟",
                    maxExamNum: v.maxExamNum ? v.maxExamNum + "次" : "不限"
                }
                content += template.format(exam);
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
        
    }

    return init;
})();