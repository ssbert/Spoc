/**
 * 包含easyui的扩展和常用的方法
 */




var VE = $.extend({}, VE);/* 全局对象 */
VE.Validate = true;
VE.Json = {};
VE.Date;
VE.DataFormatterLine = "-";
VE.PageSize = 20;
VE.GridHeight = 400;
VE.PageList = [5, 10, 15, 20, 25, 30, 35, 40];
VE.MessageConfirmTitle = "请确认";
VE.MessageConfirmMsg = "您要删除当前所选项吗？";
VE.MessageConfirmMsgEnable = "您要启用当前所选项吗？";
VE.MessageConfirmMsgDisable = "您要禁用当前所选项吗？";
VE.MessageConfirmMsgRec = "您要推荐当前所选项吗？";
VE.MessageConfirmMsgUnRec = "您要取消推荐当前所选项吗？";
VE.MessageConfirmMsgCancel = "您要取消当前所选服务项目吗？";
VE.MessageAlertTitle = "提示";
VE.MessageAlertShowType_Show = "show";
VE.MessageAlertTimeout = 1000;
VE.MessageAlertIcon_Info = "info";
VE.MessageAlertIcon_Error = "error";
VE.MessageAlertIcon_Question = "question";
VE.MessageAlertIcon_Warning = "warning";
VE.MessageAlertMsg = "请选择一条记录！";
VE.MessageAlertMsg_1 = "你选择了多条记录，请选择一条记录！";
VE.MessageAlertMsg_2 = "你选择的记录已被禁用，请先启用再修改！";
VE.MessageAlertMsg_3 = "网络异常，请稍后再试！";
VE.DatagridIconCls = "icon-tip"; // Datagrid 默认图标，做为全局变量方便统一修改
VE.FilterIconCls = "icon-search";// 查询 Panel 默认图标，做为全局变量方便统一修改
VE.FilterCollapse = true; //查询框默认折叠，做为全局变量方便统一修改
VE.FilterPanelIconClsSearch = "icon-search"; //查询 Panel 查询图标，做为全局变量方便统一修改
VE.FilterPanelIconClsClear = "icon-undo"; //查询 Panel 清空图标，做为全局变量方便统一修改
VE.Id = 0;  //获取表单Id，全局变量
VE.Ids = []; //删除时，获取所有要删除的Id
VE.Toolbar = []; //工具栏
VE.DataFormats = {};//记录对象与文本框Id的对应关系
VE.Datagrid_Method = "GET";
VE.AppPath = "";
VE.GridType_DataGrid = 'datagrid';//数据列表
VE.GridType_TreeGrid = 'treegrid'; //树形列表


// 返回 Data 
VE.FormatterDate = function FormatterDate(val) {
    if (val != null) {
        VE.Date = VE.ConvertJsonDate(val);
        if (VE.Date != null) {
            return VE.GetDate(VE.Date);
        }
    }
};

VE.getDatagridDiv = function getDatagridDiv(id) {
    //     <div id="pwdModifiry" class="easyui-dialog" data-options="width:'580px',height:'300px',closed:true"
    //     style="padding: 10px 20px;">
    //</div>
    var divObj = document.createElement("div");
    divObj.id = id;
    divObj.className = "easyui-dialog";
    divObj.style.padding = "10px 10px";
    $(divObj).attr("data-options", "closed:true");
    return divObj;
}
// 返回 DataTime
VE.FormatterDateHm = function FormatterDateHm(val) {
    if (val != null) {
        VE.Date = VE.ConvertJsonDate(val);
        if (VE.Date != null) {
            return VE.GetDateHm(VE.Date);
        }
    }
};
// 返回 DataTime
VE.FormatterDateTime = function FormatterDateTime(val) {

    if (val != null) {
        VE.Date = VE.ConvertJsonDate(val);
        if (VE.Date != null) {
            return VE.GetDateTime(VE.Date);
        }
    }
};

//JSON返回DateTime/Date('123123123')/
VE.ConvertJsonDate = function ConvertJsonDate(val) {
    if (val != null) {
        VE.Date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
        return VE.Date;
    }

    //if (val != null) {
    //    var strTime = val.replace("T", " ").substring(0, 18); //字符串日期格式           
    //    var date = new Date(Date.parse(strTime.replace(/-/g, "/"))); //转换成Data();
    //    return date;
    //}
};

