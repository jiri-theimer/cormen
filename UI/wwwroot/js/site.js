﻿//na úvod detekce mobilního zařízení přes _device
var _device = {
    isMobile: false,
    type: "Desktop",
    availHeight: screen.availHeight,
    availWidth: screen.availWidth,
    innerWidth: window.innerWidth,
    innerHeight: window.innerHeight
}
if (screen.availHeight > screen.availWidth || screen.width < 800 || screen.height < 600) {   //mobilní zařízení výšku vyšší než šířku
    _device.isMobile = true;
    _device.type = "Phone";

}

function _edit(controller, pid, header) {

    var url = "/" + controller + "/record?pid=" + pid;
    var flag = 1;
    switch (controller) {
        case "x40":
            url = "/Mail/Record?pid=" + pid;
            break;
        case "p21":
            flag = 2;
            break;
        case "j90":
        case "j92":
        case "p44":
            _notify_message("Pro tento záznam neexistuje stránka detailu.", "info");
            return;
        default:            
            break;
    }

    _window_open(url, flag, header);

}

function _clone(controller, pid, header) {
    _window_open("/" + controller + "/record?isclone=true&pid=" + pid, 1, header);

}

function _edit_full(controller, action, pid, header) {
    _window_open("/" + controller + "/" + action + "?pid=" + pid, 1, header);
}
function _clone_full(controller, action, pid, header) {
    _window_open("/" + controller + "/" + action + "?isclone=true&pid=" + pid, 1, header);
}

function _preview(controller, pid) {
    _window_open("/" + controller + "/index?pid=" + pid, 1, "Kopírovat");

}
function _append_doc(recprefix, recpid) {
    _window_open("/o23/record?recprefix=" + recprefix + "&recpid=" + recpid);
}

function _sendmail() {
    _window_open("/Mail/SendMail");
}


function _get_request_param(name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    return results ? decodeURIComponent(results[1].replace(/\+/g, '%20')) : null;
}


function _focusAndCursor(selector) {
    var input = $(selector);
    setTimeout(function () {
        // this focus on last character if input isn't empty
        tmp = input.val(); input.focus().val("").blur().focus().val(tmp).select();
    }, 200);
}

function _toolbar_warn2save_changes() {
    if ($("#toolbar_changeinfo").length) {
        $("#toolbar_changeinfo").text("Změny potvrďte tlačítkem [Uložit změny]...");
    }

}


function _notify_message(strMessage, strTemplate = "error", milisecs = "3000") {
    if (document.getElementById("notify_container")) {
        //notify div na stránce již existuje          
    } else {
        var el = document.createElement("DIV");
        $(el).css("position", "absolute");
        $(el).css("top", "0px");
        if (screen.availWidth > 500) $(el).css("left", window.innerWidth - 500);

        el.id = "notify_container";
        document.body.appendChild(el);
    }
    if (strTemplate) {
        if (strTemplate === "") strTemplate = "info";
    } else {
        strTemplate = "info";
    }


    var img = "info", intTimeoutSeconds = 5000;
    if (strMessage.length > 250) intTimeoutSeconds = 10000;

    if (strTemplate === "error") {
        img = "exclamation-triangle";
        strTemplate = "danger";
    }
    if (strTemplate === "warning") img = "exclamation";
    if (strTemplate === "success") img = "thumbs-up";
    var toast_id = "toast" + parseInt(100000 * Math.random());

    var node = document.createElement("DIV");
    node.id = "box" + parseInt(100000 * Math.random());
    var w = "400px";
    if (screen.availWidth < 400) w = "95%";

    var s = "<div id='" + toast_id + "' class='toast' data-autohide='true' data-delay='" + milisecs + "' data-animation='true' style='margin-top:10px;min-width:" + w + ";'>";
    s = s + "<div class='toast-header text-dark bg-" + strTemplate + "'><i class='fas fa-" + img + "'></i>";
    //s = s + "<strong class='mr-auto' style='color:black;'>Toast Header</strong>";
    s = s + "<div style='width:90%;'> " + strTemplate + "</div><button type='button' class='ml-2 mb-1 close' data-dismiss='toast'>&times;</button>";
    s = s + "</div>";
    s = s + "<div class='toast-body' style='font-size:130%;'>";
    s = s + strMessage;
    s = s + "</div>";
    s = s + "</div>";


    $(node).html(s);
    document.getElementById("notify_container").appendChild(node);

    if (typeof is_permanent !== "undefined") {
        if (is_permanent === true) return;
    }

    $("#" + toast_id).toast("show");



}



