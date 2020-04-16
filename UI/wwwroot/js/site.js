// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function _get_request_param(name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);    
    return results ? decodeURIComponent(results[1].replace(/\+/g, '%20')) : null;
}


function _focusAndCursor(selector) {
    var input = $(selector);
    setTimeout(function () {
        // this focus on last character if input isn't empty
        tmp = input.val(); input.focus().val("").blur().focus().val(tmp);
    }, 200);
}

function _toolbar_warn2save_changes() {
    if ($("#toolbar_changeinfo").length) {
        $("#toolbar_changeinfo").text("Změny potvrďte tlačítkem [Uložit změny]...");
    }
    
}


function _notify_message(strMessage, strTemplate = "error", milisecs="3000") {
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
    
    var s = "<div id='" + toast_id + "' class='toast' data-autohide='true' data-delay='" + milisecs+"' data-animation='true' style='margin-top:10px;min-width:"+w+";'>";
    s = s + "<div class='toast-header text-dark bg-" + strTemplate+"'><i class='fas fa-"+img+"'></i>";
    //s = s + "<strong class='mr-auto' style='color:black;'>Toast Header</strong>";
    s = s + "<div style='width:90%;'> " + strTemplate+"</div><button type='button' class='ml-2 mb-1 close' data-dismiss='toast'>&times;</button>";
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


function _handle_searchbox(searchstring,url,divid) {
    $.post(url, { expr: searchstring}, function (data) {
        $("#" + divid).css("display","block");
        $("#" + divid).html(data);


    });
}