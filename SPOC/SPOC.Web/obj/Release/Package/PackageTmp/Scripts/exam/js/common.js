
AddEvent(window, "load", Aattr);
function AddEvent(element, type, handler) {
    if (element.attachEvent) {
        element.attachEvent("on" + type, handler);
    } else if (element.addEventListener) {
        element.addEventListener(type, handler, false);
    }
}
function Aattr() {
    if (document.documentMode == 10) {
        var anchors = document.getElementsByTagName("a");
        for (var i = 0; i < anchors.length; i++) {
            var anchor = anchors[i];
            if (anchor.getAttribute("disabled") == "true" || anchor.getAttribute("disabled") == "disabled") {
                anchor.style.color = '#cccccc';
            }
        }
    }
} 

function doSort(callback) {
    $(document).ready(function () {
        $("span[class^=Nsb_r_list_tha]").each(function (i) {
            if ($("#hidSortFields").val().replace(" desc ", "") == $(this).attr("id")) {
                $(this).removeClass();
                $(this).addClass($.cookie("hidSortFieldsClass"));
            }
            else {
                $(this).removeClass();
                $(this).addClass("Nsb_r_list_tha3");
            }
            $(this).bind("click", function () {
                if ($(this).attr("class") == "Nsb_r_list_tha1" || $(this).attr("class") == "Nsb_r_list_tha3") {
                    $("#hidSortFields").val($(this).attr("id"));
                    $(this).removeClass("Nsb_r_list_tha3");
                    $(this).removeClass("Nsb_r_list_tha1");
                    $(this).addClass("Nsb_r_list_tha2");
                    $.cookie("hidSortFieldsClass","Nsb_r_list_tha2");
                }
                else {                    
                    $("#hidSortFields").val($(this).attr("id") + " desc ");
                    $(this).removeClass("Nsb_r_list_tha2");
                    $(this).addClass("Nsb_r_list_tha1");
                    $.cookie("hidSortFieldsClass", "Nsb_r_list_tha1");
                }
                eval(callback);
            });
        });
    });
}
//下面是个人中心对扩展信息控件样式修改
try {
    $(function () {
        //扩展属性控件样式
        $(".commoninput").each(function () {
            $(this).removeClass("commoninput");
            $(this).addClass("Nsb_form_s_tt");
        });
        $(".commontextarea").each(function () {
            $(this).removeClass("commontextarea");
            $(this).addClass("Nsb_form_s_ttarea");
        });
        $("select[id^='extend']").each(function () {
            $(this).addClass("Nsb_form_s_slct");
        });
    });
} catch (e) { }

//单个勾选方法
function jcomGetAllSelectedRecords(SelectName) {
    if (!SelectName)
        SelectName = "chkSelect";

    var sSelectIDs = "";
    var elementList = document.getElementsByName(SelectName);
    for (i = 0; i < elementList.length; i++) {
        if (elementList[i].checked == true) {
            sSelectIDs = sSelectIDs + elementList[i].value + ",";
        }
    }

    if (sSelectIDs != "") {
        sSelectIDs = sSelectIDs.substr(0, sSelectIDs.length - 1);
    }
    return sSelectIDs;
}

//全部勾选方法
function jcomSelectAllRecords(SelectAllName, SelectName) {
    if (!SelectAllName)
        SelectAllName = "chkSelectAll";
    if (!SelectName)
        SelectName = "chkSelect";

    var checked = document.getElementById(SelectAllName).checked;
    var sSelectedIDs = "";
    var elementList = document.getElementsByName(SelectName);
    for (i = 0; i < elementList.length; i++) {
        elementList[i].checked = checked;
        sSelectedIDs = sSelectedIDs + elementList[i].value + ",";
    }
    if (sSelectedIDs != "") sSelectedIDs = sSelectedIDs.substr(0, sSelectedIDs.length - 1);
    return sSelectedIDs;
}
function jscomNewOpenMaxWindow(url, target) {
    var tt, w, left, top, width, height;
    width = screen.width;
    height = screen.height - 60;
    left = 0;
    if (left < 0) { left = 0; }

    top = 0;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

var _printPreviewReturnParameter = "";
var _printPreviewReturnLabel = "";
//打印方法
function jscomPrintPreviewOneFolder(title, printText, hidCols, type) {
   
    _printPreviewReturnParameter = title + "&Spliter;" + printText + "&Spliter;" + hidCols + "&Stype;" + type;
    window.open("../framework/PrintPreview.aspx");
}

//打印预览回调函数
function jscomPrintPreviewCallback() {
    return _printPreviewReturnParameter;
}

//打印预览回调打印标签函数
function jscomPrintLabelCallback() {
    return _printPreviewReturnLabel;
}

function jscomPrintPreviewByLabel(title, label, printText, hidCols, type) {
    _printPreviewReturnParameter = title + "&Spliter;" + printText + "&Spliter;" + hidCols + "&Stype;" + type;
    _printPreviewReturnLabel = label;
    window.open("../framework/PrintPreview.aspx");
}
/*hidCols:需要隐藏的列数
*type:类型(1.表格内未出现纵向合并单元格 2.出现纵向合并 ||未出现可不填)
*lopping 2010-10-25
*/
function jscomPrintPreview(title, printText, hidCols, type) {
    _printPreviewReturnParameter = title + "&Spliter;" + printText + "&Spliter;" + hidCols + "&Stype;" + type;
    window.open("../framework/PrintPreview.aspx");
}

function jscomDownloadFile(fileName, fileLocation) {
    if (fileLocation == null) {
        fileLocation = "local";
    }
    window.open("../module/DownloadFile.aspx?fileName=" + fileName + "&fileLocation="+fileLocation);
}
function GetEventObject() {
    if (document.all)
        return window.event;

    func = GetEventObject.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {
            if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == 'object' && arg0.preventDefault && arg0.stopPropagation)) {
                return arg0;
            }
        }
        func = func.caller;
    }
    return null;
}
function jscomNewOpenBySize(url, target, width, height) {
    var tt, w, left, top;
    if (!width) width = screen.width;
    if (!height) height = screen.height - 60;
    left = (screen.width - width) / 2;
    if (left < 0) { left = 0; }

    top = (screen.height - 60 - height) / 2;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}
//检查输入：	只能输入非负整数 失去焦点时判断
function jscommOnBlurCheckForOnlyPositiveInteger() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        }
        else if (src.value.indexOf(".", 0) > -1) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        } else if (parseInt(src.value) < 0) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        }
    }
}
//检查输入：	只能输入非负有效数字 失去焦点时判断
function jscomOnBlurCheckForOnlyPositiveNumber() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common13", "", "请输入非负数字！"));
            src.focus();
            src.select();
        } else if (parseFloat(src.value) < 0) {
            window.alert(Translate("common13", "", "请输入非负数字！"));
            src.focus();
            src.select();
        }
    }
}
function jscomNewOpenModalDialog(url, width, height) {
    var d = new Date;
    if (url.lastIndexOf("?") == -1) {
        url = url + "?currTime=" + d.getTime();
    } else {
        url = url + "&currTime=" + d.getTime();
    }
    return showModalDialog(url, window, 'dialogWidth:' + width + 'px; dialogHeight:' + height + 'px;help:0;status:0;resizeable:1;');
}

function jscomNewOpenBySize(url, target, width, height) {
    var tt, w, left, top;
    if (!width) width = screen.width;
    if (!height) height = screen.height - 60;
    left = (screen.width - width) / 2;
    if (left < 0) { left = 0; }

    top = (screen.height - 60 - height) / 2;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}
