var serviceUrl = "/api/services/app/";

var UserRoleManage = (function() {
    function init(selfId) {
        var editIndex;
        var paramCache = {
            sort: "",
            order: "",
            skip: 0,
            pageSize: 30
        };

        this.query = function() {
            paramCache = getFormParam();
            paramCache.skip = 0;
            loadData(paramCache);
        };

        this.initDataGrid = function () {
            $("#dg").datagrid({
                onSortColumn: function (sort, order) {
                    paramCache.sort = sort;
                    paramCache.order = order;
                    self.query();
                },
                onClickCell: onDgClickCell,
                onEndEdit: onDgEndEdit
            })
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
        };

        this.changeRole = function(userId, identity) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var $dg = $("#dg");
            $dg.datagrid("loading");
            var url = serviceUrl + "Role/ChangeRole";
            nv.post(url,
                { userId: userId, identity: identity },
                function(data) {
                    $dg.datagrid("loaded");
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: "操作成功" });
                        loadData(paramCache);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        //保存修改的项
        this.save = function () {
            if (!endEditing()) {
                return;
            }
            var rows = $("#dg").datagrid("getChanges", "updated");
            if (rows.length === 0) {
                $.messager.alert("提示", "没有更改的角色", "info");
                return;
            }
            
            var roleDic = {};
            $.each(rows, function(k, v) {
                roleDic[v.id] = v.identity;
            });
            var url = serviceUrl + "Role/ChangeRole";
            var $dg = $("#dg");
            $dg.datagrid("loading");
            nv.post(url, { roleDic: roleDic }, function (data) {
                $dg.datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "操作成功！" });
                    loadData(paramCache);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        //重置修改的项
        this.reject = function() {
            $("#dg").datagrid("rejectChanges");
            editIndex = undefined;
        };

        function loadData(param) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = serviceUrl + "Role/UserPagination";
            var $dg = $("#dg");
            $dg.datagrid("loading");
            nv.post(url, param, function (data) {
                $dg.datagrid("loaded");
                if (data.success) {
                    $.each(data.result.rows, function(k, v) {
                        if (v.identity === 1) {
                            v.identityName = "学生";
                        } else if (v.identity === 2) {
                            v.identityName = "老师";
                        } else if (v.identity === 3) {
                            v.identityName = "管理员";
                        }
                    });
                    $dg.datagrid("loadData", data.result.rows)
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
            paramCache.userLoginName = $("#userLoginName").textbox("getValue");
            paramCache.userFullName = $("#userFullName").textbox("getValue");
            paramCache.userEmail = $("#userEmail").textbox("getValue");
            paramCache.userMobile = $("#userMobile").textbox("getValue");
            paramCache.userGender = $("#userGender").combobox("getValue");
            paramCache.identity = $("#identity").combobox("getValue");
            return paramCache;
        }

        function endEditing(){
            if (editIndex == undefined) {
                return true;
            }
            if ($("#dg").datagrid("validateRow", editIndex)){
                $("#dg").datagrid("endEdit", editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        function onDgClickCell(index, field) {
            var row = $("#dg").datagrid("getRows")[index];
            if (row.id === selfId) {
                return;
            }
            if (editIndex !== index) {
                if (endEditing()) {
                    $("#dg").datagrid("selectRow", index)
                            .datagrid("beginEdit", index);
                    var ed = $("#dg").datagrid("getEditor", { index: index, field: field });
                    if (ed) {
                        ($(ed.target).data("textbox") ? $(ed.target).textbox("textbox") : $(ed.target)).focus();
                    }
                    editIndex = index;
                } else {
                    setTimeout(function () {
                        $("#dg").datagrid("selectRow", editIndex);
                    }, 0);
                }
            }
        }

        function onDgEndEdit(index, row) {
            var ed = $(this).datagrid("getEditor", {
                index: index,
                field: "identity"
            });
            row.identityName = $(ed.target).combobox("getText");
        }
    }

    return init;
})();