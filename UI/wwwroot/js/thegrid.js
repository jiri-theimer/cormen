﻿var _ds;
var _j72id;
var _tg_url_data;
var _tg_url_handler;
var _tg_url_filter;
var _tg_entity;
var _tg_filterinput_timeout;
var _tg_filter_is_active;
var _tg_go2pid;
var _tg_master_entity;
var _tg_master_pid;
var _tg_contextmenuflag;
var _tg_dblclick;

function tg_init(c) {
    _tg_entity = c.entity;
    _j72id = c.j72id;
    _tg_url_data = c.dataurl;
    _tg_url_handler = c.handlerurl;
    _tg_url_filter = c.filterurl;
    _tg_go2pid = c.go2pid;
    _tg_master_entity = c.master_entity;
    _tg_master_pid = c.master_pid;    
    _tg_contextmenuflag = c.contextmenuflag;
    _tg_ondblclick = c.ondblclick;    

    tg_post_data();

    $("#container_grid").scroll(function () {
        $("#container_vScroll").width($("#container_grid").width() + $("#container_grid").scrollLeft());
    });

    $("#tabgrid0 .sortcolumn").on("click", function () {
        var field = this.id.replace("th_", "");
        tg_post_handler("sorter", "sortfield", field);
    });


    tg_setup_checkbox_handler();

    var parentElement = document.getElementById("container_grid").parentNode;
    if (parentElement.id !== "splitter_panel1") {        
        tg_adjust_for_screen(); //bude voláno až po inicializaci splitteru v mateřské stránce gridu
    }
    
    
    

    $("#tabgrid1_thead .query_textbox").on("focus", function (e) {
        $(this).select();
    });

    $("#tabgrid1_thead .query_textbox").on("input", function (e) {
        var txt1 = this;
        if (typeof _tg_filterinput_timeout !== "undefined") {
            clearTimeout(_tg_filterinput_timeout);
        }
        _tg_filterinput_timeout = setTimeout(function () {

            //your ajax stuff , 500ms zpoždění, aby se počkalo na víc stringů od uživatele 
            var field = txt1.id.replace("txtqry_", "");
            if (txt1.value !== "") {
                $("#hidqry_" + field).val(txt1.value);
                $("#hidoper_" + field).val("3")    //natvrdo doplnit operátor OBSAHUJE
                $("#txtqry_" + field).css("background-color", "red");
                $("#txtqry_" + field).css("color", "white");
            } else {
                $("#hidqry_" + field).val("");
                $("#txtqry_" + field).css("background-color", "");
                $("#txtqry_" + field).css("color", "");
            }

            
            tg_filter_send2server();
            


        }, 500);

        
    });

    $("#tabgrid1_thead button.query_button").on("click", function (e) {
        var field = this.id.replace("cmdqry_", "");
        var caption = $("#th_" + field).text();
        var coltypename = normalize_coltype_name($("#th_" + field).attr("columntypename"));

        if ($("#tg_div_filter_field").val() === field && $("#tg_div_filter").css("display") === "block") {
            tg_filter_hide_popup(); //toto filter je již otevřeno
            return;
        }
        var ofs = $(this).offset();
        var l = 10 + $(this).width() + ofs.left - $("#tg_div_filter").width();
        if (l < 0) l = 0;
        $("#tg_div_filter").css("left", l);
        $("#tg_div_filter").css("top", $(this).height() + ofs.top + 1);
        $("#tg_div_filter_header").text(caption);
        $("#tg_div_filter_field").val(field);

        $("#tg_div_filter").css("display", "block");
        tg_filter_prepare_popup(field, coltypename);
    });


    _tg_filter_is_active = tg_is_filter_active();
}