// 获取日期格式  年月日
VE.GetDate = function GetDate(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    if (month < 10) {
        month = "0" + month;
    }
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day;
    }
    return year + VE.DataFormatterLine + month + VE.DataFormatterLine + day;
};

//  获取日期时间格式  年月日时分秒
VE.GetDateTime = function GetDateTime(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    if (month < 10) {
        month = "0" + month;
    }
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day;
    }
    var hh = date.getHours();
    if (hh < 10) {
        hh = "0" + hh;
    }
    var mm = date.getMinutes();
    if (mm < 10) {
        mm = "0" + mm;
    }
    var ss = date.getSeconds();
    if (ss < 10) {
        ss = "0" + ss;
    }
    return year + VE.DataFormatterLine + month + VE.DataFormatterLine + day + " " + hh + ":" + mm + ":" + ss;
};

//  获取日期时间格式  年月日时分
VE.GetDateHm = function GetDateHm(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    if (month < 10) {
        month = "0" + month;
    }
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day;
    }
    var hh = date.getHours();
    if (hh < 10) {
        hh = "0" + hh;
    }
    var mm = date.getMinutes();
    if (mm < 10) {
        mm = "0" + mm;
    }
    return year + VE.DataFormatterLine + month + VE.DataFormatterLine + day + " " + hh + ":" + mm;
};

VE.ArrDateFormateByGet = function ArrDateFormateByGet(obj, perArr) {
     
    if (obj != null && obj != "undefined") {
        for (var p = 0; p < perArr.length; p++) { // 方法
            obj[perArr[p]] = VE.FormatterDateTime(obj[perArr[p]])
        }
    }

}

VE.ArrDateFormate = function ArrDateFormate(obj) {
  
    if (obj != null && obj != "undefined") {
        for (var p in obj) { // 方法 
            if (typeof (obj[p]) == "string") {
                obj.p
                obj[p] = VE.GetFormateDate(obj[p])
            }
        }
    }

}

VE.GetFormateDate = function GetFormateDate(v) {
   
    if (v != null && v != "") {

        if (v.indexOf("/")>0&& Date.parse(v)) {
            var d = new Date(parseInt(Date.parse(v), 10));
            return VE.GetDateTime(d);
        } else {

            return v;
        }
    } else {

        return "";
    }
}

//查询保存 时获取每个控件的值
VE.DataFormat = function (formId, dataFormat) {
    for (var item in dataFormat) {
        var getValue = "";
        if ($("#" + formId + " #" + dataFormat[item]).val() == "" || $("#" + formId + " #" + dataFormat[item]).val() == null) {
            if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-combotree]").length > 0) {
                if ($("#" + formId + " #" + dataFormat[item]).attr("multiple") + "" != "undefined"
                    && $("#" + formId + " #" + dataFormat[item]).attr("indeterminate") + "" != "undefined") {
                    var t = $("#" + formId + " #" + dataFormat[item]).combotree('tree');	// get the tree object
                    var nodes = t.tree('getChecked');		// get selected node
                    var nodeInde = t.tree('getChecked', 'indeterminate');		// get selected node
                    VE.Ids = [];
                    for (var i = 0; i < nodes.length; i++) {
                        VE.Ids.push(nodes[i].id);
                    }
                    VE.Ids.push('-');//全选和班选种的分隔符
                    for (var j = 0; j < nodeInde.length; j++) {
                        VE.Ids.push(nodeInde[j].id);
                    }
                    VE.Json[item.substring(0, item.length - 4)] = VE.Ids.join(',');
                }
                else {
                    getValue = $("#" + formId + " #" + dataFormat[item]).combotree('getValues');
                    if (getValue != "") {
                        VE.Json[item] = getValue + "";
                    }
                }

            } else if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-combobox]").length > 0) {
                getValue = $("#" + formId + " #" + dataFormat[item]).combobox('getValues');
                if (getValue != "") {
                    VE.Json[item] = getValue + "";
                }
            } else if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-combogrid]").length > 0) {
                getValue = $("#" + formId + " #" + dataFormat[item]).combogrid('getValues');
                if (getValue != "") {
                    VE.Json[item] = getValue + "";
                }
            }
            else if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-datebox]").length > 0) {
                getValue = $("#" + formId + " #" + dataFormat[item]).combogrid('getValues');
                if (getValue != "") {
                    VE.Json[item] = getValue + "";
                }
            }
            else if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-linkbutton]").length > 0) {
                getValue = $("#" + formId + " #" + dataFormat[item]).combogrid('getValues');
                if (getValue != "") {
                    VE.Json[item] = getValue + "";
                }
            }

        }
        else if ($("#" + formId + " #" + dataFormat[item] + "[class*=easyui-checkbox]").length > 0) {
            getValue = $("#" + formId + " #" + dataFormat[item])[0].checked;

            VE.Json[item] = getValue;

        }
        else {
            VE.Json[item] = $("#" + formId + " #" + dataFormat[item]).val().replace('\\/g', '\\\\');
        }
    }
};