function GetEventObject() {
    if (document.all)
        return window.event;

    func = GetEventObject.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {
            if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == 'object' && arg0.preventDefault && arg0.stopPropagation)) {
                return arg0;
            }
        }
        func = func.caller;
    }
    return null;
}
function jscomNewOpenFullScreen(url, target) {
    var w = window.open(url, target, "fullscreen=yes,toolbar=no");
    try {
        w.focus();
    } catch (e) { }
}

function jscomSelectDept(ParentDeptUid, DeptUid, OpenerName, DeptRight, IsLimitByOwner, oppositePath) {
    if (oppositePath == null) oppositePath = "../../";
    if (DeptUid == null)
        DeptUid = "";
    if (ParentDeptUid == null) ParentDeptUid = "";
    if (DeptRight == null) DeptRight = "";
    if (IsLimitByOwner == null) IsLimitByOwner = "Y";
    if (OpenerName != "") {
        parent.WebTab_CreateTab("SelectDept", Translate("business5", "", "选择部门"), "framework/user/SelectDept.aspx?OpenerName=" + OpenerName + "&DeptRight=" + DeptRight + "&IsLimitByOwner=" + IsLimitByOwner + "&ParentDeptUid=" + ParentDeptUid + "&SelectDeptUid=" + DeptUid);
    }
    else {
        jscomNewOpenBySize(oppositePath + "framework/user/SelectDept.aspx?OpenerName=" + OpenerName + "&DeptRight=" + DeptRight + "&IsLimitByOwner=" + IsLimitByOwner + "&ParentDeptUid=" + ParentDeptUid + "&SelectDeptUid=" + DeptUid, "SelectDept", 600, 480);
    }
}

//=====================Rick 获取事件的方法==================
function GetEventObject() {
    if (document.all)
        return window.event;

    func = GetEventObject.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {
            if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == 'object' && arg0.preventDefault && arg0.stopPropagation)) {
                return arg0;
            }
        }
        func = func.caller;
    }
    return null;
}
//检查输入：	只能输入有效数字 失去焦点时判断
function jscomOnBlurCheckForOnlyNumber() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common1", "", "请输入数字类型！"));
            src.focus();
            src.select();
        }
    }
}
function GetFormObject(formName) {
    if (formName == null)
        formName = "form1";
    var theForm = document.forms[formName];
    if (!theForm) {
        theForm = eval("document." + formName);
    }

    return theForm;
}
function bgiframe(objdiv) {
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    if (Sys.ie == "6.0") {
        var ifrm = document.createElement('iframe');
        ifrm.id = objdiv.id;
        ifrm.src = "javascript:false";
        ifrm.style.cssText = "display:block;position:absolute; visibility:inherit; top:0px; left:0px; width:100%; height:100%; z-index:-1; border:0;filter:Alpha(Opacity='0');";
        objdiv.insertBefore(ifrm, objdiv.firstChild);
    }
}
function GetNavigator() {
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    return Sys;
}

function ClearSeach(name) {
    var Forms = document.getElementById(name).getElementsByTagName("input");
    for (var i = 0; i < Forms.length; i++) {
        if (Forms[i].type == 'text') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'checkbox') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'radio') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'hidden') {
            Forms[i].value = "";
        }
    }
    var Forms = document.getElementById(name).getElementsByTagName("select");
    for (var i = 0; i < Forms.length; i++) {
        if (Forms[i].disabled == false)
            Forms[i].value = "";
    }
}

/*--------------------------------*/
// JScript 文件

AddEvent(window, "load", Aattr);
//window.attachEvent("onload", Aattr);
function Aattr() {
    if (document.documentMode == 10) {
        var anchors = document.getElementsByTagName("a");
        for (var i = 0; i < anchors.length; i++) {
            var anchor = anchors[i];
            if (anchor.getAttribute("disabled") == "true" || anchor.getAttribute("disabled") == "disabled") {
                anchor.style.color = '#cccccc';
            }
        }
    }
}

//window.focus();
function jscomResizeWindow(width, height) {
    var left, top;
    left = (screen.width - width) / 2;
    if (left < 0) { left = 0; }

    top = (screen.height - 60 - height) / 2;
    if (top < 0) { top = 0; }

    window.resizeTo(width, height);
    window.moveTo(left, top);
}

function jscomNewOpenFullScreen(url, target) {
    var w = window.open(url, target, "fullscreen=yes,toolbar=no");
    try {
        w.focus();
    } catch (e) { }
}

function jscomNewOpenMaxWindow(url, target) {
    var tt, w, left, top, width, height;
    width = screen.width;
    height = screen.height - 60;
    left = 0;
    if (left < 0) { left = 0; }

    top = 0;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

function jscomNewOpenBySize(url, target, width, height) {
    var tt, w, left, top;
    if (!width) width = screen.width;
    if (!height) height = screen.height - 60;
    left = (screen.width - width) / 2;
    if (left < 0) { left = 0; }

    top = (screen.height - 60 - height) / 2;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=yes,resizable=yes,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

function jscomNewOpenByFixSize(url, target, width, height) {
    var tt, w, left, top;
    if (!width) width = screen.width;
    if (!height) height = screen.height - 60;
    left = (screen.width - width) / 2;
    if (left < 0) { left = 0; }

    top = (screen.height - 60 - height) / 2;
    if (top < 0) { top = 0; }

    tt = "toolbar=no, menubar=no, scrollbars=no,resizable=no,location=no, status=yes,";
    tt = tt + "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

function jscomNewOpenBySizePos(url, target, width, height, left, top) {
    var tt;

    tt = "toolbar=no, menubar=no, scrollbars=no,resizable=yes,location=no, status=yes,";
    tt = tt + ",width=" + width + ",height=" + height;
    tt = tt + ",left=" + left + ",top=" + top;
    w = window.open(url, target, tt);
    try {
        w.focus();
    } catch (e) { }
}

function jscomNewOpenModalDialog(url, width, height) {
    var d = new Date;
    if (url.lastIndexOf("?") == -1) {
        url = url + "?currTime=" + d.getTime();
    } else {
        url = url + "&currTime=" + d.getTime();
    }
    return showModalDialog(url, window, 'dialogWidth:' + width + 'px; dialogHeight:' + height + 'px;help:0;status:0;resizeable:1;');
}

function jscomOpenParameterPasser(OpenerName, WebTabName, WebTabTitle, GetValueFunctionName, RedirectURL) {
    var url = "module/ParameterPasser.aspx?OpenerName=" + OpenerName + "&GetValueFunctionName=" + GetValueFunctionName + "&RedirectURL=" + RedirectURL;
    parent.WebTab_CreateTab(WebTabName, WebTabTitle, url);
}

function jscomGetParentFromSrc(src, parTag) {
    if (src && src.tagName != parTag)
        src = getParentFromSrc(src.parentElement, parTag);
    return src;
}

function jscomGetSubString(str, begin_pos, num) {
    return str.toString().substring(begin_pos, begin_pos + num);
}

function jscomDoNothing() {
}

//过滤特殊符号（如' "） 
function jscomFiltrateSomeKeyForKeyPress() {
    if (event.keyCode == 39 || event.keyCode == 34) {
        event.keyCode = 0;
    }
}

//只能输入数字
function jscomOnlyNumForKeypress() {
    //alert(event.keyCode);
    if (event.keyCode < 48 || event.keyCode > 57) {   //0=>48  9=>57
        event.keyCode = 0;
        return false;
    } else {
        return true;
    }
}

//设置检查框 flag =1 全部选中 或 0=全部清除
function jscomSelectCheckFlag(frm, flag) {
    var src;
    for (var i = 0; i < frm.elements.length; i++) {
        src = frm.elements[i];
        if (src.type == "checkbox") {
            if (flag == 1) {
                src.checked = true;
            } else {
                src.checked = false;
            }
        }
    }
}
//判断是否有检查框被选中
//返回 true有  false 无
function jscomIsCheckBoxSelect(frm) {
    var frm, src;

    flag = false;
    for (var i = 0; i < frm.elements.length; i++) {
        src = frm.elements[i];
        if (src.type == "checkbox" && src.checked) {
            flag = true;
            break;
        }
    }
    return flag;
}

//检查输入：	只能输入有效数字 失去焦点时判断
function jscomOnBlurCheckForOnlyNumber() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common1", "", "请输入数字类型！"));
            src.focus();
            src.select();
        }
    }
}