function _handle_searchbox(searchstring, url, divid) {
    $.post(url, { expr: searchstring }, function (data) {
        $("#" + divid).css("display", "block");
        $("#" + divid).html(data);
        if ($("#searchbox_result").css("display") === "none") {
            $("#searchbox_result").css("display", "block");
        }


    });
}


function _generate_guid() {
    var dt = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (dt + Math.random() * 16) % 16 | 0;
        dt = Math.floor(dt / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}


//--------------------- funkce pro volání ze splitter stránek --------------------------

function _splitter_init(splitterLayout, prefix) {    //splitterLayout 1 - horní a spodní panel, 2 - levý a pravý panel
    var cont = document.getElementById("splitter_container");
    var offset = $(cont).offset();
    var h = _device.innerHeight - offset.top;
    $(cont).height(h);

    if (splitterLayout === "2" && cont.className !== "splitter-container-left2right") {
        $("#splitter_container").attr("class", "splitter-container-left2right");
        $("#splitter_resizer").attr("class", "splitter-resizer-left2right");
        $("#splitter_panel1").attr("class", "splitter-panel-left");
        $("#splitter_panel2").attr("class", "splitter-panel-right");
    }
    if (splitterLayout === "1" && cont.className !== "splitter-container-top2bottom") {
        $("#splitter_container").attr("class", "splitter-container-top2bottom");
        $("#splitter_resizer").attr("class", "splitter-resizer-top2bottom");
        $("#splitter_panel1").attr("class", "splitter-panel-top");
        $("#splitter_panel2").attr("class", "splitter-panel-bottom");
    }

    if (splitterLayout === "2") {
        document.getElementById("fra_subgrid").height = h - 1;
    }


    $("#splitter_panel1").resizable({
        handleSelector: "#splitter_resizer",

        onDragStart: function (e, $el, opt) {
            //resizeHeight: false
            //$("#splitter_panel2").html("<h6>Velikost panelu ukládám do vašeho profilu...</h6>");

            return true;
        },
        onDragEnd: function (e, $el, opt) {     //splitterLayout 1 - horní a spodní panel, 2 - levý a pravý panel
            var id = $el.attr("id");
            var panel1_size = $el.height();
            var key = prefix + "_panel1_size";

            if (splitterLayout === "2") {
                panel1_size = $el.width();
            }


            _notify_message("Velikost panelu ukládám do vašeho profilu.<hr>" + key + ": " + panel1_size + "px", "info");
            localStorage.setItem(key, panel1_size);

            if (document.getElementById("tabgrid1")) {
                tg_adjust_for_screen("splitter_panel1");
            }
            if (document.getElementById("fra_subgrid")) {
                document.getElementById("fra_subgrid").contentDocument.location.reload(true);
            }

            //run_postback(key, panel1_size);          //velikost panelu se uloží přes postback             

        }
    });
}

function _splitter_resize_after_init(splitterLayout, defaultPanel1Size) {   //splitterLayout 1 - horní a spodní panel, 2 - levý a pravý panel
    var win_size = $("#splitter_container").height();
    var splitter_size = $("#splitter_resizer").height();
    var panel1_size = parseInt(defaultPanel1Size);

    if (splitterLayout === "1") {
        //výšku iframe přepočítávat pouze v režimu horní+spodní    
        if (panel1_size === 0) panel1_size = 500;
        $("#splitter_panel1").height(panel1_size);
        //alert("panel1: " + $("#splitter_panel1").height() + ", panel2: " + $("#splitter_panel2").height());

        document.getElementById("fra_subgrid").height = win_size - panel1_size - splitter_size;
    } else {
        document.getElementById("fra_subgrid").height = win_size - 1;
        if (panel1_size === 0) panel1_size = 300;
        $("#splitter_panel1").width(panel1_size);

    }
}


////z elementu se stane draggable:
function _make_element_draggable(elmnt, inner_elmnt_4hide) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "header")) {
        /* if present, the header is where you move the DIV from:*/
        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
    } else {
        /* otherwise, move the DIV from anywhere inside the DIV:*/
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {


        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        inner_elmnt_4hide.style.display = "none";
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        /* stop moving when mouse button is released:*/
        inner_elmnt_4hide.style.display = "block";
        $("#inner_elmnt_4hide").attr("disabled", "");
        document.onmouseup = null;
        document.onmousemove = null;
    }
}


function _mainmenu_init() {     //inicializace chování hlavního menu v postraní sidebar
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            this.parentElement.querySelector(".collapsed").classList.toggle("expanded");

            this.classList.toggle("caret-down");
        });
    }



}