//关闭窗口
VE.Close = function (dialog) {
    dialog.dialog("close");
};
//打开窗口
VE.Open = function (dialog) {
    dialog.dialog("open");
};

//编辑时必须选择一条记录 如果只选择一条记录返回当前选种的Id，否则返回0
//gridid 列表的Id,gridType 列表的类型，datagrid or treegrid
VE.Edit = function (gridId, gridType) {
    var rows;
    var grid = $("#" + gridId);
    if (gridType == "undefined" || gridType == undefined || gridType == VE.GridType_DataGrid) {
        rows = grid.datagrid('getSelections');
    }
    else if (gridType == VE.GridType_TreeGrid) {
        rows = grid.treegrid('getSelections');
    }
    if (rows.length == 0) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
        return 0;
    }
    if (rows.length > 1) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg_1, VE.MessageAlertIcon_Info);
        return 0;
    }
    if (rows[0].Active == false) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg_2, VE.MessageAlertIcon_Info);
        return 0;
    }
    return rows[0].id;
};

//删除
//url 是请求的Url地址
//gridId 对应的grid的Id
//isLoad true 执行grid的load事件，False 执行grid的reload事件
//gridType grid类型
VE.Delete = function (url, gridId, isLoad, gridType,reloadFunc) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return false;
    }
    VE.Ids = [];
    var rows;
    var grid = $("#" + gridId);
    if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
        rows = grid.datagrid('getSelections');
    }
    else if (gridType == "treegrid") {
        rows = grid.treegrid('getSelections');
    }
    if (rows.length == 0) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
        return false;
    }
    $.messager.confirm(VE.MessageConfirmTitle, VE.MessageConfirmMsg, function (r) {
        if (r) {
            for (var i = 0; i < rows.length; i++) {
                //if (url == "/KnowledgeLayout/Delete" || url == "/KnowledgeLayoutTab/Delete") {   //需要做特殊处理的删除操作
                //    VE.Ids.push(rows[i].Id + "-" + rows[i].GroupCode + "-" + rows[i].DictCode);
                //} else {
                var id = "";
                if (rows[i].Id == undefined) {
                    id = rows[i].id;
                }
                else {
                    id = rows[i].Id;
                }
                VE.Ids.push(id);
                //}
            }
            $.ajax({
                url: VE.AppPath + url,
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
                data: JSON.stringify({ id: VE.Ids.join(',') }),
                success: function (r) {
                    if (r.success) {
                        VE.MessageShow("删除成功");
                        if (reloadFunc) {
                            VE.ConvertFunction(reloadFunc);
                        } else {
                            VE.GridJudge(gridId, isLoad, gridType);
                        }
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
            return true;
        }
        return false;
    });
    return false;
};

VE.SingleDelete = function (url, gridId, id, isLoad, gridType) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return false;
    }
    VE.Ids = [];
    //var rows;
    //var grid = $("#" + gridId);
    //if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
    //    rows = grid.datagrid('getSelections');
    //}
    //else if (gridType == "treegrid") {
    //    rows = grid.treegrid('getSelections');
    //}
    //if (rows.length == 0) {
    //    $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
    //    return false;
    //}
    if (id != undefined || id != "") {
        VE.Ids.push(id);
    }
    $.messager.confirm(VE.MessageConfirmTitle, VE.MessageConfirmMsg, function (r) {
        if (r) {
            $.ajax({
                url: VE.AppPath + url,
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
                data: JSON.stringify({ id: VE.Ids.join(',') }),
                success: function (r) {
                    if (r.success) {
                        VE.MessageShow("删除成功");
                        VE.GridJudge(gridId, isLoad, gridType);
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
            return true;
        }
        return false;
    });
    return false;
};

VE.DeleteByUser = function (url, gridId, isLoad, gridType,index,rowId) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return false;
    }
    VE.Ids = [];
    VE.userIds = [];
    var rows;
    var selectRow;
 
    var grid = $("#" + gridId);
    if (index != null && index != undefined && index != "undefined") {
        selectRow = grid.datagrid('getRows')[index];
    } 
    else {
    if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
        rows = grid.datagrid('getSelections');
    }
    else if (gridType == "treegrid") {
        rows = grid.treegrid('getSelections');
    }
    if (rows.length == 0) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
        return false;
    }
    }
    
    $.messager.confirm(VE.MessageConfirmTitle, VE.MessageConfirmMsg, function (r) {
        if (r) {
          
            var args = [];
            var argsStr = "[";
            if (rowId != null && rowId != undefined && rowId != "undefined") {
                argsStr += JSON.stringify({ id: rowId, user_id: "" });
            }
            else if (index != null && index != undefined && index != "undefined") {
                argsStr += JSON.stringify({ id: selectRow.id, user_id: selectRow.user_id });
            }else{
            for (var i = 0; i < rows.length; i++) {

                argsStr += JSON.stringify({ id: rows[i].id, user_id: rows[i].user_id });
                argsStr += (i < rows.length - 1 ? "," : "");  
            }
        }
            argsStr += "]"   ;
            $.ajax({
                url: VE.AppPath + url,
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
              //  data: JSON.stringify({ id: VE.Ids.join(','), user_id: VE.userIds.join(',') }),
                data: argsStr,
                success: function (r) {
                    if (r.success) {
                        VE.MessageShow("删除成功");
                        VE.GridJudge(gridId, isLoad, gridType);
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
            return true;
        }
        return false;
    });
    return false;
};

VE.UpdateStatu = function (url, gridId, isLoad, gridType, index, confirmmsg, okMsg) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return false;
    }
    VE.Ids = [];
    VE.userIds = [];
    var rows;
    var selectRow;

    var grid = $("#" + gridId);
    if (index != null && index != undefined && index != "undefined") {
        selectRow = grid.datagrid('getRows')[index];
    }
    else {
        if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
            rows = grid.datagrid('getSelections');
        }
        else if (gridType == "treegrid") {
            rows = grid.treegrid('getSelections');
        }
        if (rows.length == 0) {
            $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
            return false;
        }
    }

    $.messager.confirm(VE.MessageConfirmTitle, confirmmsg, function (r) {
        if (r) {

            var args = [];
            var argsStr = "[";
            if (index != null && index != undefined && index != "undefined") {
                argsStr += JSON.stringify({ id: selectRow.id, user_id: selectRow.user_id });
            } else {
                for (var i = 0; i < rows.length; i++) {

                    argsStr += JSON.stringify({ id: rows[i].id, user_id: rows[i].user_id });
                    argsStr += (i < rows.length - 1 ? "," : "");
                }
            }
            argsStr += "]";
            $.ajax({
                url: VE.AppPath + url,
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
                //  data: JSON.stringify({ id: VE.Ids.join(','), user_id: VE.userIds.join(',') }),
                data: argsStr,
                success: function (r) {
                    if (r.success) {
                        VE.MessageShow(okMsg);
                        VE.GridJudge(gridId, isLoad, gridType);
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
            return true;
        }
        return false;
    });
    return false;
};

//禁用 启用
VE.Active = function (url, gridId, active, isLoad, gridType) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return false;
    }
    VE.Ids = [];
    var msg = "";
    var rows;
    var grid = $("#" + gridId);
    if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
        rows = grid.datagrid('getSelections');
    }
    else if (gridType == "treegrid") {
        rows = grid.treegrid('getSelections');
    }
    if (rows.length == 0) {
        $.messager.alert(VE.MessageAlertTitle, VE.MessageAlertMsg, VE.MessageAlertIcon_Info);
        return false;
    }
    if (active == true) {
        msg = VE.MessageConfirmMsgEnable;
    } else {
        msg = VE.MessageConfirmMsgDisable;
    }
    $.messager.confirm(VE.MessageConfirmTitle, msg, function (r) {
        if (r) {
            for (var i = 0; i < rows.length; i++) {
                VE.Ids.push(rows[i].id);
            }
            $.ajax({
                url: VE.AppPath + url,
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify({ id: VE.Ids.join(','), IsActive: active }),
                dataType: 'json',
                success: function (r) {
                    if (r.success) {
                        VE.MessageShow("操作成功");
                        VE.GridJudge(gridId, isLoad, gridType);
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
            return true;
        }
        return false;
    });
    return false;
};

// grid toolbar 权限加载
VE.LoodToolbar = function () {
    VE.Toolbar = [];
    var menuId = location.href.substring(location.href.indexOf('=') + 1, location.href.length);
    $.ajax({
        url: VConsts.STS_API_URL + '/Menu/GetAdminAction?argMenuId=' + menuId,
        type: 'post',
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Handler != "") {
                        var fn = new Function('return ' + data[i].Handler)();
                        VE.Toolbar.push({
                            id: data[i].Id,
                            iconCls: data[i].IconCls,
                            text: data[i].Text,
                            handler: fn
                        });
                    }
                }
            }
            return VE.Toolbar;
        }
    });
};

// Active状态列显示图标
VE.FormatterActive = function (value) {
    if (value == true) return "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-active_true\"></span>";
    else return "<span style=\"display:inline-block;height:16px;width:16px;\" class=\"icon-busy\"></span>";
};
// 
VE.FormatterBool = function (value) {
    if (value == true) return "是"
    else return "否"
}

VE.FormatterSub10 = function (value) {
    if (value) {
        if (value.length > 10) {
            return "<span title=" + value + ">" + value.substring(0, 10) + "...</span>"
        } else {
            return value;
        }
    }
}
VE.FormatterSub15 = function (value) {
    if (value) {
        if (value.length > 15) {
            return "<span title=" + value + ">" + value.substring(0, 15) + "...</span>"
        } else {
            return value;
        }
    }
}
VE.FormatterSub25 = function (value) {
    if (value) {
        if (value.length > 25) {
            return "<span title=" + value + ">" + value.substring(0, 25) + "...</span>"
        } else {
            return value;
        }
    }
}

VE.MessageShow = function (msg) {
    $.messager.show({
        title: '提示',
        msg: msg,
        timeout: VE.MessageAlertTimeout,
        showType: VE.MessageAlertShowType_Show
    });
};

//判断当前datagrid是哪种类型
VE.GridJudge = function (gridId, isLoad, gridType) {
    //判断是否为DataGrid
    var grid = $("#" + gridId);
    if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
        grid.datagrid('unselectAll');
        if (isLoad) {
            grid.datagrid('load');
        } else {
            grid.datagrid('reload');
        }
    }
    else if (gridType == "treegrid") {
        grid.treegrid('unselectAll');
        if (isLoad) {
            grid.treegrid('load');
        } else {
            grid.treegrid('reload');
        }
    }
};

// formId 对应的FormId
// dataFormat 提交标签
// url 对应的请求
// dialog 对应的弹出框Id
// datagrid 对应的 grid Id
// isDialog 如果是true 保存后关闭对话框
// isLoad 如果是 true 保存后重新加载，否则是 reload.
// ajaxType:请求类型 post、get,
// isLoad:保存后是否重新加载;true 重新加载ID ,
// loadUrl:保存后重新加载的地址; 如：CrmConsts.STS_API_URL + "/api/Employee/"  /*该参数暂时无法使用*/
VE.Save = function (formId, url, dialog, gridId, isDialog, isLoad, gridType) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return;
    }
    var b = $('#' + formId).form('validate');//Easyui 验证结果
    if (b) VE.Mask("", formId); //加载等待框
    else return;  //如果为false ，则验证没通过
    VE.Json = {}; //清空数据
    var data = VE.GetFormData(formId);
    
    $.ajax({
        url: VE.AppPath + url,
        contentType: 'application/json',
        type: "post",
        data: JSON.stringify(data),
        success: function (r) {
            VE.UnMask();
            if (r.success) {
                VE.MessageShow("操作成功");
                VE.GridJudge(gridId, isLoad, gridType);
                if (isDialog) {
                    dialog.dialog("close");
                }
            } else {
                VE.UnMask();
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
        },
        error: function (jqxhr) {
            VE.UnMask();
            var data = jqxhr.responseJSON;
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
};

//不需要参数的操作-如用户信息的同步功能。
VE.Save2 = function (formId, url) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return;
    }
    $('#' + formId).form('submit', {
        url: url,
        onSubmit: function () {
            VE.Mask();
            return $('#' + formId).form('validate');
        },
        success: function (r) {
            if (r != null) {
                r = eval('(' + r + ')');
                if (r.success) {
                    VE.UnMask();
                    VE.MessageShow(r.Msg);
                } else {
                    VE.UnMask();
                    $.messager.alert(VE.MessageAlertIcon_Info, r.Msg, r.Icon);
                }
            }
        }
    });
};

VE.Upload = function (formId, url, dialog, datagrid, isDialog, isSave, data) {
    VE.Mask("", formId); //加载等待框
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: data,
        success: function (r) {
            VE.UnMask();
            if (r.success) {
                VE.MessageShow(r.msg);
                VE.GridJudge(datagrid, isSave);
                if (isDialog) {
                    dialog.dialog("close");
                }
            } else {
                VE.UnMask();
                $.messager.alert('提示', r.Msg, r.Icon);
            }
        },
        error: function () {
            VE.UnMask();
            $.messager.alert('提示', VE.MessageAlertMsg_3, VE.MessageAlertIcon_Error);
        }
    });
}

