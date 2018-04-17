/**
 * 知识库标签编辑
 */
var serviceUrl = "/api/services/app/";
var EditLabel = (function () {
    function init(id) {
        var label = { id: guidIsEmpty(id) ? emptyGuid : id};
        var category = new nv.category.CombotreeDataClass("folderId", "lib_label");
        var editIndex = undefined;
        this.save = function () {
            var validate = $("#labelForm").form("validate");
            if (!validate) {
                return;
            }
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var param = getParam();
            var url = serviceUrl + "liblabel/createorupdate";
            //var isCreate = stringIsEmpty(param.id);
            VE.Mask("");
            nv.post(url, param, function (data) {
                VE.UnMask();
                if (data.success) {
                    label.id = data.result;
                    //if (isCreate) {
                    //    setData(data.result);
                    //}
                    evtBus.dispatchEvt("update_liblabel_list");
                    $.messager.show({ title: "提示", msg: "保存成功!" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });

        };

        this.onClickRow = function (index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index)
                        .datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }
        this.append=function() {
            if (endEditing()) {
                if ($("#dg").datagrid("getRows").length === 0) {
                    $('#dg').datagrid('appendRow', { logic: 0, id: emptyGuid });
                } else
                    {$('#dg').datagrid('appendRow', { logic: 0, id: emptyGuid });}
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex)
                    .datagrid('beginEdit', editIndex);
            }
        }
        this.removeit = function () {
            if (editIndex == undefined) { return }
            $('#dg').datagrid('cancelEdit', editIndex)
                .datagrid('deleteRow', editIndex);
            editIndex = undefined;
        }
    
        this.reject = function () {
            $('#dg').datagrid('rejectChanges');
            editIndex = undefined;
        }
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                var ed = $('#dg').datagrid('getEditor', { index: editIndex, field: 'logic' });
                var logic = $(ed.target).combobox('getText');
                $('#dg').datagrid('getRows')[editIndex]['logicName'] = logic;
                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function loadData(id) {
            VE.Mask("");
            var url = serviceUrl + "liblabel/Get?id=" + id;
            nv.get(url,
                function (data) {
                    VE.UnMask();
                    if (data.success) {
                        setData(data.result);

                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function setData(data) {
            label = data;
            $("#title").textbox("setValue", label.title);
            $("#describe").textbox("setValue", label.describe);
            $("#folderId").combotree("setValue", label.folderId);
            $.each(data.rules, function (k, v) {
                if (v.logic === 1) {
                    v.logicName = "与";
                } else if (v.logic === 0) {
                    v.logicName = "或";
                } else {
                    v.logicName = "";
                }

            });
            $("#dg").datagrid("loadData", data.rules);
        }

        function getParam() {
            label.title = $("#title").textbox("getValue");
            label.describe = $("#describe").textbox("getValue");
            label.folderId = $("#folderId").combotree("getValue");
            endEditing();
            //获取界面所有行
            label.rules = $("#dg").datagrid("getRows");//$('#dg').datagrid('getChanges');
           
            return label;
        }

        $(function () {
            category.getCategory(function () {
                if (!stringIsEmpty(id)) {
                    loadData(id);
                }
            });
        });
    }
    return init;
})();