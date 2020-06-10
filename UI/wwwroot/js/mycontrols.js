﻿function mycheckboxlist_checked(chk, hidden_id,value) {
    
    if (chk.checked === true) {
        
        $("#" + hidden_id).val(value);
       
    } else {
        
        $("#" + hidden_id).val("0");
    }

    
}
function myradiolist_checked(hidden_id, value, event_after_changevalue) {
    $("#" + hidden_id).val(value);

    if (event_after_changevalue !== "") {
        eval(event_after_changevalue + "('" + value+"')");
    }
}
function mynumber_blur(ctl, decimaldigits) {
    var num = 0;
    if (ctl.getAttribute("type") === "text") {
        var s = $(ctl).val().replace(/\s/g, '').replace(",", ".");
        num = Number(s);
    } else {
        num = Number($(ctl).val());
    }

    //val = num.toFixed(decimaldigits).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    val = num.toFixed(decimaldigits);

    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    $("#" + forid).val(val.replace(".",","));    //pro uložení na server v rámci hostitelského view, je třeba předávat desetinnou čárku a nikoliv tečku!

    //dále pro zformátování čísla navenek    
    if (val.indexOf(".") > 0) {
        arr = val.split(".");
        arr[0] = arr[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        val = arr[0] + "," + arr[1];
    } else {       
        val = val.replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    }
   
    
    val = val.replace(/[.]/g, ',');

    ctl.setAttribute("type", "text");
    ctl.value = val;


    


}

function formatNumber(num) {
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}


function mynumber_focus(ctl) {

    var s = ctl.value;
    s = s.replace(/\s/g, '');
    s = s.replace(/[,]/g, '.');
    ctl.value = s;

    var strBrowser = getBrowser();
    if (strBrowser !== "firefox" && strBrowser !== "edge") {
        ctl.setAttribute("type", "number");
    }
    ctl.select();



}



function getBrowser() {
    var browser_name = '';
    isIE = /*@cc_on!@*/false || !!document.documentMode;
    isEdge = !isIE && !!window.StyleMedia;
    if (navigator.userAgent.indexOf("Chrome") !== -1 && !isEdge) {
        browser_name = "chrome";
    }
    else if (navigator.userAgent.indexOf("Safari") !== -1 && !isEdge) {
        browser_name = "safari";
    }
    else if (navigator.userAgent.indexOf("Firefox") !== -1) {
        browser_name = "firefox";
    }
    else if ((navigator.userAgent.indexOf("MSIE") !== -1) || (!!document.documentMode === true)) //IF IE > 10
    {
        browser_name = "ie";
    }
    else if (isEdge) {
        browser_name = "edge";
    }
    else {
        browser_name = "other-browser";
    }

    return browser_name;
}


function datepicker_init(inputid) {

    $("#"+inputid).datepicker({
        format: "dd.mm.yyyy",
        todayBtn: "linked",
        clearBtn: true,
        language: "cs",
        todayHighlight: true,
        autoclose: true
    });
    
}

function datepicker_button_click(inputid) {
    $("#"+inputid).focus();
}

function datepicker_change(ctl) {
    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    var s = ctl.value;
    if (document.getElementById(forid + "_Time")) {
        s = s + " " + document.getElementById(forid + "_Time").value;
    }
    if (s.trim() === "00:00") s = "";   //je třeba to vyčistit, aby se do db uložila null value
  
    $("#" + forid).val(s);    //pro uložení na server v rámci hostitelského view
    
    
}
function datepicker_time_change(ctl) {
    
    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    var s = $("#" + forid +"helper").val()+" "+$(ctl).val();
    
    $("#" + forid).val(s);    //pro uložení na server v rámci hostitelského view

}
function datepicker_get_value(ctlClientID) {
    var value = $("#" + ctlClientID + "helper").datepicker("getDate");
    return (value);
}
function datepicker_set_value(ctlClientID, datValue) {
    $("#" + ctlClientID + "helper").datepicker("setDate", datValue);
}




/*_MyAutoComplete*/
function myautocomplete_init(c) {
    var _tableid = c.controlid + "tab1";
    $("#" + c.controlid + "_search").on("focus", function () {
        document.getElementById(c.controlid + "_search").select();
    });

    $("#" + c.controlid + "_search").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#" + _tableid + " tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#" + c.controlid +"_Dropdown").on("show.bs.dropdown", function () {
        _focusAndCursor(document.getElementById(c.controlid + "_search"));

        if ($("#" + c.dropdownid).prop("filled") === true) return;    //combo už bylo dříve otevřeno


        $.post(c.posturl, { o15flag: c.o15flag, tableid: _tableid }, function (data) {
            
            $("#" + c.controlid + "_divtab").html(data);

            $("#" + c.dropdownid).prop("filled", true);

            $("#" + _tableid + " .txz").on("click", function () {
                s = $(this).find("td:first").text();

                $("#" + c.controlid).val(s);
                $("#"+c.controlid).select();
                _toolbar_warn2save_changes();
            });

        });


    })

}