////默认查询datagrid
//VE.Filter = function (formId, dataFormat, datagrid) {
//    //VE.Json = {}; //清空数据
//    //VE.DataFormat(formId, dataFormat);
//    VE.GetFormData("#" + formId);
//    //判断是否为DataGrid
//    if (datagrid.selector == "#datagrid") {
//        datagrid.datagrid('unselectAll');
//        datagrid.datagrid('load', { filter: $.toJSON(VE.Json) });
//    }
//};

//默认查询datagrid
VE.Filter = function (formId, gridId, gridType) {
    if (!checkLogin()) {
        evtBus.dispatchEvt("show_login");
        return;
    }
    var data = VE.GetFormData(formId);
    var grid = $("#" + gridId);
    if (gridType == "undefined" || gridType == undefined || gridType == "datagrid") {
        grid.datagrid('unselectAll');
        grid.datagrid('load', { filter: $.toJSON(data) });
    }
};




//默认不清空Id的数据
VE.Clear = function (formId, datagrid) {
    VE.ClearById("id", formId, datagrid);
};


//清空Id以外的所有文本框的值
VE.ClearById = function (id, formId, datagrid) {
    $("#" + formId + " input:not(#" + id + ")").val('');
    $(".easyui-combobox").combobox("clear");
    $(".easyui-combotree").combotree("clear");
    $(".easyui-combogrid").combogrid("clear");
    $(".easyui-textbox").textbox("clear");
    $(".easyui-numberbox").numberbox("clear");
    VE.Json = {}; //清空数据
    //VE.DataFormat(formId, dataFormat);
    if (datagrid.selector == "#datagrid") {
        datagrid.datagrid('unselectAll');
        datagrid.datagrid('load', { filter: $.toJSON(VE.Json) });
    }
};

