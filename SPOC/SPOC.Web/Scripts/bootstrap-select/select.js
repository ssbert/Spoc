function LoadSelect(type, id) {
    var singleSelect, multipleSelect;
    if (id && $("#" + id).length > 0) {
        singleSelect = $('#' + id).find(".singleSelect");
        multipleSelect = $('#' + id).find(".multipleSelect");
    } else {
        singleSelect = $(".singleSelect");
        multipleSelect = $(".multipleSelect");
    }
    singleSelect.each(function () {
        if (type == 1)
            $(this).attr({ "data-live-search": "true", "data-size": "5" });
        else
            $(this).attr({ "data-live-search": "true", "data-size": "5", "width": "auto", "data-width": "auto" });
        $(this).selectpicker({ noneSelectedText: '请选择' });
        $(this).selectpicker("refresh");
    });
    multipleSelect.each(function () {
        if (type == 1)
            $(this).attr({ "data-live-search": "true", "data-size": "5" });
        else
            $(this).attr({ "data-live-search": "true", "data-size": "5", "width": "auto", "data-width": "auto" });
        $(this).attr("multiple", "multiple");
        $(this).selectpicker({ noneSelectedText: '请选择' });
        $(this).selectpicker("refresh");
    });
}