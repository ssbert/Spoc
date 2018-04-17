$.extend($.fn, {
    //参数d表示服务器unix时间戳
    fnTimeCountDown: function () {
        
        this.each(function () {
            var $this = $(this);
            var o = {
                hm: $this.find(".hm"),
                sec: $this.find(".sec"),
                minute: $this.find(".minute"),
                hour: $this.find(".hour"),
                day: $this.find(".day"),
                month: $this.find(".month"),
                year: $this.find(".year")
            };
            var f = {
                haomiao: function (n) {
                    if (n < 10) return "00" + n.toString();
                    if (n < 100) return "0" + n.toString();
                    return n.toString();
                },
                zero: function (n) {
                    var _n = parseInt(n, 10);//解析字符串,返回整数
                    if (_n > 0) {
                        if (_n <= 9) {
                            _n = "0" + _n
                        }
                        return String(_n);
                    } else {
                        return "00";
                    }
                },
                dv: function (btnObj) {
                    //var _d = $this.data("end");     
                     var  endDate = $this.data("end");    
                    var dur = (endDate - now) , mss = (endDate - now)*1000, pms = {
                        hm: "000",
                        sec: "00",
                        minute: "00",
                        hour: "00",
                        day: "00",
                        month: "00",
                        year: "0"
                    };
                    if (mss > 0) {
                        pms.hm = f.haomiao(mss % 1000);
                        pms.sec = f.zero(dur % 60);
                        pms.minute = Math.floor((dur / 60)) > 0 ? f.zero(Math.floor((dur / 60)) % 60) : "00";
                        pms.hour = Math.floor((dur / 3600)) > 0 ? f.zero(Math.floor((dur / 3600)) % 24) : "00";
                        //计算月之外的天数
                        //pms.day = Math.floor((dur / 86400)) > 0 ? f.zero(Math.floor((dur / 86400)) % 30) : "00";
                        //算总天数
                        pms.day = Math.floor((dur / 86400)) > 0 ? f.zero(Math.floor((dur / 86400))) : "00";
                        //月份，以实际平均每月秒数计算
                        pms.month = Math.floor((dur / 2629744)) > 0 ? f.zero(Math.floor((dur / 2629744)) % 12) : "00";
                        //年份，按按回归年365天5时48分46秒算
                        pms.year = Math.floor((dur / 31556926)) > 0 ? Math.floor((dur / 31556926)) : "0";
                    } else {
                        pms.year = pms.month = pms.day = pms.hour = pms.minute = pms.sec = "00";
                        pms.hm = "000";
                        $this.find("#btnGoStudy").addClass("active");
                        $this.find("#btnGoStudy").text('正在直播中');
                        //alert('结束了');
                        return pms;
                    }
                    return pms;
                },
                ui: function () {
                    //if (o.hm) {
                    //    o.hm.html(f.dv().hm);
                    //}
                    //if (o.sec) {
                    //    o.sec.html(f.dv().sec);
                    //}
                    if (o.minute) {
                       // console.log(f.dv().minute);
                        o.minute.html(f.dv().minute);
                    }
                    if (o.hour) {
                        o.hour.html(f.dv().hour);
                    }
                    if (o.day) {
                        o.day.html(f.dv().day);
                    }
                    if (o.month) {
                        o.month.html(f.dv().month);
                    }
                    if (o.year) {
                        o.year.html(f.dv().year);
                    }
                    setTimeout(f.ui, 1000);
                }
            };
            f.ui();
      
        });
    }
});