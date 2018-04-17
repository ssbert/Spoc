
var flagSrc = "css/image/examattend/flag.gif";
var noflagSrc = "css/image/examattend/flag2.gif";

//定义命名空间 newv.exam
var newv = newv ? newv : {};
var $j = jQuery.noConflict();
newv.exam = function () {
    this.Environment = new newv.exam.environment();
    this.Request = new newv.exam.request();
    this.MessageBoxManager = new newv.exam.messageBoxManager();
    this.Timer = new newv.exam.timer();
    this.paperViewUtil = new newv.exam.paperViewUtil();
};

newv.exam.prototype.Init = function () {
    window.$ = function (elementId) {
        return document.getElementById(elementId);
    };

    window.$name = function (elementName) {
        return document.getElementsByName(elementName);
    };

    /**
     * @return {boolean}
     */
    window.IsNullOrEmpty = function (stringValue) {
        return (stringValue == null || typeof (stringValue) == "undefined" || stringValue.length == 0);
    };

    return new newv.exam();
};

//与环境有关的内容
newv.exam.environment = function () {
    //浏览器类型
    this.Browser = newv.exam.environment.prototype.getBrowser();

    this.getBrowerWindowsSize = function () {
        return newv.exam.environment.prototype.getBrowerWindowsSize();
    };
    this.getElementSize = function () {
        return newv.exam.environment.prototype.getElementSize();
    };
    this.getScreenSize = function () {
        return newv.exam.environment.prototype.getScreenSize();
    };
    this.addEventListener = function (target, eventType, fnHandler) {
        return newv.exam.environment.prototype.addEventListener(target, eventType, fnHandler);
    };
    this.getEvent = function () {
        return newv.exam.environment.prototype.getEvent();
    };

    //获取浏览器Get请求的参数
    this.getRequstParam = function (key) {
        var url = document.location.href;
        var endIndex = url.indexOf("#");
        var arrStr = null;
        if (endIndex > 0)
            arrStr = url.substring(url.indexOf("?") + 1, endIndex).split("&");
        else
            arrStr = url.substring(url.indexOf("?") + 1).split("&");

        for (var i = 0; i < arrStr.length; i++) {
            var loc = arrStr[i].indexOf(key + "=");
            if (loc != -1) {
                return arrStr[i].replace(key + "=", "").replace("?", "");
            }
        }
        return "";
    };

    this.formatString = function (str, arguments) {
        var formated = str;
        for (var i = 0; i < arguments.length; i++) {
            var param = "\{" + i + "\}";
            formated = formated.replace(param, arguments[i]);
        }
        return formated;
    }
};

//浏览器类型
newv.exam.environment.prototype.getBrowser = function () {
    function browser() {
        this.ie = false;
        this.firefox = false;
        this.chrome = false;
        this.opera = false;
        this.safari = false;
    }

    var b = new browser();
    var userAgent = navigator.userAgent.toLowerCase();
    if (userAgent.match(/msie ([\d.]+)/))
        b.ie = true;
    else if (userAgent.match(/firefox\/([\d.]+)/))
        b.firefox = true;
    else if (userAgent.match(/chrome\/([\d.]+)/))
        b.chrome = true;
    else if (userAgent.match(/opera.([\d.]+)/))
        b.opera = true;
    else if (userAgent.match(/version\/([\d.]+).*safari/))
        b.safari = true;

    return b;
};

newv.exam.environment.prototype.getBrowerWindowsSize = function () {
    var de = document.documentElement;
    return {
        'width': (self.innerWidth || (de && de.offsetWidth) || document.body.offsetWidth),
        'height': (self.innerHeight || (de && de.offsetHeight) || document.body.offsetHeight)
    }
};

newv.exam.environment.prototype.getElementSize = function (element) {
    return {
        'width': (element.offsetWidth),
        'height': (element.offsetHeight)
    }
};

newv.exam.environment.prototype.getScreenSize = function () {
    return {
        'width': (screen.availWidth),
        'height': (screen.availHeight)
    }
};

newv.exam.environment.prototype.addEventListener = function (target, eventType, fnHandler) {
    if (target.addEventListener)//for dom
    {
        target.addEventListener(eventType, fnHandler, false)
    }
    else if (target.attachEvent)//for ie
    {
        target.attachEvent("on" + eventType, fnHandler);
    }
};

newv.exam.environment.prototype.getEvent = function () {
    if (window.event) {
        return window.event;
    }
    else {
        return this.getEvent.caller.arguments[0];
    }
};

//定义处理Http请求的类
newv.exam.request = function () {
    //创建XmlHttpRequest对象
    this.createRequest = function () {
        var xmlHttp;
        if (window.XMLHttpRequest) {
            xmlHttp = new XMLHttpRequest();
            if (xmlHttp.overrideMimeType) {
                xmlHttp.overrideMimeType("text/xml");
            }
        }
        else if (window.ActiveXObject) {
            try {
                xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
            }
            catch (e) {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
        }
        return xmlHttp;
    };

    this.ajax = function (url, type, data, returnDataType, requestHeader, allowCache, successEvent, errorEvent) {
        //处理第的数据
        type = type.toUpperCase();
        if (!returnDataType)
            returnDataType = "TEXT";
        else
            returnDataType = returnDataType.toUpperCase();

        if (type == "GET") {
            if (url.indexOf("?") > 0 && (data != null && data.length > 0))
                url += "&" + data;
            else
                url += "?" + data;

            data = "";
        }

        if (allowCache == null) {
            allowCache = true;
        }
        if (allowCache == false) {
            var nowDate = new Date();
            var timePara = "reqTime=" + nowDate.getHours() + nowDate.getMinutes() + nowDate.getSeconds() + nowDate.getMilliseconds();
            if (url.indexOf("?") > 0) {
                url = url + "&" + timePara;
            }
            else {
                url = url + "?" + timePara;
            }
        }

        //开始发起请求
        var xmlHttp = this.createRequest();
        if (!successEvent)
            xmlHttp.open(type, url, false);
        else
            xmlHttp.open(type, url, true);
        if (type == "POST") {
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        }
        if (requestHeader != null) {
            for (var i = 0; i < requestHeader.length; i++) {
                xmlHttp.setRequestHeader(requestHeader[i].key, requestHeader[i].value);
            }
        }
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4) //请求状态为4表示成功
            {
                if (xmlHttp.status == 200) //http状态200表示OK
                {
                    if (successEvent) {
                        var result = UtilPassRequestResult(returnDataType, xmlHttp);
                        if (result == null) {
                            if (errorEvent) {
                                errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + result);
                            }
                        }
                        else {
                            successEvent(result);
                        }
                    }
                }
                else //http返回状态失败
                {
                    if (errorEvent)
                        errorEvent(xmlHttp.status, xmlHttp.statusText);
                }
            }
        };
        xmlHttp.send(data);

        //处理返回结果
        if (!successEvent) {
            var result = UtilPassRequestResult(returnDataType, xmlHttp);
            if (result == null) {
                if (errorEvent) {
                    errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + result);
                }
            }

            return result;
        }
        else {
            return null;
        }
    };

    this.requestUrl = function (url, type, data, returnDataType, requestHeader, allowCache, successEvent, errorEvent) {
        //处理第的数据
        type = type.toUpperCase();
        if (!returnDataType)
            returnDataType = "TEXT";
        else
            returnDataType = returnDataType.toUpperCase();

        if (type == "GET") {
            if (url.indexOf("?") > 0 && (data != null && data.length > 0))
                url += "&" + data;
            else
                url += "?" + data;

            data = "";
        }

        if (allowCache == null) {
            allowCache = true;
        }
        if (allowCache == false) {
            var nowDate = new Date();
            var timePara = "reqTime=" + nowDate.getHours() + nowDate.getMinutes() + nowDate.getSeconds() + nowDate.getMilliseconds();
            if (url.indexOf("?") > 0) {
                url = url + "&" + timePara;
            }
            else {
                url = url + "?" + timePara;
            }
        }

        //开始发起请求
        var xmlHttp = this.createRequest();
        if (!successEvent)
            xmlHttp.open(type, url, false);
        else
            xmlHttp.open(type, url, true);
        if (type == "POST") {
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        }
        if (requestHeader != null) {
            for (var i = 0; i < requestHeader.length; i++) {
                xmlHttp.setRequestHeader(requestHeader[i].key, requestHeader[i].value);
            }
        }
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4) //请求状态为4表示成功
            {
                if (xmlHttp.status == 200) //http状态200表示OK
                {
                    if (successEvent) {
                        var result = UtilPassRequestResult(returnDataType, xmlHttp);
                        if (result == null) {
                            if (errorEvent) {
                                errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + result);
                            }
                        }
                        else {
                            successEvent(result);
                        }
                    }
                }
                else //http返回状态失败
                {
                    if (errorEvent)
                        errorEvent(xmlHttp.status, xmlHttp.statusText);
                }
            }
        };
        xmlHttp.send(data);

        //处理返回结果
        if (!successEvent) {
            var result = UtilPassRequestResult(returnDataType, xmlHttp);
            if (result == null) {
                if (errorEvent) {
                    errorEvent("-1", Translate("Examining7", "", "解析数据出错：") + result);
                }
            }

            return result;
        }
        else {
            return null;
        }
    };

    /**
     * @return {string}
     */
    function UtilPassRequestResult(returnDataType, xmlHttp) {
        if (returnDataType == "JSON") {
            var returnData = xmlHttp.responseText;
            if (typeof returnData !== "string" || !returnData) {
                return "";
            }
            if (/^[\],:{}\s]*$/.test(returnData.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
                .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
                .replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) {

                return window.JSON && window.JSON.parse ? window.JSON.parse(returnData) : (new Function("return " + returnData))();
            }
            return "";
        }
        else if (returnDataType == "XML") {
            return xmlHttp.responseXML;
        }
        else if (returnDataType == "JS") {
            eval(xmlHttp.responseText);
            return "";
        }
        else {
            return xmlHttp.responseText;
        }
    }

    /*
    动态加载资源文件
    url：请求文件的Url地址
    fileType:文件类型,js：Javascript脚本，vbs:VBScript脚本，css:样式表文件
    allowCache：是否允许缓存
    seccessEvent:加载文件成功的事件
    */
    this.loadResource = function (url, fileType, allowCache, successEvent) {
        if (allowCache == null) {
            allowCache = true;
        }
        if (allowCache == false) {
            var nowDate = new Date();
            var timePara = "reqTime=" + nowDate.getHours() + nowDate.getMinutes() + nowDate.getSeconds() + nowDate.getMilliseconds();
            if (url.indexOf("?") > 0) {
                url = url + "&" + timePara;
            }
            else {
                url = url + "?" + timePara;
            }
        }
        var head = document.getElementsByTagName("head")[0] || document.documentElement;
        var objElement;
        if (fileType == "js" || fileType == "vbs") {
            objElement = document.createElement("script");
            objElement.src = url;
            if (fileType == "js") {
                objElement.type = "text/javascript";
                objElement.language = "javascript";
            }
            else {
                objElement.type = "text/vbscript";
                objElement.language = "vbscript";
            }
        }
        else if (fileType == "css") {
            objElement = document.createElement("link");
            objElement.rel = "stylesheet";
            objElement.type = "text/css";
            objElement.href = url;
        }

        objElement.onload = objElement.onreadystatechange = function () {
            if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
                objElement.onload = objElement.onreadystatechange = null;
                if (successEvent) {
                    successEvent(objElement);
                }
            }
        }

        head.insertBefore(objElement, head.firstChild);
    }
};

//对话框内容
newv.exam.messageBoxManager = function () {
    this.messageBoxList = [];
    this.maxzindex = 1000;

    this.create = function (title, content, isShowCloseButton, isLock, buttonArray) {
        var messageBox = new newv.exam.messageBox();
        messageBox.title = title;
        messageBox.content = content;
        messageBox.isShowCloseButton = isShowCloseButton;
        messageBox.isLock = isLock;
        if (buttonArray != null)
            messageBox.buttonArray = buttonArray;
        zindex = this.maxzindex + 1;

        messageBox.show();
        this.messageBoxList[this.messageBoxList.length] = messageBox;
    };
    this.showWait = function () {
        this.create(Translate("Examining8", "", "请稍后..."), "<span class=\"Nsb_exam_wait\"></span><span>" + Translate("Examining8", "", "请稍后...") + "</span>", false, false, null);
    };

    this.showWaitSubmit = function (title, content) {
        this.create("请稍后...", content, false, false, null);
    };

    this.closeAll = function () {
        while (this.messageBoxList.length > 0) {
            var message = this.messageBoxList.shift();
            message.close();
        }
    }
};

newv.exam.messageBox = function () {
    this.title = "";
    this.content = "";
    this.isShowCloseButton = true;
    this.isLock = false;
    this.zindex = 1000;
    this.buttonArray = [];
    this.lockObject = null;
    this.objElement = null;

    this.show = function () {
        var windowSize = newv.exam.environment.prototype.getBrowerWindowsSize();
        var windowWidth = windowSize["width"];
        var windowHeight = windowSize["height"];

        if (this.isLock) {
            this.lockObject = document.createElement("div");
            this.lockObject.style.position = "absolute";
            this.lockObject.style.top = "0px";
            this.lockObject.style.background = "#000";
            this.lockObject.style.filter = "alpha(opacity=50)";
            this.lockObject.style.opacity = "0.5";
            this.lockObject.style.left = "0px";
            this.lockObject.style.width = windowWidth + "px";
            this.lockObject.style.height = windowHeight + "px";
            this.lockObject.style.zIndex = this.zindex;
            this.zindex += 1;
            document.body.appendChild(this.lockObject);
        }

        var str = "<div class=\"Nsb_layer_title\">";
        str += "    <h4>" + this.title + "</h4>";
        if (this.isShowCloseButton) {
            str += "<a onclick=\"ExamScriptManage.MessageBoxManager.closeAll();\" class=\"Nsb_layer_close\"></a>";
        }
        str += "</div>";
        str += "<div class=\"Nsb_exam_tips_content\">";
        str += this.content;
        str += "</div>";
        str += "<div class=\"Nsb_layer_bt31\">";
        if (this.buttonArray.length == 0) {
            if (this.isShowCloseButton) {
                str += "<a onclick=\"ExamScriptManage.MessageBoxManager.closeAll();\" class=\"Nsb_layer_btg\">" + Translate("Message1", "", "关闭") + "</a>";
            }
        }
        else {
            for (var i = 0; i < this.buttonArray.length; i++) {
                var buttonClass = (this.buttonArray[i].className) ? this.buttonArray[i].className : "Nsb_layer_btg";
                str += "<a onclick=\"" + this.buttonArray[i].script + "\" class=\"" + buttonClass + "\">" + this.buttonArray[i].title + "</a>&nbsp;";
            }
        }
        str += "</div>";

        this.objElement = document.createElement("div");
        this.objElement.className = "Nsb_layer Nsb_exam_tips";
        this.objElement.style.position = "absolute";
        this.objElement.style.zIndex = this.zindex;
        this.objElement.innerHTML = str;
        document.body.appendChild(this.objElement);
        var sClientWidth = screen.availWidth;
        var sClientHeight = screen.availHeight;
        var sleft = (sClientWidth / 2) - (this.objElement.offsetWidth / 2);
        var iTop = -40 + (sClientHeight - this.objElement.offsetHeight) / 2;
        if (iTop < 10)
            iTop = 10;
        this.objElement.style.left = sleft + "px";
        this.objElement.style.top = iTop + "px";
    };

    this.close = function () {
        if (this.lockObject != null) {
            document.body.removeChild(this.lockObject);
        }
        if (this.objElement != null) {
            document.body.removeChild(this.objElement);
        }
    }
};

