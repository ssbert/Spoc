/**
 * grid 行扩展
*/
$.extend($.fn.datagrid.defaults.view, {
    render: function (target, container, frozen) {
        var state = $.data(target, "datagrid");
        var opts = state.options;
        var rows = state.data.rows;
        var fields = $(target).datagrid("getColumnFields", frozen);
        if (frozen) {
            if (!(opts.rownumbers || (opts.frozenColumns && opts.frozenColumns.length))) {
                return;
            }
        }
        if (opts.singleSelect) {
            var ck = $(".datagrid-header-row .datagrid-header-check");
            $(ck).empty();
        }
        var table = ["<table class=\"datagrid-btable\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody>"];
        for (var i = 0; i < rows.length; i++) {
            var cls = (i % 2 && opts.striped)
                    ? "class=\"datagrid-row datagrid-row-alt\""
                    : "class=\"datagrid-row\"";
            var styleValue = opts.rowStyler ? opts.rowStyler.call(target, i,
                    rows[i]) : "";
            var style = styleValue ? "style=\"" + styleValue + "\"" : "";
            var rowId = state.rowIdPrefix + "-" + (frozen ? 1 : 2) + "-" + i;
            table.push("<tr id=\"" + rowId + "\" datagrid-row-index=\"" + i
                    + "\" " + cls + " " + style + ">");
            table.push(this.renderRow.call(this, target, fields, frozen, i,
                    rows[i]));
            table.push("</tr>");

        }
        table.push("</tbody></table>");
        // $(container).html(table.join(""));
        $(container)[0].innerHTML = table.join("");
        // 增加此句以实现,formatter里面可以返回easyui的组件，以便实例化。例如：formatter:function(){
        // return "<a href='javascript:void(0)'
        // class='easyui-linkbutton'>按钮</a>" }}
        $.parser.parse(container);
    },
    renderRow: function (target, fields, frozen, rowIndex, rowData) {
        var opts = $.data(target, "datagrid").options;
        var cc = [];
        if (frozen && opts.rownumbers) {
            var rownumber = rowIndex + 1;
            if (opts.pagination) {
                rownumber += (opts.pageNumber - 1) * opts.pageSize;
            }
            cc.push("<td class=\"datagrid-td-rownumber\"><div class=\"datagrid-cell-rownumber\">"
                            + rownumber + "</div></td>");
        }
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var col = $(target).datagrid("getColumnOption", field);
            if (col) {
                // 修改默认的value取值，改了此句之后field就可以用关联对象了。如：people.name
                var value = jQuery.proxy(function () {
                    try {
                        return eval('this.' + field);
                    } catch (e) {
                        return "";
                    }
                }, rowData)();
                var styleValue = col.styler ? (col.styler(value, rowData,
                        rowIndex) || "") : "";
                var style = col.hidden ? "style=\"display:none;" + styleValue
                        + "\"" : (styleValue
                        ? "style=\"" + styleValue + "\""
                        : "");
                cc.push("<td field=\"" + field + "\" " + style + ">");
                if (col.checkbox) {
                    var style = "";
                } else {
                    var style = styleValue;
                    if (col.align) {
                        style += ";text-align:" + col.align + ";";
                    }
                    if (!opts.nowrap) {
                        style += ";white-space:normal;height:auto;";
                    } else {
                        if (opts.autoRowHeight) {
                            style += ";height:auto;";
                        }
                    }
                }
                cc.push("<div style=\"" + style + "\" ");
                if (col.checkbox) {
                    cc.push("class=\"datagrid-cell-check ");
                } else {
                    cc.push("class=\"datagrid-cell " + col.cellClass);
                }
                cc.push("\">");
                if (col.checkbox) {
                    var type = opts.singleSelect == true ? "radio" : "checkbox";
                    cc.push("<input type=" + type + " name=\"" + field
                            + "\" value=\"" + (value != undefined ? value : "")
                            + "\"/>");
                } else {
                    if (col.formatter) {
                        cc.push(col.formatter(value, rowData, rowIndex));
                    } else {
                        cc.push(value);
                    }
                }
                cc.push("</div>");
                cc.push("</td>");
            }
        }
        return cc.join("");
    }

});
/**
* treegrid 行扩展
*/
$.extend($.fn.treegrid.defaults.view,
{
    render: function(target, container, frozen) {
        var opts = $.data(target, "treegrid").options;
        var fields = $(target).datagrid("getColumnFields", frozen);
        var rowIdPrefix = $.data(target, "datagrid").rowIdPrefix;
        if (frozen) {
            if (!(opts.rownumbers || (opts.frozenColumns && opts.frozenColumns.length))) {
                return;
            }
        }
        var self = this;
        if (this.treeNodes && this.treeNodes.length) {
            var buffer = getHtmlBuffer(frozen, this.treeLevel, this.treeNodes);
            $(container).append(buffer.join(""));
        }

        function getHtmlBuffer(frozen, treeLevel, treeNodes) {
            var targetParent = $(target).treegrid("getParent", treeNodes[0][opts.idField]);
            var length = (targetParent ? targetParent.children.length : $(target).treegrid("getRoots").length) - treeNodes.length;
            var buffer = [ "<table class=\"datagrid-btable\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody>" ];
            for (var i = 0; i < treeNodes.length; i++) {
                var row = treeNodes[i];
                if (row.state != "open" && row.state != "closed") {
                    row.state = "open";
                }
                var css = opts.rowStyler ? opts.rowStyler.call(target, row) : "";
                var styleClass = "";
                var style = "";
                if (typeof css == "string") {
                    style = css;
                } else {
                    if (css) {
                        styleClass = css["class"] || "";
                        style = css["style"] || "";
                    }
                }
                var cls = "class=\"datagrid-row " +
                    (length++ % 2 && opts.striped ? "datagrid-row-alt " : " ") +
                    styleClass +
                    "\"";
                var styleHtmlPart = style ? "style=\"" + style + "\"" : "";
                var trId = rowIdPrefix + "-" + (frozen ? 1 : 2) + "-" + row[opts.idField];
                buffer.push("<tr id=\"" +
                        trId +
                        "\" node-id=\"" +
                        row[opts.idField] +
                        "\" " +
                        cls +
                        " " +
                        styleHtmlPart +
                        ">");
                buffer = buffer.concat(self.renderRow.call(self, target, fields, frozen, treeLevel, row));
                buffer.push("</tr>");
                if (row.children && row.children.length) {
                    var tt = getHtmlBuffer(frozen, treeLevel + 1, row.children);
                    var v = row.state == "closed" ? "none" : "block";
                    buffer.push("<tr class=\"treegrid-tr-tree\"><td style=\"border:0px\" colspan=" +
                        (fields.length + (opts.rownumbers ? 1 : 0)) +
                        "><div style=\"display:" +
                        v +
                        "\">");
                    buffer = buffer.concat(tt);
                    buffer.push("</div></td></tr>");
                }
            }
            buffer.push("</tbody></table>");
            return buffer;
        };

        $.parser.parse(container);
    }
});
////更多操作
function linkbtn(value, options) {
    var opt = options || {};
    var len = opt.option.length;
    var div = $('<div></div>');
    for (var i = 0; i < len; i++) {
        var op = opt.option[i];
        if (!op.title)
            op.title = "";
        var name = op.name || undefined;
        var html = '<a href="javascript:void(0)" style="color:#166DCC" class="easyui-linkbutton" plain="true" iconCls="'
            + op.icon + '"';
        if (op.onclick) {
            if (op.hasParams == "true") {
                html = html + ' onclick=' + op.onclick + '';
            } else {
                html = html + ' onclick=' + op.onclick + '("' + value + '")';
            }

        }
        var span = $(html + ' title=' + op.title + ' name=' + name + ' id='+ op.id + ' data-id=' + value + '></a>');
        if (op.hide)
            span.css("display", "none");
        span.append(op.text);
        span.appendTo(div);
    }
    return div.html();
}

function menubtn(value, index, options) {
    var opt = options || {};
    var len = opt.option.length;
    var div = $('<div></div>');
    if ($("div[id^='submenu']"))
        $("div[id^='submenu']").remove();
    for (var i = 0; i < len; i++) {
        var op = opt.option[i];
        var span = $('<a href="javascript:void(0)" class="easyui-menubutton" style="color:#166DCC" id="menubtn'
            + index
            + '"  plain="true" iconCls="'
            + op.icon
            + '" menu="#submenu' + index + '"></a>');
        span.append(op.text);
        span.appendTo(div);
        var items = op.items || {};
        var subdiv = $('<div id="submenu' + index + '"></div>');
        for (var j = 0; j < items.length; j++) {
            var item = items[j];
            var hide = '';
            var click = '';
            if (item.hide)
                hide = 'style="display:none"';
            if (item.hasParams) {
                click =  '"' + item.onclick + '"';
            } else {
                click =  item.onclick + '("' + value + '")';
            }
            
            var sub = $('<div  iconCls="' + item.icon + '" onclick='
               + click + hide + '>'
               + item.text + '</div>');
            sub.appendTo(subdiv);
         
        }
        subdiv.appendTo(div);
    }
    return div.html();
}