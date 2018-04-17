window.SmartUploader = (function () {
    var log = WebUploader.Base.log;
    var uploaderDic = {};
    var callbackDataDic = {};
    var optionDic = {};
    var SUPPORT_EXT = {
        video: ["mpeg", "mpg", "mpe", "m2v", "qt", "mov", "mp4", "m4v", "flv", "wmv", "avi", "webm", "3gp", "3gpp", "3g2", "rv", "ogv", "mkv", "asf", "rm", "rmvb", "vob"],
        office: ["doc", "docx", "xls", "xlsx", "ppt", "pptx"],
        image: ["png", "jpg", "jpeg", "gif"],
        flash: "swf",
        zip: "zip"
    };
    var HTML_TEMP = '<li id="{id}" class="list-group-item">\
            <p class="text-info ellipsis" title="{fileName}">{fileName}</p>\
            <div class="progress">\
                <div class="progress-bar" role="progressbar" aria-valuenow="{progress}" aria-valuemin="0" aria-valuemax="100" title="{progress}%" style="width: {progress}%;">\
                   {progress}%\
                </div>\
            </div>\
        </li>';
    var HASH_LENGTH = 1024;
    var MIN_LENGTH = 5120;

    var smartuploader = {
        //文件服务器
        FILE_SERVER: "",
        //请求签名地址
        SIGN_SERVER: "",
        //外挂业务逻辑模块，上传完后根据需求处理实际的业务
        BLModule: null,
        //获取任务数
        getTaskNum: function () {
            var count = 0;
            $.each(uploaderDic, function () {
                count++;
            });
            return count;
        },
        //上传文件
        upload: function (option) {
            var guid = WebUploader.Base.guid();
            $.when(getFileHash(option.file)).then(function (hash) {
                option.param.hash = hash;
                optionDic[guid] = option;
                createWebUploader(guid, option.file, option.param);
            });
            return guid;
        },
        //暂停上传文件
        stop: function (guid) {
            var up = uploaderDic[guid];
            if (up) {
                up.stop();
            }
        },
        //重新开始上传文件
        retry: function(guid) {
            var up = uploaderDic[guid];
            if (up) {
                up.retry();
            }
        },
        //取消上传文件
        cancel: function(guid) {
            var up = uploaderDic[guid];
            if (up) {
                up.cancelFile();
                clear(guid);
            }
        }
    };

    WebUploader.Uploader.register({
        "before-send": "blockPreupload",
        "before-send-file": "preupload"
    }, {
        blockPreupload: function (block) {//分片上传前
            var owner = this.owner;
            var deferred = WebUploader.Deferred();
            var param = owner.option("formData");

            var url = smartuploader.FILE_SERVER + "api/file/check/" + param.hash + "/" + block.file.size + "/" + block.chunk;
            //与服务安验证
            $.get(url, function (data) {
                // 如果验证已经上传过
                if (data.code === 0) {
                    if (data.content.exists) {
                        deferred.reject();
                    } else {
                        //获取签名
                        $.ajax({
                            type: "POST",
                            url: smartuploader.SIGN_SERVER,
                            dataType: "json",
                            data: param,
                            success: function (ret) {
                                if (ret.Success) {
                                    param.sign = ret.Result.sign;
                                    param.timestamp = ret.Result.timestamp;
                                    owner.option("formData", param);
                                    deferred.resolve();
                                } else {
                                    log("生成签名出错");
                                    log(ret);
                                    error(owner);
                                }
                            }
                        });

                    }
                } else {
                    log("验证chunk出错");
                    log(url);
                    log(data);
                    error(owner);
                }

            }, "json");

            return deferred.promise();
        },
        preupload: function (file) {//文件上传前
            var owner = this.owner;
            var deferred = WebUploader.Deferred();
            var param = owner.option("formData");
            // md5值计算完成
            var url = smartuploader.FILE_SERVER + "api/file/check/" + param.hash + "/" + file.size + "?isOnlineType=" + param.isOnlineType;
            //与服务安验证
            $.get(url, function (data) {

                // 如果验证已经上传过
                if (data.code === 0) {
                    if (data.content.exists) {
                        var cbData = {};
                        cbData.hash = data.content.hash;
                        cbData.size = file.size;
                        cbData.isOnlineType = param.isOnlineType;
                        cbData.name = file.name;
                        cbData.ext = file.ext.toLowerCase();
                        cbData.url = getFileUrl(cbData.hash, cbData.size, cbData.ext, cbData.isOnlineType);
                        cbData.status = getFileStatus(cbData.ext, cbData.isOnlineType);
                        cbData.mime = file.type;
                        callbackDataDic[owner.guid] = cbData;
                        //跳过该文件
                        owner.skipFile(file);
                    }
                    deferred.resolve();
                } else {
                    log("验证文件出错");
                    log(url);
                    log(data);
                    deferred.reject();
                    error(this);
                }

            }, "json");

            return deferred.promise();
        }
    });

    var handle = {
        fileQueued: function (file) {
            var uploader = this;
            var param = uploader.option("formData");
            param.ext = file.ext;
            uploader.option("formData", param);
            uploader.upload();
        },
        uploadProgress: function(file, percentage) {
            var progress = Math.floor(percentage * 100);
            changeItemProgress(this.guid, progress);
        },
        uploadSuccess: function(file, ret) {
            //ret 为空时是验证了已存在文件跳过了上传，不为空时是真实服务器反馈数据
            if (ret && ret.code !== 0) {
                log("上传失败！");
                log(ret);
                error(this);
                return;
            }

            var cbData = {};
            if (ret) {
                cbData.hash = ret.content.hash;
                cbData.size = ret.content.size;
                cbData.isOnlineType = ret.content.isOnlineType;
                cbData.name = file.name;
                cbData.ext = file.ext.toLowerCase();
                cbData.url = getFileUrl(cbData.hash, cbData.size, cbData.ext, cbData.isOnlineType);
                cbData.status = getFileStatus(cbData.ext, cbData.isOnlineType);
                cbData.mime = file.type;
            } else {
                cbData = callbackDataDic[this.guid];
            }
            changeItemProgress(this.guid, 100);
            var guid = this.guid;
            try {
                var option = optionDic[guid];
                if (smartuploader.BLModule) {
                    option.success = true;
                    option.data = cbData;
                    smartuploader.BLModule[option.type](option);
                } else {
                    option.callback({ success: true, data: cbData });
                }
            } finally {
                clear(guid);
            } 
        },
        uploadError: function (file, ret) {
            log("上传失败！");
            log(ret);
            error(this);
        }
    };

    //创建Uploader
    function createWebUploader(guid, file, param) {
        if (!param) {
            param = {};
        }
        var uploader = WebUploader.create({
            swf: "lib/webuploader/Uploader.swf",
            server: smartuploader.FILE_SERVER + "api/file/upload",
            formData: param,
            timeout: 0,
            chunked: true, //开启分块上传
            fileNumLimit: 1, //只能上传1个文件
            compress: false //重要：上传的图片不压缩
        });
        uploader.guid = guid;
        uploaderDic[uploader.guid] = uploader;
        
        uploader.on("fileQueued", handle.fileQueued);
        uploader.on("uploadProgress", handle.uploadProgress);
        uploader.on("uploadSuccess", handle.uploadSuccess);
        uploader.on("uploadError", handle.uploadError);
        
        var uploaderFile = new WebUploader.File(new WebUploader.Lib.File(WebUploader.Base.guid(), file));
        addItem(uploader.guid, uploaderFile.name);
        uploader.addFiles(uploaderFile);
    }

    //获取文件链接地址
    function getFileUrl(hash, size, ext, isOnlineType) {
        ext = ext.toLowerCase();
        var fileName = hash + "_" + size;
        if (isOnlineType) {
            if (SUPPORT_EXT.video.indexOf(ext) !== -1) {
                fileName += "/" + fileName + ".m3u8";
            } else if (SUPPORT_EXT.office.indexOf(ext) !== -1) {
                if (ext === "doc" || ext === "docx" || ext === "ppt" || ext === "pptx") {
                    fileName += ".pdf";
                }
                else if (ext === "xls" || ext === "xlsx") {
                    fileName += ".html";
                }
            } else if (ext === SUPPORT_EXT.zip) {
                fileName += "/";
            } else {
                fileName += "." + ext;
            }
        } else {
            fileName += "." + ext;
        }

        var dir1 = hash.substr(28, 2);
        var dir2 = hash.substr(30, 2);
        var url = "/files/root/" + dir1 + "/" + dir2 + "/" + fileName;
        return url;
    }
    
    //获取文件状态
    function getFileStatus(ext, isOnlineType) {
        if (isOnlineType && (SUPPORT_EXT.video.indexOf(ext) !== -1 || SUPPORT_EXT.office.indexOf(ext) !== -1)) {
            return 0;
        } else {
            return 2;
        }
    }

    //错误处理
    function error(uploder) {
        var guid = uploder.guid;
        try {
            var option = optionDic[guid];
            if (smartuploader.BLModule) {
                option.success = false;
                smartuploader.BLModule[option.type](option);
            } else {
                option.callback({ success: false });
            }
        } finally {
            clear(guid);
        }
    }

    //清理工作
    function clear(guid) {
        if (callbackDataDic[guid]) {
            delete callbackDataDic[guid];
        }

        if (optionDic[guid]) {
            delete optionDic[guid];
        }

        if (uploaderDic[guid]) {
            //uploaderDic[guid].destroy();
            delete uploaderDic[guid];
        }

        removeItem(guid);
    }

    //向html中添加一个上传展示项
    function addItem(id, fileName) {
        var html = HTML_TEMP.format({ id: id, fileName: fileName, progress: 0 });
        $("#uploader-list").append(html);
    }

    //从html中移除一个上传展示项
    function removeItem(id) {
        $("#" + id).remove();
    }

    //修改展示项的进度
    function changeItemProgress(id, progress) {
        var $div = $("#" + id + " .progress-bar");
        $div.attr("aria-valuenow", progress);
        $div.attr("title", progress + "%");
        $div.css({ "width": progress + "%" });
        $div.text(progress + "%");
    }

    //获取文件hash
    function getFileHash(file) {
        var dtd = $.Deferred();
        var blobSlice = File.prototype.slice || File.prototype.mozSlice || File.prototype.webkitSlice;
        var spark = new SparkMD5.ArrayBuffer();
        var blobs = [];
        if (file.size < MIN_LENGTH) {
            blobs.push(blobSlice.call(file, 0, file.size - 1));
        } else {
            blobs.push(blobSlice.call(file, 0, HASH_LENGTH));
            var offset = Math.floor(file.size / 2) - 512;
            blobs.push(blobSlice.call(file, offset, offset + HASH_LENGTH));
            offset = file.size - HASH_LENGTH - 1;
            blobs.push(blobSlice.call(file, offset, offset + HASH_LENGTH));
        }
        var current = 0;
        var fileReader = new FileReader();
        fileReader.onload = function(e) {
            spark.append(e.target.result);
            current++;
            if (current < blobs.length) {
                loadNext();
            } else {
                var hash = spark.end();
                dtd.resolve(hash);
            }
        };

        fileReader.onError = function() {
            log("fileReader 出现了错误！");
            dtd.reject();
        };

        function loadNext() {
            fileReader.readAsArrayBuffer(blobs[current]);
        }

        loadNext();

        return dtd.promise();
    }

    return smartuploader;
})();