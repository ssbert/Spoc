var ExerciseTaskEdit = (function() {
    function init(taskId) {
        var self = this;
        var task = { id: guidIsEmpty(taskId) ? emptyGuid : taskId };
        var classes = [];
        var tabHelper = new TabHelper("tabs");
        var tabIndex = tabHelper.getTabIndex();

        var evtHandle = evtBus.addEvt("question_single_selected", function (evt) {
            if (parseInt(evt.handle) !== tabIndex) {
                return;
            }
            $("#questionText").textbox("setValue", evt.data.questionPureText);
            $("#questionId").val(evt.data.id);
        });

        $(window).unload(function() {
            evtBus.removeEvt(evtHandle);
        });

        this.save = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            if (!$("#taskForm").form("validate")) {
                return;
            }

            var param = getParam();
            var isCreate = guidIsEmpty(task.id);
            var url = apiUrl + "ExerciseManage/" + (isCreate ? "Create" : "Update");
           
            VE.Mask("");
            nv.post(url, param, function (data) {
                VE.UnMask();
                if (data.success) {
                    if (isCreate) {
                        task.id = data.result.id;
                        $.messager.show({ title: "提示", msg: "创建成功!" });
                        loadCandidateClasses();
                    } else {
                        $.messager.show({ title: "提示", msg: "保存成功!" });

                        var taskCache = $.extend({}, task);
                        var hasChange = checkHasChange(param, taskCache);
                        self.loadData(function () {
                            if (hasChange && classes.length > 0) {
                                $.messager.confirm("", "需要发送练习任务变动通知吗？", function (y) {
                                    if (y) {
                                        openEditNotification(taskCache);
                                    }
                                });
                            }
                        });
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.showClassDialog = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#classesForm").form("clear");
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建练习任务后再进行操作！", "info");
                return;
            }

            $("#classDialog").dialog("open");
        };

        this.publish = function () {

            if (!$("#classesForm").form("validate")) {
                return;
            }
            var param = { taskId: task.id, idList: $("#classIds").combotree("getValues") };
            var url = apiUrl + "ExerciseManage/Publish";
            $("#classDialog").dialog("close");
            $("#classDg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#classDg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "发布成功！" });
                    $.messager.confirm("", "需要发送练习通知吗？", function (y) {
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

        this.unpublish = function (id) {
            if (guidIsEmpty(task.id)) {
                $.messager.alert("提示", "请先创建练习任务后再进行操作！", "info");
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

                    var url = apiUrl + "ExerciseManage/Unpublish";
                    $("#classDg").datagrid("loading");
                    nv.post(url, param, function (data) {
                        $("#classDg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "取消发布成功！" });
                            self.loadData();
                            $.messager.confirm("", "需要发送取消练习任务通知吗？", function (y) {
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
            var url = apiUrl + "ExerciseManage/Get?id=" + task.id;
            VE.Mask("");
            nv.get(url, function (data) {
                VE.UnMask();
                if (data.success) {
                    task = data.result;
                    if (task.endTime == null) {
                        task.endTime = "";
                    }
                    setData();
                    classes = [];
                    $.each(task.classes, function (k, v) {
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

        this.openQuestionSelector = function() {
            var url = "/Exam/Component/QuestionSingleSelector?type=program&tabContainerId=tabs&handle=" + tabIndex;
            tabHelper.openTab("选择试题", url, "icon-ok");
        };

        this.preview = function() {
            var id = $("#questionId").val();
            if (guidIsEmpty(id)) {
                return;
            }
            var url = "/QuestionBank/Manage/Preview?id=" + id;
            tabHelper.openTab("试题预览", url);
        };

        this.showAnswerChange = function() {
            if ($("input[name='showAnswer']:checked").val() === "true") {
                $("#showAnswerTypeGroup").show();
            } else {
                $("#showAnswerTypeGroup").hide();
            }
        };

        function loadCandidateClasses() {
            $("#classForm").form("clear");
            var url = apiUrl + "ExerciseManage/GetCandidateClasses?id=" + task.id;
            nv.get(url, function (data) {
                if (data.success) {
                    $.each(data.result, function(k, v) { v.text = v.name });
                    $("#classIds").combotree("loadData", data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function setData() {
            $("#title").textbox("setValue", task.title);
            $("#endTime").datetimebox("setValue", stringIsEmpty(task.endTime) ? "" : task.endTime);
            $("#questionText").textbox("setValue", task.questionText);
            $("#questionId").val(task.questionId);
            if (task.showAnswer) {
                $("#showAnswerTypeGroup").show();
            } else {
                $("#showAnswerTypeGroup").hide();
            }
            $("input[name='showAnswer'][value='" + task.showAnswer + "']").prop("checked", true);
            $("input[name='showAnswerType'][value='" + task.showAnswerType + "']").prop("checked", true);
        }

        function getParam() {
            var param = {};
            param.id = task.id;
            param.title = $("#title").textbox("getValue");
            param.endTime = $("#endTime").datetimebox("getValue");
            param.questionId = $("#questionId").val();
            param.showAnswer = $("input[name='showAnswer']:checked").val();
            param.showAnswerType = $("input[name='showAnswerType']:checked").val();
            return param;
        }

        function checkHasChange(newObj, oldObj) {
            if (newObj.title !== oldObj.title || newObj.endTime !== oldObj.endTime) {
                return true;
            }
            return false;
        }

        //发送发布通知
        function openPublishNotification(classIds) {
            var content = "练习任务《{0}》已发布，".format(task.title);
            content += "\n" + getTaskTimeContent();
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exercise", classIds, content);
        }

        //发送取消发布通知
        function openUnpublishNotification(classIds) {
            var content = "已取消练习任务《{0}》的发布。".format(task.title);
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exercise", classIds, content);
        }

        //发送任务信息变动通知
        function openEditNotification(taskData) {
            var content = "";
            if (taskData.title !== task.title) {
                content += "原练习任务《{0}》更名为《{1}》".format(taskData.title, task.title);
            }

            if (taskData.endTime !== task.endTime) {
                if (stringIsEmpty(content)) {
                    content += "练习任务《{0}》时间更改：\n".format(task.title);
                }
                content += getTaskTimeContent();
            }

            var classIds = [];
            $.each(classes, function (k, v) {
                classIds.push(v.id);
            });
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exercise", classIds, content);
        }

        function getTaskTimeContent() {
            var endTime = stringIsEmpty(task.endTime) ? "不限" : task.endTime;
            var content = "结束时间：{0}".format(endTime);
            return content;
        }
    }

    return init;
})();