newv.exam.timer = function () {
    this.timerListeners = [];
    this.timerId = null;
    this.isruning = false;

    this.addEventListener = function (key, time, listener, isStartTimer) {
        if (!listener) {
            return;
        }

        var index = -1;
        for (var i = 0; i < this.timerListeners.length; i++) {
            if (this.timerListeners[i].key == key) {
                index = i;
                break;
            }
        }
        if (index == -1) {
            index = this.timerListeners.length;
        }
        this.timerListeners[index] = { "key": key, "time": time, "listener": listener, "starttime": time, "isActive": true };

        if (isStartTimer) {
            //启动定时器
            this.start();
        }
    };

    this.removeEventListener = function (key) {
        for (var i = 0; i < this.timerListeners.length; i++) {
            if (this.timerListeners[i].key == key) {
                this.timerListeners.splice(i, 1);
            }
        }
    };

    this.start = function () {
        if (this.isruning == true) {
            return;
        }

        this.timerId = setTimeout(this.timerElapsed, 1000);
        this.isruning = true;
    };

    this.stop = function () {
        if (this.timerId != null) {
            clearTimeout(this.timerId);
            this.timerId = null;
        }
        this.isruning = false;
    };

    this.pauseListener = function (key) {
        for (var i = 0; i < this.timerListeners.length; i++) {
            if (this.timerListeners[i].key == key) {
                this.timerListeners[i].isActive = false;
            }
        }
    };

    this.pauseAllListener = function () {
        for (var i = 0; i < this.timerListeners.length; i++) {
            this.timerListeners[i].isActive = false;
        }
    };

    this.activeListener = function (key) {
        for (var i = 0; i < this.timerListeners.length; i++) {
            if (this.timerListeners[i].key == key) {
                this.timerListeners[i].isActive = true;
            }
        }
    };

    this.activeAllListener = function () {
        for (var i = 0; i < this.timerListeners.length; i++) {
            this.timerListeners[i].isActive = true;
        }
    };

    this.timerElapsed = function () {
        try {
            for (var i = 0; i < ExamScriptManage.Timer.timerListeners.length; i++) {
                if (ExamScriptManage.Timer.timerListeners[i].isActive == true) {
                    ExamScriptManage.Timer.timerListeners[i].starttime -= 1;
                    if (ExamScriptManage.Timer.timerListeners[i].starttime <= 0) {
                        if (ExamScriptManage.Timer.timerListeners[i].listener) {
                            ExamScriptManage.Timer.timerListeners[i].listener();
                        }
                        ExamScriptManage.Timer.timerListeners[i].starttime = ExamScriptManage.Timer.timerListeners[i].time;
                    }
                }
            }
        }
        catch (e) { }

        if (ExamScriptManage.Timer.isruning == true) {
            ExamScriptManage.Timer.timerId = setTimeout(ExamScriptManage.Timer.timerElapsed, 1000);
        }
    }
};

