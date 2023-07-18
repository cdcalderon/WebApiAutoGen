/*********************************
 * CorrectionCreate.js
 * -------------------------------
 * Date:    26Jun2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Create Correction
 * Directly controls the Views/Correction/Create.cshtml page
 ********************************/
var emptyGuid = "00000000-0000-0000-0000-000000000000"
var siteSelectId = "SiteId";
var patientSelectId = "PatientId";
var correctionFormId = "CorrectionForm";
var correctionTypeDivId = "CorrectionDiv";
var correctionTypeSelectId = "CorrectionTypeId";
var reasonForCorrectionId = "ReasonForCorrection";
var submitCorrectionButtonId = "SubmitCorrection";
var patientPreLoadedHiddenId = "PatientPreLoaded";
var correctionHiddenValuesCss = "correction-hidden-values";
var correctionLoadUrl;

/*********************
 * METHODS
 ********************/
function InitCorrectionCreate(correctionLoadUrlInit) {
    correctionLoadUrl = correctionLoadUrlInit;
    SetCorrectionDisplay();
    if (!GetPatientPreLoaded()) {
        RefreshSelect($("#" + patientSelectId));
    }

    //bind up validations
    $("#" + siteSelectId).bind("change", CheckValidCorrection);
    $("#" + patientSelectId).bind("change", CheckValidCorrection);
    $("#" + correctionTypeSelectId).bind("change", CheckValidCorrection);
    $("#" + reasonForCorrectionId).bind("keyup", CheckValidCorrection);

    CheckValidCorrection();
}

function GetSiteId() {
    var select = $("#" + siteSelectId);
    var result = $(select)[0].tagName == "SELECT" ? $(select).val() : $(select).html();
    return result;
}

function GetValidationSiteId() {
    var select = $("#" + siteSelectId);
    var tagName = $(select)[0].tagName;
    var result = (tagName == "SELECT" || tagName == "INPUT") ? $(select).val() : $(select).html();
    return result;
}

function GetPatientId() {
    var result = '';

    // get accurate patient id for instance where patient is pre-selected
    if ($('input#PatientId').length > 0) {
        result = $($('input#PatientId')[0]).val();
    } else {
        var select = $("#" + patientSelectId);
        result = $(select)[0].tagName == "SELECT" ? $(select).val() : $(select).html();
    }

    return result;
}

function GetPatientPreLoaded() {
    var val = $("#" + patientPreLoadedHiddenId).val().toLowerCase();
    return val == "true";
}

function GetCorrectionTypeId() {
    return $("#" + correctionTypeSelectId).val();
}

function GetReasonForCorrection() {
    return $("#" + reasonForCorrectionId).length > 0 ? $("#" + reasonForCorrectionId).val() : null;
}

function CheckValidCorrection() {
    var submitButton = $("#" + submitCorrectionButtonId);
    enableControls(submitButton, false);

    //check for valid
    if (hasValue(GetValidationSiteId()) &&
        hasValue(GetPatientId()) &&
        hasValue(GetCorrectionTypeId()) &&
        ValidateFormType() &&
        !isNullOrWhitespace(GetReasonForCorrection()))
    {
        enableControls(submitButton, true);
    }
}

//vso #28029 - allow validation of control - jo - 13Mar2018
/**************************************
NOTE: overload this method to allow you to validate the next button from the data correction type
 *****************************************/
function ValidateFormType() {
    //this is an override
    return true;
}

function ClearValidations() {
    $('.dcf-validation-message').remove();
}

function LoadPatientsBySite(obj, url) {
    var siteId = $(obj).val();
    var siteName = $(obj).find("option:selected").text();
    $("#SiteName").val(siteName);
    if (siteId != null && siteId.length > 0) {
        var data = { SiteId: siteId };

        function success(data) {
            var patients = data.JsonData;
            var items = patients.reduce(function(total, current) {
                    total[current.Id] = current.PatientNumber;
                    return total;
                },
                {});
            LoadSelectItems(patientSelectId, items);
            $("#" + correctionTypeSelectId).val("");

            SetCorrectionDisplay();
            UpdateCorrectionDataAfterSiteSelection(siteId, url);
        }

        ajaxPost(url, data, success);
    } else {
        LoadSelectItems(patientSelectId, {});
    }
}

function UpdateCorrectionDataAfterSiteSelection(siteID, url) {
    if (siteID != null && siteID.length > 0 && siteID != emptyGuid) {
        var data = { SiteId: siteID };

        function success(data) {
            var sites = data.JsonData;                  
            $("#UseMetricMeasurements")[0].value = sites.UseMetric;
        }
        var regex = new RegExp('([^/]*)$');
        var replacementAction = regex.exec(url)[0];
        ajaxPost(url.replace(replacementAction, 'GetCountryDetailsFromSiteId'), data, success);
    } 
}

function LoadCorrectionType(clearHidden) {
    var obj = $("#" + correctionTypeSelectId);
    var correctionTypeId = $(obj).val();
    var formObject = $("#" + correctionFormId);
    var options = { dataType: "html" };

    $("#" + correctionTypeDivId).html("");

    if (clearHidden) {
        ClearValidations();

        //clear out the correction data
        $("." + correctionHiddenValuesCss).find("input[type=hidden]").each(
            function() {
                $(this).remove();
            });
    }
    if (correctionTypeId != null && correctionTypeId.length > 0) {
        function success(data) {
            $("#" + correctionTypeDivId).html(data).ready(function() {
                CheckValidCorrection();
            });
        }

        ajaxFormPost(correctionLoadUrl, formObject, success, null, options);
    }
}

function EditCorrection(obj, url) {
    $(obj).closest("form").attr("action", url);
    submitCurrentForm(obj);
}

function SetPatient(obj) {
    var patientNumber = $(obj).find("option:selected").text();
    $("#PatientNumber").val(patientNumber);
    SetCorrectionDisplay();
}


function CleanCorrection() {
    //clear out the correction data
    ClearValidations();
    $("." + correctionHiddenValuesCss).find("input[type=hidden]").each(
        function () {
            $(this).remove();
        });
}

function SetCorrectionDisplay() {
    //check the site
    var currentSiteId = GetSiteId();
    var patientSelect = $("#" + patientSelectId);
    var patientId = GetPatientId();
    var correctionTypeSelect = $("#" + correctionTypeSelectId);
    var correctionTypeId = GetCorrectionTypeId();

    enableControls(patientSelect, false);
    enableControls(correctionTypeSelect, false);


    if (currentSiteId != null && currentSiteId.length > 0) {
        enableControls(patientSelect, true);
    }

    if (patientId != null && patientId.length > 0) {
        enableControls(correctionTypeSelect, true);
    } else {
        $(correctionTypeSelect).val("");
        RefreshSelect(correctionTypeSelect);
        correctionTypeId = "";
    }

    LoadCorrectionType(false);
}