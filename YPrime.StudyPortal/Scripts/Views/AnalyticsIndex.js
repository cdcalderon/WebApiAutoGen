/*********************************
 * AnalyticsIndex.js
 * -------------------------------
 * Date:    09Nov2016
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the analytics dashboard
 * Directly controls the Analytics/Index.cshtml page
 ********************************/

var mainPanelName = "AnalyticsDashboard";
var accordianName = "AnalyticsAccordion";
var menuColumnName = "MenuColumn";
var mainColumnName = "MainColumn";
var activeColumnClass = "activeSideBar";
var currentTypeControlName = "ReportType";
var skipToolbarHide = false;
var AuditReportList =
    "4338F297-E844-40F9-BCCE-450FBD11523A,7180E81A-49FA-472F-BDF8-E65CC3B494C4,EF01F9D9-7433-42A9-AE70-8F166F9DE8D3,";

$(document).ready(initializeAnalyticsDashboard);

function getSkipToolbarHide() {
    return skipToolbarHide;
}

function setSkipToolbarHide(value) {
    skipToolbarHide = value;
}

function initAccordian(level) {
    $("#" + level + "Title").click();
}

function initializeAnalyticsDashboard() {
    $("#" + accordianName).on("shown.bs.collapse", accordianCollapseHandler);
    setSkipToolbarHide(true);
}

function accordianCollapseHandler(e) {
    var obj = $("#" + accordianName).find(".in");
    var datatype = $(obj).first().attr("data-type");
    $("#" + currentTypeControlName).val(datatype);
    $("#" + mainPanelName).html("");
}

function loadMainWindow(id, url) {
    var data = { id: id };

    $("#AnalyticsDashboard").html("");
    $("#AuditFilters").hide();
    if (AuditReportList.indexOf(id.toUpperCase() + ",") >= 0) {
        sessionStorage.setItem("URL", url);
        GetFilterParams(id);
    } else {
        ajaxLoad(url, data, callback, null, mainPanelName, {});
    }

    function callback() {
        if (typeof LoadDirectives === "function") {
            LoadDirectives();
        }
    }

}

function GetFilterParams(id) {

    $("#AuditFilters").show();

    sessionStorage.setItem("ID", id);

    $("#SiteID").focus();
    $("#SubjectID").find("option:gt(0)").remove();
    document.getElementById("SiteID").selectedIndex = "0";
    document.getElementById("SubjectID").selectedIndex = "0";
    document.getElementById("FILTER_BTN").setAttribute("disabled", "disabled");
}

function ClickAuditReport() {
    var id = sessionStorage.getItem("ID");
    var url = sessionStorage.getItem("URL");

    var siteID = $("#SiteID").val() || "ALL";
    var subjNo = $("#SubjectID").val() || "ALL";

    var data = { id: id, sites: siteID, subjNo: subjNo };

    $("#AnalyticsDashboard").html("");
    $("#AuditFilters").hide();

    ajaxLoad(url, data, callbackAudit, null, mainPanelName, {});

    function callbackAudit() {
        var msg = "";
        var site = $("#SiteID option:selected").text();
        var subj = $("#SubjectID option:selected").text();
        if (subjNo !== "ALL") {
            msg = "(Subject number: " + subj + ")";
        } else {
            msg = "(Site: " + site + ")";
        }
        var ttl = $("#RptHeaderName").text();
        ttl = ttl + " " + msg;
        sessionStorage.removeItem("ID");
        sessionStorage.removeItem("URL");


        $("#RptHeaderName").text(ttl);

        if (typeof LoadDirectives === "function") {
            LoadDirectives();
        }
    }
}

function getReportType() {
    return $("#ReportType").val();
}

function showToolbar(sideBarName, show) {
    var menubar = $("#" + menuColumnName);
    var sidebar = $("#" + sideBarName);
    var mainbar = $("#" + mainColumnName);
    var animationSpeed = 500;


    //check if this is already set
    var showing = $(sidebar).hasClass("activeSideBar");

    $(sidebar).attr("class", !show ? "" : "activeSideBar col-sm-3 col-md-3");
    $(mainbar).attr("class", !show ? "col-sm-9 col-md-9" : "col-sm-7 col-md-7");
    if (show) {
        $(sidebar).show(animationSpeed);
        $(".open-toolbar").hide(animationSpeed);
    } else {
        $(sidebar).hide(animationSpeed);
        $(".open-toolbar").show(animationSpeed);
    }
}

function loadQuestionsFromQuestionnaire(questionnaireDdlName, questionDdlName, url) {
    var questionnaireDdl = $("#" + questionnaireDdlName);
    var questionDdl = $("#" + questionDdlName);
    var questionnaireId = $(questionnaireDdl).val();

    //get the questions
    var data = {
        QuestionnaireId: questionnaireId
    };

    function success(result) {
        $(questionDdl).find("option").remove();

        var list = result.map(function(obj) {
            return $("<option>").val(obj.DataPointId).text(CleanText(obj.QuestionDisplayText));
        });
        $(questionDdl).append(list);
    }

    ajaxPost(url, data, success);
}
