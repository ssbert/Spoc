var paperEdit = {
//试卷基础信息
    PaperBaseInfoClass: (function () {
        function init(id, paperNode) {
            var examPaper = { paperTypeCode: "fix" };
            var category = new nv.category.CombotreeDataClass("folderUid", "exam_paper");
            var handle = evtBus.addEvt("total_score_change", onTotalScoreChange);
            $(window).unload(function () {
                evtBus.removeEvt(handle);
            });

            this.save = function () {
                var validate = $("#baseInfo-form").form("validate");
                if (!validate) {
                    return;
                }
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var param = getParam();
                var url = webApi + "/ExamPaper/";
                var isCreate = stringIsEmpty(param.id);
                if (isCreate) {
                    url += "Create";
                } else {
                    url += "Update";
                }
                VE.Mask("");
                nv.post(url, param, function (data) {
                    VE.UnMask();
                    if (data.success) {
                        if (isCreate) {
                            setData(data.result);
                            showList();
                            paperNode.show(examPaper.id);
                        }
                        evtBus.dispatchEvt("update_paper_list");
                        $.messager.show({ title: "提示", msg: "保存成功!" });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
                    
            };

            this.getData = function () {
                //返回一个examPaper对象的副本，避免examPaper对象在外部被修改
                return $.extend({}, examPaper);
            };

            this.setTotalScort = function (value) {
                $("#totalScore").text(value);
            };

            this.onChangeCodeMode = function (checked) {
                var isCustomCode = !checked;
                $("#paperCode").textbox("readonly", !isCustomCode)
                    .textbox({ required: isCustomCode });
                if (!isCustomCode) {
                    $("#paperCode").textbox("setValue", stringIsEmpty(examPaper.paperCode) ? "" : examPaper.paperCode);
                }
            };

            this.pageInit = function () {
                var isEmpty = guidIsEmpty(id);
                if (isEmpty) {
                    $("#nodeContainer").hide();
                    $("#questionContainer").hide();
                }
                category.getCategory(function () {
                    if (!isEmpty) {
                        loadData(id);
                        paperNode.show(id);
                    }
                });
            };

            function onTotalScoreChange() {
                loadData(examPaper.id);
            }

            function loadData(id) {
                VE.Mask("");
                var url = webApi + "/ExamPaper/Get?id=" + id;
                nv.get(url,
                    function (data) {
                        VE.UnMask();
                        if (data.success) {
                            setData(data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }

            function setData(data) {
                examPaper = data;
                var funcText = examPaper.isCustomCode ? "uncheck" : "check";
                $("#isCustomCode").switchbutton(funcText);
                $("#paperCode").textbox("setValue", examPaper.paperCode);
                $("#paperName").textbox("setValue", examPaper.paperName);
                $("#remarks").textbox("setValue", examPaper.remarks);
                $("#isSingleAsMulti").combobox("setValue", examPaper.isSingleAsMulti);
                $("#paperHardGrade").combobox("setValue", examPaper.paperHardGrade);
                $("#folderUid").combotree("setValue", examPaper.folderUid);
                $("#outdatedDate").datebox("setValue", stringIsEmpty(examPaper.outdatedDate) ? "" : examPaper.outdatedDate);
                $("#totalScore").numberbox("setValue", examPaper.totalScore);
            }

            function getParam() {
                examPaper.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
                examPaper.paperCode = $("#paperCode").textbox("getValue");
                examPaper.paperName = $("#paperName").textbox("getValue");
                examPaper.remarks = $("#remarks").textbox("getValue");
                examPaper.isSingleAsMulti = $("#isSingleAsMulti").combobox("getValue");
                examPaper.paperHardGrade = $("#paperHardGrade").combobox("getValue");
                examPaper.folderUid = $("#folderUid").combotree("getValue");
                examPaper.outdatedDate = $("#outdatedDate").datebox("getValue");
                examPaper.totalScore = $("#totalScore").numberbox("getValue");
                return examPaper;
            }

            function showList() {
                $("#nodeContainer").show();
                $("#questionContainer").show();
                $("#paperNode-dg").datagrid("resize");
                $("#paperNodeQuestion-dg").datagrid("resize");
            }
        }
        return init;
    })(),

//试卷大题
    PaperNodeClass: (function () {

        function init(paperQuestion) {
            var paperUid = "";
            var isListChanged = false;
            var tabHelper = new TabHelper("tabs");
            var tabIndex = tabHelper.getTabIndex();
            var targetId = "";
            
            var handle = evtBus.addEvt("paper_node_flush", function () {
                isListChanged = true
            });
            var handle2 = evtBus.addEvt("tabs_tab_change", function(data) {
                if (!isListChanged || tabIndex !== data.index) {
                    return;
                }
                isListChanged = false;
                getData(paperUid);
            });

            $(window).unload(function () {
                evtBus.removeEvt(handle);
                evtBus.removeEvt(handle2);
            });

            this.create = function() {
                var url = "/ExamPaper/ExamPaperNode/Edit?paperUid=" + paperUid;
                tabHelper.openTab("创建大题", url, "icon-add", ["创建大题", "编辑大题"]);
            };

            this.edit = function() {
                var row = $("#paperNode-dg").datagrid("getSelected");
                if (!row) {
                    return;
                }
                var url = "/ExamPaper/ExamPaperNode/Edit?paperUid={paperUid}&paperNodeUid={paperNodeUid}";
                url = url.format({ paperUid: paperUid, paperNodeUid: row.id });
                tabHelper.openTab("编辑大题", url, "icon-edit", ["创建大题", "编辑大题"]);
            };

            this.del = function() {
                var row = $("#paperNode-dg").datagrid("getSelected");
                if (!row) {
                    return;
                }
                var checkedRows = [row];
                var hasTargetNode = false;//如果删除的大题中包含右侧的大题试题所属于的大题则为true
                if (!guidIsEmpty(targetId)) {
                    $.each(checkedRows, function (k, v) {
                        if (targetId === v.id) {
                            hasTargetNode = true;
                            return false;
                        }
                        return true;
                    });
                }

                $.messager.confirm("删除确认",
                    "确定进行删除操作吗？",
                    function(b) {
                        if (!b) {
                            return;
                        }
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        var idArray = [];
                        $.each(checkedRows,
                            function(k, v) {
                                idArray.push(v.id);
                            });

                        $("#paperNode-dg").datagrid("loading");
                        var url = webApi + "/ExamPaperNode/Delete?ids=" + idArray.join(",");
                        nv.get(url,
                            function(data) {
                                $("#paperNode-dg").datagrid("loaded");
                                if (data.success) {
                                    evtBus.dispatchEvt("total_score_change");
                                    $.messager.show({ title: "提示", msg: "删除成功" });
                                    getData(paperUid);
                                    if (hasTargetNode) {
                                        paperQuestion.clear();//清除右侧大题试题
                                    }
                                } else {
                                    $.messager.alert("提示", data.error.message, "info");
                                }
                            });
                    });
            };

            this.show = function (id) {
                paperUid = id;
                getData(paperUid);
            };

            this.onClickCell = function (index, field, value) {
                if (field === "opt") {
                    return;
                }
                var row = $("#paperNode-dg").datagrid("getRows")[index];
                targetId = row.id;
                paperQuestion.show(row.id, row.paperNodeName, row.questionBaseTypeCode, row.paperUid);
            };

            function getData(paperUid) {
                $("#paperNode-dg").datagrid("loading");
                var url = webApi + "/ExamPaperNode/GetList?paperUid=" + paperUid;
                nv.get(url,
                    function (data) {
                        $("#paperNode-dg").datagrid("loaded");
                        if (data.success) {
                            $("#paperNode-dg").datagrid("loadData", data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }
        }

        return init;
    })(),
//大题试题
    PaperQuestionClass: (function() {

        function init() {
            var self = this;
            var isListChanged = false;
            var nodeId = "";//paperNodeId
            var typeCode = "";//questionTypeCode
            var paperId = "";
            var tabHelper = new TabHelper("tabs");
            var tabIndex = tabHelper.getTabIndex();
            var questionSelectedHandle = evtBus.addEvt("exam_question_selected", onExamQuestionSelected);
            var parentTabsChangeHandle = evtBus.addEvt("tabs_tab_change", function(data) {
                if (!isListChanged || data.index !== tabIndex) {
                    return;
                }
                isListChanged = false;
            });

            $(window).unload(function () {
                evtBus.removeEvt(questionSelectedHandle);
                evtBus.removeEvt(parentTabsChangeHandle);
            });

            this.show = function (paperNodeId, nodeName, questionBaseTypeCode, paperUid) {
                var title = "[{0}]试题信息".format(nodeName);
                $("#paperNodeQuestion-dg").datagrid({ title:  title });
                getData(paperNodeId);
                nodeId = paperNodeId;
                typeCode = questionBaseTypeCode;
                paperId = paperUid;
            };
            

            this.edit = function (rowIndex) {
                $("#paperNodeQuestion-dg").datagrid("selectRow", rowIndex);
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var row = $("#paperNodeQuestion-dg").datagrid("getRows")[rowIndex];
                var url = "/ExamPaper/ExamPaperNodeQuestion/Edit?id=" + row.id;
                var title = "编辑试题" + row.questionCode;
                
                tabHelper.openTab(title, url, "icon-edit");
            };

            this.del = function (rowIndex) {
                var checkedRows;
                if ($.isNumeric(rowIndex)) {
                    $("#paperNodeQuestion-dg").datagrid("selectRow", rowIndex);
                    var row = $("#paperNodeQuestion-dg").datagrid("getRows")[rowIndex];
                    checkedRows = [row];
                } else {
                    checkedRows = $("#paperNodeQuestion-dg").datagrid("getChecked");
                    if (checkedRows.length === 0) {
                        $.messager.alert("提示", "请先选中要删除的项", "info");
                        return;
                    }
                }
                $.messager.confirm("删除确认",
                    "确定要进行删除操作吗？",
                    function (b) {
                        if (!b) {
                            return;
                        }
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        var idArray = [];
                        $.each(checkedRows,
                            function (k, v) {
                                idArray.push(v.id);
                            });
                        var url = webApi + "/ExamPaperNodeQuestion/Delete?nodeUid=" + nodeId + "&ids="
                            + idArray.join(",");
                        VE.Mask();
                        nv.get(url,
                            function (data) {
                                VE.UnMask();
                                if (data.success) {
                                    getData(nodeId);
                                    evtBus.dispatchEvt("total_score_change");
                                    evtBus.dispatchEvt("paper_node_flush");
                                    $.messager.show({ title: "提示", msg: "删除成功" });
                                } else {
                                    $.messager.alert("提示", data.error.message, "info");
                                }
                            });
                    });
            };

            this.clear = function () {
                $("#paperNodeQuestion-dg").datagrid({title:"试题信息"});
                $("#paperNodeQuestion-dg").datagrid("loadData",[]);
            };

            this.showSelector = function () {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var url = "/Exam/Component/ExamQuestionSelector?id={id}&questionBaseTypeCode={questionBaseTypeCode}&exceptionType={exceptionType}";
                url = url.format({
                    id: nodeId,
                    questionBaseTypeCode: typeCode,
                    exceptionType: "paperNode"
                });

                tabHelper.openTab("选择试题", url, "icon-ok");
            }

            function create(ids) {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var url = webApi + "/ExamPaperNodeQuestion/Create";
                nv.post(url,
                    {
                        paperNodeUid: nodeId,
                        paperUid: paperId,
                        questionUidList: ids
                    },
                    function (data) {
                        if (data.success) {
                            getData(nodeId);
                            evtBus.dispatchEvt("total_score_change");
                            evtBus.dispatchEvt("paper_node_flush");
                            $.messager.show({ title: "提示", msg: "添加成功" });
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }

            function onExamQuestionSelected(evt) {
                if (evt.handle === nodeId) {
                    var index = tabHelper.getTabIndex();
                    tabHelper.closeTab(index);
                    tabHelper.select(tabIndex);
                    var ids = [];
                    isListChanged = true;
                    $.each(evt.data, function(k, v) {
                        ids.push(v.id);
                    });
                    create(ids);
                }
            }

            function getData(paperNodeUid) {
                $("#paperNodeQuestion-dg").datagrid("loading");
                var url = webApi + "/ExamPaperNodeQuestion/GetList?paperNodeUid=" + paperNodeUid;
                nv.get(url,
                    function (data) {
                        $("#paperNodeQuestion-dg").datagrid("loaded");
                        if (data.success) {
                            $("#paperNodeQuestion-dg").datagrid("loadData", data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }
        }

        return init;
    })()
};