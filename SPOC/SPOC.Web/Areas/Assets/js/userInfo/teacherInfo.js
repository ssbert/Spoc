var datagrid;
var dialog;
var isSave = false; // 当保存时isSave为True，不需要判断当前Form值是否更新，否则都会进行对比
var isTrue = true; //新增时为True,grid重新加载，修改时为False，grid为reload
var isEdit = 0;
var oldUsername = "";
var iframeHtml =
'<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:99%;"></iframe>';



var getSingle = function (fn) {
    var rst;
    return function () {
        return rst || (rst = fn());
    };
};
function Index() {
    return new Index.prototype.init();
}

Index.prototype = {
    constructor: Index,
    oldMobile: "",
    oldEmail: "",
    type: "",
    oldpwd: "",
    GetType: function() { return isEdit == 0 ? "true" : "update"; },

    init: function() {
        $.post('/api/services/app/TeacherInfo/BindCmb')
            .done(function(data) { //获取下拉框

                var rst = JSON.parse(data.result);
                $.each(rst,
                    function(i, v) {
                        if (v.type === 'teacherTitleCreate' || v.type === 'teacherTitle') {
                            $('#' + v.type).combobox('loadData', v.datas);
                        } else {
                            $('#' + v.type).combotree('loadData', v.datas);
                        }
                    });
            });
        var toorBarH = $("#toolbar").height();
        var fullHeight = $(window).height() - toorBarH - 3;

        datagrid = $("#dg")
            .datagrid({
                url: VE.AppPath + '/User/Teacher/Get',
                //   title: '用户信息',
                rownumbers: true,
                pagination: true,
                iconCls: VE.DatagridIconCls,
                //  height: VE.GridHeight,
                height: fullHeight,
                pageSize: VE.PageSize,
                pageList: VE.PageList,
                ctrlSelect: true,
                fitColumns: false,
                nowrap: false,
                border: true,
                singleSelect: false,
                idField: 'id',
                sortName: 'createTime',
                sortOrder: 'desc',
                onClickCell: function(rowIndex, field, value) {

                    if (field == "user_login_name") {
                        var row = $('#dg').datagrid('getRows')[rowIndex];
                        var userQueryData = { userId: row.user_id };
                        var openDialogData = "userId=" + row.user_id;
                        // User.OpenUserInfoDialog(userQueryData, openDialogData, value);
                        User.OpenUserInfoTalbeDialog(openDialogData, value);
                    }
                },
                columns: [
                    [
                        { field: 'ck', checkbox: true },
                        { field: 'id', title: 'id', width: 80, sortable: true, hidden: true },
                        { field: 'user_id', title: 'user_id', width: 80, sortable: true, hidden: true },
                        { field: 'departmentId', title: 'departmentId', width: 80, sortable: true, hidden: true },
                        { field: 'createTime', title: 'createTime', width: 80, sortable: true, hidden: true },
                        {
                            field: 'user_login_name',
                            title: '用户名',
                            width: 120,
                            sortable: true,
                            formatter: function(value, rowData, rowIndex) {
                                return "<a href='javascript:void(0);' title='点击查看明细'  >" + value + "</a>";
                            }
                        },
                        { field: 'teacherCode', title: '教师号', width: 100, sortable: true },
                        { field: 'user_name', title: '姓名', width: 60, sortable: true },
                        {
                            field: 'user_gender',
                            title: '性别',
                            width: 60,
                            sortable: true,
                            formatter: function(val) { return val == 1 ? "男" : val == 2 ? "女" : ""; }
                        },
                        { field: 'department', title: '组织架构', width: 120, sortable: true },
                        {
                            field: 'teacherTitle',
                            title: '职称',
                            width: 100,
                            sortable: true,
                            formatter: function(value, row, index) {
                                if (value == 1)
                                    return '教授';
                                else if (value == 2)
                                    return '副教授';
                                else if (value == 3)
                                    return '讲师';
                                else if (value == 4)
                                    return "助教";
                                else
                                    return "";
                            }
                        },
                        {
                            field: 'teacherJobStatusCode',
                            title: '在职状态',
                            width: 80,
                            sortable: true,
                            formatter: function(value, row, index) {
                                if (value == 1)
                                    return '在职';
                                else if (value == 2)
                                    return '停职';
                                else if (value == 3)
                                    return "离职";
                                else
                                    return "";
                            }
                        },
                        {
                            field: 'classNames',
                            title: '教学班级',
                            width: 140,
                            sortable: false
                           
                        },
                        {
                            field: ' teacherEnbleFlag',
                            title: '状态',
                            width: 70,
                            sortable: true,
                            formatter: this.formatGraduation
                        },
                        { field: '_operate', title: '操作', width: 180, formatter: this.formatOper },
                        {
                            field: 'teacherPersonalResume',
                            title: 'teacherPersonalResume',
                            width: 80,
                            sortable: true,
                            hidden: true
                        }
                    ]
                ],
                toolbar: [
                    {
                        iconCls: 'icon-reload',
                        text: '刷新',
                        handler: function () {
                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }
                            isEdit = 0;
                            //Save();
                            location.reload()
                            $('#fm').form('clear');
                        }
                    }, '-',
                    {
                        iconCls: 'icon-add',
                        text: '新增',
                        handler: function() {
                            isEdit = 0;

                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }

                            var title = "新增教师";
                            var url = "/User/Teacher/TeacherEdit";
                            parent.$("#tabs")
                                .tabs("add",
                                {
                                    title: title,
                                    content: iframeHtml.format({ title: title, url: url }),
                                    closable: true,
                                    icon: "icon-add"
                                });

                        }
                    }, '-',
                    {
                        iconCls: 'icon-20130406125519344_easyicon_net_16',
                        text: '导入',
                        handler: function() {
                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }
                            isEdit = 0;
                            $('#teacherfoImport_fm').form('clear');
                            $("#teacherInfoImportDiv").window("open").dialog('setTitle', '批量导入教师信息');;
                        }
                    }, '-',
                    {
                        id: 'export-from-excel',
                        iconCls: 'icon-20130406125647919_easyicon_net_16',
                        text: '导出',
                        handler: function() {
                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }

                        }
                    }, '-', {
                        iconCls: 'icon-busy',
                        text: '删除',
                        handler: function() {
                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }
                            obj.Delete(null);
                        }
                    }, {
                        iconCls: 'icon-sync',
                        text: '恢复',
                        handler: function() {
                            if (!checkLogin()) {
                                evtBus.dispatchEvt("show_login");
                                return;
                            }
                            obj.RecoverOpen();
                        }
                    }
                ]
            });

        var uploadExcel = new ImportQuestionClass("excel", this.Filter);

        $("#dwnExcelMould_a")
            .click(function() {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                if ($("#perList_div").is(":hidden")) {
                    $("#perList_div").show();
                } else {
                    $("#perList_div").hide()
                }

            });

        $("#exportTempt")
            .click(function() {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var names = "";
                $("[name='userPropertys']:checked")
                    .each(function(index) {
                        names += $(this).val() + ",";
                    });
                var url = $("#hidden_a").attr("href");
                $("#hidden_a").attr("href", "/User/Teacher/UserInfoExportTemplate?id=" + names);
                $("#triggerSpan").trigger("click");
            });


        $("#export-from-excel")
            .on('click',
                function() {

                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }


                    var grid = $("#dg");
                    var args = [];
                    var argsStr = "";
                    //  rows = grid.datagrid('getSelections');
                    rows = grid.datagrid('getSelections');
                    if (rows.length > 0) {
                        argsStr = "[";
                        for (var i = 0; i < rows.length; i++) {

                            argsStr += JSON.stringify({ id: rows[i].id, user_id: rows[i].user_id });
                            argsStr += (i < rows.length - 1 ? "," : "");
                        }
                        argsStr += "]";
                    }
                    var data = VE.GetFormData("UserFilterForm");
                    var filterData = $.toJSON(data);


                    $.ajax({
                        url: "/User/Teacher/GetExportDataParms",
                        type: "post",
                        data: "idList=" + argsStr + "&filter=" + filterData,
                        async: false,
                        success: function(data) {
                        }
                    });

                    window.location.href = "/User/Teacher/exportuserinfo";

                });
    },
    //绑定tree选项
    Travel: function(treeID, department) {
    },
    getTabIndex: function() {

        var tab = parent.$("#tabs").tabs("getSelected");
        return parent.$("#tabs").tabs("getTabIndex", tab);
    },
    evtHandle: evtBus.addEvt("teacherInfo_exit",
        function(tabObj) {

            parent.$("#tabs")
                .tabs("close", getTabIndex())
                .tabs("select", obj.tabIndex);

            obj.Filter();
            VE.MessageShow("操作成功");
        }),
    tabIndex: getTabIndex(),

    Edit: function (index) {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        $('#dg').datagrid('selectRow', index);

        isEdit = $('#dg').datagrid('getRows')[index].id;

        var row = $('#dg').datagrid('getRows')[index];

        var title = "编辑教师(" + row.user_login_name + ")";
        if (parent.$("#tabs").tabs('exists', title)) {
            parent.$("#tabs").tabs('select', title);

            var currTab = parent.$('#tabs').tabs('getSelected'); //获得当前tab
            var url = "/User/Teacher/TeacherEdit/" + row.id;
            parent.$('#tabs')
                .tabs('update',
                {
                    tab: currTab,
                    options: {
                        content: iframeHtml.format({ title: escape(title), url: url }),
                    }
                });

        } else {
            var url = "/User/Teacher/TeacherEdit/" + row.id;
            parent.$("#tabs")
                .tabs("add",
                {
                    title: title,
                    content: iframeHtml.format({ title: escape(title), url: url }),
                    closable: true,
                    icon: "icon-edit"
                });

        }

    },

    ArrDateFormateByGet: function(objData, perArr) {

        if (objData != null && objData != "undefined") {
            for (var p = 0; p < perArr.length; p++) { // 方法
                objData[perArr[p]] = VE.FormatterDateTime(objData[perArr[p]])
            }
        }

    },
    save: function() {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        if (isEdit == 0)
            VE.Save("fm",
                "/api/services/app/TeacherInfo/CreateTeacherInfo",
                dialog,
                "dg",
                true,
                isTrue,
                VE.GridType_DataGrid);
        else
            VE.Save("fm",
                "/api/services/app/TeacherInfo/UpdateTeacherInfoByUser",
                dialog,
                "dg",
                true,
                isTrue,
                VE.GridType_DataGrid);
    },
    setRecommendTeacher: function(index) {


        var row = $("#dg").datagrid('getRows')[index];
        var url = "/api/services/app/TeacherInfo/SetTeacherRecommend";
        var operate = function(r) {
            if (r) {
                $.ajax({
                    url: VE.AppPath + url,
                    contentType: 'application/json',
                    type: 'post',
                    dataType: 'json',
                    data: JSON.stringify({ id: row.id }),
                    success: function(r) {

                        if (r.success) {
                            VE.MessageShow("设置成功");
                            obj.Filter();
                        } else {
                            if (r.error.code == 0) {
                                $.messager.alert('提示', r.error.message, "error");
                            }
                            else if (r.error.code == 1) {
                                $.messager.alert('提示', r.error.message, "info");
                            }
                            else if (r.error.code == 2) {
                                $.messager.alert('提示', r.error.message, "warn");
                            } else {
                                $.messager.alert('提示', r.error.message, "error");
                            }
                        }
                    }
                });
            }
        }

        $.messager.confirm(VE.MessageConfirmTitle, "确定设为推荐教师？", operate);

    },

    setTeacherIsDisplay: function(index) {


        var row = $("#dg").datagrid('getRows')[index];
        var url = "/api/services/app/TeacherInfo/SetTeacherIsDisplay";
        var operate = function(r) {
            if (r) {
                $.ajax({
                    url: VE.AppPath + url,
                    contentType: 'application/json',
                    type: 'post',
                    dataType: 'json',
                    data: JSON.stringify({ id: row.id }),
                    success: function(r) {

                        if (r.success) {
                            VE.MessageShow("设置成功");
                            //VE.Filter("UserFilterForm", "dg", VE.GridType_DataGrid);
                            obj.Filter();
                        } else {
                            if (r.error.code == 0) {
                                $.messager.alert('提示', r.error.message, "error");
                            }
                            else if (r.error.code == 1) {
                                $.messager.alert('提示', r.error.message, "info");
                            }
                            else if (r.error.code == 2) {
                                $.messager.alert('提示', r.error.message, "warn");
                            } else {
                                $.messager.alert('提示', r.error.message, "error");
                            }
                        }
                    }
                });
            }
        }

        $.messager.confirm(VE.MessageConfirmTitle, "确定设置首页显示？", operate);

    },
    Delete: function(index) {
        VE.DeleteByUser("/api/services/app/TeacherInfo/DeleteTeacherInfos", "dg", false, VE.GridType_DataGrid, index);

    },
    Filter: function() {

        VE.Filter("UserFilterForm", "dg", VE.GridType_DataGrid);
    },
    Reset: function() {
        VE.Clear("UserFilterForm", "dg", VE.GridType_DataGrid);
    },
    cancel: function() {
        dialog.dialog('close');
    },
    Active: function(index) {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        var teacherEnbleFlag = $("#dg").datagrid('getRows')[index].teacherEnbleFlag;
        VE.UpdateStatu("/api/services/app/TeacherInfo/ActiveTeacherInfo",
            "dg",
            true,
            VE.GridType_DataGrid,
            index,
            teacherEnbleFlag == 0 ? "确定封禁用户?" : "确定解封用户?",
            "操作成功");
    },

    formatOper: function(val, row, index) {
      
        var option = {
            option: [
                {
                    text: '编辑',
                    icon: 'icon-edit',
                    title: '编辑',
                    onclick: 'obj.Edit'
                }
            ]
        };


        var menuOption = {
            option: [
                {
                    text: '更多',
                    icon: 'icon-list',
                    items: [
                        {
                            text: '修改密码',
                            icon: 'icon-tools',
                            onclick: 'obj.ResetPwd',
                            hide: false
                        }, {
                            text: row.teacherEnbleFlag == 0 ? "封禁用户" : "解封用户",
                            icon: 'icon-exit',
                            onclick: 'obj.Active',
                            hide: false
                        },
                        {
                            text: '删除',
                            icon: 'icon-enter',
                            onclick: 'obj.Delete',
                            hide: false
                        }
                         
                    ]
                }
            ]
        };

     
        var menus = menubtn(index, index, menuOption);


        return linkbtn(index, option) + menus;
    },
    ResetPwd: function(index) {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }

        var row = $('#dg').datagrid('getRows')[index];
        var fromObj = { userId: row.user_id, userMobile: row.user_mobile, userName: row.user_login_name }
        dialog = $("#pwdModifiry")
            .dialog({
                title: '修改密码（' + row.user_login_name + ')',
                width: 500,
                height: 300,
                closed: false,
                cache: false,
                href: VE.AppPath + '/User/User/ResetPwd',
                modal: true,
                buttons: '#dlg-pwdEdit',
                onLoad: function() {

                    // VE.UnMask();
                    if (fromObj != null) {

                        $('#pwdUpdate_form').form('load', fromObj);

                    }

                }
       
            });
    },
    ResetPwdSubmit: function() {
        VE.Save("pwdUpdate_form",
            "/api/services/app/UserInfo/UserPassWordModify",
            dialog,
            "dg",
            true,
            isTrue,
            VE.GridType_DataGrid);
    },
    formatGraduation: function (val) {
        if (!val)
            return "<span style=\"display:inline-block;height:16px;width:16px;\" title='正常' class=\"icon-active_true\"></span>";
        else return "<span style=\"display:inline-block;height:16px;width:16px;\" title='禁用' class=\"icon-busy\"></span>";

    },
    formatActive: function(val) {
      
        if (val == 1) return "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-lock\"></span>";
        else return "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-unlock\"></span>";
    },
    setEditorText: function(val) {
        editor.addListener("ready",
            function() {
                // editor准备好之后才可以使用
                editor.setContent(val);
            });
    },
    getEditorText: function() {
        return editor.getContent();
    },
    //恢复
    RecoverOpen: function() {
        $("#recovery_dialog").dialog('open').dialog('setTitle', '恢复账号');
        $("#recovery_fm").form("clear");
    },

    recoverySave: function() {
        if (!$('#recovery_fm').form('validate')) {
            return false;
        }
        var url = "/Teacher/Recovery";
        $.ajax({
            url: url,
            type: 'post',
            data: $("#recovery_fm").serialize(),
            success: function(data) {
                if (data.msg == "ok") {
                    VE.MessageShow("恢复成功");
                    obj.recoveryCancel();
                    $("#dg").datagrid("reload");
                }
                if (data.msg == "notExist") {
                    VE.MessageShow(data.ErrMsg);
                }
            }
        });
    },
    recoveryCancel: function() {
        $("#recovery_dialog").dialog("close");
    },
    checkName_Exist: function(valueName) {

        var b = "0";
        var type = this.GetType();
        if (type == "update" && valueName == this.oldUsername) {
            return true;
        }
        $.ajax({
            type: "post",
            dataType: "json",
            async: false, //是否异步执行
            url: "/api/services/app/UserInfo/CheckNameExit?type=" +
                type +
                "" +
                "&name=" +
                escape(valueName) +
                "&oldname=" +
                this.oldUsername +
                "&isRemoteCheck=false",
            success: function(data) {
                b = data.result;
            },
            error: function() {
                b = false;
            }
        });
        return b;
    },
    CheckValue_Exist: function(url, data) {
        var b = "0";
        var type = this.GetType();
        $.ajax({
            type: "post",
            dataType: "json",
            async: false, //是否异步执行
            data: data,
            url: url,
            success: function(msg) {
                b = msg.result;
            },
            error: function() {
                b = false;
            }
        });
        return b;
    },
    addInviteCode: function(index) { //添加推荐码
        var row = $("#dg").datagrid("getRows")[index];
        $("#dg").datagrid("loading");
        var url = "/api/services/app/TeacherInfo/AddInviteCode?userId=" + row.user_id;
        nv.get(url, function (data) {
            $("#dg").datagrid("loaded");
            if (data.success) {
                $.messager.show({ title: "提示", msg: "添加成功！" });
                obj.Filter();
            } else {
                $.messager.alert("提示", data.error.message, "info");
            }
        });
    }

};

