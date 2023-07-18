/*********************************
 * ChangePatientVisit.js
 * -------------------------------
 * Date:    23Aug2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Remove Subject Correction view
 * Directly controls the Views/Correction/RemoveSubject.cshtml page
 ********************************/
var YPrime = YPrime || {};
const disabledOpacity = '0.3';
const autoRemovedAttr = "auto-removed";
YPrime.ChangePatientVisit = (function () {
    "use strict";
    var GetPatientVisitsUrl;
    var GetPatientVisitUrl;
    var PatientId = null;
    var PatientVisitIdInput = "PatientVisitId";
    var PatientVisitSelectId = "RowId";
    var PatientVisitDateLabel = "PatientVisitDateLabel";
    var PatientVisitActivationDateLabel = "PatientVisitActivationDateLabel";
    var PatientVisitStatusLabel = "PatientVisitStatusLabel";
    var VisitStatusSelectCss = "VisitStatusList";
    var PatientVisitEntryField = "PatientVisitDateDCF";
    var PatientVisitActivationEntryField = "PatientActivationVisitDateDCF";
    var PatientVisitFieldSetId = "ChangePatientVisitFieldset";
    var VisitStatusUpdateNotStartedLabelId = "VisitStatusUpdateNotStartedLabel";
    var VisitStatusUpdateCompleteLabelId = "VisitStatusUpdateCompleteLabel";
    var VisitStatusUpdateInProgressLabelId = "VisitStatusUpdateInProgressLabel";
    var VisitStatusUpdateMissedLabelId = "VisitStatusUpdateMissedLabel";
    var VisitDateRemoveValueToggleName = "toggle-remove-1";
    var VisitActivationDateRemoveValueToggleName = "toggle-remove-2"
    var LoadInitialValues = false;
    var visitStatusTransitions = 
    {   //new status-current statuses
        "Complete-Missed,Not Started" : { clearAndDisableDates: false, disableCorrectionRemove: false, messageToDisplay: VisitStatusUpdateCompleteLabelId },
        "Complete-Partial,In Progress": { clearAndDisableDates: false, disableCorrectionRemove: true, messageToDisplay: VisitStatusUpdateCompleteLabelId },
        "Partial-Complete": { clearAndDisableDates: false, disableCorrectionRemove: true, messageToDisplay: null },
        "Missed-Complete": { clearAndDisableDates: true, disableCorrectionRemove: true, messageToDisplay: null },
        "Missed-Not Started,Partial,In Progress": { clearAndDisableDates: true, disableCorrectionRemove: true, messageToDisplay: VisitStatusUpdateMissedLabelId },
        "Not Started-Partial,In Progress,Complete,Missed": { clearAndDisableDates: true, disableCorrectionRemove: true, messageToDisplay: VisitStatusUpdateNotStartedLabelId },
        "In Progress-Not Started,Missed": { clearAndDisableDates: false, disableCorrectionRemove: false, messageToDisplay: VisitStatusUpdateInProgressLabelId },
        "In Progress-Partial,Complete": { clearAndDisableDates: false, disableCorrectionRemove: true, messageToDisplay: VisitStatusUpdateInProgressLabelId },
    };

    var requiredVisitDateTransitions =
    [   //current status-new statuses
        "Missed-Complete,In Progress,Partial",
        "Not Started-Complete,In Progress,Partial",
        "In Progress-Complete,Partial"
    ];

    return {
        InitChangePatientVisit: function (getPatientVisitsUrl, getPatientVisitUrl, patientId, patientVisitId) {
            $('#' + VisitStatusUpdateNotStartedLabelId).hide();
            $('#' + VisitStatusUpdateCompleteLabelId).hide();
            $('#' + VisitStatusUpdateInProgressLabelId).hide();
            $('#' + VisitStatusUpdateMissedLabelId).hide();

            function init() {
                GetPatientVisitsUrl = getPatientVisitsUrl;
                GetPatientVisitUrl = getPatientVisitUrl;
                PatientId = patientId;

                YPrime.ChangePatientVisit.CheckPatientVisitFieldsetDisplay();

                $(".correction-remove").each(
                    function () {
                        $(this).on("change", YPrime.ChangePatientVisit.RemoveItemHandler);
                    }
                );

                BindDataChangeControl(PatientVisitEntryField, 1, "VisitDate");
                BindDataChangeControl(PatientVisitActivationEntryField, 2, "ActivationDate");
                BindDataChangeControl($("." + VisitStatusSelectCss), 0, "VisitStatusTypeId");

                LoadInitialValues = typeof patientVisitId != "undefined" && patientVisitId != null;
                YPrime.ChangePatientVisit.LoadPatientVisits(patientVisitId);
            }

            $(document).ready(init);
        },

        LoadPatientVisits: function (patientVisitId) {
            var data = { PatientId: PatientId };

            function success(data) {
                var patientVisits = data.JsonData.PatientVisits;
                var visitStatusTypes = data.JsonData.VisitStatusTypes;
                var items = patientVisits.reduce(function (total, current) {
                    total[current.Id] =
                        current.VisitName + " " + (current.VisitDateDisplay ? current.VisitDateDisplay : "") + " " + current.PatientVisitStatus;
                    return total;
                },
                    {});
                LoadSelectItems(PatientVisitSelectId, items);

                var visitItems = visitStatusTypes.reduce(function (total, current) {
                    total[current.Id] = current.Name;
                    return total;
                },
                    {});
                LoadSelectItems($("." + VisitStatusSelectCss), visitItems, false, placeholderText);
                LoadDirectives();

                YPrime.ChangePatientVisit.SetCurrentPatientVisitId(patientVisitId);
                $("#" + PatientVisitSelectId).val(patientVisitId);
                RefreshSelect($("#" + PatientVisitSelectId));

                YPrime.ChangePatientVisit.LoadPatientVisit();
            }

            ajaxPost(GetPatientVisitsUrl, data, success);
        },

        CurrentPatientVisitId: function () {
            return $("#" + PatientVisitSelectId).val();
        },

        SetCurrentPatientVisitId: function (id) {
            $("#" + PatientVisitSelectId).val(id);
        },

        LoadPatientVisit: function () {
            var patientVisitId = YPrime.ChangePatientVisit.CurrentPatientVisitId();
            //retain the diary entry id to match up later for display - jo 28Aug2017
            $("#" + PatientVisitIdInput).val(patientVisitId);
            if (patientVisitId != null && patientVisitId != "") {
                var data = { PatientVisitId: patientVisitId };

                function success(data) {
                    var patientVisit = data.JsonData;

                    if (!LoadInitialValues) {
                        YPrime.ChangePatientVisit.SetChangePatientVisitDisplay();
                    } else {
                        YPrime.ChangePatientVisit.LoadChangeValues();
                    }

                    $("#" + PatientVisitDateLabel).html(patientVisit.VisitDateDisplay);
                    $("#" + PatientVisitActivationDateLabel).html(patientVisit.VisitActivationDateDisplay);
                    $("#" + PatientVisitStatusLabel).html(patientVisit.PatientVisitStatus);

                    $("#" + GetDataRowId(1, "OldDataPoint")).val(patientVisit.VisitDate);
                    $("#" + GetDataRowId(2, "OldDataPoint")).val(patientVisit.ActivationDate);
                    $("#" + GetDataRowId(0, "OldDataPoint")).val(patientVisit.PatientVisitStatusTypeId);
                    $("#" + GetDataRowId(1, "OldDisplayValue")).val(patientVisit.VisitDateDisplay);
                    $("#" + GetDataRowId(2, "OldDisplayValue")).val(patientVisit.VisitActivationDateDisplay);
                    $("#" + GetDataRowId(0, "OldDisplayValue")).val(patientVisit.PatientVisitStatus);
                    $("#" + GetDataRowId(0, "RowId")).val(patientVisit.Id);
                    $("#" + GetDataRowId(1, "RowId")).val(patientVisit.Id);
                    $("#" + GetDataRowId(2, "RowId")).val(patientVisit.Id);

                    CheckValidCorrection();

                    $('#PatientVisitDateDCF').data("DateTimePicker").date($("#VisitValkey").text());
                    $('#PatientActivationVisitDateDCF').data("DateTimePicker").date($("#ActiveVisitValkey").text());

                    SetValidationControls();
                }

                ajaxPost(GetPatientVisitUrl, data, success);
            }
        },

        SetChangePatientVisitDisplay: function () {
            YPrime.ChangePatientVisit.CheckPatientVisitFieldsetDisplay();

            $("#" + PatientVisitEntryField).val("");
            $("#" + PatientVisitActivationEntryField).val("");

            var statusSelect = $("." + VisitStatusSelectCss)
            statusSelect.val("");
            RefreshSelect(statusSelect);


            $("." + GetDataRowId(0, "NewDataPoint")).val("");
            $("." + GetDataRowId(1, "NewDataPoint")).val("");
            $("." + GetDataRowId(2, "NewDataPoint")).val("");
        },

        LoadChangeValues: function () {
            showSpinner(true);
            YPrime.ChangePatientVisit.CheckPatientVisitFieldsetDisplay();

            //load up values from back button
            if ($("#" + GetDataRowId(1, "RemoveItem")).val() == "true") {
                $("#VisitValkey").text($("#" + GetDataRowId(1, "OldDataPoint")).val());
                $("#PatientVisitDateLabel").addClass("strike-through");
                $("#PatientVisitDateDCF").addClass("disabled-panel");
                SetCorrectionApprovalData($("#EmptyValkey"), 1);
                $("#CorrectionApprovalDatas_1__RemoveItem").val("true");
                $("#toggle-remove-1").prop('checked', true);
            }
             
            if ($("#" + GetDataRowId(2, "RemoveItem")).val() == "true") {
                $("#ActiveVisitValkey").text($("#" + GetDataRowId(2, "OldDataPoint")).val());
                $("#PatientActivationVisitDateDCF").datepicker("setDate", new Date());
                $("#PatientVisitActivationDateLabel").addClass("strike-through");
                $("#PatientActivationVisitDateDCF").addClass("disabled-panel");
                SetCorrectionApprovalData($("#EmptyValkey"), 2);
                $("#CorrectionApprovalDatas_2__RemoveItem").val("true");
                $("#toggle-remove-2").prop('checked', true);
            }

            var visitActivationDate = $("#" + GetDataRowId(2, "NewDataPoint")).val();
            var visitStatusTypeId = $("#" + GetDataRowId(0, "NewDataPoint")).val();
            var visitDate = $("#" + GetDataRowId(1, "NewDataPoint")).val();

            $("#" + PatientVisitEntryField).val(visitDate);
            $("#" + PatientVisitActivationEntryField).val(visitActivationDate);
            $("." + VisitStatusSelectCss).val(visitStatusTypeId);
            RefreshSelect($("." + VisitStatusSelectCss));

            showSpinner(false);
            LoadInitialValues = false;
        },

        CheckPatientVisitFieldsetDisplay: function () {
            var hasPatientVisit = YPrime.ChangePatientVisit.CurrentPatientVisitId() != null;
            var fieldset = $("#" + PatientVisitFieldSetId);

            if (hasPatientVisit) {
                $(fieldset).show();
            } else {
                $(fieldset).hide();
            }
        },
        ValidateFormType: function () {
            var changedValues = false;
            ValidateActivationDate();

            var visitId = YPrime.ChangePatientVisit.CurrentPatientVisitId();

            if (visitId !== null && visitId !== "" && visitId !== undefined) {
                $("#ChangePatientVisitFieldset input:not([type=hidden]), #ChangePatientVisitFieldset select").each(
                    function () {
                        switch ($(this).attr("type")) {
                            case "radio":
                            case "checkbox": //remove toggle
                                changedValues = changedValues || $(this).is(":checked");
                                break;
                            case "select":
                                break;
                            default:
                                changedValues = changedValues || $(this).val() !== "";
                                break;
                        }
                    });
            }

            return changedValues;
        },
        VisitStatusUpdate: function () {
            SetValidationControls();
        },
        RemoveItemHandler: function () {
            var position = $(this).attr("position");
            if (this.checked) {
                $("label[name='PatientDateLabel_" + position + "']").addClass("strike-through");
                $("input[name='PatientDate_" + position + "']").addClass("disabled-panel");
                SetCorrectionApprovalData($("#EmptyValkey"), position)
                if ($("input[name='PatientDate_" + position + "']").val() != "") {
                    $("#CorrectionApprovalDatas_" + position + "__OldDataPoint").val($("input[name='PatientDate_" + position + "']").val());
                }
                $("#CorrectionApprovalDatas_" + position + "__RemoveItem").val("true");
            }
            else {
                $("label[name='PatientDateLabel_" + position + "']").removeClass("strike-through");
                $("input[name='PatientDate_" + position + "']").removeClass("disabled-panel");
                SetCorrectionApprovalData($("input[name='PatientDate_" + position + "']"), position)
                $("#CorrectionApprovalDatas_" + position + "__RemoveItem").val("false");
            }

            CheckValidCorrection();
        }
    };

    function DisableCorrectionRemove() {
        $(".correction-remove").each(function () {
            UpdateRemoveToggle(this, false);
            $(this).attr("disabled", true);
        });
        $(".control-label").css('opacity', disabledOpacity);
    }

    function ClearAndDisableDates() {

        // clear new dates
        $("input[name='PatientDate_1']").val('');
        $("input[name='PatientDate_2']").val('');
        $("input[name='PatientDate_1']").addClass("disabled-panel");
        $("input[name='PatientDate_2']").addClass("disabled-panel");
        SetCorrectionApprovalData($("#EmptyValkey"), 1);
        SetCorrectionApprovalData($("#EmptyValkey"), 2);

        //remove current dates
        $(".correction-remove").each(function () {
            UpdateRemoveToggle(this, true);
        });
    }

    function UpdateRemoveToggle(control, checked){
        var obj = $(control);
        if (obj.prop("checked") != checked) {
            obj.attr(autoRemovedAttr, checked);
            obj.prop("checked", checked)
            obj.trigger("change");
        }
    }

    function SetValidationControls() {
        var currentStatus = $("#" + PatientVisitStatusLabel).text();
        var newStatus = $("." + VisitStatusSelectCss + " option:selected").text();

        $('#' + VisitStatusUpdateNotStartedLabelId).hide();
        $('#' + VisitStatusUpdateCompleteLabelId).hide();
        $('#' + VisitStatusUpdateInProgressLabelId).hide();
        $('#' + VisitStatusUpdateMissedLabelId).hide();

        $("[" + autoRemovedAttr + "=true]").each(function () {
            UpdateRemoveToggle(this, false);
            $(this).attr(autoRemovedAttr, false);
        });

        [1, 2].forEach(function (position) {
            if ($("#" + GetDataRowId(position, "RemoveItem")).val() == "false") {
                $("input[name='PatientDate_" + position + "']").removeClass("disabled-panel");
                SetCorrectionApprovalData($("input[name='PatientDate_" + position + "']"), position);
            }
        });
        $(".correction-remove").removeAttr("disabled");
        $(".control-label").css('opacity', '');

        if (currentStatus != newStatus && newStatus != placeholderText)
        {
            if (requiredVisitDateTransitions.some((t => t.startsWith(currentStatus) && t.includes(newStatus))))
            {
                DisableSingleCorrectionRemove(VisitDateRemoveValueToggleName, true);
            }

            for (var transitionkey in visitStatusTransitions) {
                if (transitionkey.startsWith(newStatus) &&
                    transitionkey.includes(currentStatus)) {
                    var transitionActions = visitStatusTransitions[transitionkey];

                    if (transitionActions.messageToDisplay) {
                        $('#' + transitionActions.messageToDisplay).show();
                    }

                    if (transitionActions.disableCorrectionRemove) {
                        DisableCorrectionRemove();
                    }

                    if (transitionActions.clearAndDisableDates) {
                        ClearAndDisableDates();
                    }
                }
            }
        }

        // disable remove value toggles if there is no data to remove
        // visit date:
        if (!$("#" + GetDataRowId(1, "OldDataPoint")).val()) {
            DisableSingleCorrectionRemove(VisitDateRemoveValueToggleName);
        }

        // activation date:
        if (!$("#" + GetDataRowId(2, "OldDataPoint")).val()) {
            DisableSingleCorrectionRemove(VisitActivationDateRemoveValueToggleName);
        }
    }

    function DisableSingleCorrectionRemove(toggleName, unToggle = false) {
        $("#" + toggleName).each(function () {
            if (unToggle)
            {
                UpdateRemoveToggle(this, false);
            }
            
            $(this).attr("disabled", true);
        });
        $("[for='" + toggleName + "']").css('opacity', disabledOpacity);
    }

    function ValidateActivationDate() {

        const errorClassName = "has-error";

        var visitDatePicker = $("#" + PatientVisitEntryField);
        var activationDatePicker = $("#" + PatientVisitActivationEntryField);

        var currentVisitDate = $("#" + PatientVisitDateLabel).html();
        var currentActivationDate = $("#" + PatientVisitDateLabel).html();
        var newVisitDate = visitDatePicker.val();
        var newActivationDate = activationDatePicker.val();

        var visitDate = Date.parse(newVisitDate ? newVisitDate : currentVisitDate)
        var activationDate = Date.parse(newActivationDate ? newActivationDate : currentActivationDate)

        if (!isNaN(activationDate) &&
            !isNaN(visitDate) &&
            activationDate > visitDate) {
            visitDatePicker.parent().addClass(errorClassName);
            activationDatePicker.parent().addClass(errorClassName);
        }
        else {
            visitDatePicker.parent().removeClass(errorClassName);
            activationDatePicker.parent().removeClass(errorClassName);
        }
    }
}());