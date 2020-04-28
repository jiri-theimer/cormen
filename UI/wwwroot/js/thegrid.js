var _ds;

function tg_post_data(entity, url, j72id) {

    $.get(url, { entity: entity,url, j72id: j72id }, function (data) {

        $("#tabrid1_tbody").html(data.body);
        $("#tabgrid1_tfoot").html(data.foot);

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