function _mainmenu_select(prefix_caret, data_menu) {
    if (!document.getElementById("mainmenu_caret_" + prefix_caret)) {
        data_menu = prefix_caret;
        prefix_caret = "other";
    }

    $("#mainmenu_caret_" + prefix_caret).click();

    if (typeof data_menu !== "undefined") {
        var skupina = $("#mainmenu_caret_" + prefix_caret).closest("li");

        $(skupina).find("[data-menu=" + data_menu + "]").first().addClass("selected_menu_item");

    }
}


//vyvolání kontextového menu
function _cm(e, entity, pid) {
    
    var ctl = e.target;

    var w = $(window).width();
    var pos_left = e.clientX + $(window).scrollLeft();

    var cssname = "cm_left2right";
    if (pos_left + 300 >= w) cssname = "cm_right2left";
    var menuid = "cm_" + entity + "_"+pid;
    
    if (!document.getElementById(menuid)) {
        //div na stránce neště existuje
        var el = document.createElement("DIV");
        el.id = menuid;
        el.className = cssname;
        el.style.display = "none";
        document.body.appendChild(el);
    }


    if (ctl.getAttribute("menu_je_inicializovano") === "1") {
        return; // kontextové menu bylo již u tohoto elementu inicializováno - není třeba to dělat znovu.
    }

    if (document.getElementById("divContextMenuStatic")) {
        var data = $("#divContextMenuStatic").html();   //na stránce se nachází preferované UL statického menu v divu id=divContextMenuStatic -> není třeba ho volat ze serveru
        data = data.replace(/#pid#/g, pid);  //místo #pid# replace pravé pid hodnoty
        $("#" + menuid).html(data);
    } else {
        //načíst menu ze serveru         
        $.post("/Menu/ContextMenu", { entity: entity, pid: pid }, function (data) {
            $("#" + menuid).html(data);
        });
    }

    $(ctl).contextMenu({
        menuSelector: "#" + menuid,
        menuClicker: ctl

    });

    ctl.setAttribute("menu_je_inicializovano", "1");


}



function _reload_layout_and_close(pid, flag) {

    if (window !== top) {
        window.parent.hardrefresh(pid, flag);
        window.parent._window_close();
    } else {
        hardrefresh(pid, flag);
    }
}

function _removeUrlParam(key, sourceURL) {
    var rtn = sourceURL.split("?")[0],
        param,
        params_arr = [],
        queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";
    if (queryString !== "") {
        params_arr = queryString.split("&");
        for (var i = params_arr.length - 1; i >= 0; i -= 1) {
            param = params_arr[i].split("=")[0];
            if (param === key) {
                params_arr.splice(i, 1);
            }
        }
        rtn = rtn + "?" + params_arr.join("&");
    }
    return rtn;
}


function _update_user_ping() {

    var devicetype = "Desktop";
    if (screen.availHeight > screen.availWidth || screen.width < 800 || screen.height < 600) {   //mobilní zařízení výšku vyšší než šířku
        devicetype = "Phone";
    }

    var log = {
        j92BrowserUserAgent: navigator.userAgent,
        j92BrowserAvailWidth: screen.availWidth,
        j92BrowserAvailHeight: screen.availHeight,
        j92BrowserInnerWidth: window.innerWidth,
        j92BrowserInnerHeight: window.innerHeight,
        j92BrowserDeviceType: devicetype,
        j92RequestURL: location.href.replace(location.host, "").replace(location.protocol, "").replace("///", "")
    }


    $.post("/Home/UpdateCurrentUserPing", { c: log }, function (data) {
        //ping aktualizován
    });

}



///vrací ajax výsledek
function _load_ajax_data(strHandlerUrl, params, is_async, data_type) {
    var is_success = false;
    var ret = null;
    var is_done = false;
    if (is_async === null) is_async = false;
    if (data_type === null) data_type = "json";

    $.ajax({
        url: strHandlerUrl,
        type: "POST",
        dataType: data_type,
        async: is_async,
        cache: false,
        data: params,
        complete: function (result) {
            is_success = true;
            return (ret);
        },
        success: function (result) {
            is_done = true;
            is_success = true;
            ret = result;

        },
        error: function (e, b, error) {
            is_done = true;
            alert("load_ajax_data: " + error + "/" + strHandlerUrl);
        }
    });

    if (is_async === false) {
        return (ret);
    }

}


//vyvolání zoom info okna
function _zoom(e, entity, pid, wtype, header, url) {     //wtype: small (600px) nebo big (1050px), výchozí je small
    var ctl = e.target;
    var maxwidth = $(window).width();
    var w = 600;
    var h = 600;

    if (typeof wtype !== "undefined") {
        if (wtype === "big") {
            w = 1050;
            h = 600;
        }
    }
    if (w > maxwidth) {
        w = maxwidth;
    }
    if (typeof url === "undefined") {
        url = "/" + entity + "/Index?pid=" + pid;
    }
    if (typeof header === "undefined") {
        header = "INFO";
    }

    var menuid = "zoom_okno";
    //if (pos_left + w >= maxwidth) menuid = "cm_right2left";

    if (!document.getElementById(menuid)) {
        //div na stránce neště existuje
        var el = document.createElement("DIV");
        el.id = menuid;
        el.style.display = "none";
        document.body.appendChild(el);
    }

    $("#" + menuid).width(w);
    $("#" + menuid).height(h);


    var s = "<div id='divZoomContainer' style='width:" + w + "px;' orig_w='" + w + "' orig_h='" + h + "'>";
    s += "<div id='divZoomHeader' style='cursor: move;height:30px;background-color:lightsteelblue;padding:3px;' ondblclick='_zoom_toggle()'>" + header + "</div>";
    s += "<div id='divZoomFrame'>";
    s += "<iframe id='frazoom' src = '" + url + "' style = 'width:100%;height: " + (h - 31) + "px;' frameborder=0></iframe >";
    s += "</div>";
    s += "</div>";

    $("#" + menuid).html(s);

    _make_element_draggable(document.getElementById("zoom_okno"), document.getElementById("divZoomFrame")); //předávání nefunguje přes jquery

    if (ctl.getAttribute("menu_je_inicializovano") === "1") {

        return; // kontextové menu bylo již u tohoto elementu inicializováno - není třeba to dělat znovu.
    }



    $(ctl).contextMenu({
        menuSelector: "#" + menuid,
        menuClicker: ctl

    });

    ctl.setAttribute("menu_je_inicializovano", "1");
}

function _zoom_toggle() {
    var okno = $("#divZoomContainer");
    var offset = $(okno).offset();

    var w = $(window).width() - offset.left - 10;
    var h = $(window).height() - offset.top - 10;

    if ($(window).width() - offset.left - $(okno).width() < 30) {
        w = $("#divZoomContainer").attr("orig_w");
        h = $("#divZoomContainer").attr("orig_h");
    }

    $(okno).width(w);
    $(okno).height(h);
    $("#divZoomFrame").height(h - 31);
    $("#frazoom").height(h - 31);

}

function _string_to_date(s) {
    s = s.trim();
    s = s.replace(" ", ".").replace(":", ".");
    var arr = s.split(".");
    if (arr.length > 3) {
        return (new Date(parseInt(arr[2]), parseInt(arr[1]) - 1, parseInt(arr[0]), parseInt(arr[3]), parseInt(arr[4])));
    }
    if (arr.length === 3) {
        return (new Date(arr[2], arr[1], arr[0]));
    }
    return new Date();
}


function _date_add(date, interval, units) {
    if (!(date instanceof Date))
        return undefined;
    var ret = new Date(date); //don't change original date
    var checkRollover = function () { if (ret.getDate() != date.getDate()) ret.setDate(0); };
    switch (String(interval).toLowerCase()) {
        case 'year': ret.setFullYear(ret.getFullYear() + units); checkRollover(); break;
        case 'quarter': ret.setMonth(ret.getMonth() + 3 * units); checkRollover(); break;
        case 'month': ret.setMonth(ret.getMonth() + units); checkRollover(); break;
        case 'week': ret.setDate(ret.getDate() + 7 * units); break;
        case 'day': ret.setDate(ret.getDate() + units); break;
        case 'hour': ret.setTime(ret.getTime() + units * 3600000); break;
        case 'minute': ret.setTime(ret.getTime() + units * 60000); break;
        case 'second': ret.setTime(ret.getTime() + units * 1000); break;
        default: ret = undefined; break;
    }
    return ret;
}

function _format_number(val) {
    if (val === null) {
        return ("");
    }

    var s = val.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return (s);
}
function _format_number_int(val) {
    if (val === null) {
        return ("");
    }
    var s = val.toFixed(0).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return (s);
}

function _format_date(d, is_include_time) {

    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
    var year = d.getFullYear();
    var hour = d.getHours();
    var minute = d.getMinutes();

    if (is_include_time === true) {
        return (day + "." + month + "." + year + " " + hour + ":" + minute);
    } else {
        return (day + "." + month + "." + year);
    }

}


function p21_handle_create_client_products(p21id) {
    
    if (confirm("Chcete zaktualizovat klientské produkty a receptury podle vzoru vybraných Master produktů v licenci?") === true) {
        
        $.post("/p21/CreateClientProducts", { p21id: p21id }, function (data) {
            _notify_message(data.message,"info");


        });
    }

}