// 把查询的方法名 以字符串传递过来 如：VE.Enter('Filter'); Filter为自定义的方法。
VE.Enter = function (func) {
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            VE.ConvertFunction(func);
        }
    });
};

VE.ConvertFunction = function (func) {
    var enter = new Function(func + "()"); //把字符串转换为方法并执行。
    enter();
};

// 把 null 转换成 ""
VE.StringFormat = function (value) {
    if (value == null || value == "null" || value == "") {
        return "";
    }
    return value;
};

var mask, maskMsg;
var defMsg = '正在处理，请稍待。。。';
VE.InitMask = function (obj) {
    if (!obj) {
        if (!mask) {
            mask = $("<div class=\"datagrid-mask\"></div>").appendTo("body");
        }
        if (!maskMsg) {
            maskMsg = $("<div class=\"datagrid-mask-msg\">" + defMsg + "</div>")
                .appendTo("body").css({ 'font-size': '12px' });
        }
        mask.css({ width: "100%", height: $(document).height() });
        maskMsg.css({
            left: ($(document.body).outerWidth(true) - 190) / 2,
            top: ($(window).height() - 45) / 2
        });
    } else {
        if (!mask) {
            mask = $("<div class=\"datagrid-mask\"></div>").appendTo($("#" + obj));
        }
        if (!maskMsg) {
            maskMsg = $("<div class=\"datagrid-mask-msg\">" + defMsg + "</div>")
                .appendTo($("#" + obj)).css({ 'font-size': '12px' });
        }
        mask.css({ width: "100%", height: $(document).height() });
        maskMsg.css({
            left: ($("#" + obj).outerWidth(true) - 160) / 2,
            top: ($(window).height() - 180) / 2,
            zIndex:99999
        });
    }
};
VE.Mask = function (msg, obj) {
    VE.InitMask(obj);
    mask.show();
    maskMsg.html(msg || defMsg).show();
};
VE.UnMask = function (msg) {
    mask.hide();
    maskMsg.hide();
};

