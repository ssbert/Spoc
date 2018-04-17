var serviceUrl = "/api/services/app/";
var nv = nv ? nv : {};
nv.category = nv.category ? nv.category : {};
//分类combotree控件数据获
nv.category.CombotreeDataClass = (function() {

    function init(elementId, folderTypeCode, checked, fn) {
        var self = this;
        var dataDic = {};
        var dataArray = [];
        self.folderTypeCode = folderTypeCode;
        self.elementId = elementId;
        if (typeof (checked) == "undefined") {
            self.checked = true;
        } else {
            self.checked = checked;
        }
        this.getCategory = function(callback) {
            var url = serviceUrl + "NvFolder/Get?folderTypeCode=" + self.folderTypeCode;
            nv.get(url,
                function(data) {
                    if (data.success) {
                        dataDic = {};
                        dataArray = dataFormat(data.result);
                        $("#" + self.elementId).combotree("loadData", dataArray);
                        if (self.checked)
                            $("#" + self.elementId).combotree("setValue", dataArray[0].id);
                        if (callback) {
                            callback(dataArray);
                        }
                        self.carryCallback(fn);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        };

        this.carryCallback = function(fn){
            if (fn && Object.prototype.toString.call(fn) == "[object Function]") {
                fn();
            }
        };

        this.getDataDic = function() {
            return dataDic;
        };
        this.getDataArray = function() {
            return dataArray;
        };

        function sortNode(nodeArray) {
            nodeArray.sort(function(a, b) {
                return a.listOrder - b.listOrder;
            });
            $.each(nodeArray,
                function(i, v) {
                    if (v.children.length > 1) {
                        sortNode(v.children);
                    }
                });
        }

        function dataFormat(array) {
            $.each(array,
                function(i, v) {
                    v.text = v.folderName;
                    v.children = [];
                    dataDic[v.id] = v;
                });

            var data = [];
            $.each(dataDic,
                function(k, v) {
                    if (guidIsEmpty(v.parentUid)) {
                        data.push(v);
                    } else {
                        var parentNode = dataDic[v.parentUid];
                        if (parentNode != undefined) {
                            parentNode.children.push(v);
                        }
                    }
                });

            sortNode(data);
            return data;
        }
    }

    return init;
})();