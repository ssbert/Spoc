var component = component == null ? {} : component;

component.ExercisesBankSelector = (function() {
    function init() {
        var self = this;
        var paramCache = {
            code: "",
            title: "",
            categoryIds: [],
            subjectIds: [],
            departmentsIds: [],
            skip: 0,
            pageSize: 30
        };
        var category = new nv.category.CombotreeDataClass("categoryId", "exercisesBank");

        this.query = function () {
            paramCache = getFormParam();
            paramCache.skip = 0;
            loadData(paramCache);
        };

        this.selected = function(index) {
            var row = $("#dg").datagrid("getRows")[index];
            evtBus.dispatchEvt("exercises_bank_selected", row);
        };

        function loadData(param) {
            $("#dg").datagrid("loading");
            var url = serviceUrl + "ExercisesBank/GetPagination";
            nv.post(url,
                param,
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $("#dg")
                            .datagrid("loadData", data.result.rows)
                            .datagrid("getPager")
                            .pagination({
                                pageNumber: param.pageNumber,
                                total: data.result.total
                            });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getFormParam() {
            paramCache.code = $("#code").textbox("getValue");
            paramCache.title = $("#title").textbox("getValue");
            paramCache.categoryIds = $("#categoryId").combotree("getValues");
            return paramCache;
        }

        function initPagination() {
            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        paramCache.pageNumber = pageNumber;
                        paramCache.skip = (pageNumber - 1) * pageSize;
                        if (paramCache.skip < 0) {
                            paramCache.skip = 0;
                        }
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    },
                    onChangePageSize: function (pageSize) {
                        paramCache.pageSize = pageSize;
                        loadData(paramCache);
                    }
                });
        }

        $(function () {
            category.getCategory();
            initPagination();
            self.query();
        });
    }

    return init;
})();