// formId 对应的FormId
// GroupCode 编码
//根据GroupCode 获取字典信息
VE.GetDictByGroupCode = function GetDictByGroupCode(id, groupCode) {
    $(id).combobox({
        url: CrmConsts.STS_API_URL + "/api/Dictionary/GetDictByGroupCode?groupCode=" + groupCode,
        valueField: 'DictCode',
        textField: 'DictName'
    });
}

// formId 对应的FormId
//获取字典分组下拉树
VE.GetDictGroupTree = function GetDictGroupTree(id) {
    $(id).combotree({
        url: CrmConsts.STS_API_URL + "/api/DictGroup/GroupTree?g=g",
        editable: true
    });
}

VE.GetDictGroup = function GetDictGroupTree(id) {
    $(id).combobox({
        url: VConsts.STS_API_URL + "/Group/Get",
        valueField: 'GroupCode',
        textField: 'GroupName'
    });
}
// formId 对应的FormId
//获取字典分组下拉树
VE.GetDictTree = function GetDictGroupTree(id, groupCode, value) {
    $(id).combotree({
        url: VConsts.STS_API_URL + "/Dictionary/DictTree?groupCode=" + groupCode,
        editable: true,
        onLoadSuccess: function () {
            if (value) {
                $(id).combotree('setValue', value);
            }
        },
        onBeforeExpand: function (node) {
            if (node.children.length == 0) {
                return false;
            }
        }
    });
}


