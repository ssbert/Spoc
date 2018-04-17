window.rootpageDialog = (function () {
    var sn = 0;
    function init() {
        return {
            createDialog: function (dialogParam, contentHtml, dialogId) {
                var id = "dialig-" + sn;
                if (dialogId) {
                    id =  dialogId;
                } else {
                    sn++;
                }
                if (stringIsEmpty(contentHtml)) {
                    contentHtml = "";
                }
                var html = "<div id=\"" + id + "\">" + contentHtml+ "</div>";
                $("body").append(html);
                return $("#" + id).dialog(dialogParam);
            },
            destroy: function (id) {              
                $("#" + id).dialog("destroy").remove();
            }
        }
    }
    return init;
})();