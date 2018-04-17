//扩展字符串方法，可用{0}、{1}等来做占位符格式化
//eg1: var str = "hello {0}".format("world"); //str = "hello world"
//eg2: var str = "{key1} {key2}".format({key1:"hello", key2:"world"}); //str = "hello world"
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        var reg;
        if (arguments.length === 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args.hasOwnProperty(key)) {
                    var value = (args[key] == undefined || args[key] == null) ? "" : args[key];
                    reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, value);
                }
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
};

// 对Date的扩展，将 Date 转化为指定格式的String 
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.format = function(fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
};

//扩展旧版IE数组没有indexOf()方法的问题
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;
        var from = Number(arguments[1]) || 0;
        from = (from < 0)
	         ? Math.ceil(from)
	         : Math.floor(from);
        if (from < 0)
            from += len;
        for (; from < len; from++) {
            if (from in this &&
	          this[from] === elt)
                return from;
        }
        return -1;
    };
}

/**
 * 数组扩展删除方法
 * @param item
 * @returns {*}
 */
Array.prototype.remove = function (item) {
    var itemIndex = this.indexOf(item);
    if (itemIndex >= 0) {
        this.splice(itemIndex, 1);
        return itemIndex;
    }
    return -1;
}

//空Guid
window.emptyGuid = "00000000-0000-0000-0000-000000000000";
//webapi请求前缀
window.apiUrl = "/api/services/app/";

//字符串为空判断
function stringIsEmpty(s) {
    if (s == undefined || s === "" || s == null) {
        return true;
    } else {
        return false;
    }
}
//Guid字符串为空判断
function guidIsEmpty(guid) {
    if (guid == undefined || guid === "" || guid === null || guid === emptyGuid) {
        return true;
    } else {
        return false;
    }
}

function unixToDate (unixTime) {
    var newDate = new Date();
    newDate.setTime(unixTime * 1000);
    var y = newDate.getFullYear();
    var m = newDate.getMonth() + 1;
    var d = newDate.getDate();
    var h = newDate.getHours();
    var i = newDate.getMinutes();
    var s = newDate.getSeconds();
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d) + ' ' + (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) + ':' + (s < 10 ? '0' + s : s);
}

function dateToUnix(dateStr) {
    var newStr = dateStr.replace(/:/g, "-");
    newStr = newStr.replace(/ /g, "-");
    var arr = newStr.split("-");
    var datum = new Date(Date.UTC(arr[0], arr[1] - 1, arr[2], arr[3] - 8, arr[4], arr[5]));
    return datum.getTime() / 1000;
}
//timestr格式=> 00:00:00
function timestrToSecond(timestr) {
    var strArr = timestr.split(":");
    var second = 0;
    second += parseInt(strArr[0]) * 3600;
    second += parseInt(strArr[1]) * 60;
    second += parseInt(strArr[2]);
    return second;
}

function secondToTimestr(second) {
    function fillZero(val) {
        if (val < 10) {
            return "0" + val;
        } else {
            return "" + val;
        }
    }

    var timestr = "{h}:{m}:{s}";
    var h = second >= 3600 ? Math.floor(second / 3600) : 0;
    var hs = 3600 * h;
    var m = second >= 60 ? Math.floor((second - hs) / 60) : 0;
    var ms = 60 * m;
    var s = second - hs - ms;
    return timestr.format({ h: fillZero(h), m: fillZero(m), s: fillZero(s) });
}

function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") !== -1) {
        var str = url.substr(1);
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            var kv = strs[i].split("=");
            theRequest[kv[0]] = kv[1];
        }
    }
    return theRequest;
}

function checkLogin() {
    $.ajaxSetup({
        cache: false //关闭AJAX缓存
    });
    var res = true;
    $.ajax({
        url: "/Account/GetloginLimit",
        type: "get",
        dataType: "json",
        async:false,
        success: function (data) {
            if (data !== true) {
              //  $.messager.alert("提示", "您的账号已在其他地方登陆，请重新登录", "info");
                alert("您的账号已在其他地方登陆，请重新登录!");
              //  window.parent.location.href = "";
                res= false;
            } else {
                res= true;
            }
        }
    });
    if (!res) { return res;}
    var cookieName = "SPOC_UserInfo";
    var cookie = nv.cookie.get(cookieName);
    if (cookie == null) {
        return false;
    } else {
        nv.cookie.clearCookie(cookieName);
        nv.cookie.setCookie(cookieName,cookie);
    }
    return true;
}

var nv = nv ? nv : {};
nv.get = function(url, doneFunc) {
    $.ajax({
        url: url,
        dataType: "json",
        type: "get",
        success: function (data, textStatus, jqxhr) {
            if (doneFunc) {
                doneFunc(data, textStatus, jqxhr);
            }
        },
        error: function (jqxhr, textStatus, errorThrown) {
            if (doneFunc) {
                var data = jqxhr.responseJSON;
                doneFunc(data, textStatus, errorThrown);
            }
        }
    });

};

nv.post = function (url, param, doneFunc) {
    $.ajax({
        url: url,
        dataType: "json",
        contentType: "application/json",
        type: "post",
        data: JSON.stringify(param),
        success: function (data, textStatus, jqxhr) {
            if (doneFunc) {
                doneFunc(data, textStatus, jqxhr);
            }
        },
        error: function (jqxhr, textStatus, errorThrown) {
            if (doneFunc) {
                var data = jqxhr.responseJSON;
                doneFunc(data, textStatus, errorThrown);
            }
        }
    });
};

nv.cookie = {};
nv.cookie.get = function(name) {
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
};

nv.cookie.setCookie = function (name, obj, time) {
   
    var value = JSON.stringify(obj);
    var exp = new Date();
    if (!time) {
        time = 30 * 60 * 1000;
    }
     exp.setTime(exp.getTime() + time);//过期时间 30分钟  
     document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + "; path=/";
}
nv.cookie.clearCookie = function (cname) {
    var cvalue = "";
    var d = new Date();
    d.setTime(d.getTime() + (-1 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + ";expires= " + expires + "; path=/";
}
var webApi = '/api/services/app';

//url参数转json
function parseQueryString(url) {
    var obj = {};
    var keyvalue = [];
    var key = "", value = "";
    var paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
    for (var i in paraString) {
        keyvalue = paraString[i].split("=");
        key = keyvalue[0];
        value = keyvalue[1];
        obj[key] = value;
    }
    return obj;
}

//json转get
var parseParam = function (param, key) {
    var paramStr = "";
    if (param instanceof String || param instanceof Number || param instanceof Boolean) {
        paramStr += "&" + key + "=" + encodeURIComponent(param);
    } else {
        $.each(param, function (i) {
            var k = key == null ? i : key + (param instanceof Array ? "[" + i + "]" : "." + i);
            paramStr += '&' + parseParam(this, k);
        });
    }
    return paramStr.substr(1);
};

 