var LoginDialog = (function() {
    function init() {
        var self = this;
        var handle = evtBus.addEvt("show_login", function () {
            $("#login-dialog").dialog("open");
            if (stringIsEmpty($("#userName").textbox("getValue"))) {
                $("#userName").textbox("textbox").focus();
            } else {
                $("#password").textbox("textbox").focus();
            }
        });

        $(window)
            .unload(function() {
                evtBus.removeEvt(handle);
            });
        
        this.login = function() {
            if (!$("#login-form").form("validate")) {
                return;
            }

            $("#loginBtn").linkbutton("disable");
            var url = "/api/services/app/UserInfo/UserLoginRequset";
            VE.Mask("");
            var userName = $("#userName").textbox("getValue");
            var passWord = $("#password").textbox("getValue");
            nv.post(url, {
                UserName: userName,
                PassWord: passWord
            }, function(data) {
                VE.UnMask();
                $("#loginBtn").linkbutton("enable");
                if (data.success) {
                    $("#login-name").text(userName);
                    $("#edit-pwd").attr("onclick", "edit('"+userName+"')");
                    $("#login-dialog").dialog("close");
                    $.messager.show({ title: "提示", msg: "登录成功！" });
                    $("#password").textbox("clear");
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        this.onUserNameKeyPress = function(event) {
            if (event.keyCode === 13) {
                $("#password").textbox("textbox").focus();
            }
        };

        this.onPasswordKeyPress = function(event) {
            if (event.keyCode === 13) {
                self.login();
            }
        }
    }
    return init;
})();