function getTabIndex() {

    var tab = parent.$("#tabs").tabs("getSelected");
    return parent.$("#tabs").tabs("getTabIndex", tab);

}

Index.prototype.init.prototype = Index.prototype;
var createObj = getSingle(Index);
var obj = createObj();


$(function () {
    //初始化回车事件
    VE.Enter("obj.Filter");
    //  Inint();
    obj.init();
});

$(window).unload(function () {
    evtBus.removeEvt(obj.evtHandle);
});

/*------------验证--------------------------------*/
$.extend($.fn.validatebox.defaults.rules, {
    userNameFormatCheck: {
        validator: function (value) {
            var res = true;
            var isUsernameContainEN = usernameContainEN === "open";
            if (isUsernameContainEN) {
                res = /^(?=.*[a-zA-Z])(?=.*\d).*$/.test(value);
            }
            return res;
        },
        message: "用户名必须包含英文加数字"
    },
    nameCheckExit: {
        validator: function (value) {
            var msg = "";
            var v = obj.checkName_Exist(value);
            if (v) {
                return true;
            }
            else {
                return false;
            }
        },
        message: '该用户名已存在，请更换其它名称！'
    },
    mobileCheckExist: {
        validator: function (value) {
            var msg = "";
            //var v = obj.CheckValue_Exist("/User/User/UserMobileCheck", { "type": obj.GetType(), "checkMobile": escape(value), "oldMobile": obj.oldMobile })
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckMobileExist?type=" + obj.GetType() + "&mobile=" + escape(value) + "&oldMobile=" + obj.oldMobile + "", "")
            if (v) {
                return true;
            }
            else {
                return false;
            }
        },
        message: '该号码已存在，请更换其它号码！'
    },
    emailCheckExist: {
        validator: function (value) {
            var msg = "";
            //var v = obj.CheckValue_Exist("/User/User/UserEamilCheck", { "type": obj.GetType(), "checkEmail": escape(value), "oldEmail": obj.oldEmail })
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckEmailExist?type=" + obj.GetType() + "&email=" + escape(value) + "&oldEmail=" + obj.oldEmail + "" + "&isRemoteCheck=false", "")
            if (v) {
                return true;
            }
            else {
                return false;
            }
        },
        message: '该邮箱已存在，请更换其它邮箱！'
    },
    idCardCheckExist: {
        validator: function (value) {
            var msg = "";
            //var v = obj.CheckValue_Exist("/User/User/UserMobileCheck", { "type": obj.GetType(), "checkMobile": escape(value), "oldMobile": obj.oldMobile })
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckIdCardExist?type=" + obj.GetType() + "&idCard=" + escape(value) + "&oldIdCard=" + obj.oldIdcard + "" + "&isRemoteCheck=false", "")
            if (v) {
                return true;
            }
            else {
                return false;
            }
        },
        message: '该身份证号已存在，请更换其它身份证号！'
    },
    requiredByEdid: {
        validator: function (value) {

            if (isEdit == 0) {
                if (value != "" && value != null) {
                    return true;
                } else {
                    return false;
                }
            } else {
                if (obj.oldpwd == "") {
                    if (value != "" && value != null) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        },
        message: '该输入项必填项'
    }
});



