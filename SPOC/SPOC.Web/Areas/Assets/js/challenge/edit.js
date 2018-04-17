var serviceUrl = "/api/services/app/";

var ueToolbars = [
    [
        'source',
        "insertcode", //代码语言
        "fontfamily", //字体
        "fontsize", //字号
        "forecolor", //字体颜色
        "backcolor", //背景色
        "bold", //加粗
        "italic", //斜体
        "underline", //下划线
        "fontborder", //字符边框
        "superscript", //上标
        "subscript", //下标
        "horizontal", //分隔线
        "|",
        "formatmatch", //格式刷
        "source", //源代码
        "blockquote", //引用
        "print", //打印
        "preview", //预览
        "help",//帮助
        "fullscreen" //全屏
    ], [
        "justifyleft", //居左对齐
        "justifyright", //居右对齐
        "justifycenter", //居中对齐
        "justifyjustify", //两端对齐
        "insertorderedlist", //有序列表
        "insertunorderedlist", //无序列表
        "|",
        "link", //超链接
        "unlink", //取消链接
        "spechars", //特殊字符
        "simpleupload", //单图上传
        "insertimage", //多图上传
        //"wordimage", //图片转存
        "scrawl", //涂鸦
        //"music",//音乐
        //"insertvideo", //视频
        "|",
        "time", //时间
        "date", //日期
        "edittip " //编辑提示
    ]
];

