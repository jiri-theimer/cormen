// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function get_request_param(name) {
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