//检查输入：	只能输入非负有效数字 失去焦点时判断
function jscomOnBlurCheckForOnlyPositiveNumber() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common13", "", "请输入非负数字！"));
            src.focus();
            src.select();
        } else if (parseFloat(src.value) < 0) {
            window.alert(Translate("common13", "", "请输入非负数字！"));
            src.focus();
            src.select();
        }
    }
}

//检查输入：	只能输入整数 失去焦点时判断
function jscomOnBlurCheckForOnlyInteger() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common2", "", "请输入整型数字！"));
            src.focus();
            src.select();
        }
        else if (src.value.indexOf(".", 0) > -1) {
            window.alert(Translate("common2", "", "请输入整型数字！"));
            src.focus();
            src.select();
        }
    }
}
//检查输入：	只能输入非负整数 失去焦点时判断
function jscommOnBlurCheckForOnlyPositiveInteger() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.tagName == "INPUT") {
        if (isNaN(src.value)) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        }
        else if (src.value.indexOf(".", 0) > -1) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        } else if (parseInt(src.value) < 0) {
            window.alert(Translate("common3", "", "请输入非负整型数字！"));
            src.focus();
            src.select();
        }
    }
}

//检查输入:    只能输入日期  失去焦点时判断
function jscommOnBlurCheckForOnlyDate() {
    var ent = GetEventObject();
    var src = ent.srcElement || ent.target;
    if (src.value == "")
        return;
    if (src.tagName == "INPUT") {
        if (!jscomIsValidDate(src.value)) {
            window.alert(Translate("common4", "", "请输入日期类型！"));
            src.focus();
            src.select();
        }
    }
}

function jscomIsEmptyString(str) {
    return ((!str) || (str.length == 0));
}

//检查日期是否正确  格式2002-09-11
function jscomIsValidDate(strDate) {
    if (jscomIsEmptyString(strDate)) {
        //alert("请输入日期!");
        return false;
    }
    var lthdatestr = strDate.length;
    var tmpy = "";
    var tmpm = "";
    var tmpd = "";
    var status = 0;

    for (i = 0; i < lthdatestr; i++) {
        if (strDate.charAt(i) == '-') {
            status++;
        }
        if (status > 2) {
            //alert("请用'-'作为日期分隔符！");
            return false;
        }
        if ((status == 0) && (strDate.charAt(i) != '-')) {
            tmpy = tmpy + strDate.charAt(i)
        }
        if ((status == 1) && (strDate.charAt(i) != '-')) {
            tmpm = tmpm + strDate.charAt(i)
        }
        if ((status == 2) && (strDate.charAt(i) != '-')) {
            tmpd = tmpd + strDate.charAt(i)
        }
    }

    year = new String(tmpy);
    month = new String(tmpm);
    day = new String(tmpd)

    if ((tmpy.length != 4) || (tmpm.length > 2) || (tmpd.length > 2)) {
        //alert("日期格式错误！");
        return false;
    }
    if (!((1 <= month) && (12 >= month) && (31 >= day) && (1 <= day))) {
        //alert ("错误的月份或天数！");
        return false;
    }
    if (!((year % 4) == 0) && (month == 2) && (day == 29)) {
        //alert ("这一年不是闰年！");
        return false;
    }
    if ((month <= 7) && ((month % 2) == 0) && (day >= 31)) {
        //alert ("这个月只有30天！");
        return false;
    }
    if ((month >= 8) && ((month % 2) == 1) && (day >= 31)) {
        //alert ("这个月只有30天！");
        return false;
    }
    if ((month == 2) && (day == 30)) {
        //alert("2月永远没有这一天！");
        return false;
    }
    return true;
}


/*格式化数字
num  要格式化的数值
decimal_num	小数位数 
has_split 是否要千分为分割符 true or false
	
返回 格式化的字符串
*/
function jscomFormatNumber(num, decimal_num, has_split) {
    if (isNaN(num)) { return num; } //非数值，直接返回


    var tmp_num, tmp_decimal_num;

    tmp_decimal_num = decimal_num;
    if (isNaN(decimal_num)) { tmp_decimal_num = 0; }

    tmp_num = num * Math.pow(10, tmp_decimal_num);
    tmp_num = Math.round(tmp_num);
    tmp_num = tmp_num / Math.pow(10, tmp_decimal_num);

    if (!has_split) { return tmp_num; }

    return tmp_num; //千分为分割符 以后处理
}

function jscomToNumber(num) {
    if (num == null) return 0;
    if (num == "") return 0;
    if (isNaN(num)) { return 0; } //非数值，返回0
    return eval(num);
}

//获得指定其日的字符串
function jscomGetDateStr(ftype_name) {
    var ret_str, objDate;
    var year, month, day;

    objDate = new Date();
    year = objDate.getFullYear();
    month = objDate.getMonth() + 1;
    day = objDate.getDate();

    switch (ftype_name) {
        case "now_date": //本日
            ret_str = year + "-" + month + "-" + day;
            break;
        case "yestoday": //昨天
            objDate.setDate(objDate.getDate() - 1);
            year = objDate.getFullYear();
            month = objDate.getMonth() + 1;
            day = objDate.getDate();
            ret_str = year + "-" + month + "-" + day;
            break;
        case "month_begin": //本月初
            ret_str = year + "-" + month + "-1";
            break;
        case "month_end": //本月末
            objDate.setMonth(month);
            objDate.setDate(0);
            ret_str = year + "-" + month + "-" + objDate.getDate();
            break;
        case "pre_month_begin": //上月初
            objDate.setMonth(objDate.getMonth() - 1);
            year = objDate.getFullYear();
            month = objDate.getMonth() + 1;
            day = objDate.getDate();
            ret_str = year + "-" + month + "-1";
            break;
        case "pre_month_end": //上月末
            objDate.setMonth(month - 1);
            objDate.setDate(0);
            year = objDate.getFullYear();
            month = objDate.getMonth() + 1;
            day = objDate.getDate();
            ret_str = year + "-" + month + "-" + day;
            break;
        case "year_begin": //本年初
            ret_str = year + "-01-01";
            break;
        case "year_end": //本年末
            objDate.setMonth(12);
            objDate.setDate(0);
            ret_str = year + "-12-" + objDate.getDate();
            break;
        case "pre_year_begin": //上年初
            year = year - 1;
            ret_str = year + "-01-01";
            break;
        case "pre_year_end": //上年末
            objDate.setYear(objDate.getYear() - 1);
            objDate.setMonth(12);
            objDate.setDate(0);
            year = objDate.getFullYear();
            month = objDate.getMonth() + 1;
            day = objDate.getDate();
            ret_str = year + "-" + month + "-" + day;
            break;
        default: //本日
            ret_str = year + "-" + month + "-" + day;
            break;
    }
    return ret_str;
}

