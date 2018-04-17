var serviceUrl = "/api/services/app/";
var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%; height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';
var PolicyClass = (function () {
    function init(id) {
        var dataCache = {};
        var category = new nv.category.CombotreeDataClass("folderUid", "exam_paper");
        var handle = evtBus.addEvt("update_policy_node", function() {
            loadData();
        });
        $(window).unload(function() {
            evtBus.removeEvt(handle);
        });

        this.save = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            if (!$("#policy-form").form("validate")) {
                return;
            }
            var param = getParam();
            var url = serviceUrl + "ExamPolicy/";
            var isCreate = guidIsEmpty(id);
            if (isCreate) {
                url += "Create";
            } else {
                url += "Update";
            }
            VE.Mask("");
            nv.post(url, param, function(data) {
                VE.UnMask();
                if (data.success) {
                    if (isCreate) {
                        id = data.result.id;
                        setForm(data.result);
                        evtBus.dispatchEvt("show_policy_node", id);
                    }
                    $.messager.show({ title: "提示", msg: "保存成功！" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.reset = function() {
            setForm(dataCache);
        };

        this.onChangeCodeMode = function (checked) {
            var isCustomCode = !checked;
            $("#policyCode").textbox("readonly", !isCustomCode)
                .textbox({ required: isCustomCode });
            if (!isCustomCode) {
                $("#policyCode").textbox("setValue", stringIsEmpty(dataCache.policyCode) ? "" : dataCache.policyCode);
            }
        };

        function loadData() {
            VE.Mask("");
            var url = serviceUrl + "ExamPolicy/Get?id=" + id;
            nv.get(url, function(data) {
                VE.UnMask();
                if (data.success) {
                    setForm(data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function getParam() {
            var param = {};
            if (!guidIsEmpty(id)) {
                param.id = id;
            }
            param.policyCode = $("#policyCode").textbox("getValue");
            param.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
            param.policyName = $("#policyName").textbox("getValue");
            param.folderUid = $("#folderUid").combotree("getValue");
            param.isSingleAsMulti = $("#isSingleAsMulti").combobox("getValue");
            param.paperHardGrade = $("#paperHardGrade").combobox("getValue");
            param.outdatedDate = $("#outdatedDate").datebox("getValue");
            param.remarks = $("#remarks").textbox("getValue");
            param.paperClassCode = "exam";
            return param;
        }

        function setForm(data) {
            dataCache = data;
            var funcText = data.isCustomCode ? "uncheck" : "check";
            $("#policyCode").textbox("setValue", data.policyCode);
            $("#isCustomCode").switchbutton(funcText);
            $("#policyName").textbox("setValue", data.policyName);
            $("#folderUid").combotree("setValue", data.folderUid);
            $("#isSingleAsMulti").combobox("setValue", data.isSingleAsMulti);
            $("#paperHardGrade").combobox("setValue", data.paperHardGrade);
            $("#outdatedDate").datebox("setValue", data.outdatedDate);
            $("#remarks").textbox("setValue", data.remarks);
            $("#totalScore").numberbox("setValue", data.totalScore);
        }

        $(function () {
            category.getCategory();
            if (!guidIsEmpty(id)) {
                evtBus.dispatchEvt("show_policy_node", id);
                loadData();
            } else {
                id = emptyGuid;
                $("#policyNode-container").hide();
            }
        });
    }

    return init;
})();

var PolicyNodeListClass = (function() {
    function init(policyNode, policyItemList) {
        var isShow = false;
        var paramCache = { policyUid: emptyGuid, skip:0, pageSize:30};
        
        var handle = evtBus.addEvt("show_policy_node", function(id) {
            if (isShow) {
                return;
            }
            $("#policyNode-container").show();
            isShow = true;
            paramCache.policyUid = id;
            loadData(paramCache);
        });

        var handle2 = evtBus.addEvt("update_policy_node", function() {
            loadData(paramCache);
        });
        $(window).unload(function() {
            evtBus.removeEvt(handle);
            evtBus.removeEvt(handle2);
        });
        this.editItem = function(index) {
            $("#policyNode-dg").datagrid("selectRow", index);
            var row = getRowByIndex(index);
            policyItemList.show(row);
        };

        this.edit = function(index) {
            if ($.isNumeric(index)) {
                $("#policyNode-dg").datagrid("selectRow", index);
                var row = getRowByIndex(index);
                policyNode.show(paramCache.policyUid, row.id);
            } else {
                policyNode.show(paramCache.policyUid);
            }
        };

        this.del = function(index) {
            var rows;
            if (!$.isNumeric(index)) {
                var checkedRows = $("#policyNode-dg").datagrid("getChecked");
                if (!checkedRows || checkedRows.length === 0) {
                    $.messager.alert("提示", "请先选择要删除的项！", "info");
                    return;
                }
                rows = checkedRows;
            } else {
                $("#policyNode-dg").datagrid("selectRow", index);
                rows = [getRowByIndex(index)];
            }

            $.messager.confirm("删除确认", "确定进行删除操作吗？", function(b) {
                if (!b) {
                    return;
                }
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var idArray = [];
                $.each(rows, function(k, v) {
                    idArray.push(v.id);
                });

                $("#policyNode-dg").datagrid("loading");
                var url = serviceUrl + "ExamPolicyNode/Delete?ids=" + idArray.join(",");
                nv.get(url, function (data) {
                    $("#policyNode-dg").datagrid("loaded");
                    if (data.success) {
                        evtBus.dispatchEvt("policy_total_score_change");
                        $.messager.show({ title: "提示", msg: "删除成功" });
                        loadData(paramCache);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
            });
        };

        function loadData(param) {
            $("#policyNode-dg").datagrid("loading");
            var url = serviceUrl + "ExamPolicyNode/GetPagination";
            nv.post(url, param, function(data) {
                $("#policyNode-dg").datagrid("loaded");
                if (data.success) {
                    $("#policyNode-dg")
                        .datagrid("loadData", {
                            rows: data.result.rows,
                            total: data.result.total
                        });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function initPagination() {
            $("#policyNode-dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function(pageNumber, pageSize) {
                        paramCache.pageNumber = pageNumber;
                        paramCache.skip = (pageNumber - 1) * pageSize;
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    },
                    onChangePageSize: function(pageSize) {
                        param.pageSize = pageSize;
                        loadData(paramCache);
                    }
                });
        }

        function getRowByIndex(index) {
            return $("#policyNode-dg").datagrid("getRows")[index];
        }

        $(function() {
            initPagination();
        });
    }

    return init;
})();

var PolicyNodeClass = (function() {
    function init() {
        var self = this;
        var dataCache = {
            id: emptyGuid,
            policyUid: emptyGuid,
            questionTypeUid: emptyGuid,
            policyNodeName: "",
            policyNodeDesc: ""
        };

        this.show = function (policyUid, id) {
            clear();
            dataCache.policyUid = policyUid;
            var title = "新增大题";
            if (!guidIsEmpty(id)) {
                title = "编辑大题";
                dataCache.id = id;
                loadData();
            } else {
                self.setDefaultName();
            }
            $("#node-dialog").dialog({title: title}).dialog("open");
        };

        this.save = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            
            if (!$("#node-form").form("validate")) {
                return;
            }

            VE.Mask("");

            var param = getParam();
            var url = serviceUrl + "ExamPolicyNode/";
            if (guidIsEmpty(dataCache.id)) {
                url += "Create";
            } else {
                url += "Update";
            }
            nv.post(url, param, function(data) {
                VE.UnMask();
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "保存成功" });
                    $("#node-dialog").dialog("close");
                    evtBus.dispatchEvt("update_policy_node");
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.reset = function() {
            setForm(dataCache);
        };

        this.setDefaultName = function() {
            var text = $("#questionTypeUid").combobox("getText");
            $("#policyNodeName").textbox("setValue", text);
            dataCache.policyNodeName = text;
            dataCache.questionTypeUid = $("#questionTypeUid").combobox("getValue");
        };

        this.questionTypeChange = function() {
            self.setDefaultName();
        };

        function clear() {
            setForm({
                id: emptyGuid,
                policyUid: emptyGuid,
                questionTypeUid: "16d8bb70-41c5-11e6-b3b7-005056c00008",
                policyNodeName: "",
                policyNodeDesc: ""
            });
        }

        function loadData() {
            VE.Mask("");
            var url = serviceUrl + "ExamPolicyNode/Get?id=" + dataCache.id;
            nv.get(url, function(data) {
                VE.UnMask();
                if (data.success) {
                    setForm(data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function setForm(data) {
            dataCache = data;
            $("#questionTypeUid").combobox("setValue", data.questionTypeUid);
            $("#policyNodeName").textbox("setValue", data.policyNodeName);
            $("#policyNodeDesc").textbox("setValue", data.policyNodeDesc);
            if (data.questionNum > 0) {
                $("#questionTypeUid").combobox("disable");
            } else {
                $("#questionTypeUid").combobox("enable");
            }
        }

        function getParam() {
            var param = {
                id: dataCache.id,
                policyUid: dataCache.policyUid,
                questionTypeUid: $("#questionTypeUid").combobox("getValue"),
                policyNodeName: $("#policyNodeName").textbox("getValue"),
                policyNodeDesc: $("#policyNodeDesc").textbox("getValue")
            };
            return param;
        }
    }

    return init;
})();

var PolicyItemListClass = (function() {
    function init(policyItem) {
        var policyNode;
        var policyNodeUid;
        var dataCache;
        var questionTotalNum = 0;
        var handle = evtBus.addEvt("update_policy_item", function() {
            evtBus.dispatchEvt("update_policy_node");
            loadData();
        });
        $(window).unload(function () {
            evtBus.removeEvt(handle);
        });
        this.show = function(node) {
            policyNode = node;
            policyNodeUid = node.id;
            $("#item-list-dialog").dialog("open");
            //$("#policyItem-dg").datagrid("resize");
            loadData();
        };

        this.edit = function(index) {
            var row = getRowByIndex(index);
            $("#policyItem-dg").datagrid("selectRow", index);
            policyItem.showEdit(dataCache, row, questionTotalNum);
        };

        this.add = function () {
            var questionType = {
                questionTypeUid: policyNode.questionTypeUid,
                questionTypeName: policyNode.questionTypeName
            };
            policyItem.showAdd(dataCache, policyNodeUid, questionType, questionTotalNum);
        };

        this.del = function(index) {
            $("#policyItem-dg").datagrid("selectRow", index);

            var rows;
            if (!$.isNumeric(index)) {
                var checkedRows = $("#policyItem-dg").datagrid("getChecked");
                if (!checkedRows || checkedRows.length === 0) {
                    $.messager.alert("提示", "请先选择要删除的项！", "info");
                    return;
                }
                rows = checkedRows;
            } else {
                $("#policyItem-dg").datagrid("selectRow", index);
                rows = [getRowByIndex(index)];
            }
            $.messager.confirm("删除确认", "确定进行删除操作吗？", function(b) {
                if (!b) {
                    return;
                }

                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var idArray = [];
                $.each(rows, function(k, v) {
                    idArray.push(v.id);
                });

                $("#policyItem-dg").datagrid("loading");
                var url = serviceUrl + "ExamPolicyItem/Delete?ids=" + idArray.join(",");
                nv.get(url, function(data) {
                    $("#policyItem-dg").datagrid("loaded");
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: "删除成功" });
                        loadData();
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
            });
        };

        function loadData() {
            var url = serviceUrl + "ExamPolicyItem/GetList?policyNodeUid=" + policyNodeUid;
            $("#policyItem-dg").datagrid("loading");
            nv.get(url, function(data) {
                $("#policyItem-dg").datagrid("loaded");
                if (data.success) {
                    dataCache = data.result;
                    $("#policyItem-dg").datagrid("loadData", data.result);

                    var totalQuestionNumber = 0;
                    var totalQuestionScore = 0;
                    $.each(dataCache, function(k, v) {
                        totalQuestionNumber += v.questionNum;
                        totalQuestionScore += (v.questionScore * v.questionNum);
                    });
                    questionTotalNum = totalQuestionNumber;
                    $("#totalQuestionNumber").text(totalQuestionNumber);
                    $("#totalQuestionScore").text(totalQuestionScore);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        function getRowByIndex(index) {
            return $("#policyItem-dg").datagrid("getRows")[index];
        }
    }

    return init;
})();

var PolicyItemClass = (function() {
    function init() {
        var questionTypeUid;
        var questionTypeName;
        var policyNodeUid;
        var policyItems;
        var policyItem;
        var dialogId = "#item-edit-dialog";
        var isEdit = true;
        var questionTotalNum;
        var category = new nv.category.CombotreeDataClass("folderUid2", "question_bank");
        this.showAdd = function (items, nodeUid, questionType, questionNum) {
            isEdit = false;
            policyItems = items;
            policyItem = null;
            questionTypeUid = questionType.questionTypeUid;
            questionTypeName = questionType.questionTypeName;
            policyNodeUid = nodeUid;
            questionTotalNum = questionNum;
            $(dialogId).dialog({ title: "添加策略项" }).dialog("open");
            $("#hardGrade").combobox("loadData", getHardGradeOption()).combobox("enable");
            setFormData({
                questionTypeName: questionTypeName,
                folderUid: "",
                questionScore: 0,
                questionNum: 0,
                labelIdList: []
            });
            getQuestionNum();
        };

        this.showEdit = function (items, item, questionNum) {
            isEdit = true;
            policyItems = items;
            policyItem = item;
            questionTypeUid = item.questionTypeUid;
            questionTypeName = item.questionTypeName;
            policyNodeUid = item.policyNodeUid;
            questionTotalNum = questionNum;
            $(dialogId).dialog({ title: "编辑策略项" }).dialog("open");
            var option = {
                hardGrade: stringIsEmpty(item.hardGrade) ? "" : item.hardGrade,
                text: stringIsEmpty(item.hardGrade) ? "难度不限" : getHardGradeText("" + item.hardGrade),
                selected: true
            };
            $("#hardGrade").combobox("loadData", [option]).combobox("disable");
            setFormData(item);
            getQuestionNum();
        };

        this.save = function() {
            if (!$("#item-form").form("validate")) {
                return;
            }
            var url = serviceUrl + "ExamPolicyItem/";
            if (!isEdit) {
                url += "Create";
            } else {
                url += "Update";
            }

            VE.Mask("");
            nv.post(url, getParam(), function(data) {
                VE.UnMask();
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "保存成功" });
                    $(dialogId).dialog("close");
                    evtBus.dispatchEvt("update_policy_item");
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        function setFormData(data) {
            var folderUids = data.folderUid.split(",");
            var questionTypeName = stringIsEmpty(data.questionTypeName) ? "全部题型" : data.questionTypeName;
            $("#questionTypeName").textbox("setValue", questionTypeName);
            $("#folderUid2").combotree("setValues", folderUids);
            $("#questionScore").numberbox("setValue", data.questionScore);
            $("#questionNum").numberbox("setValue", data.questionNum);
            $("#labelIdList").combotree("setValues", data.labelIdList);
        }

        function getParam() {
            var param = {};
            param.id = (policyItem == null) ? emptyGuid : policyItem.id;
            param.policyNodeUid = policyNodeUid;
            param.questionTypeUid = questionTypeUid;
            param.hardGrade = $("#hardGrade").combobox("getValue");
            var folderUids = $("#folderUid2").combotree("getValues");
            folderUids.remove("");
            param.folderUid = folderUids.join(",");
            param.folderName = $("#folderUid2").combotree("getText"); 
            param.questionScore = $("#questionScore").numberbox("getValue");
            param.questionNum = $("#questionNum").numberbox("getValue");
            param.labelIdList = $("#labelIdList").combotree("getValues");
            return param;
        }

        function getHardGradeOption() {
            var data = [];
            var hardGrades = ["", 1, 2, 3];
            $.each(policyItems, function(k, v) {
                var index = hardGrades.indexOf(v.hardGrade);
                if (index !== -1) {
                    hardGrades.splice(index, 1);
                }
            });

            $.each(hardGrades, function(k, v) {
                if (v === "") {
                    data.push({ hardGrade: v, text: "难度不限", selected: true});
                } else {
                    data.push({ hardGrade: "" + v, text: getHardGradeText("" + v) });
                }
            });
            return data;
        }

        function getHardGradeText(val) {
            if (val === "1") {
                return "容易";
            } else if (val === "2") {
                return "中等";
            } else if (val === "3") {
                return "困难";
            } else if (stringIsEmpty(val)) {
                return "缺省";
            }
            return "" + val;
        }

        function getQuestionNum() {
            var param = {};
            param.questionTypeUid = questionTypeUid;
            var folderUid2 = $("#folderUid2").combotree("getValues");
            if (folderUid2.length > 1 || !guidIsEmpty(folderUid2[0])) {
                var lastIndex = folderUid2.length - 1;
                if (guidIsEmpty(folderUid2[lastIndex])) {
                    folderUid2 = folderUid2.slice(lastIndex, 1);
                }
                param.folderUids = folderUid2;
            }

            param.hardGrade = $("#hardGrade").combobox("getValue");
            param.labelIdList = $("#labelIdList").combotree("getValues");
            var url = serviceUrl + "QuestionBank/GetQuestionNum";
            nv.post(url, param, function(data) {
                if (data.success) {
                    var num = data.result - questionTotalNum;
                    if (policyItem != null) {
                        num += policyItem.questionNum;
                    }
                    $("#questionNum2").numberbox("setValue", num);
                    $("#questionNum").numberbox("validate");
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }
        $(function() {
            category.getCategory();
            $("#folderUid2")
                .combotree({
                    onChange: function(newVal, oldVal) {
                        if (newVal !== oldVal) {
                            getQuestionNum();
                        }
                    }
                });
            $("#hardGrade")
                .combobox({
                    onChange: function(newVal, oldVal) {
                        if (newVal !== oldVal) {
                            getQuestionNum();
                        }
                    }
                });
            $("#labelIdList").combotree({
                onChange: function(newVal, oldVal) {
                    if (newVal !== oldVal) {
                        getQuestionNum();
                    }
                }
            });
        });
    }

    return init;
})();