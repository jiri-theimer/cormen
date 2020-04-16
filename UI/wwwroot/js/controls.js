//_MyCombo
function _mycombo_init(c) {

    $("#" + c.controlid + "_search").on("focus", function () {

        document.getElementById(c.controlid + "_search").select();
    });


    $("#" + c.controlid + "_search").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#" + c.tableid + " tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });


    $("#" + c.controlid + "_clear").on("click", function () {

        var ctl = $("#" + c.dropdownid + " :input[type='hidden']");
        $(ctl[0]).val("");
        $(ctl[1]).val("");
        $("#" + c.controlid + "_cmdCombo").text("");
        $("#" + c.dropdownid).prop("filled", false);

    });


    $("#" + c.dropdownid).on("show.bs.dropdown", function (e) {
        _focusAndCursor(document.getElementById(c.controlid + "_search"));

        if ($("#" + c.dropdownid).prop("filled") == true) {
            
            return;    //combo už bylo dříve otevřeno
        }

        var prefix = $("#" + c.controlid + "_entity").val();
        $.post(c.posturl, { entity: prefix, curvalue: null, tableid: c.tableid, param1: c.param1 }, function (data) {
            $("#" + c.controlid + "_divtab").html(data);

            $("#" + c.dropdownid).prop("filled", true);

            $("#" + c.tableid + " .txz").on("click", function () {
                var v = $(this).attr("data-v");
                var t = $(this).attr("data-t");
                if (t == "" || t == null) t = $(this).find("td:first").text();

                var ctl = $("#" + c.dropdownid + " :input[type='hidden']");
                $(ctl[0]).val(v);
                $(ctl[1]).val(t);
                $("#" + c.controlid + "_cmdCombo").text(t);

                _toolbar_warn2save_changes();

                if (c.onchange != "") {
                    eval(c.onchange + "('" + v + "')");  //nastavená JS událost po výběru hodnoty
                }

            });

            $("#" + c.controlid + "_search").focus();



        });


    })

}

/*_MyAutoComplete*/
function _myac_init(c) {

    $("#"+c.controlid+"_search").on("focus", function () {
        document.getElementById(c.controlid+"_search").select();
    });

    $("#" + c.controlid + "_search").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#" + c.tableid + " tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#"+c.dropdownid).on("show.bs.dropdown", function () {
        _focusAndCursor(document.getElementById(c.controlid + "_search"));

        if ($("#"+c.dropdownid).prop("filled") == true) return;    //combo už bylo dříve otevřeno

        
        $.post(c.posturl, { o15flag: c.o15flag, tableid: c.tableid }, function (data) {
            $("#"+c.controlid + "_divtab").html(data);

            $("#"+c.dropdownid).prop("filled", true);

            $("#"+c.tableid+" .txz").on("click", function () {
                s = $(this).find("td:first").text();

                $("#"+c.controlid+"_txt1").val(s);
                //$("#"+c.controlid+"_txt1").select();
                _toolbar_warn2save_changes();
            });

        });


    })

}

