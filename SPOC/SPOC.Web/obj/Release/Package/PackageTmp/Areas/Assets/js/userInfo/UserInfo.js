var User = $.extend({}, User);/* 全局对象 */

User.OpenUserInfoDialog = function (userQueryData, openDialogData, userName) {
    if ($("#userInfo_datagrid").length == 0) {
        var comChild = VE.getDatagridDiv("userInfo_datagrid")
        document.getElementById("UserFilterForm").appendChild(comChild);
    }
  //  var row = $('#dg').datagrid('getRows')[rowIndex];
    var identity = "";
    $.ajax({
        url: "/User/User/GetUser",
        type: "post",
     //   data: { userId: row.UserId },
        data: userQueryData,
        dataType: "json",
        async: false,
        success: function (data) {

            identity = data.identity;
        }
    });
    var hrefSrc = "";

    switch (identity) {
        case 1: { hrefSrc = VE.AppPath + '/User/User/StudentInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
        case 2: { hrefSrc = VE.AppPath + '/User/User/TeacherInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
        case 3: { hrefSrc = VE.AppPath + '/User/User/AdminInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
    }
    var title = identity == 1 ? "查看学生" : (identity == 2 ? "查看教师" : "查看管理员");
    dialog = $("#userInfo_datagrid").dialog({
        title: title+'（' + userName + ')',
        width: identity==3?500:650,
        height: identity==3?200:500,
        closed: false,
        cache: false,
        href: hrefSrc,
        modal: true,
    });
}

User.OpenUserInfoTalbeDialog = function (openDialogData, userName) {
    var identity = "";
    $.ajax({
        url: "/User/User/GetUser",
        type: "post",
        //   data: { userId: row.UserId },
        data: openDialogData,
        dataType: "json",
        async: false,
        success: function (data) {

            identity = data.identity;
        }
    });
    var hrefSrc = "";
    switch (identity) {
        case 1: { hrefSrc = VE.AppPath + '/User/User/StudentInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
        case 2: { hrefSrc = VE.AppPath + '/User/User/TeacherInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
        case 3: { hrefSrc = VE.AppPath + '/User/User/AdminInfoTableDetails?' + openDialogData + '&identity=' + identity } break;
    }
    $("#detailTable_iframe").attr("src", hrefSrc);
    //$("#detailTable_iframe").height(identity == 3 ? 170 : 380)
    var title = identity == 1 ? "查看学生" : (identity == 2 ? "查看教师" : "查看管理员");
    $("#detailTable_dialog").dialog({ width: identity == 3 ? 500 : 650, height: identity == 3 ? 200 : 500 });

    $("#detailTable_dialog").dialog("open").dialog("setTitle", title + '（' + userName + ')');
   
}