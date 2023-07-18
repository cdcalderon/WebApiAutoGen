/*************************
 * ajaxHelper.js
 * J Osifchin
 * 15Jun2016
 * methods to aid in ajax posts or wrap functionality - ie spinners ui tokens etc...
 ***************************/
var waitQueue = [];

$(document).ready(function () {
    $.ajaxSetup({
        cache: false,
        dataType: "json",
        headers: { 'RequestVerificationToken': getTokenValue() }
    });

    function getTokenValue() {
        var token = $("#RequestVerificationToken").val();
        return token;
    }
});

function ajaxGet(url, successCallback, failCallback, options) {
    ajaxCall(url, null, successCallback, failCallback, "GET", options);
}

function ajaxPost(url, data, successCallback, failCallback, options) {
    ajaxCall(url, data, successCallback, failCallback, "POST", options);
}

function ajaxFormPost(url, formObject, successCallback, failCallback, options) {
    var data = getFormData(formObject);
    var formData = new FormData();

    for (var prop in data) {
        if (data.hasOwnProperty(prop)) {
            formData.append(prop, data[prop]);
        }
    }
    //post to the server
    var ajaxOptions = {
        cache: false,
        contentType: false,
        processData: false
    };
    $.extend(true, ajaxOptions, options);
    ajaxPost(url, formData, successCallback, failCallback, ajaxOptions);
}


function showPopup(html) {

    var divId = "ConfirmationEmail";

    $div = $("#" + divId);
    
    $("#mainclose").on('click', function () {
        if (typeof patientId !== "undefined") 
            if (patientId !== "") {
                reloadToUrl(patientId);
            }
    });
    $div.modal({ backdrop: 'static', keyboard: false });
}

function assignContent(divId, html) {

    $("#" + divId)[0].innerHTML = html;

    //check for load up scripts
    if ($("#" + divId).find("script").length > 0) {
        $("#" + divId).find("script").each(function () {
            eval(this.innerHTML);
        });
    }
}

function ajaxLoad(url, data, successCallback, failCallback, divId, options) {
    function loadCallback(result) {

        //div exists?
        if ($("#" + divId).length > 0) {
            assignContent(divId, result)
        }

        if (typeof successCallback == "function") {
            successCallback(result);
        }

    }

    if (typeof options == "undefined") {
        options = {};
    }
    options["dataType"] = "html";

    ajaxCall(url, data, loadCallback, failCallback, "POST", options);
}

function ajaxCall(url, data, successCallback, failCallback, type, options) {
    waitQueue.push(url);
    var ajaxOptions = {
        type: type,
        url: url,
        data: data, //double check this works as null for GET
        success: function(data) { successCallbackBase(data, successCallback, failCallback); },
        error: function(jqXHR, textStatus, errorThrown) {
            failCallbackBase(jqXHR, textStatus, errorThrown, failCallback);
        },
        complete: function (e)
        {
            waitQueue.splice(waitQueue.indexOf(url), 1);
            if ($.isEmptyObject(waitQueue)) {
                showSpinner(false);
            }
        }
    };
    if (typeof options != "undefined" && options != null) {
        $.extend(true, ajaxOptions, options);
    }
    if (typeof options == "undefined" ||
        options == null ||
        typeof options["NoSpinner"] == "undefined" ||
        options["NoSpinner"] == false) {
        showSpinner(true);
    }
    $.ajax(ajaxOptions);

}

function successCallbackBase(data, success, fail) {
    if (typeof data == "object") {
        if (data.IsDefaultAjaxObject) {
            if (data.JsonData != null && data.JsonData.length > 0) {
                //convert the jsondata into an object
                data.JsonData = eval("(" + data.JsonData + ")");
            }
            if (data.Success) {
                if (data.Message != "undefined" && data.Message != null && data.Message.length > 0) {
                    ShowResponseMessage(data);
                } else {
                    CheckResponseRedirect(data);
                }
                if (typeof success == "function") {
                    success(data);
                }
            } else {
                //this is a fail from the server
                if (typeof fail == "function") {
                    fail(data);
                }
                if (data.Message != "undefined" && data.Message != null) {
                    ShowResponseMessage(data);
                }
            }
        } else {
            if (typeof success == "function") {
                success(data);
            }
        }
    } else {
        //this is an html load call
        if (typeof success == "function") {
            success(data);
        }
    }
}

function ShowResponseMessage(data) {
    function callback() {
        CheckResponseRedirect(data);
    }

    var buttons = [createBootstrapButtonObject("Ok", null, callback)];

    showBootStrapMessage(
        (data.MessageTitle != null ? data.MessageTitle : (data.Success ? "Success" : "Error")),
        data.Message,
        buttons
    );
}

function CheckResponseRedirect(data) {
    if (data.Success && typeof data.RedirectUrl != "undefined" && data.RedirectUrl != null) {
        window.location = data.RedirectUrl;
    }
}

function failCallbackBase(jqXHR, textStatus, errorThrown, fail) {
    //this is a hard fail from ajax
    if (fail != null) {
        fail();
    } else {
        showBootStrapMessage("Error", "An error has occurred." + errorThrown);
    }
}

function submitCurrentForm(obj) {
    showSpinner(true);
    obj.form.submit();
}

function reloadPage() {
    location.reload();
}

function reloadToUrl(url) {
    location.href = url;
}

function currentPageUrl() {
    return location.protocol + "//" + location.host + location.pathname;
}

function PrintElem(elem) {

    try {
        
        var cloneddiv = $('#' + elem).clone();
        cloneddiv.find(".ceShadowBox").removeClass("ceShadowBox").addClass("print-ceShadowBox");
        cloneddiv.css("page-break-after: auto;");
        cloneddiv.css("overflow:visible");
        cloneddiv.css("-webkit-print-color-adjust: exact");

        cloneddiv.print();

    }
    catch (e) {
       
    }

    return true;
}