var NormalQuestion = (function() {

    function init(id, parentId) {
        var self = this;
        var options = []; //选择题选项，保存的对象{data:{index:index,value:value,type:type}, ue:ue}
        var answerItems = []; //问答题|操作题得分点，保存对象{index:index, value:value, ratio:ratio}
        var boxTypeCache; //单选题redio或多选题checkbox
        var dataCache = {id:id}; //缓存已有的question数据
        var ueDic = {}; //UE富文本编辑器字典，key为id, value为UE对象
        var matchTextDic = {}; //根据代码与题干匹配到的关键字 key为规则Id value为关键字数组
        var category = new nv.category.CombotreeDataClass("folderUid", "challenge_cpp");
        var tabHelper = new TabHelper("tabs");
      
        //===============Event Begin===========================
        this.onQuestionTypeChange = function(value) {
            $("input:radio[name='judge']").off();
            if (value === "single") {
                questionTypeChangeSingle();
            } else if (value === "multi") {
                questionTypeChangeMulti();
            } else if (value === "answer") {
                questionTypeChangeAnswer();
            } else if (value === "program") {
                questionTypeChangeProgram();
            } else if (value === "program_fill") {
                questionTypeChangeProgramFill();
            }  else if (value === "judge_correct") {
                questionTypeChangeJudgeCorrect();
                $("input:radio[name='judge']")
                    .on("change", function() {
                        var judgeValue = $("input[name='judge']:checked").val();
                        if (judgeValue === "Y") {
                            $(".dynamic.judge-correct-wrong").hide();
                        } else {
                            $(".dynamic.judge-correct-wrong").show();
                        }
                        $("#standardAnswer").textbox("setValue", judgeValue);
                    });

            } else {
                //选中 判断题|填空题|编程题|语音题|操作题|打字题
                questionTypeChange(value);
            }
        }

        this.onDeleteOption = function(index) {
            var optionObj;
            if (index === options.length - 1) {
                optionObj = options.pop();
                optionObj.ue.destroy();
                $("#option-form-group-" + optionObj.data.index).remove();
                $("#option-" + index).remove();
                optionObj = null;
            } else {
                optionObj = options.splice(index, 1)[0];
                optionObj.ue.destroy();
                $("#option-form-group-" + optionObj.data.index).remove();
                $("#option-" + optionObj.data.index).remove();
                var removes = options.splice(index, options.length - index);
                var length = removes.length;
                for (var i = 0; i < length; i++) {
                    var obj = removes[i];
                    obj.data.index -= 1;
                    obj.data.value = obj.ue.getContent();
                    obj.ue.destroy();
                    $("#option-form-group-" + (obj.data.index + 1)).remove();
                    $("#option-" + (obj.data.index + 1)).remove();
                    createOption(obj.data.index, obj.data.value, obj.data.type);
                    obj = null;
                }
                removes = null;
            }
        }

        this.onAddOption = function() {
            createOption(options.length, "", boxTypeCache);
        }

        this.onDeleteAnswerItem = function(index) {
            var itemObj;
            if (index === answerItems.length - 1) {
                itemObj = answerItems.pop();
                $("#answer-item-form-group-" + itemObj.index).remove();
                itemObj = null;
            } else {
                itemObj = answerItems.splice(index, 1)[0];
                $("#answer-item-form-group-" + itemObj.index).remove();
                var removes = answerItems.splice(index, answerItems.length - index);
                var length = removes.length;
                for (var i = 0; i < length; i++) {
                    var obj = removes[i];
                    obj.value = $("#answer-value-" + obj.index).textbox("getValue");
                    obj.ratio = $("#answer-ratio-" + obj.index).numberbox("getValue");
                    obj.index -= 1;
                    $("#answer-item-form-group-" + (obj.index + 1)).remove();
                    createAnswerItem(obj.index, obj.value, obj.ratio);
                    obj = null;
                }
                removes = null;
            }
        }

        this.onAddAnswerItem = function() {
            createAnswerItem(answerItems.length, "", 0);
        }

        this.onSaveOnly = function() {
            save(function () {});
        };

        this.onSaveAndClose = function() {
            save(function (success) {
                if (!success) {
                    return;
                }
                evtBus.dispatchEvt("flush_questions");
                tabHelper.closeTab();
            });
        }

        this.onSaveAndNewlyAdd = function() {
            save(function (success) {
                if (!success) {
                    return;
                }
                location.reload();
            });
        }

        this.onChangeCodeMode = function (checked) {
            var isCustomCode = !checked;
            $("#questionCode").textbox("readonly", !isCustomCode)
                .textbox({ required: isCustomCode });
            if (!isCustomCode) {
                $("#questionCode").textbox("setValue", stringIsEmpty(dataCache.questionCode) ? "" : dataCache.questionCode);
            }
        };
        this.testRun = function (language) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var apiUrl = "/api/services/app/codecomplie/compile";
            //是否多次编译
            if ($("#multiTest").prop("checked"))
                apiUrl = "/api/services/app/codecomplie/multicompile";
            var index = layer.msg("代码编译运行中",
                {
                    time: 60000,
                    icon: 16,
                    shade: 0.01,
                    skin: ''
                });
            var code = editor.getValue();

            nv.post(apiUrl,
                {
                    id: dataCache.id,
                    language: language, //1 C++ 0 C
                    param: $("#param").textbox("getValue"),
                    inputParam: $("#inputParam").textbox("getValue"),
                    code: code
                },
                function (data) {
                    $(".run-results").show();
                    layer.close(index);
                    if (data.success) {
                        if (data.result.code === 0) {
                            if (data.result.result === $("#standardAnswer").textbox("getValue")) {
                                $("#run-result").textbox("setValue", "调试通过:\r\n" + data.result.result);
                            } else {
                                $("#run-result").textbox("setValue", "调试不通过:\r\n" + data.result.result);
                            }
                           // $("#run-result").textbox("setValue",  data.result.result);
                           
                        } else {
                            $("#run-result").textbox("setValue", "编译错误:\r\n" + data.result.msg);


                        }
                       
                    } else {
                        layer.msg('编译出错!请稍后重试或联系老师:' + data.error.message);
                    }
                });


        };
        //自动匹配知识点
        this.matchLabel = function () {
            var url = serviceUrl + "liblabel/SmartSeachLabel";
            nv.post(url,
                { code: editor.getValue(), questionText: ueDic["questionText"].getContent() },
                function (data) {
                    if (data.success) {
                        var recommendlabel = $("#recommendlabel").val() == null ? [] : $("#recommendlabel").val();
                        recommendlabel = recommendlabel.concat(data.result.label);
                        $("#recommendlabel").val(recommendlabel).trigger("change");

                        //获取匹配上的文字加入到得分点
                        matchTextDic = data.result.matchText;
                        //matchAnswerItem(data.result.matchText);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }
        //============Event End===============================
        //按照属性值，查找数组对象
        function findElem(array, attr, val) {
            for (var i = 0; i < array.length; i++) {
                if (array[i][attr] === val) {
                    return i;
                }
            }
            return -1;
        }
        //根据知识点关键字匹配得分点
        function matchAnswerItem(matchText) {
            if (matchText === undefined)
                return;
            //移除为空的得分点
            //answerItems = $.grep(answerItems, function (item) {
            //    return item.value !== "";
            //});
            var items = [];
            $.each(answerItems,
                function (k, v) {
                    var selectAnswer = $("#answer-value-" + v.index).textbox("getValue");
                    var selectAnswerScores = $("#answer-ratio-" + v.index).numberbox("getValue");
                    items.push({ value: selectAnswer, ratio: selectAnswerScores });

                });

            //将匹配的文字加入得分点
            $.each(matchText,
                function (i, item) {
                    //搜索得分点是否包含当前项,不存在则新增
                    if (findElem(items, "value", item) < 0)
                        items.push({ value: item, ratio: 10 });

                });
            //重新初始化得分点
            answerItems.length = 0;
            $(".answer-item").remove();
            initAnswerItem(items);
        }
        //删除知识点同时删除知识点相关得分点
        function deleteAnswerItemByLabel(matchText) {
            if (matchText === undefined)
                return;
            //移除为空的得分点
            //answerItems = $.grep(answerItems, function (item) {
            //    return item.value !== "";
            //});

            var items = [];
            $.each(answerItems,
                function (k, v) {
                    var selectAnswer = $("#answer-value-" + v.index).textbox("getValue");
                    var selectAnswerScores = $("#answer-ratio-" + v.index).numberbox("getValue");
                    items.push({ value: selectAnswer, ratio: selectAnswerScores });

                });
            $.each(matchText,
                function (i, item) {
                    var index = findElem(items, "value", item);
                    //搜索得分点是否包含当前项,存在则删除
                    if (index >= 0)
                        items.splice(index, 1); // edit.onDeleteAnswerItem(index);
                });
            // 重新初始化得分点
            answerItems.length = 0;
            $(".answer-item").remove();
            initAnswerItem(items);
        }

        function save (callback) {
            if (!$("#edit-form").form("validate")) {
                return;
            }

            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            var questionText = ueDic["questionText"].getContent();

            if (stringIsEmpty(ueDic["questionText"].getContentTxt().trim())) {
                $.messager.alert("提示", "请填写题干！", "info");
                ueDic["questionText"].focus();
                return;
            }

            if ($.inArray($("#questionBaseTypeCode").combobox("getValue"), ["single", "multi"]) !== -1) {
                if (options.length === 0) {
                    $.messager.alert("提示", "必须有选项进行选择！", "info");
                    return;
                }

                if ($("input[name='option']:checked").length === 0) {
                    $.messager.alert("提示", "请先选择一个正确答案！", "info");
                    return;
                }

                var emptyCount = 0;
                $.each(options, function (k, v) {
                    if (stringIsEmpty(v.ue.getContentTxt().trim())) {
                        emptyCount++;
                        v.ue.focus();
                        return false;
                    }
                    return true;//其实这里可以不用return
                });
                if (emptyCount > 0) {
                    $.messager.alert("提示", "选项内容不可为空！", "info");
                    return;
                }
            }


            var param = getFormParam();
            param.questionText = questionText;

            VE.Mask("");
            var words = "创建试题";
            var method = "Create";
            if (!stringIsEmpty(dataCache.id)) {
                words = "更新试题";
                method = "Update";
                param.id = dataCache.id;
            }

            var url = serviceUrl + "ChallengeQuestion/" + method;
            nv.post(url,
                param,
                function (data) {
                    VE.UnMask();
                    if (data.success) {
                        evtBus.dispatchEvt("flush_child_questions");
                        $.messager.show({ title: "提示", msg: words + "成功" });
                        callback(true);
                        if (method === "Create") {
                            dataCache = data.result;
                            fillForm(data.result);
                        }
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                        callback(false);
                    }
                });
        }

     

        function initUEditor() {
            ueDic["questionText"] = UE.getEditor("questionText", {
                toolbars: ueToolbars,
                initialFrameWidth: 700,
                initialFrameHeight: 100,
                autoFloatEnabled: false,
                elementPathEnabled: false, //元素路径
                autoHeightEnabled : false
            });
            ueDic["questionText"].addListener('blur', function(editor) {
                edit.matchLabel();
            });
            ueDic["questionAnalysis"] = UE.getEditor("questionAnalysis", {
                toolbars: ueToolbars,
                initialFrameWidth: 700,
                initialFrameHeight: 100,
                autoFloatEnabled: false,
                elementPathEnabled: false, //元素路径
                autoHeightEnabled: false
            });
        }

        function getFormParam() {
            var param = {};
            if (dataCache) {
                for (var key in dataCache) {
                    if (dataCache.hasOwnProperty(key)) {
                        param[key] = dataCache[key];
                    }
                }
            }
            param.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
            param.folderUid = $("#folderUid").combotree("getValue");
            param.questionBaseTypeCode = $("#questionBaseTypeCode").combobox("getValue");
            param.questionCode = $("#questionCode").textbox("getValue");
            param.title = $("#title").textbox("getValue");
            param.score = $("#score").numberbox("getValue");
            //param.examTime = getExamTimeSecond();
            param.questionStatusCode = $("#questionStatusCode").combobox("getValue");
            param.hardGrade = $("#hardGrade").combobox("getValue");
            param.standardAnswer = $("#standardAnswer").textbox("getValue");
            //param.inOrder = $("#inOrder").combobox("getValue");
            param.questionAnalysis = ueDic["questionAnalysis"].getContent();
            param.listOrder = $("#listOrder").numberbox("getValue");
            param.label = $("#label").val();
            param.secLabel = $("#seclabel").val();
            if (!stringIsEmpty(parentId)) {
                param.parentQuestionUid = parentId;
            }
            if ($.inArray(param.questionBaseTypeCode, ["single", "multi"]) !== -1) {
                var selectAnswers = [];
                var standardAnswers = [];
                $.each(options, function (k, v) {
                    selectAnswers.push(v.ue.getContent());
                });
                $("input[name='option']:checked")
                    .each(function () {
                        standardAnswers.push(String.fromCharCode(parseInt($(this).val()) + 65));
                    });
                param.standardAnswer = standardAnswers.join("|");
                param.selectAnswer = selectAnswers.join("|");
                param.answerNum = selectAnswers.length;
            } else if ($.inArray(param.questionBaseTypeCode, ["answer", "operate", "program"]) !== -1) {
                var selectAnswers2 = [];
                var standardAnswers2 = [];
                var selectAnswerScores = [];
                $.each(answerItems,
                    function (k, v) {
                        var selectAnswer = $("#answer-value-" + v.index).textbox("getValue");
                        selectAnswers2.push(selectAnswer);
                    });
                if (selectAnswers2.length > 0) {
                    $("input[name='answerItem']:checked")
                        .each(function () {
                            standardAnswers2.push(String.fromCharCode(parseInt($(this).val()) + 65));
                        });
                    for (var i = 0; i < selectAnswers2.length; i++) {
                        selectAnswerScores.push($("#answer-ratio-" + i).numberbox("getValue"));
                    }

                    param.selectAnswer = selectAnswers2.join("$#$");
                    param.selectAnswerScore = selectAnswerScores.join("|");
                    param.answerNum = selectAnswers2.length;

                  
                }
            } else if (param.questionBaseTypeCode === "judge") {
                param.standardAnswer = $("input[name='judge']:checked").val();
            } else if (param.questionBaseTypeCode === "judge_correct") {
                var value = $("input[name='judge']:checked").val();
                if (value !== "N") {
                    param.standardAnswer = value;
                }
            }
            if (param.questionBaseTypeCode === "program" || param.questionBaseTypeCode === "program_fill") {
                param.param = $("#param").textbox("getValue");
                param.inputParam = $("#inputParam").textbox("getValue");
                param.multiTest = $("#multiTest").prop("checked");
                param.preinstallCode = $("#preinstallCode").textbox("getValue");
                param.standardCode = editor.getValue();
            }
            return param;
        }

        function fillForm(data) {
            $("#questionBaseTypeCode").combobox("setValue", data.questionBaseTypeCode).combobox("disable");
            var funcText = data.isCustomCode ? "uncheck" : "check";
            $("#isCustomCode").switchbutton(funcText);
            $("#folderUid").combotree("setValue", data.folderUid);
            $("#questionCode").textbox("setValue", data.questionCode);
            $("#language").combobox("setValue", data.language);
            $("#title").textbox("setValue", data.title);
            $("#score").numberbox("setValue", data.score);
            
            //$("#examTime").timespinner("setValue", formatTime(data.examTime));
            $("#questionStatusCode").combobox("setValue", data.questionStatusCode);
            $("#hardGrade").combobox("setValue", data.hardGrade);
            $("#listOrder").numberbox("setValue", data.listOrder);
            $("#standardAnswer").textbox("setValue", data.standardAnswer);
            $("#inOrder").combobox("setValue", data.inOrder);
            $("#seclabel").val(data.secLabel).trigger("change");
            $("#label").val(data.label).trigger("change");
            ueDic["questionText"].ready(function() {
                ueDic["questionText"].setContent(data.questionText);
            });
            ueDic["questionAnalysis"].ready(function() {
                ueDic["questionAnalysis"].setContent(data.questionAnalysis);
            });

            if (data.questionBaseTypeCode === "single") {
                questionTypeChangeSingle(data.selectAnswer.split("|"));
                var index = data.standardAnswer.charCodeAt() - 65;
                $("input:radio[name='option'][value='" + index + "']").attr("checked", "checked");
            } else if (data.questionBaseTypeCode === "multi") {
                questionTypeChangeMulti(data.selectAnswer.split("|"));
                var answers = data.standardAnswer.split("|");
                $.each(answers, function(k, v) {
                    var answerIndex1 = v.charCodeAt() - 65;
                    $("input:checkbox[name='option'][value='" + answerIndex1 + "']").attr("checked", "checked");
                });
            } else if (data.questionBaseTypeCode === "judge") {
                $("input:radio[name='judge'][value='" + data.standardAnswer + "']").attr("checked", "checked");
            } else if (data.questionBaseTypeCode === "judge_correct" && data.standardAnswer !== "Y") {
                $("input:radio[name='judge'][value='N']").attr("checked", "checked");
                $(".dynamic.judge-correct-wrong").show();
            } else if ($.inArray(data.questionBaseTypeCode, ["answer", "operate", "program", "program_fill"]) !== -1) {
                var itemTexts = data.selectAnswer.split("$#$");
                var ratios = data.selectAnswerScore.split("|");
                var items = [];
                for (var i = 0; i < data.answerNum; i++) {
                    items.push({ value: itemTexts[i], ratio: ratios[i] });
                }
                if (data.questionBaseTypeCode === "program") {
                    $("#param").textbox("setValue", data.param);
                    $("#inputParam").textbox("setValue", data.inputParam);
                    $("#multiTest").prop("checked", data.multiTest);
                    $("#preinstallCode").textbox("setValue", data.preinstallCode);
                    questionTypeChangeProgram(items);
                } else if (data.questionBaseTypeCode === "program_fill") {
                    $("#param").textbox("setValue", data.param);
                    $("#inputParam").textbox("setValue", data.inputParam);
                    $("#multiTest").prop("checked", data.multiTest);
                    $("#preinstallCode").textbox("setValue", data.preinstallCode);
                    questionTypeChangeProgramFill(items);
                } else {
                    questionTypeChangeAnswer(items);
                }
            }
            if (editor !== undefined && editor !== null) {
                editor.setValue(data.standardCode);
            } else {
                setTimeout(function () { editor.setValue(data.standardCode); }, 3000);
            }
        }

        //创建单选题|多选题选项
        function createOption(index, value, type) {
            var indexCode = String.fromCharCode(65 + index);
            var html = '<div id="option-form-group-{3}" class="form-group">\
            <label class="form-label">{0}：</label>\
            <script id="option-{3}" type="text/plain" style="display:inline-block;">{1}</script>\
            <label style="vertical-align:baseline;"><input type="{2}" name="option" value="{3}">正确答案</label>\
            <a id="option-delete-btn-{3}" href="javascript:void(0)" class="easyui-linkbutton" onclick="edit.onDeleteOption({3})" iconCls="icon-busy" title="删除" plain="true" style="margin-left:20px;">删除</a>\
        </div>';
            var id = "option-" + index;
            html = html.format("选项" + indexCode, value, type, index);
            $("#option-add").before(html);
            var ue = UE.getEditor(id, {
                toolbars: ueToolbars,
                initialFrameWidth: 700,
                initialFrameHeight: 100,
                autoFloatEnabled: false,
                elementPathEnabled: false, //元素路径
                autoHeightEnabled: false
            });
            options.push({
                data: {
                    index: index,
                    value: value,
                    type: type
                },
                ue: ue
            });
            $("#option-delete-btn-" + index).linkbutton();
        }

        //创建问答题|编程题得分项
        function createAnswerItem(index, value, ratio) {
            var indexCode = String.fromCharCode(65 + index);
            var html = '<div id="answer-item-form-group-{2}" class="form-group answer-item">\
            <label class="form-label">{0}：</label>\
            <input id="answer-value-{2}" class="easyui-textbox" style="width:400px" value="{1}">\
            <span style="margin-left:20px">比例：<input id="answer-ratio-{2}" class="easyui-numberbox" min="0" value="{3}" style="width:50px;">%</span>\
            <a id="answer-item-delete-btn-{2}" href="javascript:void(0)" class="easyui-linkbutton" iconCls="icon-remove" title="删除" plain="true" onclick="edit.onDeleteAnswerItem({2})" style="margin-left:20px;">删除</a>\
        </div>';
            html = html.format("得分点" + indexCode, value, index, ratio);
            $("#answer-item-add").before(html);
            answerItems.push({ index: index, value: value, ratio: ratio });
            $("#answer-value-" + index).textbox();
            $("#answer-ratio-" + index).numberbox();
            $("#answer-item-delete-btn-" + index).linkbutton();
        }

        //选中 判断题|填空题|编程题|语音题|操作题|打字题
        function questionTypeChange(type) {
            $(".dynamic").hide();
            $(".dynamic." + type).show();
        }

        //选中单选题
        function questionTypeChangeSingle(array) {
            var optionNum = 4;
            var hasOptions = false;
            if (!array && !stringIsEmpty(dataCache.id)) {
                array = dataCache.selectAnswer.split("|");
            }
            if (array) {
                optionNum = array.length;
                hasOptions = true;
            }
            boxTypeCache = "radio";
            if ($("#option-group input[name='option']").length === 0) {
                for (var i = 0; i < optionNum; i++) {
                    createOption(i, hasOptions ? array[i] : "", boxTypeCache);
                }
                $("#option-group .easyui-textbox").textbox();
                $("#option-group .easyui-linkbutton").linkbutton();
            } else {
                $("#option-group input[name='option']").attr("type", boxTypeCache);
            }

            questionTypeChange("single");
        }

        //选中多选题
        function questionTypeChangeMulti(array) {
            var optionNum = 4;
            var hasOptions = false;
            if (!array && !stringIsEmpty(dataCache.id)) {
                array = dataCache.selectAnswer.split("|");
            }

            if (array) {
                optionNum = array.length;
                hasOptions = true;
            } 
            boxTypeCache = "checkbox";
            if ($("#option-group input[name='option']").length === 0) {
                for (var i = 0; i < optionNum; i++) {
                    createOption(i, hasOptions ? array[i] : "", boxTypeCache);
                }
                $("#option-group .easyui-textbox").textbox();
                $("#option-group .easyui-linkbutton").linkbutton();
            } else {
                $("#option-group input[name='option']").attr("type", boxTypeCache);
            }

            questionTypeChange("multi");
        }

        function initAnswerItem(items) {
            $(".answer-item").remove();
            answerItems = [];
            var itemNum = 0;
            var hasItems = false;
            if (items) {
                itemNum = items.length;
                hasItems = true;
            }
            if ($("#answer-item-group input[name='answerItem']").length === 0) {
                for (var i = 0; i < itemNum; i++) {
                    createAnswerItem(i, hasItems ? items[i].value : "", hasItems ? items[i].ratio : 0);
                }
                $("#answer-item-group .easyui-textbox").textbox();
                $("#answer-item-group .easyui-linkbutton").linkbutton();
                $("#answer-item-group .easyui-numberbox").numberbox();
            }
        }

        //选中问答题
        function questionTypeChangeAnswer(items) {
            initAnswerItem(items);
            questionTypeChange("answer");
        }

        //选中编程题
        function questionTypeChangeProgram(items) {
            initAnswerItem(items);
            questionTypeChange("program");
        }
        //选中编程填空题
        function questionTypeChangeProgramFill(items) {

            initAnswerItem(items);
            questionTypeChange("program_fill");
        }
        //选中判断改错题
        function questionTypeChangeJudgeCorrect() {
            questionTypeChange("judge-correct");
            var value = $("input[name='judge']:checked").val();
            $("#standardAnswer").textbox("setValue", value);
        }

        function questionTypeAddEvent() {
            $("#questionBaseTypeCode")
                .combobox({
                    onChange: function(newValue) {
                        self.onQuestionTypeChange(newValue);
                    }
                });
        }
        

        function getQuestionDataCallback(data) {
            dataCache = data;
            fillForm(dataCache);
        }

        function getQuestionData(id, callback) {
            var url = serviceUrl + "ChallengeQuestion/Get?id=" + id;
            VE.Mask("");
            nv.get(url,
                function(data) {
                    VE.UnMask();
                    if (data.success) {
                        if (callback) {
                            callback(data.result);
                        }
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getExamTimeSecond() {
            var $examTime = $("#examTime");
            return $examTime.timespinner("getHours") * 3600 + $examTime.timespinner("getMinutes") * 60 + $examTime
                .timespinner("getSeconds");
        }
        function initLabelSelect() {
            //推荐标签初始化
            $('#recommendlabel').select2({
                placeholder: "智能推荐标签"
            }).on("select2:unselect",
                function (e) {
                    layer.confirm('请选择标签标记位置？', {
                        btn: ['主标签', '辅标签'] //按钮
                    }, function () {
                        var label = $("#label").val() == null ? [] : $("#label").val();
                        if ($.inArray(e.params.data.id, label) < 0) {
                            label.push(e.params.data.id);
                        }
                        $("#label").val(label).trigger("change");
                        //更新得分点
                        matchAnswerItem(matchTextDic[e.params.data.id]);
                        layer.closeAll();
                    }, function () {
                        var seclabel = $("#seclabel").val() == null ? [] : $("#seclabel").val();
                        if ($.inArray(e.params.data.id, seclabel) < 0) {
                            seclabel.push(e.params.data.id);
                        }
                        $("#seclabel").val(seclabel).trigger("change");
                    });
                    //将删除的标签恢复到推荐列表中
                    var recommendlabel = $("#recommendlabel").val() == null ? [] : $("#recommendlabel").val();
                    if ($.inArray(e.params.data.id, recommendlabel) < 0) {
                        recommendlabel.push(e.params.data.id);
                    }
                    $("#recommendlabel").val(recommendlabel).trigger("change");
                });
            //辅标签初始化
            $('#seclabel').select2({
                placeholder: "请选择辅知识点"
            }).on("select2:select",
                function (e) {
                    //如果主标签内含有辅标签选中的标签 删除主知识点内标签
                    var label = $("#label").val() == null ? [] : $("#label").val();
                    if ($.inArray(e.params.data.id, label) > -1) {
                        label.splice($.inArray(e.params.data.id, label), 1);
                    }
                    $("#label").val(label).trigger("change");
                }).on("select2:unselect",
                function (e) {


                });
            //主知识点初始化
            $("#label").select2({
                placeholder: "请选择主知识点"
            }).on("select2:select",
                function (e) {
                    var seclabel = $("#seclabel").val() == null ? [] : $("#seclabel").val();
                    if ($.inArray(e.params.data.id, seclabel) > -1) {
                        seclabel.splice($.inArray(e.params.data.id, seclabel), 1);
                    }
                    $("#seclabel").val(seclabel).trigger("change");
                    //更新得分点
                    matchAnswerItem(matchTextDic[e.params.data.id]);
                }).on("select2:unselect",
                function (e) {

                    deleteAnswerItemByLabel(matchTextDic[e.params.data.id]);
                });
            //加载知识点
            $.getJSON("/api/services/app/liblabel/LoadLabelForChoose",
                function (data) {
                    $('#label').empty(); //清空下拉框
                    $('#seclabel').empty(); //清空下拉框
                    $.each(data.result,
                        function (i, item) {
                            $('#recommendlabel').append("<option value='" + item.id + "'>" + item.title + "</option>");
                            $('#label').append("<option value='" + item.id + "'>" + item.title + "</option>");
                            $('#seclabel').append("<option value='" + item.id + "'>" + item.title + "</option>");
                        });


                });
        }

        $(function () {
            initLabelSelect();
            initUEditor();
            questionTypeAddEvent();
            category.getCategory(function() {
                if (stringIsEmpty(id)) {
                    self.onQuestionTypeChange("program");
                } else {
                    getQuestionData(id, getQuestionDataCallback);
                }
            });
      
            
        });
    }

    return init;
})();

var ComposeQuestion = (function() {

    function init(id) {
        var self = this;
        var dataCache = {id:id}; //缓存已有的question数据
        var category = new nv.category.CombotreeDataClass("folderUid", "challenge_cpp");
        var ueDic = {}; //UE富文本编辑器字典，key为id, value为UE对象
        var paramCache = { skip: 0, pageSize: 30, parentQuestionUid: id }; //缓存子试题分页查询条件
        var flushChildQuestions = false;//在页面tab激活时用来判断是否刷新表格样式
        var tabHelper = new TabHelper("tabs");
       
        var handle = evtBus.addEvt("flush_child_questions", function() {
            getChildQuestionData(paramCache);
            flushChildQuestions = true;
        });

        var handle2 = evtBus.addEvt("tabs_tab_change", function(data) {
            if (data.index !== tabHelper.getTabIndex() || !flushChildQuestions) {
                return;
            }
            $("#dg").datagrid("fixRowHeight");
            flushChildQuestions = false;
            getQuestionData();
        });

        $(window).unload(function() {
                evtBus.removeEvt(handle);
                evtBus.removeEvt(handle2);
            });
        
        //===============Event Begin===========================
        this.onPage = function(pageNumber, pageSize) {
            paramCache.skip = (pageNumber - 1) * pageSize;
            if (paramCache.skip < 0) {
                paramCache.skip = 0;
            }
            paramCache.pageSize = pageSize;
            getChildQuestionData(paramCache);
        };

        this.onSaveAndClose = function() {
            save(function(success) {
                if (!success) {
                    return;
                }
                evtBus.dispatchEvt("flush_questions");
                tabHelper.closeTab();
            });
        };

        this.onSaveAndNewlyAdd = function () {
            save(function (success) {
                if (!success) {
                    return;
                }
                location.reload();
            });
        };

        this.onSaveOnly = function() {
            save(function() {
            });
        }
      
        this.onAddChildQuestion = function() {
            var iframeHtml =
                '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:99%;"></iframe>';
            parent.$("#tabs")
                .tabs("add",
                {
                    title: "新增子试题",
                    content: iframeHtml.format({
                        title: "新增子试题",
                        url: "/Challenge/Manage/Edit?parentId=" + dataCache.id
                    }),
                    closable: true,
                    icon: "icon-add"
                });
        };

        this.onEditChildQuestion = function() {
            var rows = $("#dg").datagrid("getChecked");
            var iframeHtml =
                '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:99%;"></iframe>';
            $.each(rows,
                function(k, v) {
                    parent.$("#tabs")
                        .tabs("add",
                        {
                            title: "编辑子试题",
                            content: iframeHtml.format({
                                title: "编辑子试题",
                                url: "/Challenge/Manage/Edit?id={0}&parentId={1}".format(v.id, dataCache.id)
                            }),
                            closable: true,
                            icon: "icon-edit"
                        });
                });
        };

        this.onDeleteChildQuestion = function() {
            var checkedRows = $("#dg").datagrid("getChecked");

            if (!checkedRows || checkedRows.length === 0) {
                $.messager.alert("提示", "请先勾选要删除的试题！", "info");
                return;
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function(b) {
                    if (!b) {
                        return;
                    }
                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }
                    var idArray = [];
                    $.each(checkedRows,
                        function(k, v) {
                            idArray.push(v.id);
                        });

                    VE.Mask("");
                    var url = serviceUrl + "ChallengeQuestion/Delete?ids=" + idArray.join(",");
                    nv.get(url,
                        function(data) {
                            VE.UnMask();
                            if (data.success) {
                                $.messager.show({ title: "提示", msg: "删除成功!" });
                                getChildQuestionData(paramCache);
                            } else {
                                $.messager.alert("提示", data.error.message, "info");
                            }
                        });
                });
        };

        this.onChangeCodeMode = function (checked) {
            var isCustomCode = !checked;
            $("#questionCode").textbox("readonly", !isCustomCode)
                .textbox({ required: isCustomCode });
            if (!isCustomCode) {
                $("#questionCode").textbox("setValue", stringIsEmpty(dataCache.questionCode) ? "" : dataCache.questionCode);
            }
        };
        //============Event End===============================

        function save(callback) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var param = {};
            if (dataCache) {
                for (var key in dataCache) {
                    param[key] = dataCache[key];
                }
            }
            param.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
            param.questionBaseTypeCode = "compose";
            param.folderUid = $("#folderUid").combotree("getValue");
            param.questionCode = $("#questionCode").textbox("getValue");
            param.score = $("#score").numberbox("getValue");
           // param.examTime = getExamTimeSecond();
            param.questionStatusCode = $("#questionStatusCode").combobox("getValue");
            param.hardGrade = $("#hardGrade").combobox("getValue");
            param.questionText = ueDic["questionText"].getContent();
            param.questionAnalysis = ueDic["questionAnalysis"].getContent();
            param.listOrder = $("#listOrder").numberbox("getValue");

            VE.Mask("");
            var words = "创建试题";
            var method = "Create";
            if (!stringIsEmpty(dataCache.id)) {
                words = "更新试题";
                method = "Update";
                param.id = dataCache.id;
            }

            var url = serviceUrl + "ChallengeQuestion/" + method;
            nv.post(url,
                param,
                function (data) {
                    VE.UnMask();
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: words + "成功" });
                        callback(true);
                        if (method === "Create") {
                            dataCache = data.result;
                            fillForm(dataCache);
                            $("#child-table").show();
                            $("#dg").datagrid("resize");
                            getChildQuestionData(paramCache);
                        }
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                        callback(false);
                    }
                });
        }

        function initUEditor() {
            ueDic["questionText"] = UE.getEditor("questionText", {
                toolbars: ueToolbars,
                initialFrameWidth: 700,
                initialFrameHeight: 100,
                autoFloatEnabled: false,
                elementPathEnabled: false, //元素路径
                autoHeightEnabled: false
        });
            ueDic["questionAnalysis"] = UE.getEditor("questionAnalysis", {
                toolbars: ueToolbars,
                initialFrameWidth: 700,
                initialFrameHeight: 100,
                autoFloatEnabled: false,
                elementPathEnabled: false, //元素路径
                autoHeightEnabled: false
            });
        }

        function fillForm(data) {
            paramCache.parentQuestionUid = data.id;
            var funcText = data.isCustomCode ? "uncheck" : "check";
            $("#isCustomCode").switchbutton(funcText);
            $("#folderUid").combotree("setValue", data.folderUid);
            $("#questionCode").textbox("setValue", data.questionCode);
            $("#score").numberbox("setValue", data.score);
           // $("#examTime").timespinner("setValue", formatTime(data.examTime));
            $("#questionStatusCode").combobox("setValue", data.questionStatusCode);
            $("#hardGrade").combobox("setValue", data.hardGrade);
            $("#listOrder").numberbox("setValue", data.listOrder);
            ueDic["questionText"].ready(function() {
                ueDic["questionText"].setContent(data.questionText);
            });
            ueDic["questionAnalysis"].ready(function() {
                ueDic["questionAnalysis"].setContent(data.questionAnalysis);
            });
        }

        function getQuestionData() {
            var url = serviceUrl + "ChallengeQuestion/Get?id=" + dataCache.id;
            VE.Mask("");
            nv.get(url,
                function(data) {
                    VE.UnMask();
                    if (data.success) {
                        dataCache = data.result;
                        fillForm(dataCache);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getChildQuestionData(param) {
            var url = serviceUrl + "ChallengeQuestion/GetChildPagination";
            nv.post(url, param,
                function(data) {
                    if (data.success) {
                        $("#dg").datagrid("loadData", { rows: data.result.rows, total: data.result.total });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getExamTimeSecond() {
            var $examTime = $("#examTime");
            return $examTime.timespinner("getHours") * 3600 + $examTime.timespinner("getMinutes") * 60 + $examTime
                .timespinner("getSeconds");
        }
        $(function() {
            initUEditor();

            category.getCategory();
            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: self.onPage,
                    onChangePageSize: function(pageSize) {
                        paramCache.pageSize = pageSize;
                        getChildQuestionData(paramCache);
                    }
                });

            if (!stringIsEmpty(id)) {
                getQuestionData();
                paramCache.parentQuestionUid = dataCache.id;
                getChildQuestionData(paramCache);
                $("#child-table").show();
                $("#dg").datagrid("resize");
            }
        });
    }

    return init;
})();

var dgFormatter = {
    outdatedDate: function(value) {
        if (value === 0) {
            return "不限";
        }
        return new Date(value * 1000).format("yyyy-MM-dd hh:mm:ss");
    },
    questionStatusCode: function(value) {
        switch (value) {
        case "normal":
            return "正常";
        case "outdated":
            return "已过期";
        case "disabled":
            return "禁用";
        case "draft":
            return "草稿";
        default:
            return "正常";
        }
    }
};

function formatTime(second) {
    if (!$.isNumeric(second)) {
        return "00:00:00";
    }

    var h = Math.floor(second / 3600);
    var m = Math.floor(second % 3600 / 60);
    var s = second % 60;

    var fmt = function (t) {
        if (t < 10) {
            return "0" + t;
        }
        return "" + t;
    }
    return fmt(h) + ":" + fmt(m) + ":" + fmt(s);
}