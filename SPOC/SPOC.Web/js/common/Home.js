$(function () {
    InitLeftMenu();
    tabClose();
    tabCloseEven();
    //$('#menu').tree({
    //    url: '/home/GetTreeMenu',
    //    lines: true,
    //    onClick: function() {

    //        if (!checkLogin()) { //判断是否登录和是否在其他地方登陆
    //            evtBus.dispatchEvt("show_login");
    //            return;
    //        }

    //        var node = $('#menu').tree('getSelected');

    //        if (node.url != undefined && node.url != "" && node.url != null) {
    //            addTab(node.text, node.url, "");

    //        } else {
    //            if (node.state == "closed") {
    //                $('#menu').tree('expand', node.target);
    //            } else {
    //                $('#menu').tree('collapse', node.target);
    //            }
    //        }
    //        $('#menu').tree('select', node);
    //    }
    //});
});
function convert(rows) {
    var nodes = [];
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (row.parentCode == "" || row.parentCode == null) {
            
            nodes.push({
                id: row.code,
                text: row.text,
                url: row.url,
                state: row.code == "userManger" ? "" : "closed"

            });
        }
    }

    var toDo = [];
    for (var i = 0; i < nodes.length; i++) {
        toDo.push(nodes[i]);
    }
    while (toDo.length) {
        var node = toDo.shift(); 
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.parentCode == node.id) {
                var child = { id: row.code, text: row.text, url: row.url };
                if (node.children) {
                    node.children.push(child);
                } else {
                    node.children = [child];
                    if (node.id != "userManger")
                    node.state = "closed";
                }
                toDo.push(child);
            }
        }
    }
    return nodes;
}


var _menus = null;

//初始化左侧
function InitLeftMenu() {
    $("#nav").accordion({ animate: false }); //为id为nav的div增加手风琴效果，并去除动态滑动效果
    $.ajax({
        type: 'get',
        url: '/Home/GetMenu',
        success: function (data) {
            _menus = data;
            $.each(_menus, function (i, n) {//$.each 遍历_menu中的元素
                var menulist = '';
                menulist += '<ul>';
                $.each(n.ChildMenu, function (j, o) {


                    if (o.ChildMenu.length > 0) {

                        menulist += '<ul class="easyui-tree" id="menuTree">';
                        menulist += '<li>  <span>"' + o.MenuName + '"</span><ul>';
                        $.each(o.ChildMenu, function(k, p) {
                            menulist += '<li><span><a ref="' + p.MenuCode + '" href="#" rel="' + p.MenuUrl + '" >' + p.MenuName + '</a></span></li> ';

                        });
                        menulist += '</ul></li></ul>';

                    } else {
                        menulist += '<li><div><a ref="' + o.MenuCode + '" href="#" rel="' + o.MenuUrl + '" ><span class="icon ' + o.MenuIcon + '" >&nbsp;</span><span class="nav">' + o.MenuName + '</span></a></div></li> ';
                    }
                });
                menulist += '</ul>';

                $('#nav').accordion('add', {
                    title: n.MenuName,
                    content: menulist,
                    iconCls: 'icon ' + n.MenuIcon
                });

            });

            $('.easyui-accordion li a').click(function () {//当单击菜单某个选项时，在右边出现对用的内容

                if (!checkLogin()) {//判断是否登录和是否在其他地方登陆
                    evtBus.dispatchEvt("show_login");
                    return;
                }

                var tabTitle = $(this).children('.nav').text(); //获取超链里span中的内容作为新打开tab的标题

                var url = $(this).attr("rel");
                var MenuCode = $(this).attr("ref"); //获取超链接属性中ref中的内容
                var MenuIcon = getMenuIcon(MenuCode, MenuIcon);

                addTab(tabTitle, url, MenuIcon); //增加tab
                $('.easyui-accordion li div').removeClass("selected");
                $(this).parent().addClass("selected");
            }).hover(function () {
                $(this).parent().addClass("hover");
            }, function () {
                $(this).parent().removeClass("hover");
            });

            //选中第一个
            var panels = $('#nav').accordion('panels');
            var t = panels[0].panel('options').title;
            $('#nav').accordion('select', t);
        }
    });
}

//获取左侧导航的图标
function getIcon(menuid) {
    var icon = 'icon ';
    $.each(_menus.menus, function (i, n) {
        $.each(n.menus, function (j, o) {
            if (o.menuid == menuid) {
                icon += o.icon;
            }
        })
    })

    return icon;
}

//获取左侧导航的图标
function getMenuIcon(MenuCode) {
    var MenuIcon = 'icon ';
    $.each(_menus, function (i, n) {
        $.each(n.ChildMenu, function (j, o) {
            if (o.MenuCode == MenuCode) {
                MenuIcon += o.MenuIcon;
            }
        })
    })

    return MenuIcon;
}

function addTab(subtitle, url, MenuIcon) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url, subtitle),
            closable: true,
            icon: MenuIcon
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
    $('#mm-tabupdate').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        if (currTab.panel('options').title == "主页") return;
        var url = $(currTab.panel('options').content).attr('src');
        var name = $(currTab.panel('options').content).attr('name');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url, name)
            }
        })
    })
    //关闭当前
    $('#mm-tabclose').click(function () {
        var currtab_title = $('#mm').data("currtab");
        $('#tabs').tabs('close', currtab_title);
    })
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
    $("#mm-exit").click(function () {
        $('#mm').menu('hide');
    })
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