function jscomTrimString(str, trimChar) {
    if (trimChar == null) trimChar = ' ';
    var ts = "";

    if (str.length < 1) return "";

    for (var i = (str.length - 1); i != -1; i--) {
        if (str.charAt(i) != trimChar) { break; }
    }
    ts = str.substring(0, i + 1);

    for (var i = 0; i < ts.length; i++) {
        if (str.charAt(i) != trimChar) { break; }
    }
    return ts.substring(i, ts.length);
}

function jscomCancelClick() {
    var frms = document.forms;
    for (i = 0; i < frms.length; i++) {
        frms(i).reset();
    }
    return false;
}

//伸缩对象，点一下伸开，再点一下收回，并且还可以带图片显示伸缩的标至
function jscomFlexObject(obj, imageObj, onImagePath, offImagePath) {
    if (obj.style.display == "none") {
        obj.style.display = "block";
        if (imageObj && onImagePath)
            imageObj.src = onImagePath;
        //oElement.alt = "收缩";
    }
    else {
        obj.style.display = "none";
        if (imageObj && offImagePath)
            imageObj.src = offImagePath;
        //oElement.alt = "展开";
    }
}

//function jscomExportTableToExcelByCopy(tableName)
//{
//      if(tableName==null) tableName="tbList";
//      var tableObject=document.all(tableName);
//      
//            //处理超链接
//        for(i=0;i<tableObject.rows.length;i++)
//        {
//            for(j=0;j<tableObject.rows(i).cells.length;j++)
//            {
//                var cellStr="";
//                for(k=0;k<tableObject.rows(i).cells(j).children.length;k++){
//                    var c=tableObject.rows(i).cells(j).children(k);
//                    if(c.tagName=="A")
//                    {
//                       tableObject.rows(i).cells(j).innerHTML=tableObject.rows(i).cells(j).innerText;
//                       break;
//                    }
//                 }
//            }
//        }
//      
//      var oXL = new ActiveXObject("Excel.Application"); 
//      var oWB = oXL.Workbooks.Add(); 
//      var oSheet = oWB.ActiveSheet;  
//      var hang=tableObject.rows.length;//取数据源行数 
//          
//      var sel=document.body.createTextRange();
//      sel.moveToElementText(tableObject);
//      //sel.select();
//      sel.execCommand("Copy");
//      oSheet.Paste();
//      oXL.Visible = true;
//      //=============================================
//      /*var imgIndex=0;
//	  var msoShapeRectangle=1; 
//	  var left=0;
//	  var top=0;
//	  for(i=0;i <hang;i++){//在Excel中写行 
//		var lie=tableObject.rows(i).cells.length;//取数据源列数 
//		for(j=0;j <lie;j++){//在Excel中写列 
//			var cell=oSheet.Cells(i+1,j+1)
//			cell.rowHeight=tableObject.rows(i).cells(j).offsetHeight*0.75;
//			cell.columnWidth=(tableObject.rows(i).cells(j).offsetWidth-5)/8; 
//			
////			cell.Select();//选中Excel中的单元格
//			if(tableObject.rows(i).cells(j).innerHTML.toLowerCase().indexOf('<img')!=-1){//如果其HTML代码包括 <img 
//				var imgs=tableObject.rows(i).cells(j).getElementsByTagName('img');
//				
//			cell.rowHeight=imgs[0].parentElement.offsetHeight*0.75;
//			cell.columnWidth=(imgs[0].parentElement.offsetWidth-5)/8; 
//				
//				for(k=0;k<imgs.length;k++){
//					imgIndex+=1;//alert(imgIndex);alert(oSheet.Shapes.count);
//					left=cell.left; top=cell.top;
//					top=top+(cell.rowHeight-imgs[k].offsetHeight*0.75)/2;	
//					left=left+imgs[k].offsetLeft;alert(left);alert(top);
//					oSheet.Shapes(1).Delete();
//					oSheet.Shapes.AddShape(msoShapeRectangle, left, top, imgs[k].offsetWidth*0.75, imgs[k].offsetHeight*0.75).Fill.UserPicture(imgs[k].src); 
//				}
//			} 
//		}   
//	  }*/
//	   //=============================================
//	 var left=0;
//	  var top=0;
//		var imgs=tableObject.getElementsByTagName("img");
//	 var rightCell;
//		var leftCell
//		for(i=oSheet.Shapes.count;i>0;i--){
//			rightCell=oSheet.Shapes(i).BottomRightCell;
//			leftCell=oSheet.Shapes(i).TopLeftCell;
//			if(rightCell.Column!=leftCell.Column)
//				oSheet.Shapes(i).Delete();
//		}alert(oSheet.Shapes.count);alert(imgs.length);
//		for(i=0;i<imgs.length;i++){
//			var cell=oSheet.Shapes(i+1).TopLeftCell;
//			cell.rowHeight=imgs[i].parentElement.offsetHeight*0.75;//alert(cell.address);
//			cell.columnWidth=(imgs[i].parentElement.offsetWidth-5)/8; 
//			left=cell.left; top=cell.top;
//			top=top+(cell.rowHeight-imgs[i].offsetHeight*0.75)/2;	//alert(imgs[k].offsetLeft);
//			left=left+imgs[i].offsetLeft;
//			oSheet.Shapes(i+1).left=left;
//			oSheet.Shapes(i+1).top=top;
//			oSheet.Shapes(i+1).LockAspectRatio=0;//解除长宽锁定
//			oSheet.Shapes(i+1).width=imgs[i].offsetWidth*0.75;//alert((imgs[i].offsetWidth-5)/8);
//			oSheet.Shapes(i+1).height=imgs[i].offsetHeight*0.75;
//			
//		}
//      //========================================
//      //========================================
//}
function jscomExportTableToExcelByCopy(tableName) {
    if (tableName == null) tableName = "tbList";
    var tableObject = document.getElementById(tableName);
    var table = document.createElement("table");
    //var regex=/^<input(\s+)|(\s+[\n.]+\s+)type\s+=\s+('hidden')|(hidden)|("hidden")|()\s+[\s.]*\/\s+>$/;
    var cellStr = "";
    table = tableObject;
    for (i = 0; i < table.rows.length; i++) {
        for (j = 0; j < table.rows(i).cells.length; j++) {
            cellStr = table.rows(i).cells[j].innerHTML;
            table.rows(i).cells(j).innerHTML = cellStr;
            for (k = 0; k < table.rows(i).cells(j).children.length; k++) {
                var c = table.rows(i).cells(j).children(k);
                if (c.tagName == "A") {
                    table.rows(i).cells(j).innerHTML = table.rows(i).cells(j).innerText;
                    break;
                }
            }
        }
    }
    var inputs = table.getElementsByTagName("input");
    var inputLendth = inputs.length;
    for (i = inputLendth - 1; i >= 0; i--) {
        if (inputs[i].type == "hidden") {
            inputs[i].removeNode(true);
        }
    }
    var html = table.outerHTML;
    var oXL = new ActiveXObject("Excel.Application");
    var oWB = oXL.Workbooks.Add();
    var oSheet = oWB.ActiveSheet;
    var hang = table.rows.length; //取数据源行数 
    var sel = document.body.createTextRange();
    sel.moveToElementText(table);
    //sel.select();
    sel.execCommand("Copy");
    oSheet.Paste();
    oXL.Visible = true;
    oSheet.Cells.WrapText = false;
    oSheet.Cells.Font.Size = 10; //字体大小 
    //=============================================
    var left = 0;
    var top = 0;
    var imgs = table.getElementsByTagName("img");
    var rightCell;
    var leftCell;
    for (i = oSheet.Shapes.count; i > 0; i--) {
        rightCell = oSheet.Shapes(i).BottomRightCell;
        leftCell = oSheet.Shapes(i).TopLeftCell;
        if (oSheet.Shapes(i).Type == 12) {
            //var detail="("+oSheet.Shapes(i).left+","+oSheet.Shapes(i).top+","+oSheet.Shapes(i).Width*0.75+","+oSheet.Shapes(i).Height*0.75+")"; 
            //alert(detail);
            //	oSheet.Shapes.AddTextbox(1,oSheet.Shapes(i).left,oSheet.Shapes(i).top,50,oSheet.Shapes(i).Height*0.75).TextFrame.Characters.Text="aa";
            //oSheet.Shapes(oSheet.Shapes.count).Text=oSheet.Shapes(i).Text;
            oSheet.Shapes(i).Delete();
        }
        else if (oSheet.Shapes(i).Type != 13)
            oSheet.Shapes(i).Delete();
    }

    //alert(imgs[0].parentElement.clientWidth);alert(imgs[0].parentElement.offsetWidth);
    var perCharWidth = 0;
    for (i = 0; i < imgs.length; i++) {
        try {
            var cell = oSheet.Shapes(i + 1).TopLeftCell; //alert(cell.Width/cell.ColumnWidth);
            perCharWidth = cell.Width / cell.ColumnWidth;
            var width = "cell.columnWidth=" + cell.columnWidth + "; imgs[i].parentElement.offsetWidth=" + imgs[i].parentElement.offsetWidth;
            //alert(width);
            if (cell.rowHeight < imgs[i].parentElement.offsetHeight * 0.75)
                cell.rowHeight = imgs[i].parentElement.offsetHeight * 0.75;
            if (cell.columnWidth < ((imgs[i].parentElement.offsetWidth / screen.deviceXDPI) * 72) / perCharWidth)
                cell.columnWidth = ((imgs[i].parentElement.offsetWidth / screen.deviceXDPI) * 72) / perCharWidth;
            left = cell.left; top = cell.top;
            top = top + (cell.rowHeight - imgs[i].offsetHeight * 0.75) / 2;
            left = left + imgs[i].offsetLeft;
            oSheet.Shapes(i + 1).left = left;
            oSheet.Shapes(i + 1).top = top;
            oSheet.Shapes(i + 1).LockAspectRatio = 0; //解除长宽锁定
            oSheet.Shapes(i + 1).width = imgs[i].offsetWidth * 0.75;
            oSheet.Shapes(i + 1).height = imgs[i].offsetHeight * 0.75;
        } catch (e) { alert(e); }
    }
    //========================================
}

