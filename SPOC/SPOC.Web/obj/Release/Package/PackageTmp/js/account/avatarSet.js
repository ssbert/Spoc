var avatarSet = (function () {

    function init() {
        var selectArgs = { x1: 0, y1: 0, x2: 0, y2: 0, selectionW: 0, selectionH: 0, defaultImgLen: 274 };
        var defaultImgLen = 0;
        var isSave = false;
        var selectImgW = 0;
        var selectImgH = 0;
        var aElement = null;
        var defaultImgW = 0;
        var defaultImgH = 0;
        var previewLen = 0;
        var submitBtn = null;
       
        this.btnInit= function (btn) {

            submitBtn = btn;
        };
        this.btnSet=function (btn) {

            btn.disabled = true;
            $(btn).html("正在保存中...");
        };
        this.btnRecovery= function (btn) {
            btn.disabled = false;
            $(btn).html("保存");
        };
        this.defalutLenSet = function(defaultLen, previewLength) {
            defaultImgLen = defaultLen;
            previewLen = previewLength;
        };
        function preview(img, selection) {
            if (!selection.width || !selection.height)
                return;

            var scaleX = 100 / selection.width;
            var scaleY = 100 / selection.height;

            $('#preview img').css({
                //  width: Math.round(scaleX * 300),
                //  height: Math.round(scaleY * 300),
                width: Math.round(scaleX * selectImgW),
                height: Math.round(scaleY * selectImgH),
                marginLeft: -Math.round(scaleX * selection.x1),
                marginTop: -Math.round(scaleY * selection.y1)
            });

            selectArgs.x1 = selection.x1;
            selectArgs.y1 = selection.y1;
            selectArgs.x2 = selection.x2;
            selectArgs.y2 = selection.y2;
            selectArgs.selectionW = selection.width;
            selectArgs.selectionH = selection.height;
        };
        this.uploadPictureBtnClick = function() {
            if (isSave) {
                layer.closeAll('loading');
                this.btnSet(submitBtn);
                this.Save();
            } else {
                this.ImgSelect();
            }
        };
        this.ImgSelect = ImgSelect;
        function ImgSelect() {
            document.getElementById('AvatarFileUpload').click();
        };
        this.SetArgs = function(key, value) {
            selectArgs[key] = value;
        };
        this.show = function(data) {
            $(".panel-heading").html("头像设置");
            $("#AcatarImg").attr("src", data.path);
            AvatarImgSet();

            $(".col-md-2 b").hide();
            $("#AcatarImg").hide();
            $("#divPic").hide();
            $(".help-block").hide();
            $("#photo").attr("src", data.path);
            $("#preview img").attr("src", data.path);
            $("#imgTailor_div").show();

            defaultImgW = data.imgW;
            defaultImgH = data.imgH;

            imgSet();

            $('#photo').imgAreaSelect({
                x1: 0,
                y1: 0,
                x2: (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8,
                y2: (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8,
                aspectRatio: '1:1',
                handles: true,
                fadeSpeed: 200,
                onSelectChange: preview,
                persistent: true
            });

            var scaleX = previewLen / ((selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8);
            var scaleY = previewLen / ((selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8);
            $('#preview img').css({
                width: Math.round(scaleX * selectImgW),
                height: Math.round(scaleY * selectImgH),
                marginLeft: 0,
                marginTop: 0
            });


            selectArgs.x1 = 0;
            selectArgs.y1 = 0;
            selectArgs.x2 = (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8;
            selectArgs.y2 = (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8;
            selectArgs.selectionW = (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8;
            selectArgs.selectionH = (selectImgW >= selectImgH ? selectImgH : selectImgW) * 0.8;

            AddResrtButton();
        };
        this.AvatarFileUploadChange = function() {
            var set = this.SetArgs;
            var dataShow = this.show;
            $('#AvatarUpload_form').ajaxSubmit({
                dataType: "json",
                success: function(data, status) {

                    if (data.statu === "ok") {

                        set("AcatarImg", data.path);
                        dataShow(data);
                    } else {

                        layer.alert(data.msg,
                            {
                                icon: 5,
                                skin: 'layer-ext-moon' 
                            });
                    }
                }
            });
        };
        this.CreatElement = function(tagName, type, name, id, value) {
            var element = document.createElement(tagName);
            if (type !== "" && type != null && type != undefined) {
                element.type = type;
            }
            element.name = name;
            element.value = value;
            element.id = id;
            return element;
        };
        this.Save = function () {
            var args = selectArgs;
            for (var key in args) {

                $("#settings-avatar-form")
                    .append(this.CreatElement("input", "hidden", key.toString(), key.toString(), args[key]));
            }
            $("#settings-avatar-form").submit();
        };
         function AddResrtButton() {
            isSave = true;
            $("#upload-picture-btn").html("保存");

            if (aElement == null) {
                aElement = document.createElement("a");
                aElement.href = "javascript:void(0)";
                aElement.onclick = ImgSelect;
                aElement.innerHTML = "重新选择图片";
                $("#AvatarNewUpload_div").append(aElement);
            }
        };
        this.imgSet = imgSet;
        function imgSet(){
            var scale;
            var w = defaultImgW;
            var h = defaultImgH;
            if (w < defaultImgLen) {
                if (h > w) {
                    scale = w / defaultImgLen;
                    selectImgW = defaultImgLen;
                    selectImgH = Math.round(h / scale);
                } else {
                    scale = h / defaultImgLen;
                    selectImgH = defaultImgLen;
                    selectImgW = Math.round(w / scale);
                }
                $("#photo").css("width", selectImgW + "px");
                $("#photo").css("height", selectImgH + "px");
            } else if (w > defaultImgLen && h > defaultImgLen) {
                if (h > w) {
                    scale = w / defaultImgLen;
                    selectImgW = defaultImgLen;
                    selectImgH = Math.round(h / scale);
                } else {
                    scale = h / defaultImgLen;
                    selectImgW = Math.round(w / scale);
                    selectImgH = defaultImgLen;
                }
                $("#photo").css("height", selectImgH + "px");
                $("#photo").css("width", selectImgW + "px");
            } else {
                if (h > w) {
                    scale = w / defaultImgLen;
                    selectImgH = Math.round(h / scale);
                    selectImgW = defaultImgLen;
                } else {
                    scale = h / defaultImgLen;
                    selectImgW = Math.round(w / scale);
                    selectImgH = defaultImgLen;
                }
                $("#photo").css("height", selectImgH + "px");
                $("#photo").css("width", selectImgW + "px");
            }
            $("#selectImg_div").css("width", (selectImgW + 40) + "px");

            $('#photo').parent('.frame').css("width", selectImgW + "px");
            $('#photo').parent('.frame').css("height", selectImgH + "px");
        };

        this.AvatarImgSet = AvatarImgSet;

        function AvatarImgSet() {
            var scale;
            var w = $("#AcatarImg").css("width").replace("px", "");
            var h = $("#AcatarImg").css("height").replace("px", "");
            if (w < 200) {
                scale = w / 200;
                $("#AcatarImg").css("width", "200px");
                $("#AcatarImg").css("height", Math.round(h / scale) + "px");
            }};
    };

    return init;

})();