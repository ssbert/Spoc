$.extend($.fn.validatebox.defaults.rules, {
    minLength: { // 判断最小长度 
        validator: function (value, param) {
            value = $.trim(value);	//去空格 
            return value.length >= param[0];
        },
        message: '最少输入 {0} 个字符。'
    },
    length: {
        validator: function (value, param) {
            var len = $.trim(value).length;
            return len >= param[0] && len <= param[1];
        },
        message: "输入内容长度必须介于{0}和{1}之间."
    },
    selectRequired: {
        validator: function (value) {
            return value.replace(/^\s+|\s+$/g, "") !== "";
        },
        message: '该输入项为必选项'
    },
    phoneMobile: {// 验证固话和手机
        validator: function (value) {
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value) || /^(13|14|15|17|18)\d{9}$/i.test(value);
        },
        message: '格式不正确,请使用下面格式:020-88888888或15000000000'
    },
    phone: {// 验证电话号码 
        validator: function (value) {
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '格式不正确,请使用下面格式:020-88888888'
    },
    mobile: {// 验证手机号码 
        validator: function (value) {
            return /^(13|14|15|17|18)\d{9}$/i.test(value);
        },
        message: '手机号码格式不正确'
    },
    newMobile: {// 验证手机号码 
        validator: function (value) {
            return /^(13|14|15|17|18)\d{9}$/i.test(value.replace(/^\s+|\s+$/g, ""));
        },
        message: '手机号码格式不正确'
    },
    idcard: {// 验证身份证 
        validator: function (value) {
            return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
        },
        message: '身份证号码格式不正确'
    },


    intOrFloat: {// 验证整数或小数 
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    currency: {// 验证货币 
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '货币格式不正确'
    },
    qq: {// 验证QQ,从10000开始 
        validator: function (value) {
            return /^[1-9]\d{4,9}$/i.test(value);
        },
        message: 'QQ号码格式不正确'
    },
    integer: {// 验证整数 
        validator: function (value) {
            return /^[+]?[0-9]+\d*$/i.test(value);
        },
        message: '请输入整数'
    },

    chinese: {// 验证中文 
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/i.test(value);
        },
        message: '请输入中文'
    },
    english: {// 验证英语 
        validator: function (value) {
            return /^[A-Za-z]+$/i.test(value);
        },
        message: '请输入英文'
    },
    unnormal: {// 验证是否包含空格和非法字符 
        validator: function (value) {
            return /.+/i.test(value);
        },
        message: '输入值不能为空和包含其他非法字符'
    },
    username: {// 验证用户名 
        validator: function (value) {
            //    return /^[a-zA-Z][a-zA-Z0-9_]{3,15}$/i.test(value);
            return /^(?!\d{4,16}$)(?:[a-z\d_]{4,16}|[\u4E00-\u9FA5]{2,8})$/i.test(value);

        },
        //
        //  message: '用户名不合法（字母开头，允许4-16字节，允许字母数字下划线）'
        message: '用户名不合法（中、英文均可，最长16个英文或8个汉字）'
    },
    faxno: {// 验证传真 
        validator: function (value) {
            //            return /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/i.test(value); 
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '传真号码不正确'
    },
    zip: {// 验证邮政编码 
        validator: function (value) {
            return /^[1-9]\d{5}$/i.test(value);
        },
        message: '邮政编码格式不正确'
    },
    ip: {// 验证IP地址 
        validator: function (value) {
            return /d+.d+.d+.d+/i.test(value);
        },
        message: 'IP地址格式不正确'
    },
    name: {// 验证姓名，可以是中文或英文 
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/i.test(value) | /^\w+[\w\s]+\w+$/i.test(value);
        },
        message: '请输入姓名'
    },
    carNo: {
        validator: function (value) {
            return /^[\u4E00-\u9FA5][\da-zA-Z]{6}$/.test(value);
        },
        message: '车牌号码无效（例：粤J12350）'
    },
    carenergin: {
        validator: function (value) {
            return /^[a-zA-Z0-9]{16}$/.test(value);
        },
        message: '发动机型号无效(例：FG6H012345654584)'
    },
    email: {
        validator: function (value) {
            value = value.replace(/(^\s*)|(\s*$)/g, '');//去除前后空格
            return /^\w+([-.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: '请输入有效的电子邮件账号(例：abc@126.com)'
    },
    msn: {
        validator: function (value) {
            return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: '请输入有效的msn账号(例：abc@hotnail(msn/live).com)'
    }, equalTo: {
        validator: function (value, param) {

            if ($("#" + param[0].trim()).val() != "" && value != "") {
                return $("#" + param[0].trim()).val() == value;
            } else {
                return true;
            }
        },
        message: '两次输入的密码不一致！'
    },
    domain: {
        validator: function (value) {
            return /^[a-zA-Z0-9]{4,20}$/.test(value);
        },
        message: '请输入有效的域名（4至20位数字或字母的组合）。'
    },
    dateCheck: {//日期格式验证
        validator: function (value, element) {
            return !/Invalid|NaN/.test(new Date(value.replace(/-/g, "/")).toString());
        },
        message: '请输入有效的日期。'

    },
    userFullName: {
        validator: function (value) {
            return /^([\u4e00-\u9fa5]+|([a-zA-Z]+\s?)+)$/.test(value);
        },
        message: '请输入有效的姓名。'
    }
    , dateGreaterThanCompare: {//验证当前日期比对象日期大于或等于
        validator: function (value, param) {
            $.fn.validatebox.defaults.rules.dateGreaterThanCompare.message = param[1].trim();
            if ($("#" + param[0].toString().trim()).datebox('getValue') != "" && value != "") {
                var d1 = $.fn.datebox.defaults.parser(value);
                var d2 = $.fn.datebox.defaults.parser($("#" + param[0].toString().trim()).datebox('getValue'));
                return d1 >= d2;
            } else {
                return true;
            }
        },
        message: ''
    }, dateLessThanCompare: {//验证当前日期比对象日期小于或等于
        errMsg: "",
        validator: function (value, param) {

            $.fn.validatebox.defaults.rules.dateLessThanCompare.message = param[1].trim();
            if ($("#" + param[0].toString().trim()).datebox('getValue') != "" && value != "") {
                var d1 = $.fn.datebox.defaults.parser(value);
                var d2 = $.fn.datebox.defaults.parser($("#" + param[0].toString().trim()).datebox('getValue'));
                return d1 <= d2;
            } else {
                return true;
            }

        },
        message: ''
    }, codeCheck: {//code格式验证
        validator: function (value, param) {
            if (/^[a-zA-Z0-9]+$/.test(value)) {
                return true;
            }
            return false;
        },
        message: "编码只能由英文与数字组成"
    }, categoryTypeCodeCheck: {//分类类型code格式验证
        validator: function (value, param) {
            if (/^[a-zA-Z0-9_]+$/.test(value)) {
                return true;
            }
            return false;
        },
        message: "编码只能由英文数字与下划线组成"
    }, price: {
        validator: function (value, param) {
            if (/^\d{1,9}(\.\d{1,2})?$/.test(value)) {
                return true;
            }
            return false;
        },
        message: "输入的价格不正确"
    }, days: {
        validator: function (value, param) {
            if (/^[0-9]{1,11}$/.test(value)) {
                return true;
            }
            return false;
        },
        message: "请输入正确的有效天数"
    }, checkSubjectIsDel: {
        validator: function (value, param) {
            if (value.indexOf("已删除") >= 0) {
                return false;
            }
            return true
        },
        message:"学科已被删除，请重新编辑"
    }, checkGroupStructIsDel: {
        validator: function (value, param) {
            if (value.indexOf("已删除") >= 0) {
                return false;
            }
            return true
        },
        message: "组织架构已被删除，请重新编辑"
    }, checkTagIsDel: {
        validator: function (value, param) {
            if (value.indexOf("已删除") >= 0) {
                return false;
            }
            return true
        },
        message: "标签已被删除，请重新编辑"
    },
    equaldDate: {
        validator: function (value, param) {
            var d1 = $(param[0]).datetimebox('getValue'); //获取开始时间
            return value >= d1; //有效范围为大于开始时间的日期
        },
        message: '结束日期不能早于开始日期!'
    }
});