function jscomExportTableToExcel(tableName, ignoreCols, ignoreLastRows) {
    // Start Excel and get Application object.
    //if (!confirm("Exporting will spend long time if there are too many records(1000 Records will spend abount 1 minute).Are you sure to export?")) return;
    var oXL;
    try {
        oXL = new ActiveXObject("Excel.Application"); // Get a new workbook.
    } catch (e) {
        alert(Translate("common5", "", "无法调用Office对象，请确保您的机器已安装了Office并已将本系统的站点名加入到IE的信任站点列表中！"));
        return;
    }
    try {
        var oWB = oXL.Workbooks.Add();
        var oSheet = oWB.ActiveSheet;
        var table = document.getElementById(tableName);
        var nRows = table.rows.length;
        if (ignoreLastRows && ignoreLastRows.valueOf() != 0) nRows = nRows - ignoreLastRows.valueOf();
        var nCols = table.rows(0).cells.length; // Add table headers going cell by cell.
        var nCol = 0;
        if (!ignoreCols) ignoreCols = '';
        //提高效率
        if (ignoreCols == '') {
            for (i = 0; i < nRows; i++) {
                nCol = 0;
                for (j = 0; j < nCols; j++) {
                    //if (ignoreCols.indexOf(','+j+',')==-1)
                    //{
                    if (table.rows(i).cells(nCol)) oSheet.Cells(i + 1, nCol + 1).value = "'" + table.rows(i).cells(nCol).innerText;
                    nCol = nCol + 1;
                    //}
                }
            }
        } else {
            ignoreCols = ',' + ignoreCols + ',';
            for (i = 0; i < nRows; i++) {
                nCol = 0;
                for (j = 0; j < nCols; j++) {
                    if (ignoreCols.indexOf(',' + j + ',') == -1) {
                        if (table.rows(i).cells(j)) oSheet.Cells(i + 1, nCol + 1).value = "'" + table.rows(i).cells(j).innerText;
                        nCol = nCol + 1;
                    }
                }
            }
        }
        oXL.Visible = true;
        oXL.UserControl = true;
    } catch (e) {
        alert(Translate("common6", "", "导出失败！") + e);
        //alert("Exporting Fails!"+e);
    }

}

//客户端字符串编码
function ClientHtmlEncode(str) {
    var strRtn = "";
    if (!str) return strRtn;
    for (var i = str.length - 1; i >= 0; i--) {
        strRtn += str.charCodeAt(i);
        if (i) strRtn += "a"; //with a to split
    }
    return strRtn;
}

//客户端字符串解码
function ClientHtmlDecode(str) {
    var strArr;
    var strRtn = "";
    if (!str) return strRtn;
    strArr = str.split("a");
    for (var i = strArr.length - 1; i >= 0; i--) {
        if (strArr[i] != '') strRtn += String.fromCharCode(eval(strArr[i]));
    }
    return strRtn;
}

//关闭窗体时检查有没有更新，如有则刷新父窗体
function jscomCloseAndRefreshOpener(IsRefreshOpener) {
    if (!IsRefreshOpener) {
        IsRefreshOpener = false;
        try {
            if (document.getElementById("hidHasUpdate").value == "True")
                IsRefreshOpener = true;
            else
                IsRefreshOpener = false;
        } catch (e) { }
    }
    if (!top) {
        try {
            if (IsRefreshOpener == true)
                opener.RefreshForm();
        } catch (e) { }
        window.close();
    }
    else {
        try {
            if (IsRefreshOpener == true)
                top.opener.RefreshForm();
        } catch (e) { }
        top.close();
    }
}


function jcomSelectDate(sSourceControlName, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("no", sSourceControlName, oppositePath, nIndex);
}

function jcomSelectDateTime(sSourceControlName, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("yes", sSourceControlName, oppositePath, nIndex);
}

//有时间，但要用户自已选择
function jcomSelectDateOrTime(sSourceControlName, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("select", sSourceControlName, oppositePath, nIndex);
}

//功能:选择日期并比较两个日期的大小(不带时间的日期).
function jcomSelectDateWithCompare(sSourceControlName, sCompareControlName, IsBeginTime, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("no", sSourceControlName, oppositePath, nIndex);
    CompareDate(sSourceControlName, sCompareControlName, IsBeginTime);
}


//功能:选择日期并比较两个日期的大小(不带时间的日期).
function jcomSelectDateWithCompareAndName(sSourceControlName, sCompareControlName, IsBeginTime, oppositePath, nIndex, beginTimeName, endTimeName) {
    jcomOpenCalenderWithTime("no", sSourceControlName, oppositePath, nIndex);
    CompareDateWithName(sSourceControlName, sCompareControlName, IsBeginTime, beginTimeName, endTimeName);
}



