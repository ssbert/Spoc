
var Exer = exercises ? exercises : {};

exercises.ManageViewClass = (function() {
    function init() {
        var self = this;
        var paramCache = {
            code: "",
            title: "",
            categoryIds: [],
            subjectIds: [],
            departmentsIds: [],
            skip: 0,
            pageSize: 30
        };
        var category = new nv.category.CombotreeDataClass("categoryId", "exercisesBank");

        this.query = function() {
            paramCache = getFormParam();
            paramCache.skip = 0;
            loadData(paramCache);
        };

        this.add = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Exercises/Manage/Edit";
            openTab("新增子题库", url, "icon-add", ["新增子题库", "编辑子题库"]);
        };

        this.edit = function(index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var row = $("#dg").datagrid("getRows")[index];
            $("#dg").datagrid("selectRow", index);
            var url = "/Exercises/Manage/Edit?id=" + row.id;
            openTab("编辑子题库", url, "icon-edit", ["新增子题库", "编辑子题库"]);
        };

        this.del = function(index) {
            var rows;
            if ($.isNumeric(index)) {
                $("#dg").datagrid("selectRow", index);
                rows = [$("#dg").datagrid("getRows")[index]];
            } else {
                rows = $("#dg").datagrid("getChecked");
                if (rows.length === 0) {
                    $.messager.alert("提示", "请先选择要删除的项", "info");
                    return;
                }
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }
                    var idArray = [];
                    $.each(rows,
                        function (k, v) {
                            idArray.push(v.id);
                        });
                    var url = serviceUrl + "ExercisesBank/Delete?ids=" + idArray.join(",");
                    $("#dg").datagrid("loading");
                    nv.get(url, function (data) {
                        $("#dg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            loadData(paramCache);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        }

        function loadData(param) {
            $("#dg").datagrid("loading");
            var url = serviceUrl + "ExercisesBank/GetPagination";
            nv.post(url,
                param,
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $("#dg")
                            .datagrid("loadData", data.result.rows)
                            .datagrid("getPager")
                            .pagination({
                                pageNumber: param.pageNumber,
                                total: data.result.total
                            });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getFormParam() {
            paramCache.code = $("#code").textbox("getValue");
            paramCache.title = $("#title").textbox("getValue");
            paramCache.categoryIds = $("#categoryId").combotree("getValues");
            return paramCache;
        }

        function initPagination() {
            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        paramCache.pageNumber = pageNumber;
                        paramCache.skip = (pageNumber - 1) * pageSize;
                        if (paramCache.skip < 0) {
                            paramCache.skip = 0;
                        }
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    },
                    onChangePageSize: function (pageSize) {
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    }
                });
        }

        $(function() {
            category.getCategory();
            initPagination();
            self.query();
        });
    }

    return init;
})();

exercises.EditViewClass = (function() {
    function init(id, lessonId, courseId, questionListView) {
        var dataCache = {};
        var category = new nv.category.CombotreeDataClass("categoryId", "exercisesBank");

        this.save = function() {
            var validate = $("#baseInfo-form").form("validate");
            if (!validate) {
                return;
            }
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var param = getFormParam();
            var url = serviceUrl + "ExercisesBank/";
            var isCreate = stringIsEmpty(id);
            if (!isCreate) {
                param.id = id;
                url += "Update";
            } else {
                url += "Create";
            }

            VE.Mask("");
            nv.post(url, param, function(data) {
                VE.UnMask();
                if (data.success) {
                    if (isCreate) {
                        setForm(data.result);
                        questionListView.show(dataCache.id);
                    }
                    evtBus.dispatchEvt("exercises_bank_edited", dataCache);
                    $.messager.show({ title: "提示", msg: "保存成功!" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.onChangeCodeMode = function(checked) {
            var isCustomCode = !checked;
            $("#code").textbox("readonly", !isCustomCode)
                .textbox({ required: isCustomCode });
            if (!isCustomCode) {
                $("#code").textbox("setValue", stringIsEmpty(dataCache.code) ? "" : dataCache.code);
            }
        };

        function loadData(id) {
            VE.Mask("");
            var url = serviceUrl + "ExercisesBank/Get?id=" + id;
            nv.get(url, function(data) {
                VE.UnMask();
                if (data.success) {
                    setForm(data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function getFormParam() {
            return {
                title: $("#title").textbox("getValue"),
                lessonId: lessonId,
                courseId: courseId
            };
        }

        function setForm(data) {
            $("#title").textbox("setValue", data.title);
            dataCache = data;
            id = data.id;
        }

        $(function() {
            $("#question-container").hide();
            category.getCategory(function () {
                if (!stringIsEmpty(id)) {
                    loadData(id);
                    questionListView.show(id);
                }
            });
        });
    }

    return init;
})();

exercises.QuestionListViewClass = (function() {
    function init() {
        var paramCache = {skip: 0, pageSize: 30};

        var handle = evtBus.addEvt("question_list_flush", function() {
            loadData(paramCache);
        });

        var handle2 = evtBus.addEvt("exam_question_selected", function(evt) {
            if (evt.handle === paramCache.exercisesBankId) {
                var index = getTabIndex();
                
                var ids = [];
                $.each(evt.data, function (k, v) {
                    ids.push(v.id);
                });
                create(ids);

                parent.$("#tabs").tabs("close", index);
                parent.$("#tabs").tabs("select", tabIndex);
            }
        });

        $(window).unload(function() {
            evtBus.removeEvt(handle);
            evtBus.removeEvt(handle2);
        });

        this.show = function(id) {
            if (!stringIsEmpty(id)) {
                paramCache.exercisesBankId = id;
                loadData(paramCache);
            }
            $("#question-container").show();
            $("#question-dg").datagrid("resize");
        };

        this.add = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var url = "/Exam/Component/ExamQuestionSelector?id={id}&exceptionType={exceptionType}";
            url = url.format({
                id: paramCache.exercisesBankId,
                exceptionType: "exercisesBank"
            });

            openTab("选择试题", url, "icon-ok");
        };

        this.del = function(index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var rows;
            if ($.isNumeric(index)) {
                rows = [$("#question-dg").datagrid("getRows")[index]];
            } else {
                rows = $("#question-dg").datagrid("getChecked");
                if (rows.length === 0) {
                    $.messager.alert("提示", "请先选择要删除的项", "info");
                    return;
                }
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    
                    var idArray = [];
                    $.each(rows,
                        function (k, v) {
                            idArray.push(v.id);
                        });
                    var url = serviceUrl + "ExercisesBankQuestion/Delete?ids=" + idArray.join(",");
                    $("#question-dg").datagrid("loading");
                    nv.get(url, function (data) {
                        $("#question-dg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            loadData(paramCache);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        }

        function create(ids) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var param = {
                exercisesBankId: paramCache.exercisesBankId,
                questionIds: ids,
                skip: paramCache.skip,
                pageSize: paramCache.pageSize
            };
            $("#question-dg").datagrid("loading");
            var url = serviceUrl + "ExercisesBankQuestion/Add";
            nv.post(url, param, function (data) {
                $("#question-dg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "添加成功！" });
                    loadData(param);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function loadData(param) {
            $("#question-dg").datagrid("loading");
            var url = serviceUrl + "ExercisesBankQuestion/GetPagination";
            nv.post(url, param, function(data) {
                $("#question-dg").datagrid("loaded");
                if (data.success) {
                    $("#question-dg")
                        .datagrid("loadData", data.result.rows)
                        .datagrid("getPager")
                        .pagination({
                            pageNumber: param.pageNumber,
                            total: data.result.total
                        });

                    paramCache = param;
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function initPagination() {
            $("#question-dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        paramCache.pageNumber = pageNumber;
                        paramCache.skip = (pageNumber - 1) * pageSize;
                        if (paramCache.skip < 0) {
                            paramCache.skip = 0;
                        }
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    },
                    onChangePageSize: function (pageSize) {
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    }
                });
        }

        $(function() {
            initPagination();
        });
    }

    return init;
})();

function openTab(title, url, icon, titles) {
    var targetTitle;
    var hasTab = false;
    if (!$.isArray(titles)) {
        titles = [title];
    }

    $.each(titles, function (k, v) {
        if (parent.$("#tabs").tabs("exists", v)) {
            targetTitle = v;
            hasTab = true;
            return false;
        }
        return true;
    });
    if (hasTab) {
        parent.$("#tabs").tabs("select", targetTitle);
        var tab = parent.$("#tabs").tabs("getSelected");
        parent.$("#tabs").tabs("update", {
            tab: tab,
            options: {
                title: title,
                content: iframeHtml.format({ title: title, url: url }),
                icon: icon
            }
        });
    } else {
        parent.$("#tabs").tabs("add", {
            title: title,
            content: iframeHtml.format({ title: title, url: url }),
            closable: true,
            icon: icon
        });
    }
}

//获取tabs控件当前selected状态tab的index
function getTabIndex() {
    var tab = parent.$("#tabs").tabs("getSelected");
    return parent.$("#tabs").tabs("getTabIndex", tab);
}