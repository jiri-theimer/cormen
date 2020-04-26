function mycombo_init(c) {
    var _controlid = c.controlid;
    var _combo_currentFocus = -1;
    var _current_syntetic_event = "";
    var _lastFocusWasSearchbox = false;
    var _curFocusIsSearchbox = false;
    var _tabid = "tab1" + _controlid;
    var _tabbodyid = _tabid + "_tbody";
    var _cmdcombo = $("#cmdCombo" + _controlid);
    var _searchbox = $("#searchbox" + _controlid);    
    var _lookup_value = $(_searchbox).val();
    var _dropdownid = "divDropdownContainer" + _controlid;
    var _event_after_change = c.on_after_change;
    
    if (c.viewflag === "2") {//bez zobrazení search
        $("#divSearch" + _controlid).css("display", "none");
        $("#divDropdown" + _controlid).css("margin-left", "-30px");
    }
    if (c.defvalue !== "") {    //výchozí vyplněná hodnota comba
        if (c.deftext!=="") $(_cmdcombo).text(c.deftext);   

        $("[data-id=value_" + _controlid + "]").val(c.defvalue);
        $("[data-id=text_" + _controlid + "]").val(c.deftext);
        handle_update_state();
    }
    
    $("#" + _dropdownid).on("show.bs.dropdown", function () {
        
        if ($("#" + _dropdownid).prop("filled") === true) return;    //data už byla dříve načtena
        
        var entity = "p10MasterProduct";
        $.post(c.posturl, { entity: c.entity, o15flag: "", tableid: _tabid, param1: c.param1 }, function (data) {
            $("#divData"+c.controlid).html(data);
            
            $("#"+_dropdownid).prop("filled", true);
           
            $("#" + _tabid + " .txz").on("click", function () {
                record_was_selected(this);
                
                _toolbar_warn2save_changes();
            });


        });
    })


    $(_cmdcombo).on("focus", function (e) {
        _lastFocusWasSearchbox = false;
        if (e.relatedTarget !== null) {
            if (e.relatedTarget.id === "searchbox"+_controlid) {
                _lastFocusWasSearchbox = true;                
            }
        }

    })

    $(_cmdcombo).on("click", function (e) {
        if (_lastFocusWasSearchbox === true) return;

        if (_current_syntetic_event === "click_from_focus") {
            _current_syntetic_event = "";
            return;
        }

        if (_current_syntetic_event === "") {
            //normální ruční klik


            _current_syntetic_event = "focus_from_click";
            setTimeout(function () {
                //focus až po 300ms
                $(_searchbox).focus();
                $(_searchbox).select();

            }, 300);



        } else {
            _current_syntetic_event = "";

        }




    });
    $(_searchbox).on("focus", function (e) {       
      
        if (_current_syntetic_event !== "") {
            _current_syntetic_event = "";
            return;
        }
        
        if (is_dropdown_opened() === false) {
            _current_syntetic_event = "click_from_focus";
            _cmdcombo.click();
            e.stopPropagation();
            $(this).focus();
            $(this).select();
        }

    })
    $(_searchbox).on("blur", function () {
        _curFocusIsSearchbox = false;
    })

    $(_searchbox).on("click", function (e) {    
        if (_current_syntetic_event !== "") {
            return;
        }
        
        if (is_dropdown_opened() === false) {
            $("#divDropdown" + _controlid).dropdown("show");
        } else {
            if (_curFocusIsSearchbox === false) {
                _curFocusIsSearchbox = true;
                return;
            }
            $("#divDropdown" + _controlid).dropdown("hide");

        }
        

        _curFocusIsSearchbox = true;

    })
    $(_searchbox).on("input", function (e) {
        if (is_dropdown_opened() === false) {
            $("#divDropdown"+_controlid).dropdown("show");
        }


    })



    $("#searchbox"+_controlid+", #cmdCombo"+_controlid).on("keydown", function (e) {
        handle_keydown(e);
    });

    function handle_keydown(e) {
        if (e.keyCode === 27 && e.target.id === "searchbox"+_controlid) {  //ESC
            $("#divDropdown"+_controlid).dropdown("hide");
        }
        var rows_count = $("#"+_tabbodyid).find("tr").length;


        if (e.keyCode === 13 && e.target.id === "searchbox"+_controlid) {//ENTER
            if (is_dropdown_opened()) {
                var row = $("#" + _tabbodyid).find(".selrow");
                if (row.length > 0) {
                    record_was_selected(row[0]);
                }
            } else {

                $("#divDropdown"+_controlid).dropdown("show");
                e.stopPropagation();
                e.preventDefault();
            }


        }
        var destrowindex;
        if (e.keyCode === 40) {  //down
            //handle_update_state()
            if (is_dropdown_opened() === false && e.target.id === "searchbox"+_controlid) {
                $("#divDropdown"+_controlid).dropdown("show");
            }
            if (rows_count - 1 <= _combo_currentFocus) {
                _combo_currentFocus = 0

            } else {
                _combo_currentFocus++;

            }
            destrowindex = get_first_visible_rowindex(_combo_currentFocus, "down");
            if (destrowindex === -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex === -1) return;

            update_selected_row();

        }
        if (e.keyCode === 38) {  //up
            //handle_update_state();
            _combo_currentFocus--;
            destrowindex = get_first_visible_rowindex(_combo_currentFocus, "up");
            if (destrowindex === -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex === -1) _combo_currentFocus = 0;


            update_selected_row();
        }
    }

    $(_searchbox).on("keyup", function (e) {
        if (e.keyCode === 13) {
            return;
        }
        var value = $(this).val().toLowerCase();
        var x = 0;
        var rows_count = $("#" + _tabbodyid).find("tr").length;

        if ($(_searchbox).val() !== _lookup_value) {

            $("#" + _tabbodyid+" tr").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                x++;
                if (x === rows_count) {
                    recovery_selected();


                }

            });
        }
        _lookup_value = $(_searchbox).val();
    });

    $("#cmdClear" + _controlid).on("click", function () {
        
        $(_cmdcombo).text("");

        $("[data-id=value_" + _controlid + "]").val("");
        $("[data-id=text_" + _controlid + "]").val("");
        handle_update_state();
    })

    function get_first_visible_rowindex(fromindex, direction) {
        var rows_count = $("#" + _tabbodyid).find("tr").length;
        var row;
        if (direction === "down") {
            for (i = fromindex; i < rows_count; i++) {
                row = $("#" + _tabbodyid).find("tr").eq(i);
                if ($(row).css("display") !== "none") {
                    return i;
                }
            }
        }
        if (direction === "up") {
            for (i = fromindex; i >= 0; i--) {
                row = $("#" + _tabbodyid).find("tr").eq(i);
                if ($(row).css("display") !== "none") {
                    return i;
                }
            }
        }

        return -1;
    }


    function update_selected_row() {

        $("#" + _tabbodyid).find("tr").removeClass("selrow");
        if (_combo_currentFocus > -1) {
            var row = $("#" + _tabbodyid).find("tr").eq(_combo_currentFocus);

            $(row).addClass("selrow");


            var element = row[0];
            element.scrollIntoView({ block: "end", inline: "nearest" });



        }


    }

    function handle_update_state() {
        
        if ($(_cmdcombo).text() !== "") {
            $("#cmdClear"+_controlid).css("visibility", "visible");
        } else {
            $("#cmdClear"+_controlid).css("visibility", "hidden");
        }
    }


    function recovery_selected() {
        //handle_update_state();
        _combo_currentFocus = get_first_visible_rowindex(0, "down");
        update_selected_row();
    }

    function record_was_selected(row) {
        var v = $(row).attr("data-v");
        var t = $(row).attr("data-t");
        if (t === undefined) {
            t = $(row).find("td:first").text();
        }

        $(_cmdcombo).css("color", "#E83E8C");
        $(_cmdcombo).text(t);

        $("[data-id=value_" + _controlid + "]").val(v);
        $("[data-id=text_" + _controlid + "]").val(t);
              
        handle_update_state();

        if (_event_after_change !== "") {
            eval(_event_after_change + "('" + v + "')");
        }
    }

    function is_dropdown_opened() {
        if ($("#divDropdown"+_controlid).css("display") === "none") {
            return false;
        } else {
            return true;
        }
    }

}