//功能:选择日期并比较两个日期的大小(带时间的日期).
function jcomSelectDateTimeWithCompare(sSourceControlName, sCompareControlName, IsBeginTime, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("yes", sSourceControlName, oppositePath, nIndex);
    CompareDate(sSourceControlName, sCompareControlName, IsBeginTime);
}

//功能:选择日期并比较两个日期的大小(可带或不带时间的日期).
function jcomSelectDateOrTimeWithCompare(sSourceControlName, sCompareControlName, IsBeginTime, oppositePath, nIndex) {
    jcomOpenCalenderWithTime("select", sSourceControlName, oppositePath, nIndex);
    CompareDate(sSourceControlName, sCompareControlName, IsBeginTime);
}

//比较两个日期
//sSourceControlName 要选择的日期
//sCompareControlName 用来比较的日期
//IsBeginTime:判断当前选择是否为开始日期
function CompareDate(sSourceControlName, sCompareControlName, IsBeginTime) {
    var datSourceTime, datCompareTime;
    datSourceTime = document.getElementById(sSourceControlName).value;
    datCompareTime = document.getElementById(sCompareControlName).value;

    //如果其中一个为空，则不需要比较
    if (datSourceTime == "" || datCompareTime == "") {
        return;
    }

    datSourceTime = Date.parse(datSourceTime.replace(/-/g, '/ '));
    datCompareTime = Date.parse(datCompareTime.replace(/-/g, '/ '));

    //如果当前选择的是开始时间
    if (IsBeginTime) {
        if (datSourceTime > datCompareTime) {
            alert(Translate("common7", "", "开始时间不能大于结束时间"));
            document.getElementById(sSourceControlName).value = "";
        }
    }
    else {
        if (datSourceTime < datCompareTime) {
            alert(Translate("common8", "", "结束时间不能小于开始时间"));
            document.getElementById(sSourceControlName).value = "";
        }
    }
}

//比较两个日期
//sSourceControlName 要选择的日期
//sCompareControlName 用来比较的日期
//IsBeginTime:判断当前选择是否为开始日期
function CompareDateWithName(sSourceControlName, sCompareControlName, IsBeginTime, beginTimeName, enTimeName) {
    var datSourceTime, datCompareTime;
    datSourceTime = document.getElementById(sSourceControlName).value;
    datCompareTime = document.getElementById(sCompareControlName).value;

    //如果其中一个为空，则不需要比较
    if (datSourceTime == "" || datCompareTime == "") {
        return;
    }

    datSourceTime = Date.parse(datSourceTime.replace(/-/g, '/ '));
    datCompareTime = Date.parse(datCompareTime.replace(/-/g, '/ '));

    //如果当前选择的是开始时间
    if (IsBeginTime) {
        if (datSourceTime > datCompareTime) {
            alert(beginTimeName + "不能大于" + enTimeName);
            document.getElementById(sSourceControlName).value = "";
        }
    }
    else {
        if (datSourceTime < datCompareTime) {
            alert(enTimeName + "不能小于" + beginTimeName);
            document.getElementById(sSourceControlName).value = "";
        }
    }
}


function ReturnCompareDate(sSourceControlName, sCompareControlName, IsBeginTime) {
    var datSourceTime, datCompareTime;
    datSourceTime = document.getElementById(sSourceControlName).value;
    datCompareTime = document.getElementById(sCompareControlName).value;

    //如果其中一个为空，则不需要比较
    if (datSourceTime == "" || datCompareTime == "") {
        return;
    }

    datSourceTime = Date.parse(datSourceTime.replace(/-/g, '/ '));
    datCompareTime = Date.parse(datCompareTime.replace(/-/g, '/ '));

    //如果当前选择的是开始时间
    if (IsBeginTime) {
        if (datSourceTime > datCompareTime) {
            alert(Translate("common7", "", "开始时间不能大于结束时间"));
            document.getElementById(sSourceControlName).value = "";
            return false;
        }
    }
    else {
        if (datSourceTime < datCompareTime) {
            alert(Translate("common8", "", "结束时间不能小于开始时间"));
            document.getElementById(sSourceControlName).value = "";
            return false;
        }
    }
    return true;
}

//选择年月
function jcomSelectMonth(sSourceControlName, oppositePath, nIndex) {
    if (nIndex == null) nIndex = 0;
    if (oppositePath == null) oppositePath = "../../";
    var sOldDate;

    if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
        sOldDate = document.getElementsByName(sSourceControlName)[nIndex].value;
    else
        sOldDate = document.getElementById(sSourceControlName).value;
    var d = new Date;
    var strNode = showModalDialog(oppositePath + 'module/SelectMonth.aspx?currTime=' + d.getTime(), "", "dialogWidth:390px;dialogHeight:100px;status:no;scrollbars=no");
    //var strNode=window.open('../Modules/Calendar.aspx',0,"dialogWidth:320px;dialogHeight:185px;status:no");
    if (strNode != -1 && typeof (strNode) != 'undefined') {
        if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
            document.getElementsByName(sSourceControlName)[nIndex].value = strNode;
        else
            document.getElementById(sSourceControlName).value = strNode;
    }
}

function jcomOpenCalenderWithTime(HasTime, sSourceControlName, oppositePath, nIndex) {

    if (nIndex == null) nIndex = 0;
    if (oppositePath == null) oppositePath = "../../";
    var sOldDate;

    if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
        sOldDate = document.getElementsByName(sSourceControlName)[nIndex].value;
    else
        sOldDate = document.getElementById(sSourceControlName).value;
    var parameter = new Object();
    parameter.hasTime = HasTime;
    parameter.oldValue = sOldDate;
    parameter.isMulti = false;

    var height = 270;
    if (HasTime == "no") height = 230;
    var d = new Date;
    var strNode = showModalDialog(oppositePath + 'module/calendar.aspx?currTime=' + d.getTime(), parameter, "dialogWidth:455px;dialogHeight:" + height + "px;status:no;scrollbars=no;border:none;help:no");
    //var strNode=window.open('../Modules/Calendar.aspx',0,"dialogWidth:320px;dialogHeight:185px;status:no");
    if (strNode != -1 && typeof (strNode) != 'undefined') {
        if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
            document.getElementsByName(sSourceControlName)[nIndex].value = strNode;
        else
            document.getElementById(sSourceControlName).value = strNode;
    }
}

function jcomSelectMutilCalender(oppositePath) {
    if (oppositePath == null) oppositePath = "../../";
    var isMulti = true;
    var sOldDate = "";

    var parameter = new Object();
    parameter.hasTime = "no";
    parameter.oldValue = sOldDate;
    parameter.isMulti = isMulti;

    var height = 300;
    var strNode = showModalDialog(oppositePath + 'module/calendar.aspx', parameter, "dialogWidth:380px;dialogHeight:" + height + "px;status:no;scrollbars=no");
    if (strNode != -1 && typeof (strNode) != 'undefined') {
        return strNode;
    }
    else {
        return "";
    }
}

function jcomSelectTime(sSourceControlName, oppositePath, nIndex) {
    if (oppositePath == null) oppositePath = "../../";
    var sOldTime;
    if (!nIndex) {
        sOldTime = document.getElementById(sSourceControlName).value;
    } else {
        if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
            sOldTime = document.getElementsByName(sSourceControlName)[nIndex].value;
        else
            sOldTime = document.getElementById(sSourceControlName).value;
    }
    var d = new Date;
    var strNode = showModalDialog(oppositePath + 'module/SelectTime.aspx?currTime=' + d.getTime() + '&&', sOldTime, "dialogWidth:380px;dialogHeight:150px;status:no;scrollbars=no");
    if (strNode != -1 && typeof (strNode) != 'undefined') {
        if (!nIndex) {
            document.getElementById(sSourceControlName).value = strNode;
        } else {
            if (typeof (document.getElementsByName(sSourceControlName)[nIndex]) != 'undefined')
                document.getElementsByName(sSourceControlName)[nIndex].value = strNode;
            else
                document.getElementById(sSourceControlName).value = strNode;
        }
    }
}

