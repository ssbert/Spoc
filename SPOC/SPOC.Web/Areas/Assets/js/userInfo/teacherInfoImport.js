var ImportQuestionClass = (function () {

    function init(fileType, successCallback) {
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
                
                return;
            }
          /*  if ($('#perList_div').form('validate') == false) {
                return;
            }*/
            if (files.length > 0) {

                var url = "/User/Teacher/CreateTeacherFromFile"; //?departmentUid={0}
                //url = url.format(  $("#wd-departmentUid").combotree("getValue"));
                up.setOption("url", url);
                up.start();
                VE.Mask("正在导入");
                $("#teacherInfoImportDiv").window("close");
            }
        }

        function onError(up, err) {
            $.messager.alert("错误", err.message, "error");
            VE.UnMask();
            up.stop();
        }

        function onFileUploaded(up, file, res) {
          
            VE.UnMask();
            up.removeFile(file);
            var data = $.parseJSON(res.response);
            if (data.Success) {
                if (data.Result.successCount > 0) {
                    if (stringIsEmpty(data.Result.errMessage)) {
                        $.messager.show({ title: "提示", msg: " 成功导入" + data.Result.successCount + "条数据!" });
                        if (successCallback) {
                            successCallback();
                        }
                    } else {
                        //$.messager.alert("提示", "部分导入成功。\n" + data.result.errMessage, "info");
                        showError("部分导入成功", "<p><br/><h3>&nbsp;部分导入成功，共导入" + data.Result.successCount + "条数据!。</h3></p><br/>&nbsp;" + data.Result.errMessage.replace(/\n/g, "<br>"));
                    }
                    
                } else {
                    if (!stringIsEmpty(data.Result.errMessage)) {
                        //$.messager.alert("提示", "没有导入数据！\n" + data.result.errMessage, "info");
                        showError("没有导入数据", "<p><br/><h3>&nbsp;没有导入数据！</h3></p><br/>&nbsp;" + data.Result.errMessage.replace(/\n/g, "<br>"));
                    }
                }

            } else {
                $.messager.alert("提示", data.Error, "info");
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

        var url = "/User/Teacher/CreateTeacherFromFile";
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