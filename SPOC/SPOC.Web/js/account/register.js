var tt = 3;
$(function () {

    /*
    $.validator.addMethod("usernameformatcheck", function (value, element) {
        // alert("sb");
     //   var length = value.length;
        var mobile = /^(?=.*[a-zA-Z])(?=.*\d).*$/;
        return this.optional(element) || ( mobile.test(value));
    });
    $.validator.unobtrusive.adapters.addBool("usernameformatcheck");


    $.validator.addMethod("emailtemp", function (value, element) {
        if (value == false) {
            return true;
        }
        this.optional(element) || /^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]+$/i.test(value);
    });

    //扩展方法注册
    $.validator.unobtrusive.adapters.addBool("emailtemp");
    */

    $("#UserName").show();
    $("#PassWord").show();
    $(".tempText").remove();

    pwd = "";
    //  $("#RegisterMobileOrEmail").attr("placeholder", "填写你常用的手机号码作为登录帐号");
    $("#RegisterMobile").attr("placeholder", "填写你常用的手机号码作为登录帐号");
    $("#RegisterEmail").attr("placeholder", "填写你常用的邮箱");
   // $("#UserName").attr("placeholder", " 中、英文均可，最长16个英文或8个汉字");
    $("#UserName").attr("placeholder", " 中、英文均可，最长16个字符");
  
    $("#PassWord").attr("placeholder", "5-20位英文、数字、符号，区分大小写");
    $("#SmsCode").attr("placeholder", "填写你的短信验证码");
    $("#InviteCode").attr("placeholder", "如果您有邀请码，请填写您的邀请码");

    intigister();



    var flag = $("#hiddenFlag").val();
    if (flag === "") {
        $("#UserName").val("");
        $("#RegisterMobile").val("");
        $("#RegisterEmail").val("");
    }

    $.ajax({
        url: "/Account/GetRsaKey",
        type: "get",
        dataType: "json",
        async: false,
        success: function (data) {
            $("#excryptKey").val(data.encryptKey);
            $("#excryptModulusKey").val(data.ModulusKey);
        }

    });
    


    if ($("#ok_div").length > 0 && $("#ok_div").val() != null) {

        ReturnUrl();
    }

  
   
    $("#register-form").submit(function () {
       
        onSubmitCheck();

        
         
        var validator = $('#register-form').validate({

            // 错误插入位置，以及插入的内容
            errorPlacement: function (error, element) {
                $(element).parent().after(error);
            },

        
            //使用div标签，包裹提示信息，而后插入DOM
            wrapper: "div",

            //单条校验失败，后会调用此方法，在此方法中，编写错误消息如何显示 及  校验失败回调方法
            showErrors: function (errorMap, errorList) {
                // 遍历错误列表
                for (var obj in errorMap) {
                    // 自定义错误提示效果
                    $('#' + obj).parent().addClass('has-error');
                }
                // 此处注意，一定要调用默认方法，这样保证提示消息的默认效果
                this.defaultShowErrors();
            },

            // 验证成功后调用的方法
            success: function (label) {
                $(label).parent().prev().removeClass('has-error');
            }

            //点击提交按钮，校验失败后，会调用此方法
            /*invalidHandler: function(form, validator) {
                console.log("");
            },*/
        });
      
        if (!$("#register-form").valid()) {
            $("#register-btn").attr("disabled", false);
            $("#UserName").val("");
            return false;
        }
 
        var key = $("#excryptKey").val();
        setMaxDigits(130);
        var resKey = new RSAKeyPair(key, "", $("#excryptModulusKey").val());

        
     
        encryptedVal("PassWord", "EncucriptPassWord", resKey, "000000xxxxx", "password");
        encryptedVal("UserName", "EncucriptUserName", resKey, "x00000000", "text");
   

            $("#register-btn").html("正在注册....");
            $("#register-btn").attr("disabled", true);
 
    });
});

function encryptedVal(id, encrypDomId, resKey, replaceVal, type) {
    var val = $("#" + id).val();
    if (val !== "" && val !== null) {
        if (val != replaceVal) {
            var enCodeStr = escape(val);
            $("#" + encrypDomId).val(encryptedString(resKey, enCodeStr));
        }
        $("#" + id).val(replaceVal);

        oldText = $("#" + id);
        if ($(".tempTxt_input" + id).length <= 0) {
            oldText.after('<input  class="form-control input-lg tempText tempTxt_input' + id + '" type="' + type + '" value=' + val + ' autocomplete="off" />');
        }
        var newPlaceholder = $(this).siblings('.tempTxt_input' + id + '');
        $(newPlaceholder).val(val);
        newPlaceholder.show();
        oldText.hide();
    }
}

function onSubmitCheck() {
    if (!isPlaceholder()) {
        var b = true;
        $("#register-form input").each(
            function () {
                var pwdVal = $(this).attr('placeholder');
                if (pwdVal != "" && pwdVal != null) {
                    $(this).show();
                }
            }
            )
        $(".tempPwdTxt_input").hide();
        $("#PassWord").show();
    }
   
}

function intigister() {
    if ($("#user_terms")) {
        var bischecked = $('#user_terms').is(':checked');
        if (!bischecked) {
            $("#register-btn").attr("disabled", true);
        }
    }
    $("#user_terms").change(function () {
        if ($(this).is(':checked')) {
            $("#register-btn").attr("disabled", false);
        } else {
            $("#register-btn").attr("disabled", true);
        }
    });
}

function isPlaceholder() {
    var input = document.createElement('input');
    return 'placeholder' in input;
}

function ReturnUrl() {

    if (tt < 1) {
        window.location = url;
        return;
    };
    $("#ok_div").html($("#regMsg").val() + ",页面将在 " + tt + " 秒后跳转...");
    tt--;
    window.setTimeout("ReturnUrl()", 1000);
}

$(window).keydown(function (event) {
    switch (event.keyCode) {
        case 13: {
            if ($("#user_terms") && !$("#user_terms").is(':checked')) {
                return;
            }
            if ($("#register-form").find("input").is(":focus")) {
                $("#register-form").submit();
            }
        }
            break;
    }
});