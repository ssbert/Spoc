var datagrid;
var dialog;
var isSave = false; // 当保存时isSave为True，不需要判断当前Form值是否更新，否则都会进行对比
var isTrue = true; //新增时为True,grid重新加载，修改时为False，grid为reload
var isEdit = 0;
var oldUsername = "";
var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:99%;"></iframe>';
var selectRow;    
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
    oldEmail:"",
    type: "",
    oldpwd: "",
    tabTitle:"",
       
    GetType: function () { return isEdit == 0 ? "true" : "update"; },
    init: function () {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        var toorBarH = $("#toolbar").height();
        var fullHeight = $(window).height() - toorBarH - 3;
 
        $.post('/api/services/app/Department/GetDepartmentTree').done(function (data) { //获取下拉框
          
            $("#department").combotree('loadData', data);
            $("#wd-departmentUid").combotree(
                    {
                        data: data,
                        onBeforeSelect: function(node) {
                            //返回树对象 
                            var tree = $(this).tree;
                            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中 
                            var isLeaf = tree('isLeaf', node.target);
                            if (!isLeaf) {
                                return false;
                                //清除选中  
                                // $('#user_classid').combotree('clear');
                                //$("#user_classid").treegrid("unselect");
                            }
                        }
                        
                    }
                );
           
           
        });

        datagrid = $("#dg").datagrid({
            url: VE.AppPath + '/User/Student/Get',
            // title: '用户信息',
            rownumbers: true,
            pagination: true,
            
            iconCls: VE.DatagridIconCls,
            //  height: VE.GridHeight,
            height: fullHeight,
            pageSize: VE.PageSize,
            pageList: [5, 10, 15, 20, 25, 30, 35, 40,50,100,200],
            ctrlSelect: true,
            fitColumns: false,
            nowrap: false,
            border: true,

            singleSelect: false,

            idField: 'id',
            sortName: 'create_time',
            sortOrder: 'desc',
            onClickCell: function (rowIndex, field, value) {

                if (field == "user_login_name") {
                    var row = $('#dg').datagrid('getRows')[rowIndex];
                    var userQueryData = { userId: row.user_id };
                    var openDialogData = "userId=" + row.user_id;
                    //User.OpenUserInfoDialog(userQueryData, openDialogData, value);
                    User.OpenUserInfoTalbeDialog(openDialogData, value);
                }
            },
            columns: [
                [
                      { field: 'ck', checkbox: true },
                      { field: 'id', title: 'id', width: 80, sortable: true, hidden: true },
                       { field: 'user_password', title: 'user_password', width: 80, sortable: true, hidden: true },
                      { field: 'departmentId', title: 'departmentId', width: 80, sortable: true, hidden: true },
                      { field: 'user_id', title: 'user_id', width: 80, sortable: true, hidden: true },
                      {
                          field: 'user_login_name', title: '用户名', width: 120, sortable: true
                          , formatter: function (value, rowData, rowIndex) {
                              return "<a href='javascript:void(0)' title='点击查看明细'  >" + value + "</a>";
                          }
                      },
                      { field: 'user_code', title: '学号', width: 100, sortable: true },
                      { field: 'user_mobile', title: '手机号码', width: 100, sortable: true },
                      { field: 'user_email', title: '邮箱', width: 180, sortable: true },
                      { field: 'user_name', title: '姓名', width: 60, sortable: true },
                      { field: 'user_gender', title: '性别', width: 60, sortable: true, formatter: function (val) { return val == 1 ? "男" : val == 2 ? "女" : ""; } },
                      { field: 'user_faculty', title: '院系', width: 120, sortable: true },
                      { field: 'user_major', title: '专业', width: 120, sortable: true },
                      { field: 'user_class', title: '班级', width: 120, sortable: true },
                      //{ field: 'user_inviteCode', title: '学生邀请码', width: 80, sortable: true },
                      //{ field: 'user_register_inviteCode', title: '注册邀请码', width: 80, sortable: true },
                      { field: ' user_enble_flag', title: '状态', width: 70, sortable: true, formatter: this.formatGraduation },
                      //{ field: 'is_graduation', title: '是否毕业', width: 70, sortable: true, formatter: this.formatGraduation },
                      { field: 'create_time', title: '创建时间', width: 100, sortable: true, formatter: VE.FormatterDateTime },
                      { field: '_operate', title: '操作', width: 180, formatter: this.formatOper }
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
                         location.reload()
                         $('#fm').form('clear');
                     }
                 }, '-',
                  {
                      iconCls: 'icon-add',
                      text: '新增',
                      handler: function () {
                               
                          if (!checkLogin()) {
                              evtBus.dispatchEvt("show_login");
                              return;
                          }

                          obj.AddStudent(); 
                      }
                  }, '-',
                {
                   // id: 'import-from-excel',
                    iconCls: 'icon-20130406125519344_easyicon_net_16',
                    text: '导入',
                    handler: function () {

                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        isEdit = 0;
                        $('#userInfoImport_fm').form('clear');
                        $("#userInfoImportDiv").window("open").dialog('setTitle', '批量导入学生信息');;
                    }
                }, '-',
                 {
                     id: 'export-from-excel',
                     iconCls: 'icon-20130406125647919_easyicon_net_16',
                     text: '导出',
                     handler: function () {
                         isEdit = 0;
                       //  $('#fm').form('clear');
                     }
                 }, '-',
                    
                {
                    iconCls: 'icon-busy',
                    text: '删除',
                    handler: function () {
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        obj.Delete(null);
                    }
                },
                {
                    iconCls: 'icon-sync',
                    text: '恢复',
                    handler: function () {
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        obj.RecoverOpen();
                    }
                },
                
            ]
        });

        var uploadExcel = new ImportQuestionClass("excel", this.Filter, $("#wd-departmentUid"));

        $("#dwnExcelMould_a").click(function () {
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

        $("#exportTempt").click(function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var names = "";
            $("[name='userPropertys']:checked").each(function (index) {
                names += $(this).val() + ",";
            });
            var url = $("#hidden_a").attr("href");
            $("#hidden_a").attr("href", "/User/student/UserInfoExportTemplate?id=" + names);
            $("#triggerSpan").trigger("click");
        });
 
        //导出
        $("#export-from-excel").on('click', function () {
            
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

           
            var grid = $("#dg");
            var args = [];
            var argsStr = "";
            //  rows = grid.datagrid('getSelections');
            //TODO:这个导出实现逻辑过于复杂，完全没必要这样，有时间进行重构
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
                url: "/User/Student/GetExportDataParms",
                type:"post",
                data: "idList=" + argsStr + "&filter=" + filterData,
                async: false,
                success: function (data) {
                }
            });

            window.location.href = "/User/Student/exportuserinfo";
            //window.location.href = "/User/Student/exportuserinfo?idList=" + argsStr + "&filter=" + filterData;
           
             

           
        });


        this.createDiaog = getSingle(this.dynamicDialog);
    }, Travel:function  (treeID, department) {
        var i;
        if (department != null && department != "") {
            var obj = department.split(",");
            for (i = 0; i < obj.length ; i++) {
                node = $('#' + treeID).combotree('tree').tree('find', obj[i]);
                $('#' + treeID).combotree('tree').tree('check', node.target);
            }
        }
    }
    ,Edit:function (index) {
       


        
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }


         

        isEdit = $('#dg').datagrid('getRows')[index].id;

        var row = $('#dg').datagrid('getRows')[index];
           
        var title = "编辑学生(" + row.user_login_name + ")";
        if (parent.$("#tabs").tabs('exists', title)) {
            parent.$("#tabs").tabs('select', title);
            var currTab = parent.$('#tabs').tabs('getSelected'); //获得当前tab
            var url = "/User/Student/StudentEdit/" + row.id;
            parent.$('#tabs').tabs('update', {
                tab: currTab,
                options: {
                    content: iframeHtml.format({ title: escape(title), url: url }),
                }
            });
        } else { 
            var url = "/User/Student/StudentEdit/"+row.id;
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
    ArrDateFormateByGet:  function (obj,perArr) {
         
        if (obj != null && obj != "undefined") {
            for (var p = 0;p<perArr.length;p++) { // 方法
                obj[perArr[p]] = VE.FormatterDateTime(obj[perArr[p]])
            }
        }

    },

    
    save: function () {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        $("#is_graduation").val($("#is_graduationCreate")[0].checked);
        if (isEdit == 0)
            VE.Save("fm", "/api/services/app/StudentInfo/CreateStudentInfo", dialog, "dg", true, isTrue, VE.GridType_DataGrid);
        else
            VE.Save("fm", "/api/services/app/StudentInfo/UpdateStudentInfo", dialog, "dg", true, isTrue, VE.GridType_DataGrid);
    },
    Delete: function (index) {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        VE.DeleteByUser("/api/services/app/StudentInfo/DeleteStudentInfos", "dg", false, VE.GridType_DataGrid, index);
      
    },
    //查询方法
    Filter: function () {
        //var nodes = $('#department').tree('getChecked');
        //var arrays = [];
        //var id;
        //$(nodes).each(function (index, value) {
        //    if (!value.children) {
        //        id = value.id;
        //        arrays.push(id);
        //    }
        //});
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }
        VE.Filter("UserFilterForm", "dg", VE.GridType_DataGrid);
    },

    //重置
    Reset:function () {
        VE.Clear("UserFilterForm", "dg", VE.GridType_DataGrid);
    },
  
    cancel: function () {
        dialog.dialog('close');
        // $('#dlg').dialog('close');
    }, AddStudent: function () {

        var title = "新增学生";
        var url = "/User/Student/StudentEdit";
        parent.$("#tabs")
            .tabs("add",
            {
                title: title,
                content: iframeHtml.format({ title: title, url: url }),
                closable: true,
                icon: "icon-add"
            });
    },
    Active: function (index) {

        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
            return;
        }

        var enbleFlag = $("#dg" ).datagrid('getRows')[index].user_enble_flag;
        VE.UpdateStatu("/api/services/app/StudentInfo/ActiveStudentInfo", "dg", true, VE.GridType_DataGrid, index, enbleFlag == 0 ? "确定封禁用户?" : "确定解封用户?", "操作成功！");
    },

    ////操作按钮
    formatOper: function (val, row, index) {
        var rowObj = row;
        var option = {
            option: [
                {
                    text: '编辑',
                    icon: 'icon-edit',
                    title: '编辑',
                    onclick: ' obj.Edit'
                }
            ]
        };
        var menus = menubtn(index, index, {
            option: [
                {
                    text: '更多',
                    icon: 'icon-list',
                    items: [
                        {
                            text: '删除',
                            icon: 'icon-enter',
                            onclick: ' obj.Delete',
                            hide: false
                        }, {
                            text: '修改密码',
                            icon: 'icon-tools',
                            onclick: ' obj.ResetPwd',
                            hide: false
                        }, {
                            text: rowObj.user_enble_flag == 0 ? "封禁用户" : "解封用户",
                            icon: 'icon-exit',
                            onclick: ' obj.Active',
                            hide: false
                        }
                    ]
                }
            ]
        });
       
        return linkbtn(index, option) + menus;
    },
     

    formatGraduation: function (val) {

        if (val === 0) return "<span style=\"display:inline-block;height:16px;width:16px;\" title='正常' class=\"icon-active_true\"></span>";
        else return "<span style=\"display:inline-block;height:16px;width:16px;\"  title='禁用' class=\"icon-busy\"></span>";

    }, ResetPwd: function (index) {
        if (!checkLogin()) {
            evtBus.dispatchEvt("show_login");
          
            return;
        }
 
        var row = $('#dg').datagrid('getRows')[index];
        var fromObj = { userId: row.user_id, userMobile: row.user_mobile, userName: row.user_login_name }
        dialog = $("#pwdModifiry").dialog({
            title: '修改密码（' + row.user_login_name+')',
            width: 500,
            height: 300,
            closed: false,
            cache: false,
            href: VE.AppPath + '/User/User/ResetPwd',
            modal: true,
            buttons: '#dlg-pwdEdit',
            onLoad: function () {
                if (fromObj != null) {
                    $('#pwdUpdate_form').form('load', fromObj);
                }
            }
        });
    },
    ResetPwdSubmit: function () {
        VE.Save("pwdUpdate_form", "/api/services/app/UserInfo/UserPassWordModify", dialog, "dg", true, isTrue, VE.GridType_DataGrid);
    },
    formatActive: function (val) {

        if (val == 1) return "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-lock\"></span>";
        else return  "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-unlock\"></span>";

    }, getTabIndex: function () {
       
        var tab = parent.$("#tabs").tabs("getSelected");
        return parent.$("#tabs").tabs("getTabIndex", tab);
    }, evtHandle: evtBus.addEvt("studentInfo_exit", function (tabObj) {
        
        parent.$("#tabs").tabs("close", getTabIndex())
                    .tabs("select", obj.tabIndex);
        obj.Filter();
        VE.MessageShow("操作成功");
    })
    ,
    tabIndex: getTabIndex(),

    //恢复
    RecoverOpen: function () {
        $("#recovery_dialog").dialog('open').dialog('setTitle', '恢复账号');
        $("#recovery_fm").form("clear");
    },

    recoverySave: function () {
        if (!$('#recovery_fm').form('validate')) {
            return false;
        }
        var url = "/Student/Recovery";
        $.ajax({
            url: url,
            type: 'post',
            data: $("#recovery_fm").serialize(),
            success: function (data) {
                if (data.msg == "ok") {
                    VE.MessageShow("恢复成功");
                    obj.recoveryCancel();
                    $("#dg").datagrid("reload");
                }
                if (data.msg == "notExist") {
                    //错误消息停留3S
                    VE.MessageAlertTimeout = 3000;
                    VE.MessageShow(data.ErrMsg);  
                }
            }
        });
    },
    recoveryCancel: function () {
        $("#recovery_dialog").dialog("close");
    },
    checkName_Exist: function (valueName) {


        var b = "0";
        var type = this.GetType();
        var statu = false;
        if (type == "update" && valueName == this.oldUsername) {
            return true
        }
        /*  $.ajax({
              type: "GET",
              dataType: "json",
              //  dataType: "jsonp",
              //   jsonp: "callback",
              async: false,//是否异步执行
              url: "/User/User/UserNameCheck?type=" + type + "" + "&checkname=" + escape(valueName) + "&oldName=" + this.oldUsername,
              // url: urlSrc,
              success: function (data) {
                  b = data;
                  //  var obj=eval(data)
                  // b = data.IsSuccess;
              },
              error: function (errorMSG) {
                  b = false;
              }
          });*/
        $.ajax({
            type: "post",
            dataType: "json",
            async: false,//是否异步执行
            url: "/api/services/app/UserInfo/CheckNameExit?type=" + type + "" + "&name=" + escape(valueName) + "&oldname=" + this.oldUsername + "&isRemoteCheck=false",
            success: function (data) {
                b = data.result;
            },
            error: function (errorMSG) {
                b = false;
            }
        });
        return b;
    }, CheckValue_Exist: function (url, data) {
        var b = "0";
        var type = this.GetType();
        $.ajax({
            type: "post",
            dataType: "json",
            async: false,//是否异步执行
            data: data,
            url: url,
            success: function (msg) {
                b = msg.result;
            },
            error: function (errorMSG) {
                b = false;
            }
        });
        return b;
    },
};
    
Index.prototype.init.prototype = Index.prototype;
var createObj = getSingle(Index);
var obj = createObj();


function getTabIndex() {
       
    var tab = parent.$("#tabs").tabs("getSelected");
    return parent.$("#tabs").tabs("getTabIndex", tab);
     
}

$(function () {
    //初始化回车事件
    VE.Enter("obj.Filter");

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
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckEmailExist?type=" + obj.GetType() + "&email=" + escape(value) + "&oldEmail=" + obj.oldEmail +""+"&isRemoteCheck=false", "")
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



 