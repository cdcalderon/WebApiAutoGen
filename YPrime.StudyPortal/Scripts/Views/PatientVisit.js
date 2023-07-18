/*********************************
 * PatientVisit.js
 * -------------------------------
 * Date:    21Apr2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the questionnaire page
 * Directly controls the Views/Question/PatientVisit.cshtml page
 */
var questionAttribute = "question-id";
var questionCheckboxControlCss = "btn-checkbox";
var visibilityRuleAttribute = "visibility-rule-id";
var enabledRuleAttribute = "enabled-rule-id";
var checkingPage = false;

function InitQuestionActions(checkPageUrl) {
    $('input[type="radio"]').each(
        function() {
            BindRadio(checkPageUrl);
        });
    $("." + questionCheckboxControlCss).each(
        function() {
            BindCheckbox(checkPageUrl);
        });

    //$(".datepicker").each(function () {

    //    var min = $(this).attr("min");
    //    var max = $(this).attr("max");

    //    $(this).datepicker({
    //        minDate: min != "" ? new Date(min) : null,
    //        maxDate: max != "" ? new Date(max) : null
    //    });
    //});

    //this is moved to uiHelper.js directives
    //$(".dobdatepicker").each(function () {
    //    var min = new Date($(this).attr("min"));
    //    var max = new Date($(this).attr("max"));

    //    $(this).datepicker({
    //        changeMonth: true,
    //        changeYear: true,
    //        minDate: min,
    //        maxDate: max,
    //        yearRange: min.getFullYear() + ":" + max.getFullYear()
    //    });
    //});

    $("#AgeYearSelect").change(function() {
        UpdateAgeSelector();
    });

    $(window).resize(function() {
        SetRadioButtons();
    });

    UpdateAgeSelector();

    CheckPageRules(checkPageUrl);
}

function CheckPageRules(url) {
    if (!checkingPage) {
        checkingPage = true;
        var formObject = $("form");
        ajaxFormPost(url, formObject, ProcessControls, null, { NoSpinner: true });
    }
}

function ProcessControls(data) {
    for (var prop in data.JsonData) {
        SetQuestionDisplay(prop, data.JsonData[prop].Enabled, data.JsonData[prop].Visible);
    }
    checkingPage = false;
}

function SetQuestionDisplay(questionId, enabled, visible) {
    //get the div
    var div = $("div[" + questionAttribute + '="' + questionId + '"]');
    if (visible) {
        $(div).show();
    } else {
        $(div).hide();
    }
    //remove Required hidden field if it is no longer visible
    var required = $("input[" + questionAttribute + '="' + questionId + '"][required="True"]');
    if (required.length > 0) {
        required.val(visible);
    }

    enableControls(div, enabled);
}

/**********Binding Methods*********/
function BindRadio(url) {
    $(this).bind("change", function() { CheckPageRules(url); });
}

function BindCheckbox(url) {
    $(this).bind("click", function() { CheckPageRules(url); });
}


function UpdateAgeSelector() {
    var year = $("#AgeYearSelect").find(":selected").val();

    var yearNum = parseInt(year);

    var diff = new Date().getFullYear() - yearNum;

    $("#UpperAgeRange").html(diff);
    $("#UpperAgeRangeRadio").val(diff);

    var diffLower = diff - 1;

    $("#LowerAgeRange").html(diffLower);
    $("#LowerAgeRangeRadio").val(diffLower);
}

/**********Radio/Checkbox Methods*********/
function SetRadioButtons() {
    $(".radio-group").each(function() {
        //Get the buttons widths
        var $buttons = $(this).children(".btn.btn-radio").not(".drugkitreturnall");
        var buttonWidths = $buttons.map(function() {
            var minWidth = $buttons.css("min-width");
            if (minWidth === "0px") { //use minWidth if we have it
                return $(this).outerWidth();
            } else {
                return parseInt(minWidth);
            }
        }).get();

        //Use parent width to determine vertical / horizontal
        var buttonPadding = $buttons.outerWidth() - $buttons.innerWidth();
        var adjustedParentWidth = $(this).parent().width() - (buttonPadding * $buttons.length);
        var adjustedButtonWidth = Math.max.apply(Math, buttonWidths);
        var adjustedButtonsWidth = adjustedButtonWidth * $buttons.length;

        //Set min-width on intial render
        if ($buttons.css("min-width") === "0px") {
            $buttons.css({ "min-width": adjustedButtonWidth + "px" });
        }

        //Horizontal / Vertical
        if (adjustedButtonsWidth < adjustedParentWidth) {
            $(this).addClass("horizontal");
            var width = adjustedParentWidth / $buttons.length;
            $buttons.outerWidth(width);
        } else {
            $(this).addClass("vertical");
            $buttons.width(adjustedParentWidth);
        }
    });
}