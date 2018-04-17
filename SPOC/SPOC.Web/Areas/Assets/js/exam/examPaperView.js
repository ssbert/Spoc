
var exam = {

};



exam.UserPaperViewModule = (function () {
    var scope, http, sce;

    function UserPaperViewModule(moduleName, controllerName) {
        var module = angular.module(moduleName, []);
        module.controller(controllerName, controllerFunc);
        module.filter('to_trusted', ['$sce', function ($sce) {
            return function (text) {
                return $sce.trustAsHtml(text);
            };
        } ]);
       
    }

    function controllerFunc($scope, $http, $sce) {
        scope = $scope;
        http = $http;
        sce = $sce;
        scope.filterType = "";
        changeViewRange();
        scope.ChangeViewRange = changeViewRange;
    }

    function changeViewRange() {

        var url = "/api/services/app/ExamExam/GetUserPaperView";
        var params = {
            examGradeUid: examGradeUid,
            filterType: $('#tabFilter input[name="uncertain"]:checked').val()
        };

        http.post(url, params).success(function (data) {
            scope.data = data.result;
            //页面加载完毕动态获取网页高度 设置body高度
            setTimeout(function () {

                $("#divBody").css("height", $("form").height());
                $("#divBody").css("overflow", "scroll");
            }, 500);
        });
    }
    
    return UserPaperViewModule;
})();


