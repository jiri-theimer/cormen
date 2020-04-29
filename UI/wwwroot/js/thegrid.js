var _ds;
var _j72id;
var _tg_url_data;
var _tg_url_handler;
var _tg_entity;
var _tg_filterinput_timeout;
var __tg_filter_is_active;

function tg_init(c) {
    _tg_entity = c.entity;
    _j72id = c.j72id;
    _tg_url_data = c.dataurl;
    _tg_url_handler = c.handlerurl;

    tg_post_data();

    $("#container_grid").scroll(function () {
        $("#container_vScroll").width($("#container_grid").width() + $("#container_grid").scrollLeft());
    });

    $("#tabgrid0 .sortcolumn").on("click", function () {
        var field = this.id.replace("th_", "");
        tg_post_handler("sorter", "sortfield", field);
    });


    tg_setup_checkbox_handler();


    $("#tabgrid1_thead .query_textbox").on("input", function (e) {
        var txt1 = this;
        if (typeof _tg_filterinput_timeout !== "undefined") {
            clearTimeout(_tg_filterinput_timeout);
        }
        _tg_filterinput_timeout = setTimeout(function () {

            //your ajax stuff , 500ms zpoždění, aby se počkalo na víc stringů od uživatele 
            var field = txt1.id.replace("txtqry_", "");
            if (txt1.value !== "") {
                $("#hidqry_" + field).val("3ßß" + txt1.value);       //natvrdo doplnit operátor OBSAHUJE
                $("#txtqry_" + field).css("background-color", "red");
                $("#txtqry_" + field).css("color", "white");
            } else {
                $("#hidqry_" + field).val("");
                $("#txtqry_" + field).css("background-color", "");
                $("#txtqry_" + field).css("color", "");
            }

            _tg_filter_is_active = tg_is_filter_active();
            if (_tg_filter_is_active === true) {
                $("#cmdDestroyFilter").css("display", "block");

            } else {
                $("#cmdDestroyFilter").css("display", "none");

            }

            tg_post_handler("filter", field, txt1.value);


        }, 700);
    });


}

function tg_post_data() {

    $.post(_tg_url_data, { entity: _tg_entity, j72id: _j72id }, function (data) {

        $("#tabgrid1_tbody").html(data.body);
        $("#tabgrid1_tfoot").html(data.foot);
        $("#divPager").html(data.pager);
        tg_refresh_sorter(data.sortfield, data.sortdir);

        tg_adjust_parts_width();

        var basewidth = $("#tabgrid0").width();
        $("#tabgrid1").width(basewidth);
        $("#tabgrid2").width(basewidth);



        tg_setup_selectable();
    });

}

function tg_refresh_sorter(sortfield, sortdir) {
    $("#tabgrid0 .sortcolumn").each(function () {
        $(this).text(this.title);
    });
    var ths = $("#th_" + sortfield);
    if (sortdir === "asc") {
        $(ths).html($(ths).html() + "&nbsp; &#128314;");
    }
    if (sortdir === "desc") {
        $(ths).html($(ths).html() + "&nbsp; &#128315;");
    }
}

function tg_post_handler(strOper, strKey, strValue) {
    //_notify_message("odesílá se: oper: " + strOper + ", key: " + strKey + ", value: " + strValue);
    $.post(_tg_url_handler, { j72id: _j72id, oper: strOper, key: strKey, value: strValue }, function (data) {
        _notify_message("vrátilo se: oper: " + strOper + ", key: " + strKey + ", value: " + strValue);
        $("#thegrid_message").text(data.message);
        $("#tabgrid1_tbody").html(data.body);
        $("#tabgrid1_tfoot").html(data.foot);
        $("#divPager").html(data.pager);

        if (strOper === "sorter") {
            tg_refresh_sorter(data.sortfield, data.sortdir);
        }




        tg_adjust_parts_width();

        var basewidth = $("#tabgrid0").width();
        $("#tabgrid1").width(basewidth);
        $("#tabgrid2").width(basewidth);


        tg_setup_selectable();
    });

}

function tg_adjust_parts_width() {

    $("#container_vScroll").width($("#container_grid").width() + $("#container_grid").scrollLeft());
}


