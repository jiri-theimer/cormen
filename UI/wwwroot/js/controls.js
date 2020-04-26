


//_MyCombo
function _mycombo_init(c) {
    var _combo_currentFocus = -1;
    var searchbox = $("#" + c.controlid + "_search");
    var tabbody = $("#" + c.tableid + "_tbody");
    var rows_count = $(tabbody).find("tr").length;
    var lookup = $(searchbox).val();

    function handle_update_state() {
        tabbody = $("#" + c.tableid + "_tbody");
        rows_count = $(tabbody).find("tr").length;
    }
    function get_first_visible_rowindex(fromindex, direction) {
        if (direction == "down") {
            for (i = fromindex; i < rows_count; i++) {
                var row = tabbody.find("tr").eq(i);
                if ($(row).css("display") != "none") {
                    return i;
                }
            }
        }
        if (direction == "up") {
            for (i = fromindex; i >=0; i--) {                
                var row = tabbody.find("tr").eq(i);
                if ($(row).css("display") != "none") {
                    return i;
                }
            }
        }
        
        return -1;
    }

    
   
    $(searchbox).on("focus", function () {

        $(searchbox).select();
    });


    $(searchbox).on("keyup", function () {
        var value = $(this).val().toLowerCase();
        var x = 0;
        if ($(searchbox).val() != lookup) {
            $("#" + c.tableid + "_tbody tr").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                x++;
                if (x == rows_count) {
                    recovery_selected();

                    _notify_message("filtr: "+$(searchbox).val());
                }

            });
        }
        lookup = $(searchbox).val();

        
    });
   

    $(searchbox).on("keydown", function (e) {
        if (e.keyCode == 40) {  //down
            handle_update_state()

            if (rows_count - 1 <= _combo_currentFocus) {
                _combo_currentFocus = 0

            } else {
                _combo_currentFocus++;                
                                              
            }
            var destrowindex = get_first_visible_rowindex(_combo_currentFocus,"down");
            if (destrowindex == -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex == -1) return;
            
            update_selected_row();
            
        }
        if (e.keyCode == 38) {  //up
            handle_update_state();
            _combo_currentFocus--;
            var destrowindex = get_first_visible_rowindex(_combo_currentFocus, "up");
            if (destrowindex == -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex == -1) _combo_currentFocus = 0;
            

            update_selected_row();
        }
        //_notify_message("aktuální pozice: " + _combo_currentFocus);
        
    });

    function recovery_selected() {
        handle_update_state();
        _combo_currentFocus = get_first_visible_rowindex(0,"down");
        _notify_message(_combo_currentFocus);
        update_selected_row();
    }
    function update_selected_row() {

        $(tabbody).find("tr").removeClass("selrow");
        if (_combo_currentFocus > -1) {
            var row = tabbody.find("tr").eq(_combo_currentFocus);

            $(row).addClass("selrow");
        }
        
        
    }
    


    $("#" + c.controlid + "_clear").on("click", function () {

        var ctl = $("#" + c.dropdownid + " :input[type='hidden']");
        $(ctl[0]).val("");
        $(ctl[1]).val("");
        $("#" + c.controlid + "_cmdCombo").text("");
        $("#" + c.dropdownid).prop("filled", false);

    });

    


    $("#" + c.dropdownid).on("show.bs.dropdown", function (e) {
        _focusAndCursor(searchbox);

        if ($("#" + c.dropdownid).prop("filled") == true) {
            
            return;    //combo už bylo dříve otevřeno
        }

        var prefix = $("#" + c.controlid + "_entity").val();
        
        $.post(c.posturl, { entity: prefix, curvalue: null, tableid: c.tableid, param1: c.param1 }, function (data) {
            
            $("#" + c.controlid + "_divtab").html(data);

            $("#" + c.dropdownid).prop("filled", true);

            recovery_selected();

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