//与考试界面试卷相关的操作
newv.exam.paperViewUtil = function () {
    //定义属性
    this.isMixOrder = "";
    this.paperXml = "";
    this.paperTemplatePath = "";
    this.paperVersion = "";

    this.currentPaperNodeIndex = 1;
    this.currentQuestionIndex = 0;
    this.currentQuestionUid = "";

    this.examInfo = null;
    this.examPaper = null;
    this.examGrade = null;
    this.examSysSetting = null;
    this.paperViewPanel = null;
    this.paperNavigatorPanel = null;
    this.questionJudgePolicy = null;
    this.examDoModeCode = null;
    this.saveAsnwerObj = null;
    this.errorFlagTimes = 0;

    this.stepName = "";
    this.requestStep = {
        "getRequstParam": {
            "beginMsg": Translate("Examining9", "", "正在初始化页面..."),
            "completedMsg": Translate("Examining10", "", "初始化页面完成"),
            "stepAPI": "getRequstParam",
            "nextStep": "loadExamInfo",
            "errorTitle": Translate("Examining11", "", "初始化页面出错"),
            "errorMsg": Translate("Examining12", "", "[examUid]参数丢失，请重新参加！")
        },
        "loadExamInfo": {
            "beginMsg": Translate("Examining13", "", "正在加载考试信息数据..."),
            "completedMsg": Translate("Examining14", "", "加载考试信息数据完成"),
            "stepAPI": "loadExamInfo",
            "nextStep": "loadUserExamData",
            "errorTitle": Translate("Examining15", "", "加载考试信息出错"),
            "errorMsg": Translate("Examining16", "", "服务器返回以下信息：{0} {1}。 请与系统管理员联系！")
        },
        "loadUserExamData": {
            "beginMsg": Translate("Examining20", "", "正在加载考生数据..."),
            "completedMsg": Translate("Examining21", "", "加载考生数据完成"),
            "stepAPI": "loadUserExamData",
            "nextStep": "checkUserExamData",
            "errorTitle": Translate("Examining22", "", "加载考生数据失败"),
            "errorMsg": Translate("Examining16", "", "服务器返回以下信息：{0} {1}。 请关闭页面后重试！")
        },
        "checkUserExamData": {
            "beginMsg": Translate("Examining23", "", "正在检查浏览器配置..."),
            "completedMsg": Translate("Examining24", "", "检查浏览器配置完成"),
            "stepAPI": "checkUserExamData",
            "nextStep": "initAttendExam",
            "errorTitle": Translate("Examining25", "", "出现错误"),
            "errorMsg": "{0} {1}。"
        },
        "initAttendExam": {
            "beginMsg": Translate("Examining26", "", "正在申请并初始化考试..."),
            "completedMsg": Translate("Examining27", "", "申请并初始化考试完成"),
            "stepAPI": "initAttendExam",
            "nextStep": "initExamPaper",
            "errorTitle": Translate("Examining28", "", "获取初始化考试出错"),
            "errorMsg": Translate("Examining16", "", "服务器返回以下信息：{0} {1}。 请与系统管理员联系！")
        },
        "initExamPaper": {
            "beginMsg": Translate("Examining29", "", "正在加载试题页面，请稍后..."),
            "completedMsg": Translate("Examining30", "", "试题页面加载完成"),
            "stepAPI": "initExamPaper",
            "nextStep": "",
            "errorTitle": Translate("Examining31", "", "获取始化试题页面出错"),
            "errorMsg": Translate("Examining16", "", "服务器返回以下信息：{0} {1}。 请与系统管理员联系！")
        }
    };

    //获取随机数
    this.GetRandomNum = function (min, max) {
        var Range = max - min;
        var Rand = Math.random();
        return (min + Math.round(Rand * Range));
    };

    this.getXmlDocument = function () {
        try { //Internet Explorer
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        } catch (e) {
            try { //Firefox, Mozilla, Opera, etc.
                xmlDoc = document.implementation.createDocument("", "", null);
            } catch (e) {
                alert(e.message)
            }
        }
        try {
            xmlDoc.async = false;
            return xmlDoc;
        }
        catch (e) {
            alert(e.message)
        }
        return null;
    };

    /**
     * @return {string}
     */
    this.LoadPaperView = function () {
        if (typeof XPathEvaluator != 'undefined') {
            XMLDocument.prototype.selectSingleNode = Element.prototype.selectSingleNode = function (xpath) {
                var x = this.selectNodes(xpath);
                if (!x || x.length < 1) return null;
                return x[0];
            }
            XMLDocument.prototype.selectNodes = Element.prototype.selectNodes = function (xpath) {
                var xpe = new XPathEvaluator();
                var nsResolver = xpe.createNSResolver(this.ownerDocument == null ? this.documentElement : this.ownerDocument.documentElement);
                var result = xpe.evaluate(xpath, this, nsResolver, 0, null);
                var found = [];
                var res;
                while (res = result.iterateNext())
                    found.push(res);
                return found;
            }
        }
        if (this.paperXml == null || this.paperXml.length == 0) {
            return Translate("Examining32", "", "未发现试卷内容") + "(paperXml)";
        }
        var xmlDoc = null;
        if (typeof ActiveXObject != 'undefined' && typeof GetObject != 'undefined') {
            xmlDoc = this.getXmlDocument();
            if (xmlDoc == null) {
                alert(Translate("Examining1", "", "考试出现异常！试卷格式不对．"));
                return "";
            }
            try {
                xmlDoc.loadXML(this.paperXml);
                if (xmlDoc.documentElement == null) {
                    alert(Translate("Examining1", "", "考试出现异常！试卷格式不对．"));
                    return;
                }
            }
            catch (e) {
                alert(Translate("Examining1", "", "考试出现异常！试卷格式不对．"));
                return "";
            }
        }
        else if (typeof DOMParser != 'undefined') {
            xmlDoc = (new DOMParser()).parseFromString(this.paperXml, 'text/xml');
            if (xmlDoc.documentElement == null) {
                alert(Translate("Examining1", "", "考试出现异常！试卷格式不对．"));
                return "";
            }
        }

        this.paperVersion = "2.0";
        this.paperViewPanel.innerHTML = userExamObject.PaperHtml + "<br/><br/>";;

        //显示导航条在前面
        this.paperNavigatorPanel.innerHTML = $("tdNavigatorHidden").innerHTML;

        //添加代码编辑器
        var $editorDivs = jQuery(".code_editor");
        var viewUtil = this;
        if ($editorDivs.length > 0) {
            require(['vs/editor/editor.main'], function() {
                $editorDivs.each(function () {
                    var $div = jQuery(this);
                    var language = $div.attr("language");
                    if (language === "python3") {
                        language = "python";
                    }
                    var editor = monaco.editor.create(this,
                        {
                            language: language
                        });
                    var id = this.id.replace("Answer_", "");
                    codeEditorDic[id] = editor;
                    codeEditors.push(editor);
                    editor.onDidBlurEditor(function(){
                        var code = editor.getValue();
                        var hasAnswer = (code != "" && code != null);
                        viewUtil.SetQuestionAnswerStatus(id, hasAnswer);
                    });
                    var answer = programAnswerDic[id];
                    if (answer) {
                        answer = decodeURIComponent(answer);
                        editor.setValue(answer);
                    } else {
                        if ($div.attr("type_code") === "program_fill") {
                            var preinstallCode = jQuery("#preinstallCode_" + id).text();
                            preinstallCode = decodeURIComponent(preinstallCode);
                            editor.setValue(preinstallCode);
                        }
                    }
                });
                //禁用全局粘贴功能
                if (!window.allowPasteCode) {
                    $(document).on("paste",
                        ".inputarea",
                        function() {
                            return false;
                        });
                }
            });
        }

        //检查上一题,下一题状态
        this.CheckButtonStatus();
        if (this.examDoModeCode == "question") {
            //转到第一题
            this.GoToQuestion(1, 1, 1);
        }

        return "";
    };

    this.GetXmlText = function (node) {
        if (node.text != undefined)
            return node.text;
        else
            return node.textContent;
    };

    this.SetUserAnswer = function () {
        //获取考生答案
        //var examGradeUid = this.examGrade.id;
        //var examUserUid = this.examGrade.userUid;

        //var localUserExamAnswer = this.saveAsnwerObj.GetUserExamAnswer(examUserUid, this.examGrade.examUid, examGradeUid);

        var userAnswer = this.examGrade.userAnswer;
        //if (localUserExamAnswer != "") userAnswer = localUserExamAnswer;
        //加载考生答案
        var xmlDoc = this.getXmlDocument();
        xmlDoc.async = false;
        if (userAnswer != "" && userAnswer != null) {
            if (typeof ActiveXObject != 'undefined' && typeof GetObject != 'undefined') {
                xmlDoc.loadXML(userAnswer);
            } else {
                xmlDoc = (new DOMParser()).parseFromString(userAnswer, 'text/xml');
            }
        }

        var elements = xmlDoc.getElementsByTagName("current_paper_node_index");
        if (elements && elements.length > 0) {
            this.currentPaperNodeIndex = elements[0].childNodes[0].nodeValue;
        }

        elements = xmlDoc.getElementsByTagName("current_question_index");
        if (elements && elements.length > 0) {
            this.currentQuestionIndex = elements[0].childNodes[0].nodeValue;
        }
        if ($("trPaperNode_" + this.currentPaperNodeIndex) != null) {
            $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
        }
        if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) {
            $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
        }
        if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) {
            $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
        }


        var examAnswersElements = xmlDoc.getElementsByTagName("exam_answers");
        var examAnswerElements = [];
        for (var i = 0; i < examAnswersElements.length; i++) {
            var els = examAnswersElements[i].getElementsByTagName("exam_answer");
            if (els && els.length > 0) {
                for (var j = 0; j < els.length; j++) {
                    examAnswerElements.push(els[j]);
                }
            }
        }
        //var objNodeList = xmlDoc.documentElement.selectNodes("exam_answers/exam_answer");
        for (var i = 0; i < examAnswerElements.length; i++) {
            var examAnswerEl = examAnswerElements[i];
            var question_uid = XmlSingleNodeText(examAnswerEl, "question_uid");
            var answer_text = XmlSingleNodeText(examAnswerEl, "answer_text");
            var answer_time = 0;

            var answerTime = XmlSingleNodeText(examAnswerEl, "answer_time");
            if (answerTime !== "") {
                answer_time = parseInt(answerTime);
            }
            var isSetBookmark = "N";
            var isSetBookmarkTemp = XmlSingleNodeText(examAnswerEl, "is_set_bookmark");
            if (isSetBookmarkTemp !== "") {
                isSetBookmark = isSetBookmarkTemp;
            }
            var isReadQuestion = "N";
            var isReadQuestionTemp = XmlSingleNodeText(examAnswerEl, "is_read");
            if (isReadQuestionTemp !== "") {
                isReadQuestion = isReadQuestionTemp;
            }

            if ($("hidQuestionBaseTypeCode_" + question_uid) == null) {
                //找不到该题则忽略
                continue;
            }
            var question_base_type_code = $("hidQuestionBaseTypeCode_" + question_uid).value;

            //赋上基本信息
            $("hidUserAnswerTime_" + question_uid).value = answer_time;
            if (isSetBookmark == "Y")
                this.SetQuestionBookmarkStatus(question_uid, isSetBookmark);
            if ($("hidIsRead_" + question_uid) != null)
                $("hidIsRead_" + question_uid).value = isReadQuestion;

            //赋上答案信息
            var arrAnswerOption = null;
            if (question_base_type_code == "single" || question_base_type_code == "multi" || question_base_type_code == "judge" || question_base_type_code == "eva_single" || question_base_type_code == "eva_multi") {
                arrAnswerOption = $name("Answer_" + question_uid);
                for (j = 0; j < arrAnswerOption.length; j++) {
                    try {
                        arrAnswerOption[j].checked = ("|" + answer_text + "|").indexOf("|" + arrAnswerOption[j].value + "|") > -1;
                    } catch (e) { }
                }
            }
            else if (question_base_type_code == "fill")     //填空题
            {
                var arrOneAnswer = answer_text.split("|");
                if (arrOneAnswer.length > 1) {
                    arrAnswerOption = $name("Answer_" + question_uid);
                    for (j = 0; j < arrOneAnswer.length; j++) {
                        try {
                            arrOneAnswer[j] = arrOneAnswer[j].replace("&Vertical;", "|");
                            if (arrAnswerOption.length > j) {
                                arrAnswerOption[j].value = arrOneAnswer[j];
                                this.CheckFillQuestionAnswerLength(arrAnswerOption[j]);
                            }
                        } catch (e) { }
                    }
                }
                else {
                    try {
                        answer_text = answer_text.replace("&Vertical;", "|");
                        $("Answer_" + question_uid).value = answer_text;
                    } catch (e) { }
                }
            }
            else if (question_base_type_code == "judge_correct") {
                var checkBoxValue = answer_text;
                if (answer_text == "Y") {
                    checkBoxValue = "Y";
                    $("Answer_" + question_uid).value = "";
                }
                else {
                    checkBoxValue = "N";
                    answer_text = answer_text.replace("&Vertical;", "|");
                    $("Answer_" + question_uid).value = answer_text;
                }
                for (var j = 0; j < $name("JudgeCorrect_" + question_uid).length; j++) {
                    try {
                        $name("JudgeCorrect_" + question_uid)[j].checked = ("|" + checkBoxValue + "|").indexOf("|" + $name("JudgeCorrect_" + question_uid)[j].value + "|") > -1;
                    } catch (e) { }
                }
            }
            else if (question_base_type_code == "program" || question_base_type_code == "program_fill") { //编程题
                var editor = codeEditorDic[question_uid];
                if (editor) {
                    answer_text = decodeURIComponent(answer_text);
                    editor.setValue(answer_text);
                } else {
                    programAnswerDic[question_uid] = answer_text;
                }
            } else {
                try {
                    answer_text = answer_text.replace("&Vertical;", "|");
                    $("Answer_" + question_uid).value = answer_text;
                } catch (e) { }
            }
        }

        //检查考生答案
        var noAnswerQuestionCount = this.GetNoAnswerQuestionCount();

        if (this.examInfo.examDoModeCode == "question") {
            //转到第一题
            this.GoToQuestion(1, 1, 1);
        }
    };

    this.SetQuestionBookmarkStatus = function (questionUid, isSetBookmark) {
        var questionIndexPath = this.GetQuestionIndexPathByQuestionUid(questionUid);
        if (questionIndexPath == null || questionIndexPath == "") return;
        if (isSetBookmark == "Y") {
            if ($("lnkQuestion_" + questionIndexPath).className == 'QuestionNavigatorText') {
                if ($("tdQuestionVavigator_" + questionUid) != null)
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag';
            }
            else if ($("lnkQuestion_" + questionIndexPath).className == 'QuestionNavigatorText_No_Answer') {
                if ($("tdQuestionVavigator_" + questionUid) != null)
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag_no_answer';
            }

            if ($("spanQuestionMark_" + questionUid) != null) {
                $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_flag';
                $("spanQuestionMark_" + questionUid).src = flagSrc;
            }
        }
        else {
            if ($("lnkQuestion_" + questionIndexPath).className == 'Nsb_exam_flag') {
                if ($("tdQuestionVavigator_" + questionUid) != null)
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag';
            }
            else if ($("lnkQuestion_" + questionIndexPath).className == 'QuestionBookmarkText_No_Answer') {
                if ($("tdQuestionVavigator_" + questionUid) != null)
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag_no_answer';
            }

            if ($("spanQuestionMark_" + questionUid) != null) {
                $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_no_flag';
                $("spanQuestionMark_" + questionUid).src = noflagSrc;
            }
        }
    };

    this.CheckFillQuestionAnswerLength = function (objControl) {
        try {
            if (objControl == null)
                return;
            var iCount = objControl.value.replace(/[^\u0000-\u00ff]/g, "aa");
            if (iCount == 0)
                return;
            var width = iCount.length * 8;
            if (width > 100)
                objControl.style.width = width;
            else
                objControl.style.width = 100;
        }
        catch (e) {
        }
    };

    this.CheckButtonStatus = function () {
        var examMode = this.examInfo.examDoModeCode;
        if (examMode == "question") {
            try {
                //检查是否为第一题
                if (this.currentPaperNodeIndex == 1 && this.currentQuestionIndex == 1) {
                    $("btnPreQuestion").style.display = "none";
                }
                else {
                    $("btnPreQuestion").style.display = "";
                }

                //检查是否为最后一题
                var nextQuesitonIndex = this.currentQuestionIndex + 1;
                var paperNodeIndex = this.currentPaperNodeIndex;
                var isLastQuestion = false;
                if ($("tblQuestion_" + paperNodeIndex + "_" + nextQuesitonIndex) == null) {
                    paperNodeIndex = paperNodeIndex + 1;
                    nextQuesitonIndex = 1;
                    if ($("trPaperNode_" + paperNodeIndex) == null) {
                        isLastQuestion = true;
                    }
                }
                if (isLastQuestion) {
                    $("btnNextQuestion").style.display = "none";
                }
                else {
                    $("btnNextQuestion").style.display = "";
                }
            }
            catch (e)
            { }
        }
    };

    this.SetQuestionBookmark = function (paper_node_index, parent_question_index, question_index) {
        var questionIndexPath = paper_node_index + "_" + parent_question_index + "_" + question_index;
        var questionUid = this.GetQuestionUidByQuestionIndex(paper_node_index, parent_question_index, question_index);
        if ($("tdQuestionVavigator_" + questionUid) != null) {
            if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_flag_no_answer') {
                $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag_no_answer';
                if ($("spanQuestionMark_" + questionUid) != null) {
                    $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_no_flag';
                    $("spanQuestionMark_" + questionUid).src = noflagSrc;
                }
            }
            else if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_no_flag_no_answer') {
                $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag_no_answer';
                if ($("spanQuestionMark_" + questionUid) != null) {
                    $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_flag';
                    $("spanQuestionMark_" + questionUid).src = flagSrc;
                }
            }
            else if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_flag') {
                $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag';
                if ($("spanQuestionMark_" + questionUid) != null) {
                    $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_no_flag';
                    $("spanQuestionMark_" + questionUid).src = noflagSrc;
                }
            }
            else if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_no_flag') {
                $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag';
                if ($("spanQuestionMark_" + questionUid) != null) {
                    $("spanQuestionMark_" + questionUid).className = 'Nsb_exam_flag';
                    $("spanQuestionMark_" + questionUid).src = flagSrc;
                }
            }
        }
    };

    this.GetQuestionUidByQuestionIndex = function (paperNodeIndex, parentQuestionIndex, questionIndex) {
        if ($("hidQuestion_" + paperNodeIndex + "_" + parentQuestionIndex + "_" + questionIndex) != null)
            return $("hidQuestion_" + paperNodeIndex + "_" + parentQuestionIndex + "_" + questionIndex).value;
        else
            return "";
    };

    this.GetQuestionIndexPathByQuestionUid = function (questionUid) {
        if ($("lnkVavigator_" + questionUid) != null)
            return $("lnkVavigator_" + questionUid).title;
        else
            return "";
    };

    this.SetQuestionAnswerStatus = function (questionUid, hasAnswer) {
        var questionIndexPath = this.GetQuestionIndexPathByQuestionUid(questionUid);
        if (questionIndexPath == null || questionIndexPath == "") return;
        if (hasAnswer == false) {
            if ($("tdQuestionVavigator_" + questionUid) != null) {
                if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_flag') {
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag_no_answer';
                }
                else if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_no_flag') {
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag_no_answer';
                }
            }
        }
        else {
            if ($("tdQuestionVavigator_" + questionUid) != null) {
                if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_flag_no_answer') {
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_flag';
                }
                else if ($("tdQuestionVavigator_" + questionUid).className == 'Nsb_exam_nav_no_flag_no_answer') {
                    $("tdQuestionVavigator_" + questionUid).className = 'Nsb_exam_nav_no_flag';
                }
            }
        }
    };

    this.LoadQuestionText = function (questionUid) {
        var xmlDoc = this.getXmlDocument();
        xmlDoc.async = false;
        xmlDoc.loadXML(this.paperXml);

        var paperNodeList = xmlDoc.documentElement.selectNodes("exam_paper_nodes/exam_paper_node");
        var isFinished = false;
        for (var i = 0; i < paperNodeList.length; i++) {
            var paperNode = paperNodeList[i];
            var paperNodeQuestionNodeList = paperNode.selectNodes("exam_paper_node_questions/exam_paper_node_question");
            var paperNodeQuestionNum = paperNodeQuestionNodeList.length;
            for (var j = 0; j < paperNodeQuestionNum; j++) {
                var tempNode = paperNodeQuestionNodeList[j];
                var question_uid = tempNode.selectSingleNode("question_uid").text;
                var question_base_type_code = tempNode.selectSingleNode("question_base_type_code").text;
                var question_text = tempNode.selectSingleNode("question_text").text;
                if (question_uid == questionUid) {
                    if (question_base_type_code == "fill") {
                        $("spanQuestionText" + questionUid).innerHTML = this.ReplaceFillInContent(question_text, question_uid);
                    }
                    else {
                        $("spanQuestionText" + questionUid).innerHTML = question_text;
                    }
                    isFinished = true;
                    break;
                }
            }
            if (isFinished) {
                break;
            }
        }
    };

    /**
     * @return {string}
     */
    this.ReplaceFillInContent = function (Content, questionUid) {
        var lowLinePos = Content.indexOf("_");
        var fillInBoxCount = 0;
        var preLowLinePos = -1;     //上一个下划线位置
        var lowLineCount = 0;
        var oneAnswer = "";
        var oneInputBoxString = "";
        var i = 0;
        var controlName = "Answer_" + questionUid;
        var preControls = document.getElementsByName(controlName);
        while (lowLinePos > -1) {
            //如果前一个下划线不是前一个字符
            if (lowLinePos != preLowLinePos + 1) {
                //如果下划线个数大于3
                if (lowLineCount >= 3) {
                    if (i < preControls.length) {
                        oneAnswer = preControls[i].value;
                    }
                    else {
                        oneAnswer = "";
                    }
                    oneInputBoxString = "<input type='text' class='QuestionInputText' id='Answer_" + questionUid + "' name='Answer_" + questionUid + "'  maxlength='1950' class='LineText' style='overflow:visible;BACKGROUND-COLOR: #ffff66;width:" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onkeyup=\"CheckFillQuestionAnswerLength(this)\" onchange=\"SetQuestionAnswerStatus('" + questionUid + "',true)\" />";
                    Content = Content.substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + Content.substring(preLowLinePos + 1);
                    //重设原来那个下划线的位置
                    lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
                    //fillInBoxCount = fillInBoxCount + 1;
                    i = i + 1;
                }
                //重新开始
                lowLineCount = 1;
            }
            else {
                lowLineCount = lowLineCount + 1;
            }
            preLowLinePos = lowLinePos;
            lowLinePos = Content.indexOf("_", lowLinePos + 1);
        }
        if (lowLineCount >= 3) {
            if (i < preControls.length) {
                oneAnswer = preControls[i].value;
            }
            else {
                oneAnswer = "";
            }
            //找到一个填空题
            oneInputBoxString = "<input type='text' class='QuestionInputText' id='Answer_" + questionUid + "' name='Answer_" + questionUid + "'  maxlength='1950' class='LineText' style='overflow:visible;BACKGROUND-COLOR: #ffff66;width:" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onkeyup=\"CheckFillQuestionAnswerLength(this);\" onchange=\"SetQuestionAnswerStatus('" + questionUid + "',true)\" />";
            Content = Content.substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + Content.substring(preLowLinePos + 1);
            //重设原来那个下划线的位置
            lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
            fillInBoxCount = fillInBoxCount + 1;
        }
        return Content + "";
    };

    /**
     * @return {number}
     */
    this.GetNoAnswerQuestionCount = function () {
        var total = 0;
        if ($("hidQuestionUid") == null) return;

        var questionUidList = $name("hidQuestionUid");
        total = questionUidList.length;
        var noAnswerQuestionNo = "";
        var noAnswerQuestionCount = 0;
        for (var i = 0; i < total; i++) {
            var questionUid = questionUidList[i].value;
            var hasAnswer = this.CheckOneContent(i);

            if (hasAnswer == false)      //没做
            {
                noAnswerQuestionCount = noAnswerQuestionCount + 1;
            }

            //设置试题的已做没做状态
            this.SetQuestionAnswerStatus(questionUid, hasAnswer);

        }
        return noAnswerQuestionCount;
    };

    /**
     * @return {boolean}
     */
    this.CheckOneContent = function (i) {
        var questionUid = $name("hidQuestionUid")[i].value;
        var questionBaseTypeCode = $("hidQuestionBaseTypeCode_" + questionUid).value;

        if (questionBaseTypeCode == "compose") {
            return true;
        }
        if ($("Answer_" + questionUid) == null)
            return true;    //没有答案的,只是文章

        var slen = $name("Answer_" + questionUid).length;
        if (questionBaseTypeCode == "single" || questionBaseTypeCode == "multi" || questionBaseTypeCode == "judge" || questionBaseTypeCode == "eva_single" || questionBaseTypeCode == "eva_multi") {
            if (slen <= 1) {
                if ($("Answer_" + questionUid).checked == true)
                    return true;
            } else {
                for (j = 0; j < slen; j++) {
                    if ($name("Answer_" + questionUid)[j].checked == true)
                        return true
                }
            }
        } else if (questionBaseTypeCode == "program") {
            var editor = codeEditorDic[questionUid];
            var code = editor ? editor.getValue() : programAnswerDic[questionUid];
            return code != "" && code != null;
        } else {
            if (slen <= 1) {
                if ($("Answer_" + questionUid).value != "")
                    return true;
            } else {
                for (j = 0; j < slen; j++) {
                    if ($name("Answer_" + questionUid)[j].value != "")
                        return true
                }
            }
        }
        return false;
    };

    /**
     * @return {number}
     */
    this.GetBookmarkQuestionCount = function () {
        var total = 0;
        if ($("hidQuestionUid") == null) return;

        var questionUidList = $name("hidQuestionUid");
        total = questionUidList.length;
        var bookmarkQuestionCount = 0;
        for (var i = 0; i < total; i++) {
            var questionUid = questionUidList[i].value;
            if ($("spanQuestionMark_" + questionUid) != null && $("spanQuestionMark_" + questionUid).className == 'Nsb_exam_flag') {
                bookmarkQuestionCount = bookmarkQuestionCount + 1;
            }
        }
        return bookmarkQuestionCount;
    };

    this.CheckPaperPass = function () {
        var noAnswerQuestionCount = this.GetNoAnswerQuestionCount();
        var bookmarkQuestionCount = this.GetBookmarkQuestionCount();
        return (noAnswerQuestionCount > 0 || bookmarkQuestionCount > 0);
    };

    this.CheckPaper = function () {
        var noAnswerQuestionCount = this.GetNoAnswerQuestionCount();
        var bookmarkQuestionCount = this.GetBookmarkQuestionCount();
        if (noAnswerQuestionCount > 0 || bookmarkQuestionCount > 0) {
            ExamScriptManage.MessageBoxManager.create(Translate("Examining33", "", "试题检查"), Translate("Examining34", "", "没做的试题有：") + noAnswerQuestionCount + Translate("Examining35", "", "个,已在试题导航中显示，并且已有") + bookmarkQuestionCount + Translate("Examining36", "", "个试题已做标注。"), true, false, null);
        }
        else {
            ExamScriptManage.MessageBoxManager.create(Translate("Examining33", "", "试题检查"), Translate("Examining37", "", "全部试题都已完成！"), true, false, null);
        }
    };

    /**
     * @return {string}
     */
    this.GetCurrentQuestionUid = function () {
        if ($("hidQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex + "_0") != null) {
            return $("hidQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex + "_0").value;
        }
        else {
            if ($("hidQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex + "_" + this.currentQuestionIndex) != null)
                return $("hidQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex + "_" + this.currentQuestionIndex).value;
            else
                return "";
        }
    };

    this.SaveQuestionUserAnswerTime = function () {
        if (this.examInfo.examDoModeCode == "question") {
            var currentQuestionUid = this.GetCurrentQuestionUid();
            try {
                if ($("hidUserAnswerTime_" + currentQuestionUid) != null)
                    $("hidUserAnswerTime_" + currentQuestionUid).value = GetQuestionUserAnswerTime();
            } catch (e) {
                //如果不存在此题则忽略
            }
        }
    };

    /**
     * @return {string}
     */
    this.DecodeStr = function (str) {
        if (str.length == 0) {
            return "";
        }
        var result = "";
        var ramNum = parseInt(str.substring(0, 1));
        for (var i = 1; i < str.length; i += 4) {
            var asc = parseInt("0x" + str.substring(i, i + 4), 16);
            asc = asc - ramNum;
            result = result + (String.fromCharCode(asc));
        }
        return result;
    };

    this.GetStandardAnswerByPaperVersion = function (standardAnswer) {
        try {
            if (this.paperVersion.length == 0) {
                return standardAnswer;
            }
            else if (this.paperVersion == "2.0") {
                standardAnswer = this.DecodeStr(standardAnswer);
            }
            return standardAnswer;
        }
        catch (e) {
            return standardAnswer;
        }
    };

    this.TrimSplitChar = function (str) {
        return str.replace(/(^\|*)|(\|*$)/g, "");
    };

    /**
     * @return {string}
     */
    this.SortUserExamAnswer = function (answer_text) {
        var arrUserAnswer;
        answer_text = answer_text.replace(" ", "");
        if (answer_text.indexOf(",") > -1) {
            arrUserAnswer = answer_text.split(",");
        }
        else if (answer_text.indexOf("，") > -1) {
            arrUserAnswer = answer_text.split("，");
        }
        else if (answer_text.indexOf("︱") > -1) {
            arrUserAnswer = answer_text.split("︱");
        }
        else if (answer_text.indexOf("|") > -1) {
            arrUserAnswer = answer_text.split("|");
        }
        else {
            arrUserAnswer = this.ConvertStringToArray(answer_text);
        }
        arrUserAnswer.sort();
        var retValue = "";
        for (var i = 0; i < arrUserAnswer.length; i++) {
            if (i == arrUserAnswer.length - 1) {
                retValue += arrUserAnswer[i];
            }
            else {
                retValue += arrUserAnswer[i] + "|";
            }
        }
        return retValue;
    };

    this.ConvertStringToArray = function (strValue) {
        var arrValue = [];
        var len = strValue.length;
        for (i = 0; i < len; i++) {
            arrValue[i] = strValue.charAt(i);
        }
        return arrValue;
    };

    /**
     * @return {string}
     */
    this.GetStandardAnswerFormat = function (standardAnswer) {
        //新的方法
        standardAnswer = standardAnswer.toUpperCase(); //转为大写
        var answerLength = standardAnswer.length;
        var newStandardAnswer = "";
        for (m = 0; m < answerLength; m++) {
            var charCode = standardAnswer.charCodeAt(m);
            if (charCode >= 65 && charCode <= 90) {
                newStandardAnswer += standardAnswer.charAt(m) + "|";
            }
        }
        if (newStandardAnswer.length > 0)
            newStandardAnswer = newStandardAnswer.substr(0, newStandardAnswer.length - 1);

        return newStandardAnswer;
    };

    this.GetSelectAnswerScoreFormat = function (selectAnswerScore) {
        if (selectAnswerScore.indexOf(",") > -1) {
            selectAnswerScore = selectAnswerScore.replace(/,/g, "|");
        }
        else if (selectAnswerScore.indexOf("，") > -1) {
            selectAnswerScore = selectAnswerScore.replace(/，/g, "|");
        }
        else if (selectAnswerScore.indexOf("︱") > -1) {
            selectAnswerScore = selectAnswerScore.replace(/︱/g, "|");
        }
        else if (selectAnswerScore.indexOf("｜") > -1) {
            selectAnswerScore = selectAnswerScore.replace(/｜/g, "|");
        }
        else if (selectAnswerScore.indexOf("、") > -1) {
            selectAnswerScore = selectAnswerScore.replace(/、/g, "|");
        }

        return selectAnswerScore;
    };

    /**
     * @return {string}
     */
    this.GetQuesitonTypeJudgePolicy = function (questionTypeUid) {
        try {
            if (questionTypeUid != "" && this.questionJudgePolicy[questionTypeUid] != null && this.questionJudgePolicy[questionTypeUid] != 'undefined')
                return this.questionJudgePolicy[questionTypeUid];
            else
                return "";
        }
        catch (e) {
            return "";
        }
    };

    this.FilterQuestionAnswerForFill = function (answer) {
        var pattern = /[^\u4e00-\u9fa5a-zA-Z0-9]/g;
        answer = answer.replace(pattern, "");
        answer = answer.toLowerCase();
        return answer;
    };

    /**
     * @return {string}
     */
    this.GetQuestionBookmarkStatus = function (questionUid) {
        var isSetBookmark = "N";
        if ($("spanQuestionMark_" + questionUid).className == 'Nsb_exam_flag') {
            isSetBookmark = "Y";
        }
        else {
            isSetBookmark = "N";
        }

        return isSetBookmark;
    };

    /**
     * @return {string}
     */
    this.GetUserExamAnswerXML = function (examGradeUid) {
        //先保存时间
        this.SaveQuestionUserAnswerTime();
        var objDate = new Date();
        var nowDateTime = objDate.getFullYear() + "-" + (objDate.getMonth() + 1) + "-" + objDate.getDate() + " " + objDate.getHours() + ":" + objDate.getMinutes() + ":" + objDate.getSeconds();

        try {
            var controlName = "";
            var answer_text = "";
            var questionUid = "";
            var question_base_type_code = "";
            var standardAnswer = "";
            var paper_question_score = 0;
            var answer_time = 0;
            var judge_score = 0;
            var judge_result_code = "";

            var grade_score = 0;  //总分

            var errorScore = 0; //答错的分数
            var rightScore = 0; //回答正确的分数
            var noAnswerScore = 0; //未回答的错误分数

            var oneQuestionAnswerXML = "";
            var allQuestionAnswerXML = "";

            allQuestionAnswerXML = "<?xml version = \"1.0\" encoding=\"gb2312\" standalone = \"no\"?>\r\n";
            allQuestionAnswerXML = allQuestionAnswerXML + "<exam_grade_object>\r\n";
            allQuestionAnswerXML = allQuestionAnswerXML + "    <exam_answers>\r\n";

            if ($("hidQuestionUid") == null) {
                return "";
            }

            //评卷策略
            var questionTypeUid = "";
            var judgePolicyCode = "";
            var each_option_score = 0;

            var length = 0;
            length = $name("hidQuestionUid").length;

            var arrStandardAnswer;
            var arrUserAnswer;

            var questionUidArr = $name("hidQuestionUid");
            for (var i = 0; i < length; i++) {
                answer_text = "";
                questionUid = questionUidArr[i].value;
                controlName = "Answer_" + questionUid;
                question_base_type_code = $("hidQuestionBaseTypeCode_" + questionUid).value;
                standardAnswer = $("hidStandardAnswer_" + questionUid).value;
                standardAnswer = this.GetStandardAnswerByPaperVersion(standardAnswer);
                paper_question_score = $("hidPaperQuestionScore_" + questionUid).value;
                if ($("hidQustionTypeUid_" + questionUid) != null)
                    questionTypeUid = $("hidQustionTypeUid_" + questionUid).value;

                if ($(controlName) == null) {
                    continue;
                }

                //提取考生答案
                var subLength = $name(controlName).length;
                for (j = 0; j < subLength; j++) {
                    if (question_base_type_code == "single" || question_base_type_code == "multi" || question_base_type_code == "judge" || question_base_type_code == "eva_single" || question_base_type_code == "eva_multi") {
                        if ($name(controlName)[j].checked == true) answer_text = answer_text + "|" + $name(controlName)[j].value;
                    }
                    else if (question_base_type_code == "judge_correct") {
                        if ($name("JudgeCorrect_" + questionUid)[0].checked == true) {
                            answer_text = $name("JudgeCorrect_" + questionUid)[0].value;
                        }
                        else if ($name("JudgeCorrect_" + questionUid)[1].checked == true) {
                            answer_text = $name("JudgeCorrect_" + questionUid)[1].value;
                        }
                        //如果是错误,则看是否填写了改正
                        if (answer_text == "N") {
                            if ($(controlName).value != "")
                                answer_text = "|" + $(controlName).value;
                            else
                                answer_text = "|" + answer_text;
                        }
                        else {
                            answer_text = "|" + answer_text;
                        }
                    }
                    else if (question_base_type_code == "fill") {
                        answer_text = answer_text + "|" + $name(controlName)[j].value.replace(/&/g, '&amp;').replace(/\|/g, "&#166;");
                    }
                    else if (question_base_type_code == "program" || question_base_type_code == "program_fill") {
                        var editor = codeEditorDic[questionUid];
                        if (editor) {
                            answer_text = encodeURIComponent(editor.getValue());
                        }
                    }
                    else {
                        answer_text = answer_text + "|" + $name(controlName)[j].value.replace(/&/g, '&amp;').replace(/\|/g, "&#166;");
                    }
                }
                if (answer_text != "") {
                    if (question_base_type_code == "fill")
                        answer_text = answer_text.substring(1, answer_text.length);
                    else
                        answer_text = this.TrimSplitChar(answer_text);
                }
                //开始评分
                judge_score = 0;
                judge_result_code = "error";
                if (answer_text != "" && answer_text == standardAnswer) {
                    judge_score = paper_question_score;
                }
                else if (question_base_type_code == "multi") {
                    //多选题考生答案作排序
                    answer_text = this.SortUserExamAnswer(answer_text);
                    standardAnswer = this.GetStandardAnswerFormat(standardAnswer);
                    var multiJudgePolicy = this.GetQuesitonTypeJudgePolicy(questionTypeUid);
                    if (multiJudgePolicy != "") {
                        var arrMultiJudgePolicy = multiJudgePolicy.split("|");
                        judgePolicyCode = arrMultiJudgePolicy[0];
                        if (arrMultiJudgePolicy.length == 2 && arrMultiJudgePolicy[1] != "") {
                            each_option_score = eval(arrMultiJudgePolicy[1]);
                            if (isNaN(each_option_score)) each_option_score = 0;
                        }
                        else {
                            each_option_score = 0;
                        }
                    }
                    else {
                        judgePolicyCode = "";
                        each_option_score = 0;
                    }

                    //把答案转成数组
                    if (standardAnswer.indexOf("|") > -1) {
                        arrStandardAnswer = standardAnswer.split("|");
                    }
                    else {
                        arrStandardAnswer = this.ConvertStringToArray(standardAnswer);
                    }
                    if (answer_text.indexOf("|") > -1) {
                        arrUserAnswer = answer_text.split("|");
                    }
                    else {
                        arrUserAnswer = this.ConvertStringToArray(answer_text);
                    }

                    //多选题考生答案作排序
                    if (answer_text == standardAnswer) {
                        judge_score = paper_question_score;
                    }
                    else if (judgePolicyCode == "multi_incomplete_right" && each_option_score > 0) {
                        for (j = 0; j < arrUserAnswer.length; j++) {
                            //答对一个得一个的分
                            if (standardAnswer.indexOf(arrUserAnswer[j]) > -1)     //选对一个
                                judge_score += each_option_score;
                            else
                                judge_score = judge_score - each_option_score;   //得错一个扣一个的分
                        }
                        //如果负分则为0,如果超过则为最大分
                        if (judge_score < 0)
                            judge_score = 0;
                        else if (judge_score > paper_question_score)
                            judge_score = paper_question_score;
                    }
                    else if (judgePolicyCode == "multi_partiel_right_score" && each_option_score > 0) {
                        for (j = 0; j < arrUserAnswer.length; j++) {
                            //答对一个得一个的分
                            if (standardAnswer.indexOf(arrUserAnswer[j]) > -1)
                                judge_score += each_option_score;
                            else {
                                judge_score = 0; //有错误选项则得0分
                                break;
                            }
                        }

                        //如果负分则为0,如果超过则为最大分
                        if (judge_score < 0)
                            judge_score = 0;
                        else if (judge_score > paper_question_score)
                            judge_score = paper_question_score;
                    }
                    else if (judgePolicyCode == "multi_wrong_sub" && each_option_score > 0) {
                        var rightNum = 0;
                        var wrongNum = 0;
                        for (j = 0; j < arrUserAnswer.length; j++) {
                            //答对一个得一个的分
                            if (standardAnswer.indexOf(arrUserAnswer[j]) > -1)     //选对一个
                                rightNum = rightNum + 1;
                            else
                                wrongNum = wrongNum + 1;   //得错一个扣一个的分
                        }
                        judge_score = (paper_question_score / arrStandardAnswer.length) * rightNum - each_option_score * wrongNum;

                        //如果负分则为0,如果超过则为最大分
                        if (judge_score < 0)
                            judge_score = 0;
                        else if (judge_score > paper_question_score)
                            judge_score = paper_question_score;
                    }
                    else if (judgePolicyCode == "multi_partiel_right") {
                        var arrMultiAnswerText = answer_text.split("|");
                        var arrMultiAtandardAnswer = standardAnswer.split("|");
                        var multiFoundError = false;
                        for (var ma = 0; ma < arrMultiAnswerText.length; ma++) {
                            var multiIsRight = false;
                            for (var mi = 0; mi < arrMultiAtandardAnswer.length; mi++) {
                                if (arrMultiAnswerText[ma] == arrMultiAtandardAnswer[mi]) {
                                    multiIsRight = true;
                                    break;
                                }
                            }
                            if (multiIsRight == false)
                                multiFoundError = true;
                        }
                        if (!multiFoundError) {
                            judge_score = paper_question_score * (arrMultiAnswerText.length / arrMultiAtandardAnswer.length);
                        }

                    }
                }
                //改版后遗漏了判断题开启评卷策略“答对得分,不答不得分,答错扣分”的情况 Lopping 2012-09-28
                else if (question_base_type_code == "judge") {
                    var judgePolicy = this.GetQuesitonTypeJudgePolicy(questionTypeUid);
                    if (judgePolicy != "") {
                        var arrJudgePolicy = judgePolicy.split("|");
                        judgePolicyCode = arrJudgePolicy[0];
                        if (judgePolicyCode == "judge_right_or_wrong") {
                            //做错倒扣分
                            judge_score = 0 - paper_question_score;
                        }
                    }
                }
                else if (question_base_type_code == "eva_single" || question_base_type_code == "eva_multi") {
                    answer_text = this.GetStandardAnswerFormat(answer_text);
                    if (answer_text.indexOf("|") > -1) {
                        arrUserAnswer = answer_text.split("|");
                    }
                    else {
                        arrUserAnswer = this.ConvertStringToArray(answer_text);
                    }

                    selectAnswerScore = $("hidSelectAnswerScore_" + questionUid).value;
                    selectAnswerScore = this.GetSelectAnswerScoreFormat(selectAnswerScore);
                    arrSelectAnswerScore = selectAnswerScore.split("|");
                    var m = 0;
                    for (m = 0; m < arrUserAnswer.length; m++) {
                        var _index = eval(arrUserAnswer[m].charCodeAt(0)) - 65;
                        if (_index >= 0 && _index < arrSelectAnswerScore.length) {
                            judge_score += eval(arrSelectAnswerScore[_index]);
                        }
                    }
                }
                else if (question_base_type_code == "fill") {
                    arrStandardAnswer = standardAnswer.split('|');
                    arrUserAnswer = answer_text.split('|');
                    var answerNum = arrStandardAnswer.length;
                    if (this.FilterQuestionAnswerForFill(standardAnswer) == this.FilterQuestionAnswerForFill(answer_text)) {
                        judge_score = paper_question_score;
                    }
                    else {
                        for (j = 0; j < arrUserAnswer.length; j++) {
                            if (j >= arrStandardAnswer.length) break;
                            //判断一个空是否有多个答案
                            var isMultipleAnswers = false;
                            if (arrStandardAnswer[j].toLowerCase().indexOf("&") > -1) {
                                isMultipleAnswers = true;
                            }

                            if (this.FilterQuestionAnswerForFill(arrUserAnswer[j]) == this.FilterQuestionAnswerForFill(arrStandardAnswer[j]) && !isMultipleAnswers) {
                                judge_score += eval(paper_question_score) / eval(answerNum);
                            }
                            else {
                                //如果不是完全配配，则还要看是否是几个中任一个都行,中间用&分隔
                                if (isMultipleAnswers) {
                                    var arrOneAnswer = arrStandardAnswer[j].split('&');
                                    for (var k = 0; k < arrOneAnswer.length; k++) {
                                        //只要是其中一个都得分
                                        if (this.FilterQuestionAnswerForFill(arrUserAnswer[j]) == this.FilterQuestionAnswerForFill(arrOneAnswer[k])) {
                                            judge_score += eval(paper_question_score) / eval(answerNum);
                                            break;
                                        }
                                    }
                                }
                            }
                        } //end for
                    }
                }
                else if (question_base_type_code == "single") {
                    standardAnswer = this.GetStandardAnswerFormat(standardAnswer);
                    if (answer_text != "" && answer_text == standardAnswer) {
                        judge_score = paper_question_score;
                    }
                }
                else if (question_base_type_code == "typing") {
                    if ($("hidJudgeScore_" + questionUid) != null) {
                        judge_score = eval($("hidJudgeScore_" + questionUid).value);
                    }
                }
                else if (question_base_type_code == "operate") {
                    if ($("hidJudgeScore_" + questionUid) != null) {
                        if ($("hidJudgeScore_" + questionUid).value == "")
                            $("hidJudgeScore_" + questionUid).value = "0";
                        judge_score = eval($("hidJudgeScore_" + questionUid).value);
                    }
                }
                else if (question_base_type_code == "judge_correct") {
                    //这里只处理不全对的,全对在的上面已处理,只得一半分
                    if (standardAnswer != "Y" && answer_text != "Y") {
                        judge_score = paper_question_score / 2;
                    }
                }
                else if (question_base_type_code == "answer") {
                    if (answer_text != "" && standardAnswer != "" && answer_text == standardAnswer) {
                        judge_score = eval(document.getElementById("hidJudgeScore_" + questionUid).value);
                    }
                    else {
                        var hidSelectAnswer = document.getElementById("hidSelectAnswer_" + questionUid).value;
                        if (hidSelectAnswer != "") {
                            var arrHidSelectAnswer = hidSelectAnswer.split("|");
                        }
                        var selectAnswerScore = document.getElementById("hidSelectAnswerScore_" + questionUid).value;
                        selectAnswerScore = this.GetSelectAnswerScoreFormat(selectAnswerScore);
                        var arrSelectAnswerScore = selectAnswerScore.split("|");
                        if (arrHidSelectAnswer != "" && typeof (arrHidSelectAnswer) != "undefined") {
                            for (m = 0; m < arrHidSelectAnswer.length; m++) {
                                if (this.FilterQuestionAnswerForAnswer(answer_text, arrHidSelectAnswer[m]) == true) {
                                    var j = paper_question_score * (arrSelectAnswerScore[m] / 100);
                                    judge_score = eval(judge_score + j);
                                }
                            }
                            if (judge_score < 0)//出现负分将分数置零
                            {
                                judge_score = 0;
                            }
                            if (judge_score > paper_question_score)//得分比实际分数高，则强行为实际分数
                            {
                                judge_score = paper_question_score;
                            }
                        }
                        else {
                            judge_score = 0;
                        }
                    }
                }
                

                if (answer_text == "") {
                    judge_score = 0;  //未答强行为0
                }

                if (judge_score == 0) {
                    if (answer_text == "")
                        judge_result_code = "";
                    else
                        judge_result_code = "error";
                }
                else if (eval(judge_score) == eval(paper_question_score))
                    judge_result_code = "right";
                else
                    judge_result_code = "middle";

                //是否要扣除错误试题的分数
                var isReadQuestion = "N";
                var isDeductScore = "N";
                if ($("hidIsRead_" + questionUid) != null) {
                    isReadQuestion = $("hidIsRead_" + questionUid).value;
                }
                if (this.examInfo.examDoModeCode == "question" && this.examInfo.isDeductScoreWhenError == "Y" && isReadQuestion == "Y") {
                    isDeductScore = "Y";
                }

                //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确, 
                if (isDeductScore == "Y") {
                    if (judge_score == 0 && question_base_type_code != "eva_single" && question_base_type_code != "eva_multi") {
                        if (answer_text == "")
                            noAnswerScore += eval(paper_question_score);
                        else
                            errorScore += eval(paper_question_score);

                        judge_score = 0 - eval(paper_question_score);
                    }
                    else {
                        rightScore += eval(judge_score);
                    }
                }

                grade_score = grade_score + eval(judge_score);

                var isSetBookmark = this.GetQuestionBookmarkStatus(questionUid);
                answer_time = eval($("hidUserAnswerTime_" + questionUid).value);

                if (answer_text != "" || eval(answer_time) > 0 || isSetBookmark == "Y" || isDeductScore == "Y") {
                    //处理 answer_text 的特殊字符（不处理 | 符号)
                    //var xmlDocT = new ActiveXObject("Microsoft.XMLDOM");
                    //if (!xmlDocT) {
                    //    xmlDocT = new ActiveXObject("Msxml2.DOMDocument");
                    //}

                    oneQuestionAnswerXML = "";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "        <exam_answer>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <question_uid>" + questionUid + "</question_uid>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <answer_text>" + answer_text + "</answer_text>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <answer_time>" + answer_time + "</answer_time>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <judge_score>" + judge_score + "</judge_score>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <judge_result_code>" + judge_result_code + "</judge_result_code>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <judge_remarks></judge_remarks>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <is_set_bookmark>" + isSetBookmark + "</is_set_bookmark>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "            <is_read>Y</is_read>\r\n";
                    oneQuestionAnswerXML = oneQuestionAnswerXML + "        </exam_answer>\r\n";
                    allQuestionAnswerXML = allQuestionAnswerXML + oneQuestionAnswerXML;
                    //alert(oneQuestionAnswerXML);
                }
            }
        } catch (e) {
            alert(Translate("Examining38", "", "保存答卷时出错:") + e.message);
        }

        allQuestionAnswerXML = allQuestionAnswerXML + "    </exam_answers>\r\n";
        var noAnswerQuestionNum = this.GetNoAnswerQuestionCount();

        allQuestionAnswerXML = allQuestionAnswerXML + "    <grade_score>" + grade_score + "</grade_score>\r\n";
        allQuestionAnswerXML = allQuestionAnswerXML + "    <exam_grade_uid>" + examGradeUid + "</exam_grade_uid>\r\n";
        allQuestionAnswerXML = allQuestionAnswerXML + "    <last_update_time>" + nowDateTime + "</last_update_time>\r\n";
        allQuestionAnswerXML = allQuestionAnswerXML + "    <no_answer_question_num>" + noAnswerQuestionNum + "</no_answer_question_num>\r\n";
        allQuestionAnswerXML = allQuestionAnswerXML + "</exam_grade_object>\r\n";

        return allQuestionAnswerXML;
    };

    //处理简答题答案 lopping 2011-09-06
    /**
     * @return {boolean}
     */
    this.FilterQuestionAnswerForAnswer = function (userAnswer, selectAnswer) {
        var pattern = /[^\u4e00-\u9fa5a-zA-Z0-9]/g;
        userAnswer = userAnswer.replace(pattern, "");
        userAnswer = userAnswer.toLowerCase();
        userAnswer = userAnswer.replace(/[\ |\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\-|\_|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\.|\>|\/|\?]/g, "");
        selectAnswer = selectAnswer.replace(pattern, "");
        selectAnswer = selectAnswer.toLowerCase();
        selectAnswer = selectAnswer.replace(/[\ |\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\-|\_|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\.|\>|\/|\?]/g, "");
        return (userAnswer.indexOf(selectAnswer) >= 0);
    };

    //锁定考试页面，不允许再答题
    this.LockPaper = function () {
        try {
            var controlName = "";
            if ($("hidQuestionUid") == null) return;

            var arrQuestionUid = $name("hidQuestionUid");
            var length = arrQuestionUid.length;
            for (var i = 0; i < length; i++) {
                this.LockQuestions(arrQuestionUid[i].value);
            }
        } catch (e) { }
    };

    //锁定试题，不允许再答题
    this.LockQuestions = function (questionUids) {
        try {
            var controlName = "";
            if (questionUids == "") return;

            var arrQuestionUid = questionUids.split(",");
            var length = arrQuestionUid.length;
            for (i = 0; i < length; i++) {
                controlName = "Answer_" + arrQuestionUid[i];
                if ($(controlName) == null) continue;

                var subLength = $name(controlName).length;
                for (j = 0; j < subLength; j++) {
                    $name(controlName)[j].disabled = true;
                }

                //可选项
                controlName = "spanSelectAnswer_" + arrQuestionUid[i];
                if ($(controlName) != null) {
                    subLength = $name(controlName).length;
                    for (var j = 0; j < subLength; j++) {
                        $name(controlName)[j].disabled = true;
                    }
                }
                //操作题
                if ($("btnUsingOffice_" + arrQuestionUid[i]) != null) {
                    $("btnUsingOffice_" + arrQuestionUid[i]).disabled = true;
                }
                //语音题
                if ($("btnUsingVoice_" + arrQuestionUid[i]) != null) {
                    $("btnUsingVoice_" + arrQuestionUid[i]).disabled = true;
                }
                //打字题
                if ($("btnBeginTyping_" + arrQuestionUid[i]) != null) {
                    $("btnBeginTyping_" + arrQuestionUid[i]).disabled = true;
                }
                //上传文件
                if ($("btnUploadFile_" + arrQuestionUid[i]) != null) {
                    $("btnUploadFile_" + arrQuestionUid[i]).disabled = true;
                }
            }
        } catch (e) { }
    };

    //解锁考试页面，允许再答题
    this.UnLockPaper = function () {
        try {
            var controlName = "";
            if ($("hidQuestionUid") == null) return;

            var arrQuestionUid = $name("hidQuestionUid");
            var length = arrQuestionUid.length;
            for (var i = 0; i < length; i++) {
                this.UnlockQuestions(arrQuestionUid[i].value);
            }
        } catch (e) { }
    };

    //解锁试题，不允许再答题
    this.UnlockQuestions = function (questionUids) {
        try {
            var controlName = "";
            if (questionUids == "") return;

            var arrQuestionUid = questionUids.split(",");
            var length = arrQuestionUid.length;
            var i = 0;
            for (i = 0; i < length; i++) {
                //如果是限时的试题被锁住就不能再答题了
                if (this.GetQuestionIsLockedByTimeLimit(arrQuestionUid[i]) == true) continue;

                controlName = "Answer_" + arrQuestionUid[i];
                if ($(controlName) == null) continue;

                var subLength = $name(controlName).length;
                var j = 0;
                for (j = 0; j < subLength; j++) {
                    $name(controlName)[j].disabled = false;
                }
                //可选项
                controlName = "spanSelectAnswer_" + arrQuestionUid[i];
                if ($(controlName) != null) {
                    subLength = $name(controlName).length;
                    for (j = 0; j < subLength; j++) {
                        $name(controlName)[j].disabled = false;
                    }
                }

                //操作题
                if ($("btnUsingOffice_" + arrQuestionUid[i]) != null) {
                    $("btnUsingOffice_" + arrQuestionUid[i]).disabled = false;
                }
                //语音题
                if ($("btnUsingVoice_" + arrQuestionUid[i]) != null) {
                    $("btnUsingVoice_" + arrQuestionUid[i]).disabled = false;
                }
                //打字题
                if ($("btnBeginTyping_" + arrQuestionUid[i]) != null) {
                    $("btnBeginTyping_" + arrQuestionUid[i]).disabled = false;
                }
                //上传文件
                if ($("btnUploadFile_" + arrQuestionUid[i]) != null) {
                    $("btnUploadFile_" + arrQuestionUid[i]).disabled = false;
                }
            }
        } catch (e) { }
    };

    /**
     * @return {boolean}
     */
    this.GetQuestionIsLockedByTimeLimit = function (questionUid) {
        var paperQuestionExamTime = eval($("hidPaperQuestionExamTime_" + questionUid).value);
        var userAnswerTime = eval($("hidUserAnswerTime_" + questionUid).value);
        return (paperQuestionExamTime > 0 && userAnswerTime >= paperQuestionExamTime);
    };

    this.InitQuestionInfoWhenByQuestion = function () {
        if (this.examInfo.examDoModeCode == "question") {
            var currentQuestionUid = this.GetCurrentQuestionUid();
            if (currentQuestionUid != "") {
                var questionExamTime = eval($("hidPaperQuestionExamTime_" + currentQuestionUid).value);
                var userAnswerTime = eval($("hidUserAnswerTime_" + currentQuestionUid).value);
                if (this.examInfo.isDeductScoreWhenError == "Y" && $("hidIsRead_" + currentQuestionUid) != null)
                    $("hidIsRead_" + currentQuestionUid).value = "Y";
                isQuestionLock = this.GetQuestionIsLockedByTimeLimit(currentQuestionUid);

                if (isQuestionLock == true) {
                    this.LockQuestions(currentQuestionUid);
                }
                InitQuestionTimeWhenByQuestion(currentQuestionUid, questionExamTime, userAnswerTime, isQuestionLock);
            }
        }
    };

    this.GoToQuestion = function (paperNodeIndex, parentQuestionIndex, questionIndex) {
        this.SaveQuestionUserAnswerTime();

        if (this.examInfo.examDoModeCode == "question") {
            if ($("trPaperNode_" + paperNodeIndex) != null && $("tblQuestion_" + paperNodeIndex + "_" + parentQuestionIndex) != null) {

                if ($("trPaperNode_" + this.currentPaperNodeIndex) != null) $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "none";
                if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
                if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) {
                    this.StopMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));
                    $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
                }

                this.currentPaperNodeIndex = eval(paperNodeIndex);
                this.currentQuestionIndex = eval(parentQuestionIndex);


                $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
                if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null)
                    $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
                $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
                this.ShowMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));
            }

        }
        else {
            if ($("tblPaperNode_" + paperNodeIndex) != null && $("tblPaperNode_" + paperNodeIndex).style.display == "none") {
                $("tblPaperNode_" + paperNodeIndex).style.display = "block";
            }
        }
        this.InitQuestionInfoWhenByQuestion();

        //定位试题
        if ($("lnkQuestion_" + paperNodeIndex + "_" + parentQuestionIndex + "_" + questionIndex) != null) {
            try {
                window.location.hash = "lnkQuestion_" + paperNodeIndex + "_" + parentQuestionIndex + "_" + questionIndex;
                if ($("tblPaperNode_" + paperNodeIndex) != null && $("tblPaperNode_" + paperNodeIndex).style.display == "none") {
                    $("tblPaperNode_" + paperNodeIndex).style.display = "block";
                }
                $("lnkQuestion_" + paperNodeIndex + "_" + parentQuestionIndex + "_" + questionIndex).focus();

            } catch (e) { }
        }

        //检查上一题,下一题状态
        this.CheckButtonStatus();

        //重新设定代码编辑器的大小
        $j.each(codeEditors, function(index, editor){
            editor.layout();
        });
    };

    this.GoToPreQuestion = function () {
        this.SaveQuestionUserAnswerTime();

        if (this.currentPaperNodeIndex == 1 && this.currentQuestionIndex == 1) return;

        if ($("trPaperNode_" + this.currentPaperNodeIndex) != null) $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "none";
        if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
        if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) {
            this.StopMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));
            $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
        }

        var oldQuestionIndex = this.currentQuestionIndex;

        this.currentQuestionIndex = this.currentQuestionIndex - 1;

        if (this.currentQuestionIndex == 0) {
            this.currentPaperNodeIndex = this.currentPaperNodeIndex - 1;
            //查找最后一个题号
            var tempQuestionIndex = 1;
            while ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + tempQuestionIndex) != null) {
                tempQuestionIndex = tempQuestionIndex + 1;
            }
            this.currentQuestionIndex = tempQuestionIndex - 1;

            if ($("trPaperNode_" + this.currentPaperNodeIndex) == null) {
                this.currentPaperNodeIndex = this.currentPaperNodeIndex - 1;
                this.currentQuestionIndex = oldQuestionIndex;
            }
        }

        $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
        if ($("tblPaperNode_" + this.currentPaperNodeIndex) != null && $("tblPaperNode_" + this.currentPaperNodeIndex).style.display == "none") {
            $("tblPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
        }
        $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";
        this.ShowMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));

        this.InitQuestionInfoWhenByQuestion();
        //检查上一题,下一题状态
        this.CheckButtonStatus();
    };

    this.GoToNextQuestion = function () {
        this.SaveQuestionUserAnswerTime();

        if ($("trPaperNode_" + this.currentPaperNodeIndex) != null) $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "none";
        if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
        if ($("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) != null) {
            this.StopMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));
            $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "none";
        }

        var oldQuestionIndex = this.currentQuestionIndex;
        this.currentQuestionIndex = this.currentQuestionIndex + 1;

        if ($("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex) == null) {
            this.currentPaperNodeIndex = this.currentPaperNodeIndex + 1;
            this.currentQuestionIndex = 1;

            if ($("trPaperNode_" + this.currentPaperNodeIndex) == null) {
                this.currentPaperNodeIndex = this.currentPaperNodeIndex - 1;
                this.currentQuestionIndex = oldQuestionIndex;
            }
        }

        $("trPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
        if ($("tblPaperNode_" + this.currentPaperNodeIndex) != null && $("tblPaperNode_" + this.currentPaperNodeIndex).style.display == "none") {
            $("tblPaperNode_" + this.currentPaperNodeIndex).style.display = "block";
        }
        $("tblQuestion_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex).style.display = "block";

        this.ShowMediaPlayer(this.currentPaperNodeIndex + "_" + this.currentQuestionIndex, $("trQuestionElement_" + this.currentPaperNodeIndex + "_" + this.currentQuestionIndex));

        this.InitQuestionInfoWhenByQuestion();
        //检查上一题,下一题状态
        this.CheckButtonStatus();
        //重新设定代码编辑器的大小
        $j.each(codeEditors, function(index, editor){
            editor.layout();
        });
    };

    this.StopMediaPlayer = function (questionIndex, parentElement) {
        if (parentElement == null)
            return;
        var objectList = parentElement.getElementsByTagName("object");
        for (var i = 0; i < objectList.length; i++) {
            objectList[i].style.display = "none";
            try {
                objectList[i].controls.Stop();
            }
            catch (e) {
                try {
                    objectList[i].DoStop();
                }
                catch (e1) {
                    try {
                        var objectHtml = objectList[i].outerHTML;
                        var objText = document.createElement("input");
                        objText.setAttribute("type", "hidden");
                        objText.setAttribute("id", "hidObjectHtml" + questionIndex + i);
                        objText.setAttribute("name", "hidObjectHtml" + questionIndex + i);
                        objText.setAttribute("value", objectHtml);
                        parentElement.appendChild(objText);
                        objectList[i].outerHTML = "<object id=\"replaceObject_" + questionIndex + i + "\"></object>";
                    }
                    catch (e3) {
                    }
                }
            }
        }
    };

    this.ShowMediaPlayer = function (questionIndex, parentElement) {
        if (parentElement == null)
            return;
        var objectList = parentElement.getElementsByTagName("object");
        for (var i = 0; i < objectList.length; i++) {
            try {
                if ($("hidObjectHtml" + questionIndex + i) != null) {
                    objectList[i].outerHTML = $("hidObjectHtml" + questionIndex + i).value;
                    parentElement.removeChild($("hidObjectHtml" + questionIndex + i));
                    try {
                        if (objectList[i].CurrentFrame() == 0) {
                            objectList[i].Play();
                        }
                    }
                    catch (e) {
                    }
                }

                objectList[i].style.display = "";
            }
            catch (e1) {

            }
        }
    };

    //答案保存及提交操作
    /**
     * @return {string}
     */
    this.SaveAnswerToLocal = function (isBackupToBox) {
        var examGradeUid = this.examGrade.id;
        if (examGradeUid == null || examGradeUid == "") {
            return "";
        }

        //客户端保存答案
        $("lnkSaveAsnwer").disabled = true;
        var userAnswer = this.GetUserExamAnswerXML(examGradeUid);
       

        $("lnkSaveAsnwer").disabled = false;

        return userAnswer;
    };

    this.SaveAnswerToServer = function (isAsyn, successEvent, errorEvent) {
        //客户端保存答案
        var userAnswer = this.SaveAnswerToLocal(false);
        if (userAnswer == "") {
            returnMessage = "errorexption";
            return returnMessage;
        }

        //把答案保存到服务器
        $("lnkSaveAsnwer").disabled = true;
        if (isAsyn) {

            $j.ajax({
                url: webApi("/ExamExam/SaveAnswerToServer"),
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
                data: JSON.stringify({ examGradeUid: this.examGrade.id, userAnswer: encodeURIComponent(userAnswer) }),
                success: function (data) {

                    var jsonData = UtilPassJsonResult(data.result);
                    $("lnkSaveAsnwer").disabled = false;
                    if (jsonData.hasError == true) {
                        if (errorEvent)
                            errorEvent("-1", jsonData.message);
                    }
                    else {
                        if (successEvent)
                            successEvent(jsonData);
                    }
                },
                error: function (xmlHttpRequest) {
                    errorEvent(xmlHttpRequest.status, xmlHttpRequest.statusText);

                }
            });
        }
        else {
            var returnMessage = ExamScriptManage.Request.requestUrl("/ExamExam/SaveAnswerToServer", "POST", "examUid=" + this.examInfo.id + "&examGradeUid=" + this.examGrade.id + "&userAnswer=" + encodeURIComponent(userAnswer), "JSON", null, false, null, null);
            $("lnkSaveAsnwer").disabled = false;
            return returnMessage;
        }

    };

    this.SaveUserAnswerAndClose = function (isAsyn) {
        ClearWindowCloseEvent();
        ExamScriptManage.MessageBoxManager.closeAll();
        ExamScriptManage.MessageBoxManager.showWait();
        this.LockPaper();
        this.UnLockExamWindow();

        $("lnkSubmitPaper").disabled = true;
        if (isAsyn) {
            this.SaveAnswerToServer(isAsyn, function (returnMessage) {
                ExamScriptManage.MessageBoxManager.closeAll();
                UtilPassSaveAnswerResult(returnMessage);
            }, function (errorCode, errorMessage) {
                DisableExamButton();
                ExamScriptManage.MessageBoxManager.closeAll();
                ExamScriptManage.MessageBoxManager.create(Translate("Examining39", "", "保存答案出错"), errorMessage, true, false, null);
            })
        }
        else {
            var returnMessage = this.SaveAnswerToServer(isAsyn, null, null);
            UtilPassSaveAnswerResult(returnMessage);
        }
        ExamScriptManage.MessageBoxManager.closeAll();
        /*
        //暂时屏蔽该代码
        var SmartReuqestCommand = new newvSmartReuqestCommand(localServerUrl);
        SmartReuqestCommand.CloseWindow()
        */
    };

    function UtilPassSaveAnswerResult(returnMessage) {

        //         $("main").style.display = "none";
        //            CloseMe(false);
        //        return;
        if (returnMessage.hasError == false) {
            var examGradeUid = userExamObject.ExamGrade.id;
            var examUserUid = userExamObject.ExamGrade.userUid;
            var examUid = userExamObject.ExamGrade.examUid;
            ExamScriptManage.paperViewUtil.saveAsnwerObj.DeleteUserExamAnswer(examUserUid, examUid, examGradeUid);
            $("main").style.display = "none";
            CloseMe(false);
        }
        else {
            DisableExamButton();
            ExamScriptManage.MessageBoxManager.create(Translate("Examining39", "", "保存答案出错"), returnMessage.message, true, false, null);
        }
    }

    this.SubmitPaper = function (isAsyn, paraMessage, times) {
        ClearWindowCloseEvent();
        ExamScriptManage.MessageBoxManager.closeAll();
        ExamScriptManage.MessageBoxManager.showWait();
        //客户端保存答案
        var userAnswer = this.SaveAnswerToLocal(false);
        var examGradeUid = this.examGrade.id;
        this.LockPaper();
        this.UnLockExamWindow();
        if (userAnswer == "errorexption") {
            DisableExamButton();
            ExamScriptManage.MessageBoxManager.closeAll();
            ExamScriptManage.MessageBoxManager.create(Translate("Examining39", "", "保存答案出错"), errorMessage, true, false, null);
        }
        //把答案保存到服务器
        $("lnkSubmitPaper").disabled = true;
        var sendData = "examUid=" + this.examInfo.id + "&examGradeUid=" + examGradeUid + "&userAnswer=" + encodeURIComponent(userAnswer) + "&errorFlagTimes=" + ExamScriptManage.paperViewUtil.errorFlagTimes;
        if (paraMessage != null && paraMessage.length > 0) {
            sendData = sendData + "&" + paraMessage;
        }

        if (isAsyn == true) {
            $j.ajax({
                url: webApi("/ExamExam/SubmitPaper"),
                contentType: 'application/json',
                type: 'post',
                dataType: 'json',
                data: JSON.stringify({ examUid: this.examInfo.id, examGradeUid: examGradeUid, userAnswer: encodeURIComponent(userAnswer) }),
                success: function (data) {
                    var returnData = UtilPassJsonResult(data.result);

                    ExamScriptManage.MessageBoxManager.closeAll();
                    UtilPassSubmitResult(returnData);
                    hasSubmitPaper = false;
                },
                error: function (xmlHttpRequest) {

                    if (times == undefined) {
                        times = 0;
                    }
                    ExamScriptManage.MessageBoxManager.closeAll();
                    var returnData = {};
                    returnData.hasError = true;
                    returnData.returnCode = "ForbitSubmit";
                    returnData.message = "您的网络繁忙，请重新提交试卷！" + errorMessage;
                    UtilPassSubmitResultError(returnData, isAsyn, times);
                }
            });
        }
        else {
            var returnMessage = ExamScriptManage.Request.requestUrl(commandReuqestUrl, "POST", sendData, "JSON", null, false, null, null);
            ExamScriptManage.MessageBoxManager.closeAll();
            UtilPassSubmitResult(returnMessage);
            hasSubmitPaper = false;
        }
    };

    function UtilPassSubmitResultError(returnMessage, isAsyn, times) {
        if (times == undefined) {
            times = 0;
        }
        times++;
        ExamScriptManage.paperViewUtil.UnLockPaper();
        try { SetWindowCloseEvent(); } catch (e) { }
        ExamScriptManage.paperViewUtil.LockExamWindow();
        $("lnkSubmitPaper").disabled = false;

        var tips = "提交失败，正在自动尝试第" + times + "次重新提交......";
        if (times >= 2) {
            tips += "</br></br>原因:计算机与服务器连接可能发生中断;";
            tips += "</br>方法一: 继续等待并按新提示进行操作";
            tips += "</br>方法二: 点击右上角“暂停”下次继续本考试";

        }

        ExamScriptManage.MessageBoxManager.showWaitSubmit("提示", tips);
        if (times == 1) {
            setTimeout("ExamScriptManage.paperViewUtil.SubmitPaper(" + isAsyn + ",'userUid=" + userExamObject.ExamGrade.userUid + "'," + times + ")", 3000);
        }
        else
            if (times == 2) {
                setTimeout("ExamScriptManage.paperViewUtil.SubmitPaper(" + isAsyn + ",'userUid=" + userExamObject.ExamGrade.userUid + "'," + times + ")", 50000);
            }
            else
                if (times == 3) {
                    setTimeout("ExamScriptManage.paperViewUtil.SubmitPaper(" + isAsyn + ",'userUid=" + userExamObject.ExamGrade.userUid + "'," + times + ")", 10000);

                }
                else {
                    ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), "您的网络繁忙，请重新手动提交试卷！", true, false, null);
                }
    }

    function UtilPassSubmitResult(returnMessage) {
        if (returnMessage.hasError == true) {
            if (returnMessage.errorCode == null && returnMessage.returnCode != "ForbitSubmit") {
                DisableExamButton();
                hasSubmitPaperError = true;
                ExamScriptManage.Timer.stop();
                var buttons = [];
                buttons[0] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
                ExamScriptManage.MessageBoxManager.create(Translate("Examining41", "", "提交试卷出错"), returnMessage.message, false, true, buttons);
            } else {
                ExamScriptManage.paperViewUtil.UnLockPaper();
                try { SetWindowCloseEvent(); } catch (e) { }
                ExamScriptManage.paperViewUtil.LockExamWindow();
                $("lnkSubmitPaper").disabled = false;
                ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), returnMessage.message, true, false, null);
            }
        } else {
            if (ExamScriptManage.paperViewUtil.examInfo.examClassCode == "race") {
                var passGateNum = parseInt(returnMessage.passGateNum);
                if (isNaN(passGateNum)) {
                    passGateNum = 0;
                }
                if (passGateNum >= ExamScriptManage.paperViewUtil.examInfo.gateNum) {
                    SetRaceEnd();
                }
                if ((typeof (returnMessage.errorCode) == "undefined" || returnMessage.errorCode != null) && returnMessage.returnCode == "RaceFail") {
                    SetRaceEnd();
                    ExamScriptManage.paperViewUtil.ShowMessageAfterSubmit(returnMessage.message, false);
                } else {
                    ExamScriptManage.paperViewUtil.ShowMessageAfterSubmit(returnMessage.message, false);
                }
            } else {
                var allowViewPaper = true;
                if (returnMessage.isAllowViewPaper != null && returnMessage.isAllowViewPaper == "N") {
                    allowViewPaper = false;
                }
                ExamScriptManage.paperViewUtil.ShowMessageAfterSubmit(returnMessage.message, allowViewPaper);
            }
        }
    }

    this.EndRaceExamining = function () {
        var returnMessage = ExamScriptManage.Request.requestUrl(commandReuqestUrl, "POST", "command=EndRaceExamining&examUid=" + this.examInfo.id + "&examGradeUid=" + this.examGrade.id, "JSON", null, false, null, null);
        if (returnMessage.hasError == true) {
            DisableExamButton();
            $("main").style.display = "";
            ExamScriptManage.MessageBoxManager.closeAll();
            ExamScriptManage.MessageBoxManager.create(Translate("Examining43", "", "结束闯关出错"), returnMessage.message, true, false, null);
        }
        else {
            CloseMe(false);
        }
    };

    this.StartNextRaceExaming = function () {
        ClearWindowCloseEvent();
        var nowDate = new Date();
        window.location.href = "AttendExamNew.aspx?examUid=" + this.examInfo.id + "&reqTime=" + nowDate.getHours() + nowDate.getMinutes() + nowDate.getSeconds() + nowDate.getMilliseconds();
    };

    this.ShowMessageAfterSubmit = function (message, isAllowViewPaper) {
        ClearWindowCloseEvent();
        $("main").style.display = "none";
        var examGradeUid = this.examGrade.id;
        var examUserUid = this.examGrade.userUid;
        var examUid = this.examGrade.examUid;
        ExamScriptManage.Timer.stop();
        this.saveAsnwerObj.DeleteUserExamAnswer(examUserUid, examUid, examGradeUid);
        var buttons = [];
        if (this.examInfo.examClassCode == "race") {
            if (isRaceEnd == true) {
                buttons[0] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
            }
            else {
                buttons[0] = { "title": Translate("Examining44", "", "下一关"), "className": "Nsb_layer_btb", "script": "ExamScriptManage.paperViewUtil.StartNextRaceExaming()" };
                buttons[1] = { "title": Translate("Examining45", "", "结束闯关"), "className": "Nsb_layer_btg", "script": "ExamScriptManage.paperViewUtil.EndRaceExamining()" };
            }
        }
        else {
            if (this.examSysSetting.IsForceJoinQuestionnaireAfterSubmitPaper) {
                if (this.examInfo.questionireUidName.length > 0) {
                    var quesnaireUids = this.examInfo.questionireUidName.split(",");
                    var quesnaireUid = quesnaireUids[0];
                    if (quesnaireUid != "") {
                        //jscomNewOpenMaxWindow("../../qns/QnsAnswerSheet.aspx?quesnaireUid=" + quesnaireUid + "&openerName=", 'ExamQnsAnswerSheet');
                    }
                }
            }

            if (this.examInfo.gradeReleaseType == "by_human") {
                buttons[0] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
            }
            else if (isAllowViewPaper) {
                if (this.examPaper.paperClassCode == "testing" && this.examInfo.isAllowSeeReport == "Y") {
                    buttons[0] = { "title": Translate("Examining46", "", "查看答卷"), "className": "Nsb_layer_btb", "script": "jscomNewOpenMaxWindow('/Exam/Manage/UserExamPreview?examGradeUid=" + examGradeUid + "','ExamPaperPreview')" };
                    buttons[1] = { "title": Translate("Examining47", "", "测评报告"), "className": "Nsb_layer_btb", "script": "jscomNewOpenMaxWindow('ShowEvaluateReport.aspx?isSmartGateWindow=Y&examGradeUid=" + examGradeUid + "','ExamPaperReport')" };
                    buttons[2] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
                }
                else {
                    buttons[0] = { "title": Translate("Examining46", "", "查看答卷"), "className": "Nsb_layer_btb", "script": "jscomNewOpenMaxWindow('/Exam/Manage/UserExamPreview?examGradeUid=" + examGradeUid + "','ExamPaperPreview')" };
                    buttons[1] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
                    //新版本gate无法关闭预览窗口 暂时屏蔽查看答卷
                    //buttons[0] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
                }
            }
            else if (this.examPaper.paperClassCode == "testing" && this.examInfo.isAllowSeeReport == "Y") {
                buttons[0] = { "title": Translate("Examining47", "", "测评报告"), "className": "Nsb_layer_btb", "script": "jscomNewOpenMaxWindow('ShowEvaluateReport.aspx?isSmartGateWindow=Y&examGradeUid=" + examGradeUid + "','ExamPaperReport')" };
                buttons[1] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
            }
            else {
                buttons[0] = { "title": Translate("Examining40", "", "关闭窗口"), "className": "Nsb_layer_btg", "script": "CloseMe(false)" };
            }
        }
        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), message, false, true, buttons);
    };

    this.LockExamWindow = function () {
        if (this.examInfo.isOpenBook == "N") {
            //锁定键盘
            this.saveAsnwerObj.LockKeyboard();

            //是否为全屏模式
            var browerSize = ExamScriptManage.Environment.getBrowerWindowsSize();
            var screenSize = ExamScriptManage.Environment.getScreenSize();


            /*** 该段代码是以前解决跨浏览器闭卷考试的问题的。非IE浏览器闭卷考试就会出现红框
            if (browerSize.width < screenSize.width - 30 || browerSize.height < screenSize.height - 10) {
            $("main").style.border = "3px dashed red";
            //处理失去焦点事件
            document.onfocusout = function (e) {
            var height = document.body.clientHeight;
            try {
            if (document.layers) {
            var x = e.pageX;
            var y = e.pageY;
            }
            if (document.all) {
            var x = event.clientX;
            var y = event.clientY;
            }

            if (y < 0 || y > height) {
            if (ExamScriptManage.paperViewUtil.errorFlagTimes >= 2) {
            submitPaper(true);
            }
            else {
            ExamScriptManage.paperViewUtil.errorFlagTimes += 1;
            ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("AntiExam4", "", "请在试卷红色虚线区域内操作！否则超过一次系统将自动提交试卷！"), true, false, null);
            }
            }
            }
            catch (e) {
            }
            }
            }**/
        }

        //禁止菜单 for IE5+ 
        document.oncontextmenu = function () {
            if (!isAllowContextMenu) {
                event.cancelBubble = true
                event.returnValue = false;
                return false;
            }
        }
        //禁止菜单 for all others 
        /*
        document.onmousedown = function () {
            if (!isAllowContextMenu) {
                try {
                    if (window.Event) {
                        if (e.which == 2 || e.which == 3)
                            return false;
                    }
                    else if (event.button == 2 || event.button == 3) {
                        event.cancelBubble = true
                        event.returnValue = false;
                        return false;
                    }
                }
                catch (e) {
                    return false;
                }
            }
        }
        */
        //禁止选择
        document.onselectstart = function () {
            if (!isAllowSelect) {
                return false;
            }
        }
        //处理按键事件
        document.onkeyup = function () {
            var IsOpenbookExam = (examInfo.is_open_book == "Y") ? true : false;
            var IsNormalOpenBrowser = true;
            var ent = ExamScriptManage.Environment.getEvent();

            var browerSize = ExamScriptManage.Environment.getBrowerWindowsSize();
            var screenSize = ExamScriptManage.Environment.getScreenSize();
            if (browerSize.width < screenSize.width - 30 || browerSize.height < screenSize.height - 10) {
                IsNormalOpenBrowser = false;
            }

            //如果是开卷考试，不向下执行 lopping 2011-02-16
            if (IsOpenbookExam == true) {
                return;
            }
            //return;
            //如果是开卷考试，或是练习，不用往下执行    
            if (IsOpenbookExam == false && IsNormalOpenBrowser == false) {
                if ((ent.keyCode == 91) || (ent.altKey == true && ent.keyCode == 9)) {
                    if (ExamScriptManage.paperViewUtil.errorFlagTimes >= 2) {
                        submitPaper(true);
                    }
                    else {
                        ExamScriptManage.paperViewUtil.errorFlagTimes += 1;
                        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("Hanger2", "", "如果超过两次点击Win键及Alt + Tab键页面将自动提交！"), true, false, null);
                    }
                }

            }
        }
        document.onkeydown = function () {
            if (window.allowPasteCode) {
                return;
            }
            var IsOpenbookExam = (examInfo.is_open_book == "Y") ? true : false;
            var ent = ExamScriptManage.Environment.getEvent();
            //禁止ESC键和F12调试键
            if (ent.keyCode == 27 || ent.keyCode == 123)
                ent.returnValue = false;

            //禁用复制 ctrl+67  ctrl+c 粘贴,ctrl+E
            if ((ent.ctrlKey && event.keyCode == 67) || (ent.ctrlKey && event.keyCode == 69)) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            //禁用组合键ctrl+W
            if (ent.ctrlKey && ent.keyCode == 87) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            //禁用打印键ctrl+P
            if (ent.ctrlKey && event.keyCode == 80) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            //禁止剪切文件 ctrl+88  ctrl+x 
            if (ent.ctrlKey && event.keyCode == 88) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            //禁止全选：ctrl+65 ctrl+a
            if (ent.ctrlKey && ent.keyCode == 65) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            //文本区
            if (ent.altKey && event.ent == 115) {
                ent.shift = -1;
                ent.keyCode = 0;
                ent.returnValue = false;
                return;
            }

            //禁用复制 ctrl+86  ctrl+V 粘贴
            if (ent.ctrlKey && ent.keyCode == 86) {
                if (!isAllowSelect) {
                    ent.keyCode = 0;
                    ent.returnValue = false;
                    return;
                }
            }

            if (((event.ctrlKey) && ((event.keyCode == 78) || (event.keyCode == 82))) || ((event.altKey) && (event.keyCode == 115))
                || (event.keyCode == 112 || event.keyCode == 114 || event.keyCode == 115 || event.keyCode == 116 || event.keyCode == 91)) {
                //112F1,114F3,115F4,116f5,禁止16Shift,17Ctrl,18Alt,91win,但实际上禁止16Shift,17Ctrl,18Alt,91win不生效

                ent.shift = -1;
                ent.keyCode = 0;
                ent.returnValue = false;
                return;
            }

            //处理IE7+点了导航栏后按退格键会清空所有答案
            if (ent.keyCode == 8) {
                ent.keyCode = 0;
                return;
            }

            //禁用 ctrl+alt+tab
            if ((ent.ctrlKey == true && ent.altKey == true && ent.keyCode == 46) || (ent.ctrlKey && ent.altKey) || (ent.keyCode == 91) || (ent.altKey == true && ent.keyCode == 9)) {
                if (!isAllowSelect) {
                    if (ExamScriptManage.paperViewUtil.errorFlagTimes >= 2) {
                        submitPaper(true);
                    }
                    else {
                        ExamScriptManage.paperViewUtil.errorFlagTimes += 1;
                        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("Examining52", "", "如果超过2次点击组合键[Ctrl + Alt]或者[Ctrl + Alt+ Del]考卷将自动提交并结束本次考试！") + "<br/><br/>" + Translate("Examining53", "", "误操作次数：") + ExamScriptManage.paperViewUtil.errorFlagTimes, true, false, null);
                    }
                }
            }
        }
    };

    this.UnLockExamWindow = function () {
        //解锁键盘
        this.saveAsnwerObj.UnlockKeyboard();
        if (document.onfocusout != null)
            document.onfocusout = null;
        document.oncontextmenu = null;
        document.onmousedown = null;
        document.onselectstart = null;
        document.onkeydown = null;
        document.onkeydown = null;
    };

    //层的隐藏/显示/全屏
    this.OpenOperateWindow = function (sUrl, isFullSize, option) {
        var divFloatBg = $("divOperatePanelBg");
        var divFloat = $("divOperatePanel");
        var documentSize = ExamScriptManage.Environment.getBrowerWindowsSize();
        divFloatBg.style.width = documentSize.width + "px";
        divFloatBg.style.height = documentSize.height + "px";
        var screenSize;
        if (isFullSize == true) {
            screenSize = ExamScriptManage.Environment.getScreenSize();
            divFloat.style.width = screenSize.width + "px";
            divFloat.style.height = screenSize.height + "px";
            divFloat.style.top = 0;
            divFloat.style.left = 0;
        }
        else {
            divFloat.style.width = option.width + "px";
            divFloat.style.height = option.height + "px";
            screenSize = ExamScriptManage.Environment.getScreenSize();
            divFloat.style.top = (screenSize.height - option.height) / 2 + "px";
            divFloat.style.left = (screenSize.width - option.width) / 2 + "px";
        }

        var iFFloat = $("ifrOperate");
        iFFloat.src = sUrl;
        iFFloat.style.width = parseInt(divFloat.style.width, 10) + "px";
        iFFloat.style.height = parseInt(divFloat.style.height, 10) + "px";

        divFloatBg.style.display = "block";
        divFloat.style.display = "block";

        if (IsNullOrEmpty(this.currentQuestionUid) == false)
            this.StopMediaPlayer(this.currentQuestionUid, $("panelQuestionText_" + this.currentQuestionUid));
    };

    this.CloseOperateWindow = function () {
        var divFloatBg = $("divOperatePanelBg");
        var divFloat = $("divOperatePanel");
        divFloatBg.style.display = "none";
        if (divFloat.style.display == "none") {
            return;
        }

        divFloat.style.display = "none";
        var iFFloat = $("ifrOperate");
        iFFloat.src = "about:blank";

        if (IsNullOrEmpty(this.currentQuestionUid) == false)
            this.ShowMediaPlayer(this.currentQuestionUid, $("panelQuestionText_" + this.currentQuestionUid));
    };

    this.OpenContractVoiceFile = function (examGradeUid, questionUid, examUid, paperQuestionExamTime) {
        //暂时屏蔽此题型
        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("", "", "暂时不支持此题型 后续开放！"), true, false, null);
        return;
    };

    this.OpenContractTextFile = function (examGradeUid, questionUid, examUid, examQuestionType) {
        //暂时屏蔽此题型
        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("", "", "暂时不支持此题型 后续开放！"), true, false, null);
        return;
    };

    this.TypingOpenWindow = function (examGradeUid, questionUid, questionExamTime, paperQuestionScore) {
        //暂时屏蔽此题型
        ExamScriptManage.MessageBoxManager.create(Translate("Examining42", "", "提示"), Translate("", "", "暂时不支持此题型 后续开放！"), true, false, null);
        return;
    };

    this.OpenHtmlEditor = function (controlName, fileRootPath) {
        if (fileRootPath == null) fileRootPath = "";

        this.OpenOperateWindow("../../framework/HtmlEditor/HtmlEditor.aspx?FileRootPath=" + fileRootPath + "&openType=iframe", false, { "width": 760, "height": 560 });
    };

    this.OpenUploadFile = function (questionUid) {
        var FileRootPath = "ExamAnswer\\Exam_" + this.examInfo.examUid + "\\ExamGrade_" + this.examGrade.id + "\\" + questionUid;
        var galleryscript = '../../framework/HtmlEditor/HtmlEditRes/ftb.insertattachment.aspx?returnFunction=GetUploadFileHtml&FileRootPath=' + FileRootPath;
        var tt, w, left, top;
        width = 450;
        height = 300;
        left = (screen.width - width) / 2;
        if (left < 0) { left = 0; }

        top = (screen.height - 60 - height) / 2;
        if (top < 0) { top = 0; }

        tt = 'toolbar=no, menubar=no, scrollbars=yes,resizable=no,location=no, status=yes,';
        tt = tt + 'width=' + width + ',height=' + height + ',left=' + left + ',top=' + top;
        w = window.open(galleryscript, 'insertimage', tt);
        try {
            w.focus();
        } catch (e) { }
    };

    this.onRequestError = function (status, statusText) {
        var msgTitle, msgCount;
        var stepObj = ExamScriptManage.paperViewUtil.requestStep[this.stepName];
        if (stepObj != null) {
            msgTitle = stepObj.errorTitle;
            msgCount = ExamScriptManage.Environment.formatString(stepObj.errorMsg, [status, statusText]);
        }
        else {
            msgTitle = Translate("Examining48", "", "出现未知错误");
            msgCount = ExamScriptManage.Environment.formatString(Translate("Examining16", "", "服务器返回以下信息：{0} {1}。 请与系统管理员联系！"), [status, statusText]);
        }

        ExamScriptManage.paperViewUtil.showErrorMessage(msgTitle, msgCount);
    };

    this.onCheckBrowserError = function (msg) {
        var buttons = [];
        msg += "<br/><br/><a href='../../module/checkIE.htm' style='color:blue;font-size:18px' target='_blank'>" + Translate("Examining54", "", "点击进入浏览器检测") + "</a>";
        buttons[0] = { "title": Translate("Message1", "", "关闭"), "className": "Nsb_layer_btg", "script": "var SmartReuqestCommand = new newvSmartReuqestCommand('" + localServerUrl + "');SmartReuqestCommand.CloseWindow()" };
        ExamScriptManage.MessageBoxManager.create(Translate("Examining55", "", "浏览器检测失败"), msg, false, true, buttons);
    };

    this.runStep = function () {
        var stepInfo = ExamScriptManage.paperViewUtil.requestStep[this.stepName];
        showLogMessage("ongoing", stepInfo.beginMsg);
        eval(stepInfo.stepAPI + "()");
    };

    this.runNextStep = function () {
        var stepObj = this.requestStep[this.stepName];
        if (stepObj == null) {
            var msgTitle = Translate("Examining48", "", "出现未知错误");
            var msgCount = Translate("Examining49", "", "执行操作[") + this.stepName + Translate("Examining50", "", "]失败，请与系统管理员联系！");
            this.showErrorMessage(msgTitle, msgCount);
            return;
        }
        showLogMessage("completed", stepObj.completedMsg);
        if (stepObj.nextStep.length > 0) {
            this.stepName = stepObj.nextStep;
            this.runStep();
        }
    };

    this.showErrorMessage = function (title, content) {
        var buttons = [];

        buttons[0] = { "title": Translate("Examining51", "", "关闭页面"), "className": "Nsb_layer_btg", "script": "var SmartReuqestCommand = new newvSmartReuqestCommand('" + localServerUrl + "');SmartReuqestCommand.CloseWindow()" };
        ExamScriptManage.MessageBoxManager.create(title, content, false, true, buttons);
    }
};