VE.IsNull = function (str) {
    if (typeof (str) == "undefined") {
        return "";
    }
    else if (str == null) {
        return "";
    }
    else {
        return str;
    }
}


//在取消保存时对比当前表单数据与原有加载的数据是否相等
//formID:窗体的ID
//vData:加载时获取到的数据
//IsClose：是否需要提示框
//isSave:是否为保存（若为保存，则不出现提示数据改变的窗口）
//返回值：true，false；
VE.GetFormControlValue = function (formID, vData, isClose, isSave) {
    VE.GetFormData(formID);
    var isTrue = true;//默认当前Form中的数据与加载时的数据相等
    var json = JSON.parse($.toJSON(VE.Json));
    for (var item in json) {
        if (vData[item] == undefined) {
            continue;
        }
        if (VE.IsNull(vData[item]) != json[item]) {
            isTrue = false;//在关闭窗口时，如果 某一项数据与加载时的数据不相等，则将isTrue置为false
            break;
        }
    }
    if (isSave == false) {
        if (isClose == true) {
            return VE.IsCloseWindow(isTrue);
        }
        else {
            return isTrue;
        }
    }
}


//isTrue：false 显示对话框（是否关闭该窗口）、True：不显示
VE.IsCloseWindow = function (isTrue) {
    var isClose = false;//默认当前窗口不可关闭
    if (isTrue == false) {//关闭窗口时，如果当前窗口的数据与加载后的数据不相等则提示是否关闭该窗口
        if (confirm('数据已经更改，是否继续关闭该窗口！')) {
            isClose = true;
        } else {
            isClose = false;
        }
    }
    else {
        isClose = true;
    }
    return isClose;
}

