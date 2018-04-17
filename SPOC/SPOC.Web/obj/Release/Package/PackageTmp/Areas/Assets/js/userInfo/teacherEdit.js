var isEdit = 0;
var oldUsername = "";
var editor1;
KindEditor.ready(function (K) {
    editor1 = K.create('#teacherPersonalResume', {
        cssPath: '/Script/kindeditor/4.1.10/plugins/code/prettify.css',
        uploadJson: '/User/Teacher/TeacherResumeImgUpload',
        fileManagerJson: '/User/Teacher/TeacherResumeImgUpload',
        //allowFileManager: true,
        width: "360px",
        resizeMode: 1,
        minWidth: 360,
        allowPreviewEmoticons: false,
        items: ['bold', 'italic', 'underline', 'forecolor', '|', 'insertorderedlist', 'insertunorderedlist', '|', 'link', 'unlink', 'image', '|', 'removeformat', 'source'],
        afterCreate: function () {
            var self = this;
            K.ctrl(document, 13, function () {
                self.sync();
            });
            K.ctrl(self.edit.doc, 13, function () {
                self.sync();
            });
        }
    });

});

function student(container) {
    // this.container = container;
    // this.tiles = new Array(16);
    this.isEdit = 0;
    this.type = "";
    this.oldUsername = "";
    this.oldMobile = "";
    this.oldEmail = "";
    this.oldIdcard = "";
    this.oldpwd = "";
}
student.prototype = {
    init: function () {
        $.post('/api/services/app/TeacherInfo/BindCmb').done(function (data) { //获取下拉框

            var rst = JSON.parse(data.result);
            $.each(rst, function (i, v) {
                if (v.type == 'teacherTitleCreate' || v.type == 'teacherTitle') {
                    $('#' + v.type).combobox('loadData', v.datas);

                } else {
                    $('#' + v.type).combotree('loadData', v.datas);

                }
            });

            if (stacherId == "") {
                $('#user_password').textbox({ required: true });

                isEdit = 0;
                $('#teacherEdit_form').form('clear');
            } else {
                obj.dataBind();
            }
        });

      
    },
    GetType: function () { return isEdit == 0 ? "insert" : "update"; },
    formCheck: function (formId) {

        var b = $('#' + formId).form('validate');//Easyui 验证结果
        if (b) VE.Mask("", formId); //加载等待框

        return b;
    }, ArrDateFormateByGet: function (obj, perArr) {

        if (obj != null && obj != "undefined") {
            for (var p = 0; p < perArr.length; p++) { // 方法
                obj[perArr[p]] = VE.FormatterDateTime(obj[perArr[p]]);
            }
        }

    },
    
    formBind: function (row) {

        isEdit = row.id;
        $('#teacherEdit_form').form('clear');
        row.teacherTitleCreate = row.teacherTitle;
        oldUsername = row.user_login_name;
        obj.oldMobile = row.user_mobile;
        obj.oldEmail = row.user_email;
        obj.oldIdcard = row.teacherIdCode;
        //VE.ArrDateFormate(row);
        row.user_password = "";
        $('#user_password').textbox({ required: false });
        obj.ArrDateFormateByGet(row, ["teacherBirthday", "teacherEntryDate", "teacherStartworkDate", "teacherGraduateDate", "createTime", "updateTime", "loginTime"]);
        row.departmentId = (row.departmentId == null || row.departmentId == "undefined") ? "" : row.departmentId;

        $('#teacherEdit_form').form('load', row);
       
        $('#classId').combotree('setValues', row.classIds.split(","));
        editor1.html(row.teacherPersonalResume);
    },
    dataBind: function () {
        var bind = this.formBind;
        //GetStudent
        var row = null;

        $.ajax({
            url: "/User/Teacher/GetTeacher/",
            type: "post",
            dataType: "json",
            data: { "id": stacherId },
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

        evtBus.dispatchEvt("teacherInfo_exit", { "tabIndex": tabIndex, "tabTitle": tabTitle });
    },
    save: function (url, formId) {
        /*  var b = $('#' + formId).form('validate');//Easyui 验证结果
          if (b) { VE.Mask("", formId); } //加载等待框
          else {
              return;
          }*/
        var close = this.closeTab;
        VE.Json = {}; //清空数据
        var data = VE.GetFormData(formId);
        var nodes = $("#classId").combotree("tree").tree("getChecked");
        var classIds = [];
        $.each(nodes,
            function(k, v) {
                if (!v.children || v.children.length === 0) {
                    classIds.push(v.id);
                }
            });
        data["classId"] = classIds.toString();
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
        editor1.sync();

        if (isEdit == 0)
        { url = "/api/services/app/TeacherInfo/CreateTeacherInfo"; }
        else
        { url = "/api/services/app/TeacherInfo/UpdateTeacherInfoByUser"; }
        if (this.formCheck("teacherEdit_form")) {
            this.save(url, "teacherEdit_form")
        }
    }, checkName_Exist: function (valueName) {
        var b = "0";
        var type = this.GetType();
        var statu = false;
        if (type == "update" && valueName == oldUsername) {
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
            url: "/api/services/app/UserInfo/CheckNameExit?type=" + type + "" + "&name=" + escape(valueName) + "&oldname=" + this.oldUsername,
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
    }, evtHandle: evtBus.addEvt("studentInfo_bind", function (row) {
        this.formBind(row);

    })
}

var obj = new student("");
$(function () {
    obj.init();


});

 
/********-------------------------------验证---------------------------*/


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
            // var v = obj.CheckValue_Exist("/User/User/UserMobileCheck", { "type": obj.GetType(), "checkMobile": escape(value), "oldMobile": obj.oldMobile })
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
    }, idCardCheckExist: {
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
    }, requiredByEdid: {
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
