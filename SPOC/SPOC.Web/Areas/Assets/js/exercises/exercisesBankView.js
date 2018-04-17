var ExercisesBankViewClass = (function() {
    function init(param) {
        var layerIndex = parent.layer.getFrameIndex(window.name);
//lessonId, listOrder, questionType, standardAnswer
        this.submit = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }

            $(".answer").attr("disabled", "disabled");
            var userAnswer = getUserAnswer();
            var isPass = param.standardAnswer === userAnswer;
            var $passText = $("#passText");
            if (isPass) {
                $passText.text("恭喜您，答对了！");
                $passText.addClass("text-success");
            } else {
                $passText.text("真可惜，答错了！");
                $passText.addClass("text-danger");
            }

            nv.post("/api/services/app/ExercisesBankLearn/Submit", {
                id: param.learnId,
                userAnswer: userAnswer,
                listOrder: param.listOrder,
                questionId: param.questionId,
                lessonId: param.lessonId,
                isPass: isPass
            });
        };

        this.next = function() {
            window.location.href = "/StudyPlatform/Learn/ExercisesBankView?lessonId=" + param.lessonId 
                + "&listOrder=" + param.nextListOrder;
        };

        this.jump = function(listOrder) {
            window.location.href = "/StudyPlatform/Learn/ExercisesBankView?lessonId=" + param.lessonId 
                + "&listOrder=" + listOrder;
        };

        this.close = function (courseId, lessonId) {
            try {
                lessonLearn.saveExerciseLearnRecord(courseId, lessonId,closeTab);
            } catch (ex)
            { }
           
        };
        function closeTab() {
            parent.layer.close(layerIndex);
        }
        function getUserAnswer() {
            switch (param.questionType) {
            case "fill":
                return getFillUserAnswer();
            case "multi":
            case "single":
                return getSingleMultiUserAnswer();
            case "judge":
                return getJudgeUserAnswer();
            case "answer":
                return getAnswerUserAnswer();
            default:
                return "";
            }
        }

        //填空题判断
        function getFillUserAnswer() {
            var array = [];
            $("input.answer").each(function () {
                var $this = $(this);
                var index = parseInt($this.attr("index"));
                array[index] = $this.val();
            });
            var userAnswer = array.join("|");
            return userAnswer;
        }

        //单选、多选判断
        function getSingleMultiUserAnswer() {
            var array = [];
            $("input.answer:checked").each(function () {
                var $this = $(this);
                var index = parseInt($this.attr("index"));
                array.push({ index: index, value: $this.val() });
            });
            var answerArray = [];
            if (array.length > 1) {
                array.sort(function(a, b) {
                    return a.index - b.index;
                });
            }
            for (var i = 0; i < array.length; i++) {
                answerArray.push(array[i].value);
            }
            var userAnswer = answerArray.join("|");
            return userAnswer;
        }

        //判断题判断
        function getJudgeUserAnswer() {
            var userAnswer = $("input.answer:checked").val();
            return userAnswer;
        }
        //问答题判断
        function getAnswerUserAnswer() {
            var userAnswer = $(".answer").val();
            return userAnswer;
        }
    };

    return init;
})();