//结束时间不能小于开始时间的判断方法
function jcomSelectTimeWithCompare(sSourceControlName, sCompareControlName, IsBeginTime, oppositePath, nIndex) {
    jcomSelectTime(sSourceControlName, oppositePath, nIndex);

    //比较判断时间
    var datSourceTime, datCompareTime;
    datSourceTime = document.getElementById(sSourceControlName).value;
    datCompareTime = document.getElementById(sCompareControlName).value;

    //如果其中一个为空，则不需要比较
    if (datSourceTime == "" || datCompareTime == "") {
        return;
    }

    datSourceTime = datSourceTime.replace(new RegExp('\:', 'gm'), '');
    datCompareTime = datCompareTime.replace(new RegExp('\:', 'gm'), '');

    //去掉前面的0
    datSourceTime = datSourceTime.replace(/\b(0+)/gi, "");
    datCompareTime = datCompareTime.replace(/\b(0+)/gi, "");

    datSourceTime = parseInt(datSourceTime);
    datCompareTime = parseInt(datCompareTime);

    //如果当前选择的是开始时间
    if (IsBeginTime) {
        if (datSourceTime > datCompareTime) {
            alert(Translate("common7", "", "开始时间不能大于结束时间"));
            document.getElementById(sSourceControlName).value = "";
        }
    }
    else {
        if (datSourceTime < datCompareTime) {
            alert(Translate("common8", "", "结束时间不能小于开始时间"));
            document.getElementById(sSourceControlName).value = "";
        }
    }

}

function jcomOpenHtmlEditor(FileRootPath, OpenerName) {
    if (FileRootPath == null) FileRootPath = "";
    parent.WebTab_CreateTab("HtmlEditor", Translate("common9", "", "HTML编辑器"), "framework/HtmlEditor/HtmlEditor.aspx?OpenerName=" + OpenerName + "&FileRootPath=" + FileRootPath);
}


function jscomTextBoxSelectText(textObject, fromPos, toPos) {
    fromPos = parseInt(fromPos)
    toPos = parseInt(toPos)

    if (isNaN(fromPos) || isNaN(toPos))
        return;

    var rng = textObject.createTextRange();

    rng.moveEnd("character", -textObject.value.length)
    rng.moveStart("character", -textObject.value.length)

    rng.collapse(true);

    rng.moveEnd("character", toPos)
    rng.moveStart("character", fromPos)
    rng.select();
}

function jscomGetTextBoxCursorPos(textObject, eventX, eventY) {
    /*
    var rng = textObject.createTextRange();
    alert(eventX+","+eventY);
    rng.moveToPoint(eventX,eventY);
    alert("ok");
    rng.moveStart("character",-textObject.value.length)	     
    return rng.text.length;
    */

    var oTR = textObject.createTextRange();
    var oSel = document.selection.createRange();
    var textLength = textObject.value.length;
    var line, total, cl;
    oTR.moveToPoint(oSel.offsetLeft, oSel.offsetTop);
    oTR.moveStart("character", -1 * textLength);
    total = oTR.text.length;
    //alert("oTR.text.length="+oTR.text.length);
    return total;
    //window.status = "行: " + line +", 列: " + char + ", 第 " + total + " 个字符"
}


function jscomMoveListItem(SourceList, TargetList) {
    var nIndex = SourceList.selectedIndex;
    //选中的第一项索引 0开始
    //alert(nIndex);
    if (nIndex == -1) {
        alert(Translate("common10", "", "请先选择一项！"));
        return;
    }
    var len = SourceList.length; //选中项 TargtList是要进入的列表
    var start = 0;
    var tag = 0;
    //记录是否在目标列表中已经存在选中项，0为没有，1为有 
    for (i = 0; i < len; i++) {
        tag = 0;
        nIndex = SourceList.selectedIndex;
        //alert(nIndex);
        if (nIndex == -1) {
            return;
        }
        var objSelected = new Option(SourceList[nIndex].text, SourceList[nIndex].value);
        for (j = 0; j < TargetList.length; j++) {
            //此循环用于删除重复项
            if (TargetList[j].value == SourceList[nIndex].value) {
                tag = 1;
                SourceList.options[nIndex] = null;
                break;
            }
        }
        if (tag == 0) {
            TargetList.options[TargetList.length] = objSelected;
            SourceList.options[nIndex] = null;
            TargetList.options[TargetList.length - 1].selected = true;
        }
    }
    //     if(navigator.appName=="Netscape") 
    //     { 
    //       history.go(0);
    //     }
}

function jscomMoveAllListItem(SourceList, TargetList) {
    var nIndex = -1;
    //选中的第一项索引 0开始
    //alert(nIndex);
    var len = SourceList.length; //选中项 TargtList是要进入的列表
    var start = 0;
    var tag = 0;
    //记录是否在目标列表中已经存在选中项，0为没有，1为有 
    for (i = 0; i < len; i++) {
        tag = 0;
        //nIndex =SourceList.selectedIndex;
        //每次都是第一个，因为加一个就会移掉一个
        nIndex = 0;
        //alert(nIndex);
        if (nIndex == -1) {
            return;
        }
        var objSelected = new Option(SourceList[nIndex].text, SourceList[nIndex].value);
        for (j = 0; j < TargetList.length; j++) {
            //此循环用于删除重复项
            if (TargetList[j].value == SourceList[nIndex].value) {
                tag = 1;
                SourceList.options[nIndex] = null;
                break;
            }
        }
        if (tag == 0) {
            TargetList.options[TargetList.length] = objSelected;
            SourceList.options[nIndex] = null;
            TargetList.options[TargetList.length - 1].selected = true;
        }
    }
    //if(navigator.appName=="Netscape") 
    //{ 
    //  history.go(0);
    //}
}

//将数组里的值加入到列表框
function jscomAddItemToListBox(arrSourceValue, arrSourceText, TargetList) {
    if (arrSourceValue.length != arrSourceText.length) {
        alert(Translate("common11", "", "数据有误，无法添加到列表框！"));
        return;
    }

    var len = arrSourceValue.length;
    var start = 0;
    var tag = 0;
    //记录是否在目标列表中已经存在选中项，0为没有，1为有 
    for (i = 0; i < len; i++) {
        tag = 0;
        var objSelected = new Option(arrSourceText[i], arrSourceValue[i]);
        for (j = 0; j < TargetList.length; j++) {
            //此循环用于删除重复项
            if (TargetList[j].value == arrSourceValue[i]) {
                tag = 1;
                break;
            }
        }
        if (tag == 0) {
            TargetList.options[TargetList.length] = objSelected;
            TargetList.options[TargetList.length - 1].selected = true;
        }
    }
    //     if(navigator.appName=="Netscape") 
    //     { 
    //       history.go(0);
    //     }
}

//将所有选中的项目移除
function jscomRemoveSelectedItemFromListBox(SourceList) {
    var nIndex = SourceList.selectedIndex;
    //选中的第一项索引 0开始
    //alert(nIndex);
    if (nIndex == -1) {
        alert(Translate("common12", "", "请至少选择一项！"));
        return;
    }
    var len = SourceList.length; //选中项 TargtList是要进入的列表
    var start = 0;
    var tag = 0;
    //记录是否在目标列表中已经存在选中项，0为没有，1为有 
    for (i = 0; i < len; i++) {
        tag = 0;
        nIndex = SourceList.selectedIndex;
        if (nIndex == -1) {
            return;
        }
        SourceList.options[nIndex] = null;
    }
    //     if(navigator.appName=="Netscape") 
    //     { 
    //       history.go(0);
    //     }
}