//获取表单控件的值；
//formID：表单的ID（form、div....）
//组织成json对象
VE.GetFormData = function (formId) {
    VE.Json = {};  //清空现有Json值
    $("#" + formId + " *").each(function (index) {
        var vDom = $(this);//获取当前元素
        var id = vDom.attr("id");//获取当前节点的ID值
        var $id = " #" + id;
     
        if (id + "" != "undefined") {

            switch (true) {
                case vDom.is("[class*=easyui-datebox]"):
                    VE.Json[id] = vDom.datebox('getValue');
                    break;
                case vDom.is("[class*=easyui-datetimebox]"):
                    VE.Json[id] = vDom.datetimebox('getValue');
                    break;
                case vDom.is(":text"): //获取普通文本框的值
                    var value = vDom.val();
                    if (id) {
                        VE.Json[id] = value.replace('\\/g', '\\\\');
                    }
                    break;
                case vDom.is("[class*=easyui-combotree]")://获取下拉树控件的值
                    if (vDom.attr("multiple") + "" != "undefined" && vDom.attr("indeterminate") + "" != "undefined") {//下拉多选树
                        var t = vDom.combotree('tree');	// 获取树节点项
                        var nodes = t.tree('getChecked');		// 获取所有选中的节点
                        var nodeInde = t.tree('getChecked', 'indeterminate');		// 获取所有选中的节点
                        VE.Ids = [];
                        for (var i = 0; i < nodes.length; i++) {
                            VE.Ids.push(nodes[i].id);
                        }
                        VE.Ids.push('-');//全选和班选种的分隔符
                        for (var j = 0; j < nodeInde.length; j++) {
                            VE.Ids.push(nodeInde[j].id);
                        }
                        VE.Json[id.substring(0, id.length - 4)] = VE.Ids.join(',');
                    }
                    else {//普通的拉下树
                        var t = vDom.combotree('tree');
                        var n = t.tree('getSelected');
                        if (n) {
                            VE.Json[id + "Name"] = n.text + "";
                        }
                        getValue = vDom.combotree('getValues');
                        VE.Json[id] = getValue + "";
                    }
                    break;
                case vDom.is("[class*=easyui-combobox]")://普通下拉框
                    VE.Json[id + "Name"] = vDom.combobox('getText');
                    VE.Json[id] = vDom.combobox('getValues') + "";
                    break;
                case vDom.is("[class*=easyui-combogrid]")://table
                    getValue = vDom.combogrid('getValues');
                    VE.Json[id] = getValue + "";
                    break;
                case vDom.is("[class*=easyui-checkbox]")://复选框
                    getValue = vDom[0].checked;
                    VE.Json[id] = getValue;
                    break;
                case vDom.is("textarea")://多行文本框
                    getValue = vDom.val().replace('\\/g', '\\\\');
                    VE.Json[id] = getValue;
                    break;
                default:
                    getValue = vDom.val().replace('\\/g', '\\\\');
                    VE.Json[id] = getValue;
                    break;
            }
        }
    });
    return JSON.parse($.toJSON(VE.Json));
}


VE.getCookie = function (cookName) {
    if (document.cookie.length > 0) {
        var start = document.cookie.indexOf(name + "=");
        if (start !== -1) {
            start = start + name.length + 1;
            var end = document.cookie.indexOf(";", start);
            if (end === -1) {
                end = document.cookie.length;
            }
            var value = unescape(document.cookie.substring(start, end));
            var obj = JSON.parse(value);
            return obj;
        }
    }
    return null;
}

 

$.fn.datebox.defaults.cleanText = '清空';//给datebox拓展清空按钮

(function ($) {
    var buttons = $.extend([], $.fn.datebox.defaults.buttons);
    buttons.splice(1, 0, {
        text: function (target) {
            return $(target).datebox("options").cleanText
        },
        handler: function (target) {
            $(target).datebox("setValue", "");
            $(target).datebox("hidePanel");
        }
    });
    $.extend($.fn.datebox.defaults, {
        buttons: buttons
    });

})(jQuery)