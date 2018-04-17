var ufoBusinessLogic = (function () {
    var bl = {
        coursePicture:function(option) {
            if (!option.success) {
                option.callback({ success: false });
                return;
            }
            createUploadFile(option.data, function (fileId) {
                if (!fileId) {
                    option.callback({ success: false });
                }
                option.data.fileId = fileId;
                updateCoursePicture(option);
            });
        },
        courseVideo: function (option) {
            if (!option.success) {
                option.callback({ success: false });
                return;
            }
            createUploadFile(option.data, function (fileId) {
                if (!fileId) {
                    option.callback({ success: false });
                }
                option.data.fileId = fileId;
                updateCourseVideo(option);
            });
        },
        uploadFile: function(option) {
            if (!option.success) {
                option.callback({ success: false });
                return;
            }
            createUploadFile(option.data, function (fileId) {
                if (!fileId) {
                    option.callback({ success: false });
                }
                option.data.fileId = fileId;
                option.callback({ success: true, data: option.data });
            });
        },
        coursewareFile: function(option) {
            if (!option.success) {
                option.callback({ success: false });
                return;
            }
            createUploadFile(option.data, function (fileId) {
                if (!fileId) {
                    option.callback({ success: false });
                }
                option.data.fileId = fileId;
                coursewareFile(option);
            });
        }
    };

    
    //创建文件信息记录
    function createUploadFile(fileData, callback) {
        var param = {
            hash: fileData.hash,
            size: fileData.size,
            isOnlineType: fileData.isOnlineType,
            status: fileData.status,
            name: fileData.name,
            url: fileData.url
        };
        var url = "/api/services/app/Upload/Create";
        nv.post(url, param, function (data) {
            if (data.success) {
                evtBus.dispatchEvt("refresh_pending_file_count");
                callback(data.result.id);
            } else {
                callback();
            }
        });
    }
    
    //更新课程图片
    function updateCoursePicture(option) {
        var url = "/api/services/app/Course/UpdateCoursePicture";
        nv.post(url, { courseId: option.courseId, fileId: option.data.fileId }, function (data) {
            if (data.success) {
                checkData(data, option);
            } else {
                option.callback({ success: false });
            }
        });
    }

    //更新课程视频预览
    function updateCourseVideo(option) {
        var url = "/api/services/app/Course/UpdateCourseVideo";
        nv.post(url, { courseId: option.courseId, fileId: option.data.fileId }, function (data) {
            if (data.success) {
                checkData(data, option);
            } else {
                option.callback({ success: false });
            }
        });
    }

    function coursewareFile(option) {
        var url = "/api/services/app/Courseware/UpdateFile";
        var fileTypeDic = {
            cover: 0,
            video: 1,
            material: 2
        };
        nv.post(url, { coursewareId: option.coursewareId, fileId: option.data.fileId, type: fileTypeDic[option.fileType] }, function (data) {
            if (data.success) {
                checkData(data, option);
            } else {
                option.callback({ success: false });
            }
        });
    }

    function checkData(data, option) {
        if (data.success) {
            option.callback({ success: true, data: option.data });
        } else {
            option.callback({ success: false });
        }
    }

    return bl;
})();