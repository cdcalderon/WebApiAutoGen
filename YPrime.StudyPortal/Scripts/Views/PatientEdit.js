/*********************************
 * PatientEdit.js
 * -------------------------------
 * Date:    30Mar2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Patient Edit Page
 * Directly controls the Views/Patient/edit.cshtml page
 ********************************/

var mainContentDiv = "main-content";
var mainPatientWindow = "main-patient-window";
var headerBar = "header-bar";
var footerBar = "footer-bar";
var _controllerName;
var _controllerRoot

function initEditPatient(controllerName, controllerRoot) {
    _controllerName = controllerName;
    _controllerRoot = controllerRoot;
    var gridNames = [];
    $("div.bhoechie-tab-menu>div.list-group>a").click(function(e) {
        e.preventDefault();
        $(this).siblings("a.active").removeClass("active");
        $(this).addClass("active");
        var index = $(this).index();
        $("div.bhoechie-tab>div.bhoechie-tab-content").removeClass("active");
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(index).addClass("active");
    });

    $(window).resize(
        function() {
            if (typeof adjustMainWindow == "function") {
                adjustMainWindow();
            }
        });

    setTimeout(adjustMainWindow, 1);
}

function adjustMainWindow() {
    var offset = 110;
    var fullWindowHeight = $(window).height(); //$('#' + mainContentDiv).height();
    var headerHeight = $("#" + headerBar).height();
    var footerHeight = $("#" + footerBar).height();
    var breadCrumbHeight = $("#" + mainContentDiv).find(".breadcrumb").height();
    var mainWindowHeight = fullWindowHeight - headerHeight - footerHeight;

    $(".bhoechie-tab").each(function() {
        $(this).height(mainWindowHeight - breadCrumbHeight - offset);
    });
}

function addBringYourOwnDeviceCode(url) {
    var data = {};
    var failCallback = null;
    var options = {};

    function successCallback(data) {
        var json = data.JsonData;
        var url = _controllerRoot + _controllerName; 
        window.location.href = url;
    }

    ajaxPost(url, data, successCallback, failCallback, options);
}

function showScoreCalculation(id) {
    var div = $("#" + id);
    if ($(div).hasClass("hidden")) {
        $(div).removeClass("hidden");
    } else {
        $(div).addClass("hidden");
    }
}