var Login = (function() {
    function init(rsaKey, modulusKey, skipUrl) {
        this.doLogin = function () {
            var $loginFail = $("#loginFail").hide();

            if (!$("#loginForm").valid()) {
                return;
            }

            var $btn = $("#loginBtn")
                .text("正在登录..")
                .attr("disabled", "true");

            var url = "/Account/DoLogin";
            var param = getParam();

            $.post(url, param, function(data) {
                $btn.text("登录")
                    .removeAttr("disabled");
                if (data.result.code === 0) {
                    if (param.rememberMe) {
                        var time = 7 * 24 * 60 * 60 * 1000;//7天
                        nv.cookie.setCookie("spoc_rememberMe", { userLoginName: $("#userLoginName").val() }, time);
                    }
                    //教师与管理员直接跳转到后台
                    if (data.result.result === "admin") {
                        skipUrl = "/AdminHome/Index/";
                    }
                    window.location.href = skipUrl;
                } else {
                    $loginFail.html(data.result.msg);
                    $loginFail.show();
                }
            });
        };

        function getParam() {
            setMaxDigits(130);
            var key = new RSAKeyPair(rsaKey, "", modulusKey);
            var p = {};
            p.userLoginName = encryptedString(key, escape($("#userLoginName").val()));
            p.userPassWord = encryptedString(key, escape($("#userPassWord").val()));
            p.rememberMe = $("#rememberMe").prop("checked") ? true : false;
            return p;
        }
    }

    return init;
})();


var Register = (function () {
   
    function init(rsaKey, modulusKey) {
        this.doRegister = function () {
            
            var $registerFail = $("#registerFail").hide();
            if (!$("#registerForm").valid()) {
                return;
            }
            var $btn = $("#btnRegister").attr("disabled", "true");
            var url = "/Account/Register";
            var param = getParam();
            if (param.ClassId === "") {
                $btn.removeAttr("disabled");
                $("#registerFail").html("");// 清空数据
                $("#registerFail").append("请选择班级！");
                $registerFail.show();
                return;
            }
            $.post(url, param, function (data) {
                $btn.removeAttr("disabled");
                if (data.success) {
                    //询问框
                    layer.alert('您的注册信息已提交!请等待老师审核。');
                   
                    //window.location.href = "home/index";
                } else {
                    $("#registerFail").html("");// 清空数据
                    $("#registerFail").append( data.msg ); 
                    $registerFail.show();
                }
            });
        };
        function getParam() {
            setMaxDigits(130);
            var key = new RSAKeyPair(rsaKey, "", modulusKey);
            var p = {};
            p.EncucriptUserName = encryptedString(key, escape($("#registerLoginName").val()));
            p.EncucriptPassWord = encryptedString(key, escape($("#passWord").val()));
            p.UserFullName = escape($("#userFullName").val());
            p.RegisterEmail = ($("#userEmail").val());
            p.ClassId = $('#Class').val();
            return p;
        }
    }

    return init;
})();