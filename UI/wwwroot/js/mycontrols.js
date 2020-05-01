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
    $("#" + forid).val(ctl.value);    //pro uložení na server v rámci hostitelského view

}