function tg_post_data() {
    var params = {
        entity: _tg_entity,
        j72id: _j72id,
        go2pid: _tg_go2pid,
        master_entity: _tg_master_entity,
        master_pid: _tg_master_pid,
        contextmenuflag: _tg_contextmenuflag,
        ondblclick: _tg_ondblclick
    }    
    
    $.post(_tg_url_data, {tgi:params}, function (data) {        
        
        refresh_environment_after_post("first_data", data);

        if (_tg_go2pid !== null && _tg_go2pid !== 0) {
            tg_go2pid(_tg_go2pid);
        }
        

        
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
    var params = {
        j72id: _j72id,
        oper: strOper,
        key: strKey,
        value: strValue,
        master_entity: _tg_master_entity,
        master_pid: _tg_master_pid,
        contextmenuflag: _tg_contextmenuflag,
        ondblclick: _tg_ondblclick
    }    
    $.post(_tg_url_handler, { tgi: params}, function (data) {
       // _notify_message("vrátilo se: oper: " + strOper + ", key: " + strKey + ", value: " + strValue);

        refresh_environment_after_post(strOper,data);
    });

}


function refresh_environment_after_post(strOper,data) {    
    $("#thegrid_message").text(data.message);
    $("#tabgrid1_tbody").html(data.body);
    $("#tabgrid1_tfoot").html(data.foot);
    $("#divPager").html(data.pager);

    if (strOper === "sorter" || strOper ==="first_data") {
        tg_refresh_sorter(data.sortfield, data.sortdir);
    }

    tg_adjust_parts_width();

    var basewidth = $("#tabgrid0").width();
    $("#tabgrid1").width(basewidth);
    $("#tabgrid2").width(basewidth);


    tg_setup_selectable();

    
}

function tg_adjust_parts_width() {

    $("#container_vScroll").width($("#container_grid").width() + $("#container_grid").scrollLeft());
}


function tg_setup_selectable() {    
    _ds = new DragSelect({
        selectables: document.getElementsByClassName('selectable'), // node/nodes that can be selected. This is also optional, you could just add them later with .addSelectables.
        selectedClass: "selrow",
        area: document.getElementById("container_vScroll"), // area in which you can drag. If not provided it will be the whole document.
        customStyles: false,  // If set to true, no styles (except for position absolute) will be applied by default.
        multiSelectKeys: ['ctrlKey', 'shiftKey', 'metaKey'],  // special keys that allow multiselection.
        multiSelectMode: false,  // If set to true, the multiselection behavior will be turned on by default without the need of modifier keys. Default: false
        autoScrollSpeed: 20,  // Speed in which the area scrolls while selecting (if available). Unit is pixel per movement. Set to 0 to disable autoscrolling. Default = 1
        onDragStart: function (element) {}, // fired when the user clicks in the area. This callback gets the event object. Executed after DragSelect function code ran, befor the setup of event listeners.
        onDragMove: function (element) {}, // fired when the user drags. This callback gets the event object. Executed before DragSelect function code ran, after getting the current mouse position.
        onElementSelect: function (element) { }, // fired every time an element is selected. (element) = just selected node
        onElementUnselect: function (element) { }, // fired every time an element is de-selected. (element) = just de-selected node.
        callback: function (elements) {
            // fired once the user releases the mouse. (elements) = selected nodes.
            $("#tg_selected_pids_pre").val($("#tg_selected_pids").val());
            var pid_pre = $("#tg_selected_pid").val();
            if (elements.length===0) {
                return
            }
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

            if (pid !== pid_pre) {
                thegrid_handle_event("rowselect", pid); //povinná metoda na hostitelské stránce gridu!
            }
            
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

function tg_go2pid(pid) {       //již musí být ze serveru odstránkováno!
    if (document.getElementById("r" + pid)) {
        var row = document.getElementById("r" + pid);
        _ds.addSelection(row);
        $("#tg_selected_pids").val(pid);
        row.scrollIntoView(true);

        //var rowpos = $(row).position();
        //$("#container_vScroll").scrollTop(rowpos.top);
    }
    
}

function tg_select(records_count) {     //označí prvních X (records_count) záznamů
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
    $("#cmdDestroyFilter").css("display", "none");
    
    $("#tr_header_query").find("input:hidden").each(function () {
        if (this.id.indexOf("hidqry_") > -1 && this.value !== "") {
            
            $("#cmdDestroyFilter").css("display", "block");
            b = true;
            return b;
        }
        if (this.id.indexOf("hidoper_") > -1 && this.value !== "3" && this.value !== "" && this.value !== "0") {

            $("#cmdDestroyFilter").css("display", "block");
            b = true;
            return b;
        }
    });
    return b;
}


//filtrování z MT6:
function tg_qryval_keydown(e) {
    if (e.keyCode === 13) {
        tg_filter_ok();
        return false;
    }
    if (e.keyCode === 27) {
        tg_filter_hide_popup();
        return false;
    }

}


function tg_filter_clear() {
    $("#tr_header_query").find("input:hidden").each(function () {
        if (this.id.substr(0, 3) === "hid") {
            this.value = "";
            var field = this.id.replace("hidqry_", "");
            if (document.getElementById("qryalias_" + field)) {
                $("#qryalias_" + field).html("");
            }
            if (document.getElementById("txtqry_" + field)) {
                $("#txtqry_" + field).val("");
                $("#txtqry_" + field).css("background-color", "");
            }


        }
    });
    
    
    
    tg_filter_send2server();

}
function normalize_coltype_name(coltypename) {
    if (coltypename ==="num0" || coltypename === "num3" || coltypename === "num") coltypename = "number";
    if (coltypename === "datetime") coltypename = "date";
    return (coltypename);
}
function tg_filter_ok() {
    $("#cmdDestroyShowOnlyPID").css("display", "none");
   
    if ($("input[name='chlfilter']:checked").length === 0) {
        _notify_message("Musíte zaškrtnout jeden z filtrovacích operátorů.", "warning");
        return;
    }
    var c1 = document.getElementById("qryval1");
    var c2 = document.getElementById("qryval2");
    var field = $("#tg_div_filter_field").val();
    var coltypename = normalize_coltype_name($("#th_" + field).attr("columntypename"));
    
    
    var operator = $("input[name='chlfilter']:checked").val();
    var av = tg_filter_operator_as_alias(operator);

    var fv = "";

    if (coltypename === "bool") {
        fv = operator;
    }
    if (coltypename === "string") {
        fv = c1.value;
        if (fv === "" && (operator === "3" || operator === "4" || operator === "5" || operator === "6" || operator === "7")) {
            _notify_message("Musíte vyplnit filtrovací výraz.", "warning");
            c1.focus();
            return;
        }
        if (operator === "0" || operator === "1" || operator === "2") fv = "";
    }
    if (coltypename === "number" || coltypename === "date") {
        if (operator === "4" && (c1.value === "" || c2.value === "")) {
            _notify_message("Musíte vyplnit hodnoty od - do.", "error");
            c1.focus();
            return;
        }
    }

    var filter_before = tg_is_filter_active();

    $("#qryalias_" + field).css("visibility", "visible");

    if (coltypename === "number") {
        fv = c1.value + "|" + c2.value;
        if (operator === "0" || operator === "1" || operator === "2" || operator === "10" || operator === "11") fv = "";
        if (av === "" && operator !== "0") av = c1.value + " - " + c2.value;
    }
    if (coltypename === "date") {
        fv = c1.value + "|" + c2.value;
        if (operator === "0" || operator === "1" || operator === "2") fv = "";
        if (av === "" && operator !== "0") av = c1.value + "<br>" + c2.value;
    }
    if (coltypename === "string") {
        if (av === "" && operator !== "0") av = c1.value;
        if (operator === "5") av = "[*=] " + av;
        if (operator === "6") av = "[=] " + av;
        if (operator === "7") av = "[<>] " + av;
        
        $("#txtqry_" + field).css("display", "block");
        if (operator === "3" || operator === "0") {
            $("#qryalias_" + field).css("visibility", "hidden");
            $("#txtqry_" + field).css("display", "block");
            $("#txtqry_" + field).val(fv);
        } else {
            $("#txtqry_" + field).css("display", "none");
        }
        if (operator === "3" && fv !== "") {
            $("#txtqry_" + field).css("background-color", "red");
            $("#txtqry_" + field).css("color", "white");
        } else {
            $("#txtqry_" + field).css("background-color", "");
            $("#txtqry_" + field).css("color", "");
        }
        
    }
    
    if (operator === "0") {
        $("#hidqry_" + field).val("");
    } else {
        $("#hidqry_" + field).val(fv);
    }

    $("#qryalias_" + field).html(av);
    
    
    $("#hidoper_" + field).val(operator);

    _tg_filter_is_active = tg_is_filter_active();
    //if (filter_before !== _tg_filter_is_active && _tg_filter_is_active === true) {
    
    //    //tg_raise_page_event("filter-change");   //odeslat událost do mateřské stránky
    //}
    if (_tg_filter_is_active === false && filter_before === true) {
        tg_filter_clear();
        
        //tg_raise_page_event("filter-clear");   //odeslat událost do mateřské stránky
    }

    tg_filter_hide_popup();
    tg_filter_send2server();

}

function tg_get_qry_value(field, coltypename) {
    var ret = {
        operator: -1,
        filtervalue: "",
        c1value: "",
        c2value: "",
        aliasvalue: ""
    }    
    
    ret.operator = parseInt($("#hidoper_" + field).val());    
    var s = $("#hidqry_" + field).val();
    ret.filtervalue = s;
    if (s === "") {
        return ret;
    }
    

    if ((coltypename === "date" || coltypename === "number") && ret.filtervalue !== "") {
        arr = ret.filtervalue.split("|");
        ret.c1value = arr[0];
        ret.c2value = arr[1];
        ret.aliasvalue = ret.c1value + " - " + ret.c2value;
    }
    if (coltypename === "string") {
        ret.aliasvalue = ret.filtervalue;
    }

    ret.aliasvalue = tg_filter_operator_as_alias(ret.operator);

    return ret;
}

function tg_filter_operator_as_alias(operator) {
    operator = String(operator);
    if (operator === "1") return "Je prázdné";
    if (operator === "2") return "Není prázdné";
    if (operator === "8") return "ANO";
    if (operator === "9") return "NE";
    if (operator === "10") return "&gt;0";
    if (operator === "11") return "0 nebo prázdné";
    return "";
}


function tg_filter_radio_change(ctl) {
    $("#hidoper_" + $("#tg_div_filter_field").val()).val(ctl.value);

    if (ctl.value === "0" || ctl.value === "1" || ctl.value === "2" || ctl.value === "8" || ctl.value === "9" || ctl.value === "10" || ctl.value === "11") {
        
        
        tg_filter_hide_popup();
        tg_filter_ok();
    }
    if (ctl.value ==="3" || ctl.value === "4" || ctl.value === "5" || ctl.value === "6" || ctl.value === "7") {
        $("#tg_div_filter_inputs").css("visibility", "visible");
        document.getElementById("qryval1").focus();
        document.getElementById("qryval1").select();
    }

}


function tg_filter_prepare_popup(field, coltypename) {
    var c1 = document.getElementById("qryval1");
    var c2 = document.getElementById("qryval2");
    var curqryvalue = tg_get_qry_value(field, coltypename);
    
    $(c1).datepicker("destroy");
    $(c2).datepicker("destroy");
    c1.attributes["type"].value = "text"
    c2.attributes["type"].value = "text";
    $(c1).css("display", "block");
    $(c2).css("display", "block");
    c1.value = ""
    c2.value = "";

    $("#tg_div_filter_radios").find("div").css("display", "none");
    $("#tg_div_filter_inputs").css("visibility", "visible");
    $("#tg_div_filter_inputs").find("label").css("visibility", "visible");

    $("#tg_div_filter input:radio").prop("checked", false);
    
    if (curqryvalue.operator !== -1) {        
        $("#chkf" + curqryvalue.operator).prop("checked", true);
    }

    if (coltypename === "date") {
        
        $("#tg_div_filter [cdate|='1']").css("display", "block");
        if (curqryvalue.operator === -1) {
            $("#chkf4").prop("checked", true)
        } else {
            c1.value = curqryvalue.c1value;
            c2.value = curqryvalue.c2value;
        }
        
        $(c1).datepicker({
            format: "dd.mm.yyyy",
            todayBtn: "linked",
            clearBtn: true,
            language: "cs",
            todayHighlight: true,
            autoclose: true
        });
        $(c2).datepicker({
            format: "dd.mm.yyyy",
            todayBtn: "linked",
            clearBtn: true,
            language: "cs",
            todayHighlight: true,
            autoclose: true
        });
    }

    if (coltypename === "number") {
        $("#tg_div_filter [cnumber|='1']").css("display", "block");
        c1.attributes["type"].value = "number";
        c2.attributes["type"].value = "number";
       
        if (curqryvalue.operator === -1) {
            $("#chkf4").prop("checked", true)
        } else {
            c1.value = curqryvalue.c1value;
            c2.value = curqryvalue.c2value;
        }
        c1.focus(), c1.select();
    }

    if (coltypename === "string") {
        if (curqryvalue.operator === -1) $("#chkf3").prop("checked", true);
        c1.value = curqryvalue.filtervalue;
        $("#tg_div_filter [cstring|='1']").css("display", "block");
        $("#tg_div_filter_inputs").find("label").css("visibility", "hidden");
        $(c2).css("display", "none");
        c1.focus(), c1.select();
    }
    if (coltypename === "bool") {
        if (curqryvalue.operator === -1) $("#chkf0").prop("checked", true);
        $("#tg_div_filter_inputs").css("visibility", "hidden");
        $("#tg_div_filter [cbool|='1']").css("display", "block");

        $("#tg_div_filter_inputs").find("label").css("visibility", "hidden");
        $(c2).css("display", "none");

    }

    //var operator = $("input[name='chlfilter']:checked").val();
    
    
    if (curqryvalue.operator === 0 || curqryvalue.operator === 1 || curqryvalue.operator === 2) {
        $("#tg_div_filter_inputs").css("visibility", "hidden");
    }


}


function tg_filter_hide_popup() {
    $("#tg_div_filter").css("display", "none");
    $("#tg_div_filter_header").text("");


}


function tg_filter_send2server() {      
    //odeslat filtrovací podmínku na server, na serveru musí odpovídat třídě BO.TheGridColumnFilter
    var rec;
    var ret = [];    
    $("#tr_header_query").find("input[type='hidden']").each(function () {
        if (this.id.indexOf("hidqry_") >= 0) {
            var fieldname = this.id.replace("hidqry_", "");
            var coltypename = normalize_coltype_name($("#th_" + fieldname).attr("columntypename"));
            var oper = $("#hidoper_" + fieldname).val();
            var val = $(this).val();
            
            if (coltypename === "string") {                
                if (val !== "" && oper === "") oper = "3";
            }

            
            if (val !== "" || parseInt(oper) > 0) {
                rec = {
                    field: fieldname,
                    value: val,
                    oper: oper
                }
                //alert("val: "+val+", rec.value: " + rec.value + ", oper: " + rec.oper + ", field: " + rec.field);

                ret.push(rec);
            }
        }
        
        
    });

    var params = {
        j72id: _j72id,        
        master_entity: _tg_master_entity,
        master_pid: _tg_master_pid,
        contextmenuflag: _tg_contextmenuflag,    
        ondblclick: _tg_ondblclick,
    }    

    
    $.post(_tg_url_filter, {tgi:params, filter: ret}, function (data) {
        
        refresh_environment_after_post("filter", data);


        _tg_filter_is_active = tg_is_filter_active();
        if (_tg_filter_is_active === true) {
            $("#cmdDestroyFilter").css("display", "block");

        } else {
            $("#cmdDestroyFilter").css("display", "none");

        }

    });


    
}

function tg_cm(e) {     //vyvolání kontextového menu k vybranému záznamu
    var link = e.target;
    var pid = link.parentNode.parentNode.id.replace("r","");
    
    _cm(e, _tg_entity, pid);
}


function tg_adjust_for_screen(strParentElementID) {
    if (!document.getElementById("tabgrid0")) return;

    
            
    var w0 = 0;
    if (typeof strParentElementID !=="undefined") {
        //výška gridu bude odvozená podle nadřízeného elementu strParentElementID
        var parentElement = document.getElementById(strParentElementID);
        var hh = $(parentElement).height() - $("#tabgrid0").height() - $("#tabgrid2").height() - $("#divPagerContainer").height() - 2;
        if ($("#tabgrid1_tfoot").height() <= 2) {
            hh = hh - 35;   //rezerva pro tfoot s TOTALS
        }
        w0 = $(parentElement).width();

        if ($("#tabgrid0").width() > w0) {
            hh = hh - 20;   //je vidět horizontální scrollbara, ubereme výšku, aby byla vidět, 20: odhad výšky scrollbary            
        }
        hh = hh + 5;
        $("#container_vScroll").css("height", hh + "px");



    } else {
        //režim 1 panelu, bez splitter
        var div_horizontal = $("#container_grid");
        var offset = $(div_horizontal).offset();
        var h_vertical = _device.innerHeight - offset.top - $("#tabgrid0").height() - $("#tabgrid2").height() - $("#divPagerContainer").height();
        
        w0 = document.getElementById("container_grid").scrollWidth; //nefunguje přes jquery
        var w1 = $(div_horizontal).width();        
        if (w0 > w1) h_vertical = h_vertical - 20;   //je vidět horizontální scrollbara, ubereme výšku, aby byla vidět
        if ($("#tabgrid1_tfoot").height() <= 2) {
            h_vertical = h_vertical - 35;   //rezerva pro tfoot s TOTALS
        }

        h_vertical = parseInt(h_vertical);    
        
        $("#container_vScroll").css("height", h_vertical + "px");
        $(document.body).css("overflow-y", "hidden");
        
    }

    $("#container_vScroll").width($("#container_grid").width() + $("#container_grid").scrollLeft());

    //var basewidth = $("#tabgrid0").width();
    //$("#tabgrid1").width(basewidth);
    //$("#tabgrid2").width(basewidth);
    

    
}

function tg_dblclick(row) {
    _edit(_tg_entity.substr(0, 3), row.id.replace("r", ""));
}

function tg_export(format) {
    var url = "/TheGrid/GridExport?j72id=" + _j72id + "&format=" + format + "&master_pid=" + _tg_master_pid + "&master_entity=" + _tg_master_entity;
    location.replace(url);

}