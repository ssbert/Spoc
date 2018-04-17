//
function newvSmartReuqestCommand(serverUrl) {
    var localhostServerUrl = serverUrl;
    //删除用户的考试答案信息
    this.DelUserAnswer = function (examUserUid, examUid, examGradeUid) {
        var urlTemplate = "[{ \"userUid\": \"" + examUserUid + "\", \"filePathType\":\"1\" , \"filePath\": \"UserAnswer/" + examUserUid + "/" + examUid + "/\", \"fileName\": \"" + examGradeUid + ".nav\"}]";
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/COMMANDREQUEST.NEWV",
            data: "MethodName=FileDel&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (data) {
            },
            error: function (data, status, err) {
            }
        });

    };
    this.SetlocalhostServerUrl = function (ServerUrl) {
        localhostServerUrl = ServerUrl;
    };
    //保存考生试卷答案
    /* this.WriteUserAnswer = function (userAnswer, examUid, examUserUid, examGradeUid) {
     var urlTemplate = "[{ \"userUid\": \"" + examUserUid + "\", \"filePathType\":\"1\" , \"filePath\": \"UserAnswer/" + examUserUid + "/" + examUid + "/\", \"fileName\": \"" + examGradeUid + ".nav\", \"key\": \"34678459\",\"content\": \"" + encodeURI(userAnswer) + "\"}]";
     $j("#RequstArgs").val(urlTemplate);
     var turnForm = document.getElementById("formpost");
     turnForm.action = localhostServerUrl + "/COMMANDREQUEST.NEWV";
     turnForm.submit();
     }*/
    this.WriteUserAnswer = function (userUid, examUid, examArrangeUid, examPaperUid, examGradeUid, userAnswer, isBackupToBox) {

        if (isBackupToBox == null) {
            isBackupToBox = true;
        }
        var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"examUid\":\"" + examUid + "\" ,\"examArrangeUid\":\"" + examArrangeUid + "\" ,\"examGradeUid\":\"" + examGradeUid + "\" , \"examPaperUid\":\"" + examPaperUid + "\",\"fileContent\": \"" + encodeURI(userAnswer) + "\",\"backupToPartner\": \"" + "true" + "\",\"backupToBox\": \"" + isBackupToBox + "\"}]";

        $j("#RequstArgs").val(urlTemplate);
        $j("#MethodName").val("CacheExam2");
        var turnForm = document.getElementById("formpost");
        //var ifm = document.getElementById("iframeName");
        turnForm.action = localhostServerUrl + "/EXAMREQUEST.NEWV";
        //turnForm.target = ifm;
        turnForm.submit();

    };


    /*  this.WriteUserAnswer = function (userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,userAnswer) {
     jQuery.support.cors = true;
     var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"examUid\":\""+examUid+"\" ,\"examArrangeUid\":\""+examArrangeUid+"\" ,\"examGradeUid\":\""+examGradeUid+"\" , \"examPaperUid\":\""+examPaperUid+"\",\"fileContent\": \"" + encodeURI(userAnswer) + "\"}]";
     $j.ajax({
     type: "POST",
     url: localhostServerUrl+"/EXAMREQUEST.NEWV",
     data: "MethodName=CacheExam2&RequstArgs=" + urlTemplate,
     jsonp: "callback=?",
     jsonp: 'jsoncallback',
     jsonpCallback: "callback",
     dataType: "jsonp",
     //contentType: "application/json; charset=utf-8",
     success: function (data) {
     },
     error: function (data, status,err) {
     }
     });
     }  */
    this.ReadExamCache2 = function (userUid, examUid, examArrangeUid, examPaperUid, examGradeUid) {
        var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"examUid\":\"" + examUid + "\" ,\"examArrangeUid\":\"" + examArrangeUid + "\" ,\"examGradeUid\":\"" + examGradeUid + "\" , \"examPaperUid\":\"" + examPaperUid + "\",\"recoverFromPartner\": \"" + "true" + "\",\"recoverFromBox\": \"" + "true" + "\"}]";
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/EXAMREQUEST.NEWV",
            data: "MethodName=ReadExamCache2&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (data) {
            },
            error: function (data, status, err) {
            }
        });
    };
    //加密考生答案答案信息
    this.EncryptText = function () {
    };
    //解密考生答案信息
    this.DecryptText = function () {
    };
    this.CloseWindow = function () {
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/EXAMREQUEST.NEWV?MethodName=StopExam",
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (callback) {
            },
            error: function (data, status, err) {
            }
        });
    };
   
    this.WriteUserAnswerToServer = function (userUid, examUid, examArrangeUid, examPaperUid, examGradeUid, successEvent, errorEvent) {
        var domain = GetWebServerUrl() + "/ExamRequestSmartGetCommand.aspx?command=SaveAnswerToServer";
        var reportStatusUrl = GetWebServerUrl() + "/ExamRequestSmartGetCommand.aspx?command=SetExamPause&examGradeUid=" + examGradeUid;
        reportStatusUrl = "";
        var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"directSubmit\": \"" + "true" + "\",\"examUid\":\"" + examUid + "\" ,\"examArrangeUid\":\"" + examArrangeUid + "\" ,\"examGradeUid\":\"" + examGradeUid + "\" , \"examPaperUid\":\"" + examPaperUid + "\", \"domain\":\"" + escape(domain) + "\", \"reportStatusUrl\":\"" + escape(reportStatusUrl) + "\"}]";
        $j.ajax({
            type: 'POST',
            url: localhostServerUrl + "/EXAMREQUEST.NEWV",
            data: "MethodName=SubmitExam2&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (callback) {
                var result = UtilPassRequestResult("json", callback);
                if (result[0].isSuccess != "true") {
                    if (errorEvent) {
                        errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + result[0].errorMsg);
                    }
                }
                else {
                    successEvent(result[0].errorMsg);
                }

            },
            error: function (data, status, err) {

                errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + err);
            }
        });
    };
    this.SubmitExam2 = function (userUid, examUid, examArrangeUid, examPaperUid, examGradeUid, userAnswer, successEvent, errorEvent) {
        var domain = GetWebServerUrl() + "/ExamRequestSmartGetCommand.aspx?command=SubmitPaperByBox"
        var reportStatusUrl = GetWebServerUrl() + "/ExamRequestSmartGetCommand.aspx?command=SetExamPause&examGradeUid=" + examGradeUid;
        var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"examUid\":\"" + examUid + "\" ,\"examArrangeUid\":\"" + examArrangeUid + "\" ,\"examGradeUid\":\"" + examGradeUid + "\" , \"examPaperUid\":\"" + examPaperUid + "\", \"domain\":\"" + escape(domain) + "\", \"reportStatusUrl\":\"" + escape(reportStatusUrl) + "\"}]";
        $j("#RequstArgs").val(urlTemplate);
        $j("#MethodName").val("SubmitExam2");
        $j("#Result").val((userAnswer));

        var turnForm = document.getElementById("formpost");
        var ifm = document.getElementById("iframeName");
        turnForm.action = localhostServerUrl + "/EXAMREQUEST.NEWV";
        turnForm.submit();

    };
    this.StartSmartExam = function () {
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/EXAMREQUEST.NEWV?MethodName=StopExam",
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (callback) {
            },
            error: function (data, status, err) {
            }
        });
    };
    this.ExamOperation = function (examUserUid, examUid, examGradeUid, examArrangeUid, examQuestionUid, examQuestionType, examQuestionContent, domainUrl) {
        var urlTemplate = "[{ \"userUid\": \"" + examUserUid + "\", \"examUid\":\"" + examUid + "\" , \"examGradeUid\":\"" + examGradeUid + "\",\"examArrangeUid\":\"" + examArrangeUid + "\",\"examQuestionUid\": \"" + examQuestionUid + "\",\"examQuestionType\":\"" + examQuestionType + "\",\"examQuestionContent\":\"" + encodeURIComponent(encodeURIComponent(examQuestionContent)) + "\",\"domainUrl\":\"" + escape(domainUrl) + "\"}]";
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/EXAMREQUEST.NEWV",
            data: "MethodName=StartExamQuestion&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (data) {
            },
            error: function (data, status, err) {
            }
        });

    };

    this.LookExamQuestionOperation = function (examUserUid, examUid, examGradeUid, examQuestionUid, examQuestionType, examQuestionContent, domainUrl) {
        var urlTemplate = "[{ \"userUid\": \"" + examUserUid + "\", \"examGradeUid\": \"" + examGradeUid + "\",\"examArrangeUid\": \"" + examUserUid + "\",\"examUid\":\"" + examUid + "\" , \"examQuestionUid\": \"" + examQuestionUid + "\",\"examQuestionType\":\"" + examQuestionType + "\",\"examQuestionContent\":\"" + encodeURIComponent(encodeURIComponent(examQuestionContent)) + "\",\"domainUrl\":\"" + escape(domainUrl) + "\"}]";
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/EXAMREQUEST.NEWV",
            data: "MethodName=LookExamQuestion&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (data) {
            },
            error: function (data) {
            }
        });
    };

    this.Comptime = function comp(beginTime) {

        var d = new Date();
        var dd = d.getMonth() + 1;
        var endTime = d.getFullYear() + "-" + dd + "-" + d.getDate() + "  " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
        var beginTimes = beginTime.substring(0, 10).split('-');
        var endTimes = endTime.substring(0, 10).split('-');
        beginTime = beginTimes[1] + '-' + beginTimes[2] + '-' + beginTimes[0] + ' ' + beginTime.substring(10, 19);
        endTime = endTimes[1] + '-' + endTimes[2] + '-' + endTimes[0] + ' ' + endTime.substring(10, 20);
        var a = (Date.parse(endTime) - Date.parse(beginTime)) / 1000;
        if (a == 0) {
            return true;
        }
        if (a < 120 && a > 0) {
            return true;
        } else {
            return false;
        }
    };
    this.FileRead = function (userUid, filePathType, filePath, fileName, successEvent, errorEvent, dataType) {

        var urlTemplate = "[{ \"userUid\": \"" + userUid + "\", \"filePathType\": \"" + filePathType + "\",\"filePath\": \"" + filePath + "\",\"fileName\":\"" + fileName + "\"}]";
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/COMMANDREQUEST.NEWV",
            data: "MethodName=FileRead&RequstArgs=" + urlTemplate,
            jsonp: "callback=?",
            jsonpCallback: "callback",
            dataType: "jsonp",
            success: function (data) {
                //data = eval(data);
                successEvent(UtilPassRequestResult(dataType, decodeURIComponent(data[0].result[0].content)));

            },
            error: function (data) {
                errorEvent("", data[0].errorMsg);
            }

        });

    };
    this.zipFileRead = function (filePath, successEvent, errorEvent, dataType) {

        jQuery.support.cors = true;
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/newvexampaper/" + filePath,
            data: "",
            //jsonp: "callback=?",
            //jsonpCallback: "callback",
            dataType: "text",
            success: function (data) {

                successEvent(UtilPassRequestResult(dataType, (data)));

            },
            error: function (data, status, err) {
                errorEvent("", data);
            }

        });

    };

    this.zipFileReadExamInfo = function (filePath, successEvent, errorEvent, dataType) {

        jQuery.support.cors = true;
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/newvexampaper/" + filePath,
            data: "",
            jsonp: "callback=?",
            jsonpCallback: "ExamInfoCallBack",
            dataType: "jsonp",
            success: function (data) {

                successEvent(data);

            },
            error: function (data, status, err) {
                errorEvent("", data);
            }

        });

    };

    this.zipFileReadExamArrangeInfo = function (filePath, successEvent, errorEvent, dataType) {

        jQuery.support.cors = true;
        $j.ajax({
            type: "GET",
            url: localhostServerUrl + "/newvexampaper/" + filePath,
            data: "",
            dataType: "jsonp",
            jsonp: "callback=?",
            jsonpCallback: "ExamArrangeCallBack",
            success: function (data) {
                successEvent(data);
            },
            error: function (data, status, err) {
                errorEvent("", data);
            }
        });

    };

    function UtilPassRequestResult(returnDataType, content) {
        if (returnDataType == "JSON") {
            var returnData = content;
            if (typeof returnData !== "string" || !returnData) {
                return null;
            }

            if (/^[\],:{}\s]*$/.test(returnData.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
                    .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
                    .replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) {

                return window.JSON && window.JSON.parse ? window.JSON.parse(returnData) : (new Function("return " + returnData))();
            } else {

                return null;
            }
        }
        else if (returnDataType == "XML") {
            return content;
        }
        else if (returnDataType == "JS") {
            eval(content);
            return "";
        }
        else {
            return content;
        }
    }

    function callback(data) {


    }
}


