var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';
var isEdit = 0;
function student() {
    this.isEdit = 0;
    this.type = "";
    this.oldUsername = "";
    this.oldMobile = "";
    this.oldEmail = "";
    this.oldIdcard = "";
    this.oldpwd = "";
    this.tabHelper = new TabHelper("tabs");
    this.selfTabIndex = 0;
}
student.prototype = {
    init: function () {
        //$.post('/api/services/app/Department/GetDepartmentTree').done(function (data) { //获取下拉框

        //    $("#user_classid").combotree(
        //        {
        //            data: data,
        //            onBeforeSelect: function(node) {
        //                //返回树对象 
        //                var tree = $(this).tree;
        //                //选中的节点是否为叶子节点,如果不是叶子节点,清除选中 
        //                var isLeaf = tree('isLeaf', node.target);
        //                if (!isLeaf) {
        //                    return false;
        //                    //清除选中  
        //                    // $('#user_classid').combotree('clear');
        //                    //$("#user_classid").treegrid("unselect");
        //                }
        //            },
        //            onSelect: function(item) {
        //                var parent = item;
        //                var tree = $('#user_classid').combotree('tree');
        //                var path = new Array();
        //                do {
        //                    path.unshift(parent.text);
        //                    var parent = tree.tree('getParent', parent.target);
        //                } while (parent);
        //                var pathStr = '';
        //                for (var i = 0; i < path.length; i++) {
        //                    pathStr += path[i];
        //                    if (i < path.length - 1) {
        //                        pathStr += ' - ';
        //                    }
        //                }
        //                $("#user_classid").combotree('setValue', item.id);
        //                $('#user_classid').combotree('setText', pathStr);
        //            }
        //        }
        //    );
        //    if (stuId == "") {
        //        $('#user_password').textbox({ required: true });
        //        isEdit = 0;
        //        $('#fm').form('reset');
        //    } else {
        //        obj.dataBind();
        //    }
        //});
        $("#user_facultyid").combobox({
            url:  "/api/services/app/Department/CmbFaculty", //获取专业
            valueField: "id",
            textField: "name",
            panelHeight: 304,
            editable: true, //允许手动输入
            onSelect: function (item) {
                var url = '/api/services/app/Department/CmbMajor?facultyId=' + item.id;
                $('#user_majorid').combobox('clear');
                $('#user_majorid').combobox('reload', url);
                $('#user_majorid').combobox({
                    onSelect: function (data) {
                        var url = '/api/services/app/Department/CmbClass?majorId=' + data.id;
                        $('#user_classid').combobox('clear');
                        $('#user_classid').combobox('reload', url);
                       
                    }
                });
            }, onLoadSuccess: function() {
                if (stuId == "") {
                    $('#user_password').textbox({ required: true });
                    isEdit = 0;
                    $('#fm').form('reset');
                } else {
                    obj.dataBind();
                }
            }
        });
     
        obj.selfTabIndex = obj.tabHelper.getTabIndex();
     
    },
    GetType: function () { return isEdit == 0 ? "insert" : "update"; },
    formCheck: function (formId) {

        var b = $('#' + formId).form('validate');//Easyui 验证结果
        if (b) VE.Mask("", formId); //加载等待框

        return b;
    },
    ArrDateFormateByGet: function (obj, perArr) {

        if (obj != null && obj != "undefined") {
            for (var p = 0; p < perArr.length; p++) { // 方法
                obj[perArr[p]] = VE.FormatterDateTime(obj[perArr[p]])
            }
        }

    },
    dateOnselect: function (obj) {
        $(obj).next('span').find('input').focus();
    },
 
    formBind: function (row) {

        isEdit = row.id;
        obj.oldUsername = row.user_login_name;
        obj.oldMobile = row.user_mobile;
        obj.oldEmail = row.user_email;
        obj.oldpwd = row.user_password;
        obj.oldIdcard = row.user_idcard;
        obj.oldAgentUserId = row.agent_user_id;
        obj.oldAgentUserName = row.agent_user_name;

        $('#fm').form('clear');
        obj.ArrDateFormateByGet(row, ["user_birthday", "user_admission", "graduation_date", "create_time", "updateTime", "loginTime"]);

        $('#user_password').textbox({ required: false });
        row.user_password = "";
        $('#fm').form('load', row);
        setTimeout(function () { $('#user_classid').combobox('setValue', row.user_classid);   }, 100);
        
        $("#is_graduationCreate")[0].checked = row.is_graduation;
    },
    dataBind: function () {
        var bind = this.formBind;
        //GetStudent
        var row = null;

        $.ajax({
            url: "/User/Student/GetStudent/",
            type: "post",
            dataType: "json",
            data: { "id": stuId },
            async: false,
            success: function (jsonDta) {

                bind(jsonDta);
            },
            error: function (errStatu) {

            }
        });



    },
    closeTab: function () {



        var tab = parent.$("#tabs").tabs("getSelected");
        var tabIndex = parent.$("#tabs").tabs("getTabIndex", tab);

        var tabTitle = tab.panel('options').tab[0].textContent;

        evtBus.dispatchEvt("studentInfo_exit", { "tabIndex": tabIndex, "tabTitle": tabTitle });
    },
    save: function (url, formId) {
        var close = this.closeTab;
        VE.Json = {}; //清空数据
        var data = VE.GetFormData(formId);
        var isSuccess = false;
        $.ajax({
            url: VE.AppPath + url,
            contentType: 'application/json',
            type: "post",
            async: false,
            data: JSON.stringify(data),
            success: function (r) {
                VE.UnMask();
               VE.MessageShow("操作成功");
               isSuccess = true;
               
            },
            error: function (jqxhr) {
                var data = jqxhr.responseJSON;
                VE.UnMask();
                isSuccess = false;
                if (data.error.code == 0) {
                    $.messager.alert('提示', data.error.message, "error");
                }
                else if (data.error.code == 1) {
                    $.messager.alert('提示', data.error.message, "info");
                }
                else if (data.error.code == 2) {
                    $.messager.alert('提示', data.error.message, "warn");
                } else {
                    $.messager.alert('提示', data.error.message, "error");
                }
            }
        });

        if (isSuccess) {
            close();
        }
    },
    editSave: function () {
        var url = "";
        $("#is_graduation").val($("#is_graduationCreate")[0].checked);
        if (isEdit == 0)
        { url = "/api/services/app/StudentInfo/CreateStudentInfo"; }
        else
        { url = "/api/services/app/StudentInfo/UpdateStudentInfo"; }
        if (this.formCheck("fm")) {
            this.save(url, "fm")
        }
    },
    checkName_Exist: function (valueName) {

        var b = "0";
        var type = this.GetType();
        var statu = false;
        if (type == "update" && valueName == this.oldUsername) {
            return true;
        }
      
        $.ajax({
            type: "post",
            dataType: "json",
            async: false,//是否异步执行
            url: "/api/services/app/UserInfo/CheckNameExit?type=" + type + "" + "&name=" + escape(valueName) + "&oldname=" + this.oldUsername,
            success: function (data) {
                b = data.result;
            },
            error: function () {
                b = false;
            }
        });
        return b;
    },
    CheckValue_Exist: function (url, data) {
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
    evtHandle: evtBus.addEvt("studentInfo_bind", function (row) {
        
        this.formBind(row);

    })
}

var obj = new student();
$(function () {
    obj.init();
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
        message:  "用户名必须包含英文加数字"  
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
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckMobileExist?type=" + obj.GetType() + "&mobile=" + escape(value) + "&oldMobile=" + obj.oldMobile + "&isRemoteCheck=true", "")
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
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckEmailExist?type=" + obj.GetType() + "&email=" + escape(value) + "&oldEmail=" + obj.oldEmail + "", "")
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
            var v = obj.CheckValue_Exist("/api/services/app/UserInfo/CheckIdCardExist?type=" + obj.GetType() + "&idCard=" + escape(value) + "&oldIdCard=" + obj.oldIdcard + "", "")
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
