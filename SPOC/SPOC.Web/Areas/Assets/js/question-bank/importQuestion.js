var ImportQuestionClass = (function() {
    //questionType 区分题库或挑战
    function init(fileType, successCallback,questionType) {
        if (fileType !== "excel" && fileType !== "word") {
            return;
        }

        function initUploader(param) {
            var pluploadPath = "~/Assets/lib/plupload-2.1.2/";
            var uploader = new plupload.Uploader({
                runtimes: "html5,flash,silverlight,html4",
                browse_button: param.buttonId,
                url: param.url,
                flash_swf_url: pluploadPath + "Moxie.swf",
                silverlight_xap_url: pluploadPath + "Moxie.xap",
                multi_selection: false,
                filters: {
                    max_file_size: "10mb",
                    mime_types: [param.mimeType]
                }
            });

            uploader.init();
            uploader.bind("FilesAdded", onFilesAdded);
            uploader.bind("Error", onError);
            uploader.bind("FileUploaded", onFileUploaded);
        }

        function onFilesAdded(up, files) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                up.removeFile(files[0]);
                return;
            }

            if (!$("#import-form").form("validate")) {
                up.removeFile(files[0]);
                $.messager.alert("提示", "表单有误！", "info");
                return;
            }
            if (files.length > 0) {
                var url = "/QuestionBank/Manage/CreateFromFile?folderUid={0}";
                if (questionType && questionType==="challenge")
                    url = "/Challenge/Manage/CreateFromFile?folderUid={0}";
                url = url.format($("#wd-folderUid").combotree("getValue"));

                up.setOption("url", url);
                up.start();
                VE.Mask("正在导入");
                $("#wd").window("close");
            }
        }

        function onError(up, err) {
            //$.messager.alert("错误", err.message, "error");

            VE.UnMask();
            up.stop();
        }

        function onFileUploaded(up, file, res) {
            VE.UnMask();
            up.removeFile(file);
            var data = $.parseJSON(res.response);
            if (data.Success) {
                var successCount = data.Result.successCount;
                if (successCount > 0) {
                    if (stringIsEmpty(data.Result.errMessage)) {
                        $.messager.show({ title: "提示", msg: "导入成功，一共导入" + successCount + "道试题！"  });
                    } else {
                        showError("部分导入成功", "<p><h3>部分导入成功。</h3></p><br/>" + data.Result.errMessage.replace(/\n/g, "<br>"));
                    }
                    if (successCallback) {
                        successCallback();
                    }
                } else {
                    if (!stringIsEmpty(data.Result.errMessage)) {
                        showError("没有导入数据", "<p><h3>没有导入数据！</h3></p><br/>" + data.Result.errMessage.replace(/\n/g, "<br>"));
                    }
                }

            } else {
                $.messager.alert("提示", data.Error.Message, "info");
            }
        }

        function showError(title, html) {
            parent.$("#tabs")
                .tabs("add",
                {
                    title: title,
                    content: html,
                    closable: true,
                    icon: "icon-2012080404218"
                });
        }

        var url = "/QuestionBank/Manage/CreateFromFile";
        if (questionType && questionType === "challenge")
            url = "/Challenge/Manage/CreateFromFile";
        if (fileType === "excel") {
            initUploader({
                buttonId: "import-from-excel",
                url: url,
                mimeType: { title: "Office Excel", extensions: "xls,xlsx" }
            });
        } else if (fileType === "word") {
            initUploader({
                buttonId: "import-from-word",
                url: url,
                mimeType: { title: "Office Word", extensions: "doc,docx" }
            });
        }
    }

    return init;
})();