//将数组里的值加入到列表框
function jscomLocateItemInListBox(TargetList, locateItemTexts) {
    var arrLocateItem = locateItemTexts.split(",");
    var len = arrLocateItem.length;
    var start = 0;
    var tag = 0;
    //记录是否在目标列表中已经存在选中项，0为没有，1为有 
    for (i = 0; i < TargetList.length; i++) {
        TargetList.options[i].selected = false;
        for (j = 0; j < len; j++) {
            //此循环用于删除重复项
            if (TargetList[i].text.indexOf(arrLocateItem[j]) >= 0) {
                TargetList.options[i].selected = true;
                break;
            }
        }
        //         if(navigator.appName=="Netscape") 
        //         { 
        //            history.go(0);
        //         }
    }
}

//从ID串中取得第一个ID
function jscomGetFirstIDFromIDs(ids) {
    if (ids.indexOf(",") >= 0) ids = ids.substr(0, ids.indexOf(","));
    return ids;
}

function jscomCheckedQuestionAnswer(radioObject, index) {
    return false;
    try {
        var obj = document.getElementsByName(radioObject)[index];
        if (obj.checked) {
            //选择题选中后，禁止再次点后面的答案时清除所选项 lopping 2011-02-21
            //obj.checked=false;
        }
        else {
            //  obj.checked = true;
        }
    }
    catch (e) {

    }
}

//类型为Textarea填空题、回答题，检查是否为空的时候禁止查看按钮
function jscomCheckedTextareaQuestionAnswer(radioObject) {
    var selectInputUid = radioObject.toString().replace("Answer", "");
    try {
        if ($("#hidIsRepeatAnswer").val() != "Y" && $("#" + radioObject).val().length != 0) {
            $("#" + selectInputUid).find('a').removeAttr('disabled').focus();
        }
        if ($("#" + radioObject).val().trim().length == 0) {
            $("#" + selectInputUid).find('a').attr('disabled', 'disabled');
        }
    }
    catch (e) {
    }
}
function jscomCheckedFillQuestionAnswer(radioObject) {
    var selectInputUid = radioObject.toString().replace("Answer", "");
    try {
        if ($("#hidIsRepeatAnswer").val() != "Y" && $("#" + radioObject).val().length != 0) {
            $("#" + selectInputUid).find('a').removeAttr('disabled').focus();
        }
        if ($("#" + radioObject).val().trim().length == 0) {
            $("#" + selectInputUid).find('a').attr('disabled', 'disabled');
        }
    }
    catch (e) {
    }
}

//练习改错题
//正确
function JudgeCorrectRightInput(radioObject) {
    $('#Answer' + radioObject).val('');
    $('#trAnswer' + radioObject).css('display', 'none');
    $("#" + radioObject).find('a').removeAttr('disabled').focus();
}
//错误
function JudgeCorrectErrorInput(radioObject) {
    $('#trAnswer' + radioObject).css('display', '');
    $("#" + radioObject).find('a').removeAttr('disabled').focus();
}


//填空题为input类型的检查
function jscomCheckedInputQuestionAnswer(radioObject, index) {
    var selectInputUid = radioObject.toString().replace("Answer", "");
    try {
        if ($("#hidIsRepeatAnswer").val() != "Y") {
            $("#" + selectInputUid).find('a').removeAttr('disabled');
        }
    }
    catch (e) {
    }
}

function jscomSelectTure(radioObject) {
    alert(radioObjec.toString().replace("", ""));
}

function jscomSetPasswordBoxValue(conName, conValue) {
    try {
        var obj = document.getElementById(conName)
        if (obj == null)
            return;
        obj.value = conValue;
    }
    catch (e) {
        //alert(e.message);
    }
}

/*
把字符串转化为日期, 如果格式不正确返回空值
*/
function jscomParseStringToDateTime(strDate) {
    try {
        execScript('dt=CDate("' + strDate + '")', 'vbs');
        return dt;
    }
    catch (e) {
        return "";
    }
}

function ClearDate(name) {
    var Forms = document.getElementById(name).getElementsByTagName("input");
    for (var i = 0; i < Forms.length; i++) {
        if (Forms[i].type == 'text') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'checkbox') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'radio') {
            if (Forms[i].disabled == false)
                Forms[i].value = "";
        }
        else if (Forms[i].type == 'hidden') {
            Forms[i].value = "";
        }
    }
    var Forms = document.getElementById(name).getElementsByTagName("select");
    for (var i = 0; i < Forms.length; i++) {
        if (Forms[i].disabled == false)
            Forms[i].value = "";
    }
}

function GetFormObject(formName) {
    if (formName == null)
        formName = "form1";
    var theForm = document.forms[formName];
    if (!theForm) {
        theForm = eval("document." + formName);
    }

    return theForm;
}

function GetBrowserType() {
    function browser() {
        this.ie = false;
        this.firefox = false;
        this.chrome = false;
        this.opera = false;
        this.safari = false;
    }
    var userAgent = navigator.userAgent.toLowerCase();
    if (userAgent.match(/msie ([\d.]+)/))
        browser.ie = true;
    else if (userAgent.match(/firefox\/([\d.]+)/))
        browser.firefox = true;
    else if (userAgent.match(/chrome\/([\d.]+)/))
        browser.chrome = true;
    else if (userAgent.match(/opera.([\d.]+)/))
        browser.opera = true;
    else if (userAgent.match(/version\/([\d.]+).*safari/))
        browser.safari = true;

    return browser;
}

function GetEventObject() {
    if (document.all)
        return window.event;

    func = GetEventObject.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {
            if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == 'object' && arg0.preventDefault && arg0.stopPropagation)) {
                return arg0;
            }
        }
        func = func.caller;
    }
    return null;
}

//创建XMLhttpRequest对象
function createXmlHttpRequest() {
    var xmlHttp;
    if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest();

        if (xmlHttp.overrideMimeType) {
            xmlHttp.overrideMimeType("text/xml");
        }
    }
    else if (window.ActiveXObject) {
        try {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }
    return xmlHttp;
}
function bgiframe(objdiv) {
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    if (Sys.ie == "6.0") {
        if (objdiv == null) return;
        var ifrm = document.createElement('iframe');
        ifrm.id = objdiv.id;
        ifrm.src = "javascript:false";
        ifrm.style.cssText = "display:block;position:absolute; visibility:inherit; top:0px; left:0px; width:100%; height:100%; z-index:-1; border:0;filter:Alpha(Opacity='0');";
        objdiv.insertBefore(ifrm, objdiv.firstChild);
    }
}
  //获取浏览器Get请求的参数
        this.getRequstParam = function (key) {
        var url = document.location.href;
        var endIndex = url.indexOf("#");
        var arrStr = null;
        if (endIndex > 0)
            arrStr = url.substring(url.indexOf("?") + 1, endIndex).split("&");
        else
            arrStr = url.substring(url.indexOf("?") + 1).split("&");

        for (var i = 0; i < arrStr.length; i++) {
            var loc = arrStr[i].indexOf(key + "=");
            if (loc != -1) {
                return arrStr[i].replace(key + "=", "").replace("?", "");
                break;
            }
        }
        return "";
    }
