var saveAsnwerByControl = function () {
    this.DeleteUserExamAnswer = function (examUserUid, examUid, examGradeUid) {
        try {
            var errorMessage = this.controlObject.DelUserAnswer(examUserUid, examUid, examGradeUid);
            if (errorMessage != "true") {
               
            }
        }
        catch (e) {
           
        }
    }

   this.SaveUserExamAnswerToServer = function (userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,successEvent, errorEvent) {
        try {
            var writeResult = this.controlObject.WriteUserAnswerToServer(userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,successEvent, errorEvent);
        }
        catch (e) {
            alert("示：" + e.message);
        }
    }
    this.SaveUserExamAnswer = function (userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,userAnswer,isBackupToBox) {
        try {
            var writeResult = this.controlObject.WriteUserAnswer(userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,userAnswer,isBackupToBox);
        }
        catch (e) {
            alert("示：" + e.message);
        }
    }

    this.SubmitExam2 = function (userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,userAnswer,successEvent, errorEvent) {
        try {
            var writeResult = this.controlObject.SubmitExam2(userUid,examUid,examArrangeUid,examPaperUid,examGradeUid,userAnswer,successEvent, errorEvent);
        }
        catch (e) {
            alert("示：" + e.message);
        }
    }


    //锁住键盘特定键
    this.LockKeyboard = function () {
        try {
            this.controlObject.lockKeyboard('all');
        } catch (e) {
        }
        try {
            this.controlObject.CheckWinLock();
        } catch (e) {
        }
    }

    //解除键盘特定键
    this.UnlockKeyboard = function () {
        try {
            this.controlObject.unlockKeyboard('all');
        } catch (e) {
        }
    }

    //清空剪切版
    this.ClearClipboard = function (datatype) {
        try {
            if (typeof (datatype) == "undefined" || datatype == "") {
                this.controlObject.ClipboardClear();
            }
            else {
                this.controlObject.ClipboardClearByType("b4fNedFvkTIwKdB7CpZNhY8rPS36put46hAazqnsUoRfZc/y3nmzwL5M6iL0fmYKYFfDX7BP9vo=", datatype);
            }
        } catch (e) { }
    }
}