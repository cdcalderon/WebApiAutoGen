/*********************************
 * ExportCreate.js
 * -------------------------------
 * Date:    05Oct2016
 * Author:  BS
 * Desc:    Javascript to help functionality on the create export partial view
 * Directly controls the Views/Export/_Create.cshtml page
 ********************************/
var ExportGridRefreshInMilliseconds = 20000;

function initExportCreate(url) {
    $("#siteSelectList").change(function(e) {
        ajaxPost(url, { siteId: $(this).val() }, updatePatientSelectSuccess);
    });
    setInterval(function() {
            updateExportGrids();
        },
        ExportGridRefreshInMilliseconds);
}

function updatePatientSelectSuccess(data) {
    var patientSelectList = $("#patientSelectList");
    patientSelectList.find("option").remove();
    var patients = data.reduce(function(total, current) {
            total[current.Value] = current.Text;
            return total;
        },
        {});
    LoadSelectItems(patientSelectList, patients, true);
}

function createExportSuccess(data, status, xhr) {
    if ($(".validation-summary-errors ul", $(data)).length > 0) {
        reinitDataPickers();
        return;
    }
    $(".datepicker").val("");
    LoadDirectives();
    ShowResponseMessage({ MessageTitle: "Info", Message: "Export successfully created." });
    updateExportGrids();
    $("input[type=text]:enabled, textarea, .datepicker").val(null);
    $("select option:first-child").attr("selected", "selected");
    $("[data-gridname='CompletedExportsGrid'] tbody tr:first").addClass("grid-row-selected");
}

function createExportFailure(data) {
    updateExportGrids();
    ShowResponseMessage({ MessageTitle: "Error", Message: "Failed to create export." });
}

function runExportSuccess(data) {
    updateExportGrids();
    ShowResponseMessage({ MessageTitle: "Info", Message: "Successfully created export." });
}

function runExportFailure(data) {
    updateExportGrids();
    ShowResponseMessage({ MessageTitle: "Error", Message: "Failed to run export." });
}

function updateExportGrids() {
    $("#export").DataTable().ajax.reload();
}

function reinitDataPickers() {
    $('.datepicker').datepicker().attr('autocomplete', 'off');
}