function tg_setup_selectable() {
    _ds = new DragSelect({
        selectables: document.getElementsByClassName('selectable'), // node/nodes that can be selected. This is also optional, you could just add them later with .addSelectables.
        selectedClass: "selrow",
        area: document.getElementById('container_vScroll'), // area in which you can drag. If not provided it will be the whole document.
        customStyles: false,  // If set to true, no styles (except for position absolute) will be applied by default.
        multiSelectKeys: ['ctrlKey', 'shiftKey', 'metaKey'],  // special keys that allow multiselection.
        multiSelectMode: false,  // If set to true, the multiselection behavior will be turned on by default without the need of modifier keys. Default: false
        autoScrollSpeed: 20,  // Speed in which the area scrolls while selecting (if available). Unit is pixel per movement. Set to 0 to disable autoscrolling. Default = 1
        onDragStart: function (element) { }, // fired when the user clicks in the area. This callback gets the event object. Executed after DragSelect function code ran, befor the setup of event listeners.
        onDragMove: function (element) { }, // fired when the user drags. This callback gets the event object. Executed before DragSelect function code ran, after getting the current mouse position.
        onElementSelect: function (element) { }, // fired every time an element is selected. (element) = just selected node
        onElementUnselect: function (element) { }, // fired every time an element is de-selected. (element) = just de-selected node.
        callback: function (elements) {
            // fired once the user releases the mouse. (elements) = selected nodes.
            $("#tg_selected_pids_pre").val($("#tg_selected_pids").val());

            var pid = elements[0].id.replace("r", "");
            $("#tg_selected_pid").val(pid);
            $("#tg_selected_pids").val(pid);
            $("#tabgrid1").find("input:checkbox").prop("checked", false);
            $("#chk" + pid).prop("checked", true);


            if (elements.length > 1) {
                var pids = [];
                for (i = 0; i < elements.length; i++) {
                    pid = elements[i].id.replace("r", "");
                    pids.push(pid);
                    $("#chk" + pid).prop("checked", true);
                }
                $("#tg_selected_pids").val(pids.join(","));


            }
            _last_ds_selected_pids = $("#tg_selected_pids").val();


        }



    });
}



function tg_adjust_col_widths() {
    var cols_tab0 = $("#tr_header_headline th");
    var cols_tab1 = $("#tabgrid1_tbody tr:first").find("td");
    if (cols_tab1.length === 0) {
        return;
    }
    var cols_tab2 = $("#tabgrid1_tr_totals th");
    $("#tabgrid1_tbody").width($("#tabgrid1").width());

    for (i = 0; i < cols_tab0.length; i++) {
        var w = $(cols_tab0[i]).width();
        $(cols_tab1[i]).width(w);
        if (cols_tab2.length > 0) {
            $(cols_tab2[i]).width(w);
        }

    }
}


function tg_setup_checkbox_handler() {
    $("#tabgrid1").on("change", "input:checkbox", function () {
        //znegovat zaškrtnutí, protože selector už to předtím zaškrtl
        var pid = this.id.replace("chk", "");

        var pids = $("#tg_selected_pids_pre").val();
        var arr = [];
        arr = pids.split(",");
        var x = arr.indexOf(pid);
        if (x > -1) {
            arr.splice(x, 1);
        } else {
            arr.push(pid);
        }



        _ds.clearSelection();

        for (var i = 0; i < arr.length; i++) {
            _ds.addSelection($("#r" + arr[i]));
            $("#chk" + arr[i]).prop("checked", true);
        }
        $("#tg_selected_pids").val(arr.join(","));


        $("#tg_chkAll").prop("checked", false);



    });
}


function tg_select(records_count) {
    var arr = [];
    _ds.clearSelection();
    var rows = $("#tabgrid1_tbody tr");
    for (var i = 0; i < records_count; i++) {
        var pid = rows[i].id.replace("r", "");
        arr.push(pid);
        _ds.addSelection(rows[i]);
    }


    $("#tg_selected_pids").val(arr.join(","));
}

function tg_pager(pageindex) {  //změna stránky
    tg_post_handler("pager", "pagerindex", pageindex);

}
function tg_pagesize(ctl) {//změna velikosti stránky
    tg_post_handler("pager", "pagesize", ctl.value);
}





function tg_is_filter_active() {
    var b = new Boolean;
    $("#tr_header_query").find("input:hidden").each(function () {
        if (this.value !== "" && this.id.substr(0, 3) === "hid") {
            b = true;
            return b;
        }
    });
    return b;
}