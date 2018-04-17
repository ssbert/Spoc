
var serviceUrl = "/api/services/app/";

var AddUserView = (function() {
    function init() {
        var self = this;
        var paramCache = {
            userLoginName: "",
            userFullName: "",
            userMobile: "",
            userEmail: "",
            userGender: "",
            roleId: roleId,
            identity: identity,
            skip: 0,
            pageSize: 30
        };
        var paramRoleCache = {
            userLoginName: "",
            userFullName: "",
            userMobile: "",
            userEmail: "",
            userGender: "",
            identity: identity,
            roleId: roleId,
            skip: 0,
            pageSize: 30
        };
     
        this.query = function() {
            paramCache = getFormParam();
            paramCache.skip = 0;
            loadData(paramCache);
        };
        this.queryRole = function () {
            paramRoleCache = getRoleFormParam();
            paramRoleCache.skip = 0;
            loadRoleData(paramRoleCache);
        };
        this.addToSelected = function () {
            var checkedRows = $("#user-dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先进行选择", "info");
                return;
            }
            self.save();

        };

        this.remove = function() {
            var checkedRows = $("#selected-dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先进行选择", "info");
                return;
            }
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $.messager.confirm("删除确认", "确定进行删除操作吗？", function (b) {
                if (!b) {
                    return;
                }
                var userIdList = [];
                $.each(checkedRows, function (k, v) {
                    userIdList.push(v.userId);
                });
                var url = serviceUrl + "RoleManage/DeleteUserRole";
                var param = {
                    userIdList: userIdList,
                    roleId: roleId
                };

                VE.Mask("");
                nv.post(url, param, function (data) {
                    VE.UnMask();
                    if (data.success) {
                        self.query();
                        self.queryRole();
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
            });
        };

        this.save = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
           
            var checkedRows = $("#user-dg").datagrid("getChecked");
            var userIdList = [];
            $.each(checkedRows, function (k, v) {
                userIdList.push(v.userId);
            });
            
            var url = serviceUrl + "RoleManage/AddUserRole";
            var param = {
                userIdList: userIdList,
                roleId: roleId
            };

            VE.Mask("");
            nv.post(url, param, function(data) {
                VE.UnMask();
                if (data.success) {
                   self.query();
                   self.queryRole();
                    $.messager.show({ title: "提示", msg: "添加成功！" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.GetRoleData = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var url = serviceUrl + "RoleManage/GetRoleManageById?id="+roleId;

            VE.Mask("");
            nv.get(url, function (data) {
                VE.UnMask();
                if (data.success) {
                    if (data.result.length === 0) {
                        return;
                    }                
                    $('#selected-dg').datagrid("getPanel").panel("setTitle", "已选择(" + data.result.roleName + ")");
                   
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
            //加载表格数据
            self.query();
            self.queryRole();
        };

        this.initDataGrid = function() {
            $("#user-dg")
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
            $("#selected-dg")
              .datagrid("getPager")
              .pagination({
                  onSelectPage: function (pageNumber, pageSize) {
                      paramRoleCache.pageNumber = pageNumber;
                      paramRoleCache.skip = (pageNumber - 1) * pageSize;
                      if (paramRoleCache.skip < 0) {
                          paramRoleCache.skip = 0;
                      }
                      paramRoleCache.pageSize = pageSize;
                      loadRoleData(paramRoleCache);
                  },
                  onChangePageSize: function (pageSize) {
                      paramRoleCache.pageSize = pageSize;
                      loadRoleData(paramRoleCache);
                  }
              });
         
        };

       

        function loadData(param) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var url = serviceUrl + "RoleManage/GetUserPagination";
            var $userDg = $("#user-dg");
            $userDg.datagrid("loading");
            nv.post(url, param, function(data) {
                $userDg.datagrid("loaded");
                if (data.success) {
                    $userDg.datagrid("loadData", data.result.rows)
                        .datagrid("getPager")
                        .pagination({
                            pageNumber: param.pageNumber,
                            total: data.result.total
                        });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };
        function loadRoleData(param) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var url = serviceUrl + "RoleManage/GetRoleUserPagination";
            var $userDg = $("#selected-dg");
            $userDg.datagrid("loading");
            nv.post(url, param, function (data) {
                $userDg.datagrid("loaded");
                if (data.success) {
                    $userDg.datagrid("loadData", data.result.rows)
                        .datagrid("getPager")
                        .pagination({
                            pageNumber: param.pageNumber,
                            total: data.result.total
                        });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };
        function getFormParam() {
            paramCache.userLoginName = $("#userLoginName").textbox("getValue");
            paramCache.userFullName = $("#userFullName").textbox("getValue");
            paramCache.userEmail = $("#userEmail").textbox("getValue");
            paramCache.userMobile = $("#userMobile").textbox("getValue");
            paramCache.userGender = $("#userGender").combobox("getValue");
            paramCache.roleId = roleId;
            paramCache.identity = identity;
            return paramCache;
        }
        function getRoleFormParam() {
            paramRoleCache.userLoginName = $("#userLoginName1").textbox("getValue");
            paramRoleCache.userFullName = $("#userFullName1").textbox("getValue");
            paramRoleCache.userEmail = $("#userEmail1").textbox("getValue");
            paramRoleCache.userMobile = $("#userMobile1").textbox("getValue");
            paramRoleCache.userGender = $("#userGender1").combobox("getValue");
            paramRoleCache.roleId = roleId;
            paramRoleCache.identity = identity;
            return paramRoleCache;
        }
    }

    return init;
})();