var ExamScriptManage = window.ExamScriptManage = newv.exam.prototype.Init();


/*
下面的方法是为了与老的版本相兼容
*/
var iframePaper = window;

//伸缩对象，点一下伸开，再点一下收回，并且还可以带图片显示伸缩的标至
function jscomFlexObject(obj, imageObj, onImagePath, offImagePath) {
    if (obj.style.display == "none") {
        obj.style.display = "block";
        if (imageObj && onImagePath)
            imageObj.src = onImagePath;
        //oElement.alt = "收缩";
    }
    else {
        obj.style.display = "none";
        if (imageObj && offImagePath)
            imageObj.src = offImagePath;
        //oElement.alt = "展开";
    }
}

function jscomCheckedQuestionAnswer(radioObject, index) {
    try {
        var obj = document.getElementsByName(radioObject)[index];
        if (obj.checked) {
        }
        else {
            obj.checked = true;
        }
    }
    catch (e) {
    }
}

function SetQuestionBookmark(paper_node_index, parent_question_index, question_index) {
    ExamScriptManage.paperViewUtil.SetQuestionBookmark(paper_node_index, parent_question_index, question_index);
}
function SetQuestionAnswerStatus(questionUid, hasAnswer) {
    ExamScriptManage.paperViewUtil.SetQuestionAnswerStatus(questionUid, hasAnswer);
}
function LoadQuestionText(questionUid) {
    ExamScriptManage.paperViewUtil.LoadQuestionText(questionUid);
}
function GoToQuestion(questionUid, parentQuestionIndex, questionIndex) {
    ExamScriptManage.paperViewUtil.GoToQuestion(questionUid, parentQuestionIndex, questionIndex);
}
function GoToPreQuestion() {
    ExamScriptManage.paperViewUtil.GoToPreQuestion();
}
function GoToNextQuestion() {
    ExamScriptManage.paperViewUtil.GoToNextQuestion();
}
function CheckFillQuestionAnswerLength(objControl) {
    ExamScriptManage.paperViewUtil.CheckFillQuestionAnswerLength(objControl);
}

function XmlSingleNodeText(el, nodeName) {
    var elements = el.getElementsByTagName(nodeName);
    if (elements && elements.length > 0) {
        return elements[0].childNodes[0].nodeValue;
    }
    return "";
}