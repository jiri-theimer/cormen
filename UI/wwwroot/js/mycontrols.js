function mycheckboxlist_checked(chk, hidden_id,value) {
    
    if (chk.checked === true) {
        //$("#" + chklist_name + "_" + chk.value).attr("name", chklist_name) //set element name
        $("#" + hidden_id).val(value);
       
    } else {
        //$("#" + chklist_name+"_"+chk.value).attr("name", "xxx")   //reset element name
        $("#" + hidden_id).val("0");
    }

    var s = document.getElementsByName(chklist_name)[0].value;
    alert(s);
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
    val = val.replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    val = val.replace(/[.]/g, ',');

    ctl.setAttribute("type", "text");
    ctl.value = val;


    


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


function mystitky_select(event, entity, o51ids) {
    
    _zoom(event,null,null,"small","/o51/MultiSelect?entity=" + entity + "&o51ids=" + o51ids);
    
}