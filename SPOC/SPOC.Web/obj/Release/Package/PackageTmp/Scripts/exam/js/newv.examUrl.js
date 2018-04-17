//站点地址
function GetExamUrl() {
    var doMainUrl = "http://127.0.0.1:32889";
    this.GetUrl = function () {
        return doMainUrl;
    };
    this.SetUrl = function (value) {
        doMainUrl = value;
    }
}
