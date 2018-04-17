function OpeartUpload(saveUrlId, showId, onUploadSuccess, fromImgUploadBtnId, action, btnId) {
    this.onUploadSuccess = onUploadSuccess;
    this.formId = fromImgUploadBtnId+"_from";
  //  this.fromImgUploadBtnId = "AvatarFileUpload";
    this.action = action;
    this.btnId = btnId;
    this.fromImgUploadBtnId = fromImgUploadBtnId;
    this.showArgus = {
        saveUrlId: saveUrlId,
        showId: showId
    };

    this.alert = $.messager;
}
OpeartUpload.prototype = {
    createUpload: function () {
        
        if (this.formId === null || this.formId === "") {
            return;
        }
        if ($("#" + this.formId).length > 0) {
            var d=new Date();
            this.fromImgUploadBtnId = this.fromImgUploadBtnId + "_" + d.getMinutes().toString() + d.getMinutes().toString();
            this.formId = this.fromImgUploadBtnId + "_from";
        }
        this.createUploadFrom();

        var fromImgUploadBtnId = this.fromImgUploadBtnId;
        $("#" + this.btnId).click(function () {
            document.getElementById(fromImgUploadBtnId).click();
        });
    },
    createUploadFrom: function () {
        var form = document.createElement("form");
        form.name=this.formId;
        form.id = this.formId;
        form.style.display = "none";
       // form.style.display = "block";
        form.method = "post";
        form.enctype = "multipart/form-data";
        form.action = this.action;
        form.target = "upload_target";

        var input = document.createElement("input");
        input.type = "file";
        input.id = this.fromImgUploadBtnId;
        input.name = this.fromImgUploadBtnId;

        var uploadsuccess = this.getOnUploadSuccess();
        var imgChange = this.imgUploadChange;
        var uploadFormId = this.formId;
        $(input).change(function () { imgChange(uploadFormId,uploadsuccess); });
        form.appendChild(input);
        document.body.appendChild(form);

    },
    imgUploadChange: function (uploadFormId, uploadsuccess) {
        
        $('#' + uploadFormId).ajaxSubmit({
            dataType: "json",
          //  dataType: 'html',//可以不加，默认接受类型为html,这里主要解决火狐下的JSON解析异常问题
            
            success: uploadsuccess
        });
    },
    getUploadForm:function(){
        return document.getElementById(this.formId);
    },
    
    getOnUploadSuccess: function () {
        var alertObj = this.alert;

        if (this.onUploadSuccess === undefined || this.onUploadSuccess === null) {
            var saveUrlId = this.showArgus.saveUrlId;
            var showId = this.showArgus.showId;
         //   html, status
            return function ( data, response) {
               
                if (response) {
                  //  alert("nb");
                   // var model = JSON.parse(data);//将接受的html类型返回值转成JSON
                    var model = data;
                    if (model === "faile") {
                        if (alertObj === null || alertObj === undefined) {
                            
                            alert(model.msg);
                            
                        } else {
                            alertObj.alert('操作提示', model.msg, "error");
                        }
                        
                    }else if (model.statu == "ok") {
                        $("#" + saveUrlId).val(model.url);
                        $("#" + showId).removeAttr("style");
                        $("#" + showId)[0].src = model.thumbPath + "?t=" + Math.random();
                    } else {

                        if (alertObj === null || alertObj === undefined) {
                            alert(model.msg);
                        } else {
                            alertObj.alert('操作提示', model.msg, "error");
                        }
                    }
                }

            }
        } else {

            return this.onUploadSuccess;
        }
    }

};




var opraeteUpload = (function () {
    return {

        createUpload: function (saveUrlId, showId, onUploadSuccess, fromImgUploadBtnId, action, btnId) {
            var obj = new OpeartUpload(saveUrlId, showId, onUploadSuccess, fromImgUploadBtnId, action, btnId);
            obj.createUpload();
        },
        getBrowserType: function () {
             
            
            var userAgent = window.navigator.userAgent.toLowerCase(); //取得浏览器的userAgent字符串  
            var isOpera = userAgent.indexOf("opera") > -1; //判断是否Opera浏览器  
            var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("msie") > -1 && !isOpera; //判断是否IE浏览器  
            var isEdge = userAgent.indexOf("Windows NT 6.1; Trident/7.0;") > -1 && !isIE; //判断是否IE的Edge浏览器  
            var isFF = userAgent.indexOf("firefox") > -1; //判断是否Firefox浏览器  
            var isSafari = userAgent.indexOf("safari") > -1 && userAgent.indexOf("chrome") == -1; //判断是否Safari浏览器  
            var isChrome = userAgent.indexOf("chrome") > -1 && userAgent.indexOf("safari") > -1; //判断Chrome浏览器  

            if (isIE) {
                var reIE = new RegExp("msie (\\d+\\.\\d+);");
                reIE.test(userAgent);
                var fIEVersion = parseFloat(RegExp["$1"]);
                if (fIEVersion == 7)
                { return "IE7"; }
                else if (fIEVersion == 8)
                { return "IE8"; }
                else if (fIEVersion == 9)
                { return "IE9"; }
                else if (fIEVersion == 10)
                { return "IE10"; }
                else if (fIEVersion == 11)
                { return "IE11"; }
                else
                { return "0" }//IE版本过低  
            }//isIE end  

            if (isFF) { return "FF"; }
            if (isOpera) { return "Opera"; }
            if (isSafari) { return "Safari"; }
            if (isChrome) { return "Chrome"; }
            if (isEdge) { return "Edge"; }
        }
    };
})();

