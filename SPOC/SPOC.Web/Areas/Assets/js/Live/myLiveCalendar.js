(function (angular) {
    angular.module('liveDate-app', []).controller('liveDate-ctrl', Controller);

    function Controller($scope, $http) {
        // 获取要随时间而更改的日期dom元素
        var dateArea = $('.day-area');
        // 获取左右切换按钮
        var arrLeft = $('.arrow-left');
        var arrRight = $('.arrow-right');
        var todayShow = $('.today-show');
        var weekday = $('.day li span');
       
        // 获取当前日期
        var date = new Date(),
            yeath = date.getFullYear(),
            month = date.getMonth() + 1,
            day = date.Format("yyyy-MM-dd"),

        // 存储当前日期变量
            currentDate = date,
            currentYeath = yeath,
            currentMonth = month,
            currentDay = date.getDate(),
        // 定义日期区间的终止日期变量	
            lastDate,
            lastYeath,
            lastMonth,
            lastDay,
        // 定义日期区间初始首日期，用于返回初始化日期
            initDate,
            initYeath,
            initMonth,
            initDay,
        // 定义日期区间初始终止日期，用于返回初始化日期
            initLastDate,
            initLastYeath,
            initLastMonth,
            initLastDay;
        //$scope.liveDate = null;
        toggleDay({
            'dateArea': dateArea,
            'arrLeft': arrLeft,
            'arrRight': arrRight,
            'todayShow': todayShow,
            'weekday': weekday,
            'ajax': {
                'type': 'get',
                'url': '/api/services/app/live/GetLiveLessonCalendar',
                'data': {
                    'frist': day,
                    'last': lastDay,
                    'searchType':searchType
                },
                'success': function (data) {
                    if (data.success) {
                        $scope.liveDate = data.result;
                        $scope.$apply();
                    } else {

                    }
                },
                'beforeSend': function (xhr) {

                }
            }
        });


        function toggleDay(obj) {
            var dateArea = obj.dateArea,
                arrLeft = obj.arrLeft,
                arrRight = obj.arrRight,
                todayShow = obj.todayShow,
                weekday = obj.weekday;
           var  ajaxType = obj.ajax.type || 'get',
            ajaxUrl = obj.ajax.url,
            ajaxBeforeSend = obj.ajax.beforeSend,
            ajaxSuccess = obj.ajax.success,
            ajaxData = obj.ajax.data;

            // 给Date对象添加addDay方法
            Date.prototype.addDay = function (dates) {
                var num = 24 * 60 * 60 * 1000;
                return new Date(Date.parse(this) + dates * num);
            };

            if (date.getDay() != 1) {
                date = date.addDay(-(date.getDay() - 1));
                initDate = date;
                yeath = initYeath = date.getFullYear();
                month = initMonth = date.getMonth() + 1;
                day = initDay = date.getDate();
            } else {
                initDate = date;
                initYeath = yeath;
                initMonth = month;
                initDay = day;
            }

            lastDate = initLastDate = date.addDay(6);
            lastYeath = initLastYeath = lastDate.getFullYear();
            lastMonth = initLastMonth = lastDate.getMonth() + 1;
            lastDay = initLastDay = lastDate.getDate();
            ajaxData.frist = date.Format("yyyy-MM-dd");
            ajaxData.last = lastDate.Format("yyyy-MM-dd");
            getData();
            //弹窗右上角日期渲染
            todayShow.text(currentYeath + '年' + currentMonth + '月' + currentDay + '日');

            // 获取要随时间而更改的日期dom元素
            setDay();
            writeDay(weekday, 6);
            function setDay() {
                if (yeath == initYeath && lastYeath == initYeath) {
                    dateArea.text(month + '月' + day + '日至' + lastMonth + '月' + lastDay + '日');
                } else if (yeath != initYeath && lastYeath == initYeath || yeath != initYeath && lastYeath != initYeath) {
                    dateArea.text(yeath + '年' + month + '月' + day + '日至' + lastYeath + '年' + lastMonth + '月' + lastDay + '日');
                } else if (yeath == initYeath && lastYeath != initYeath) {
                    dateArea.text(month + '月' + day + '日至' + yeath + '年' + lastMonth + '月' + lastDay + '日');
                }

            };
            $scope.arrLeft = function () {
                getLastDate(-1);
                getPreDate(-6);
                setDay();
                writeDay(weekday, 6);
                getData();
            }
            $scope.arrRight = function () {
                getPreDate(1);
                getLastDate(6);
                setDay();
                writeDay(weekday, 6);
                getData();
            }
            $scope.todayShow = function () {
                date = initDate;
                month = initMonth;
                lastDay = initLastDate.Format("yyyy-MM-dd");
                lastMonth = initLastMonth;            
                setDay();
                writeDay(weekday, 6);
                ajaxData.frist = date.Format("yyyy-MM-dd");
                ajaxData.last = lastDay;
                getData();
            }
            $scope.gotoLive = function (courseId,lessonId) {
                window.open("/StudyPlatform/Learn/Live?courseId=" + courseId + "&lessonId=" + lessonId);
            }

            // 获取日期区间的起点日期方法
            function getPreDate(num) {
                date = lastDate.addDay(num);
                yeath = date.getFullYear();
                month = date.getMonth() + 1;
                day = date.Format("yyyy-MM-dd");//date.getDate();
                ajaxData.frist = day;
                ajaxData.last = lastDay;
      
            };

            // 获取日期区间的终点日期方法
            function getLastDate(num) {
                lastDate = date.addDay(num);
                lastYeath = lastDate.getFullYear();
                lastMonth = lastDate.getMonth() + 1;
                lastDay = lastDate.Format("yyyy-MM-dd"); //lastDate.getDate();
                ajaxData.frist = day;
                ajaxData.last = lastDay;
            };

            // 渲染星期对应的日期显示方法
            function writeDay(obj, num) {
                obj.removeClass('active');
                for (var i = 0 ; i <= num; i++) {
                    var date2 = date.addDay(i);
                    yeath = date2.getFullYear();
                    month = date2.getMonth() + 1;
                    day = date2.getDate();
                    $(obj[i]).text(day);
                    if (day == currentDay && month == currentMonth && yeath == currentYeath) {
                        $(obj[i]).addClass('active');
                    }
                }
            };

            function getData() {
                $.ajax({
                    'url': ajaxUrl,
                    'type': ajaxType,
                    'data': ajaxData,
                   // 'beforeSend': ajaxBeforeSend(xhr),
                    'success': ajaxSuccess
                });
            }
    }
   

    }
    Date.prototype.Format = function (fmt) { //author: meizz 
        var o = {
            "M+": this.getMonth() + 1, //月份 
            "d+": this.getDate(), //日 
            "h+": this.getHours(), //小时 
            "m+": this.getMinutes(), //分 
            "s+": this.getSeconds(), //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }
})(angular, window);