function mystitky_multiselect(event, entity) {
    var o51ids = $("#TagPids").val();
    _zoom(event, null, null, "small", "★Zatřídit do kategorií...", "/o51/MultiSelect?entity=" + entity + "&o51ids=" + o51ids);
    
}


/*taghelper mycombochecklist*/
function mycombochecklist_init(c) {
    
    $("#divDropdown" + c.controlid).on("click.bs.dropdown", function (e) {
        e.stopPropagation();                                    //click na dropdown oblast nemá zavírat dropdown div
    });

    $("#value_alias_" + c.controlid).click(function () {
        $("#cmdCombo" + c.controlid).dropdown("toggle");        //click na textbox se má chovat stejně jako click na tlačítko cmdCombo
    });
    
    $("#divDropdownContainer" + c.controlid).on("show.bs.dropdown", function (e) {

        $("#divDropdown" + c.controlid).css("margin-left", 25 + $("#divDropdown" + c.controlid).width() * -1);      //šírka dropdown oblasti má být zleva 100% jako celý usercontrol


        if ($("#divDropdown" + c.controlid).prop("filled") === true) return;    //combo už bylo dříve otevřeno
        
        $.post(c.posturl, { controlid: c.controlid, entity: c.entity, selectedvalues: c.selectedvalues, masterprefix: c.masterprefix, masterpid: c.masterpid, param1:c.param1 }, function (data) {
            
            $("#divData" + c.controlid).html(data);

            $("#divDropdown" + c.controlid).prop("filled", true);

            $("input:checkbox[name='chk"+c.controlid+"']").click(function () {
                var checked = $(this).prop("checked");

                var vals = [];
                var lbls = [];
                var strLabel = "";
                $("input:checkbox[name=chk"+c.controlid+"]:checked").each(function () {
                    vals.push($(this).val());
                    strLabel = $("label[for=" + $(this).attr("id") + "]").text();
                    lbls.push(strLabel);
                    
                });
                var s = vals.join(",");

                $("#" + c.controlid).val(s);

                s = lbls.join(",");
                $("#value_alias_" + c.controlid).val(s);

                
            });

            $("#cmdCheckAll" + c.controlid).click(function () {
                var vals = [];
                var lbls = [];
                var strLabel = "";
                $("input:checkbox[name=chk" + c.controlid + "]").each(function () {
                    $(this).prop("checked", true);
                    vals.push($(this).val());
                    strLabel = $("label[for=" + $(this).attr("id") + "]").text();
                    lbls.push(strLabel);
                });
                var s = vals.join(",");
                $("#" + c.controlid).val(s);
                s = lbls.join(",");
                $("#value_alias_" + c.controlid).val(s);
            });

            $("#cmdUnCheckAll" + c.controlid).click(function () {
                $("input:checkbox[name=chk" + c.controlid + "]").each(function () {
                    $(this).prop("checked", false);

                });
                $("#" + c.controlid).val("");
                $("#value_alias_" + c.controlid).val("");
            });


        });


    });

    

}