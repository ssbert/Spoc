var mainPlatform = {
   
	init: function(){

	    this.bindEvent();
	    this.initLeftMenu();
	    tabCloseEven();
	},
	initLeftMenu: function() {
	    $.ajax({
	        type: 'get',
	        url: '/Home/GetTreeMenu',
	        success: function (data) {
                //渲染菜单
	            mainPlatform.render(data);
	        }
	    });
	},
	bindEvent: function(){
		var self = this;
		// 顶部大菜单单击事件
		//$(document).on('click', '.pf-nav-item', function() {
        //   // $('.pf-nav-item').removeClass('current');
        //    //$(this).addClass('current');

        //    // 渲染对应侧边菜单
        //   // var m = $(this).data('menu');
        //   // self.render(menu[m]);
		//});
		$(document).on('click', '.sider-nav li', function () {
		    $('.sider-nav li').removeClass('current');
		    $(this).addClass('current');
		});
        $(document).on('click', '.sider-nav-s li', function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $('.sider-nav-s li').removeClass('active');
            $(this).addClass('active');
            var tabTitle = $(this).children("a").children('.sider-nav-title').text();
            addTab(tabTitle, $(this).data('src'), $(this).data('icon')); //增加tab
            //$('iframe').attr('src', $(this).data('src'));
        });

        $(document).on('click', '.pf-logout', function() {
            layer.confirm('您确定要退出吗？', {
              icon: 4,
			  title: '确定退出' //按钮
            }, function () {
                location.href = "/Home/LoginOut";
			});
        });
        //左侧菜单收起
        $(document).on('click', '.toggle-icon', function() {
            $(this).closest("#pf-bd").toggleClass("toggle");
            setTimeout(function() {
                $(window).resize();
            }, 300);
        });

       

        $(document).on('click', '.pf-notice-item', function() {
            $('#pf-page').find('iframe').eq(0).attr('src', 'backend/notice.html');
        });
	},
    //渲染菜单
	render: function (menu) {
	    var current,
			html = [];
	    $.each(menu, function (i, n) {
	        if (current!=0) {
	            current = 0;
	            html.push('<li class="current" title="' + n.text + '"><a href="javascript:;"> <span class="iconfont sider-nav-icon"> ' + n.icon + ' </span><span class="sider-nav-title">' + n.text + '</span><i class="iconfont">&#xe642;</i></a>');
	        } else {
	            html.push('<li  title="' + n.text + '"><a href="javascript:;"><span class="iconfont sider-nav-icon"> ' + n.icon + ' </span><span class="sider-nav-title">' + n.text + '</span><i class="iconfont">&#xe642;</i></a>');
	        }
	        html.push('<ul class="sider-nav-s">');
	        $.each(n.children, function (j, o) {
	           
	            html.push('<li data-src="' + o.url + '" data-code="' + o.code + '" data-icon="' + o.icon + '"  title="' + o.text + '" ><a href="javascript:;"><span class="sider-nav-title">' + o.text + '</span></a>');
	        });
	        html.push("</ul>");
	        html.push("</li>");

	    });
	    $(".sider-nav").append(html.join(''));

	}

};
mainPlatform.init();

/* 考虑其他地方调用 保留原home.js方法 */
//获取左侧导航的图标
function getIcon(menuid) {
    var icon = 'icon ';
    $.each(_menus.menus, function(i, n) {
        $.each(n.menus, function(j, o) {
            if (o.menuid == menuid) {
                icon += o.icon;
            }
        });
    });

    return icon;
}

//获取左侧导航的图标
function getMenuIcon(MenuCode) {
    var MenuIcon = 'icon ';
    $.each(_menus, function(i, n) {
        $.each(n.ChildMenu, function(j, o) {
            if (o.MenuCode == MenuCode) {
                MenuIcon += o.MenuIcon;
            }
        });
    });

    return MenuIcon;
}

function addTab(subtitle, url, MenuIcon) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url, subtitle),
            closable: true,
            icon: ""
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}
//打开新Tab页窗口进行数据选择
function addSelectTab(subtitle, url) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url, subtitle),
            closable: true,
            icon: ""
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}
function createFrame(url, name) {
    var s = '<iframe name="' + name + '" scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:99%;"></iframe>';
    return s;
}

function tabClose() {
    /*双击关闭TAB选项卡*/
    $(".tabs-inner").dblclick(function () {
        var subtitle = $(this).children(".tabs-closable").text();
        $('#tabs').tabs('close', subtitle);
    })
    /*为选项卡绑定右键*/
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        var subtitle = $(this).children(".tabs-closable").text();

        $('#mm').data("currtab", subtitle);
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}
//绑定右键菜单事件
function tabCloseEven() {
    //刷新
    $('#mm-tabupdate').click(function() {
        var currTab = $('#tabs').tabs('getSelected');
        if (currTab.panel('options').title == "主页") return;
        var url = $(currTab.panel('options').content).attr('src');
        var name = $(currTab.panel('options').content).attr('name');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url, name)
            }
        });
    });
    //关闭当前
    $('#mm-tabclose').click(function() {
        var currtab_title = $('#mm').data("currtab");
        $('#tabs').tabs('close', currtab_title);
    });
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            $('#tabs').tabs('close', t);
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            //msgShow('系统提示','后边没有啦~~','error');
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });

    //退出
    $("#mm-exit").click(function() {
        $('#mm').menu('hide');
    });
}

//弹出信息窗口 title:标题 msgString:提示信息 msgType:信息类型 [error,info,question,warning]
function msgShow(title, msgString, msgType) {
    $.messager.alert(title, msgString, msgType);
}

//add by kelei
var observer = {
    clientList: {},
    listen: function (key, fn) {
        if (!this.clientList[key]) {
            this.clientList[key] = [];
        }
        this.clientList[key].push(fn);
    },
    trigger: function () {
        var key = Array.prototype.shift.call(arguments),
            fns = this.clientList[key];
        if (!fns || fns.length === 0) {
            return false;
        }
        for (var i = 0; i < fns.length; i++) {
            fns[i].apply(this, arguments);
        }
        return this;
    },
    remove: function (key, fn) {
        var fns = this.clientList[key];
        if (!fns) {
            return;
        }
        if (!fn) {
            fns && (fns.length = 0);
        } else {
            for (var i = 0; i < fns.length - 1; i++) {
                if (fns[i] === fn) {
                    fns.splice(i, 1);
                }
            }
        }
    }
};