/*********************************
 * MergePatient.js
 * -------------------------------
 * Date:    12Sep2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Merge patient view
 * Directly controls the Views/Correction/ChangeQuestionnaireInfo.cshtml page
 ********************************/
var YPrime = YPrime || {};

YPrime.MergePatient = (function() {
    "use strict";
    var GetDuplicatePatientsViewUrl;
    var MergePatientId = null;
    var LoadInitialValues = false;
    var CorrectionId;
    var MergePatientDivId = "MergePatientDiv";
    var MergePatientPanelCss = "merge-patient";
    var PatientIdAttribute = "patientid";
    var PositionAttribute = "position";
    var MergeValueCss = "merge-value";
    var MergePatientVisitCss = "merge-patientvisit";
    var MergeDiaryCss = "merge-diary";
    var MergeStatusCss = "merge-status";
    var ApprovalCountId = "ApprovalCount";
    var ApprovalPositionAttribute = "approvalposition";
    var SelectedClass = "selected";
    var RemovedStatusTypeId = "RemovedStatusTypeId";
    var RemovedStatusTypeDisplay = "RemovedStatusTypeDisplay";
    var MergeLabel = "MergeLabel";

    return {
        InitMergePatient: function(getDuplicatePatientsViewUrl, patientId, correctionId, selectedPatientId) {
            function init() {
                GetDuplicatePatientsViewUrl = getDuplicatePatientsViewUrl;
                MergePatientId = patientId;
                CorrectionId = correctionId;
                YPrime.MergePatient.LoadMergedPatients(patientId, selectedPatientId);
                CheckValidCorrection();
            }

            ValidateFormType = YPrime.MergePatient.HasSelectedPatient;

            $(document).ready(init);
        },
        GetApprovalCount: function() {
            return $("#" + ApprovalCountId).val() * 1;
        },
        GetRemovedStatusTypeId: function() {
            return $("#" + RemovedStatusTypeId).val();
        },
        GetRemovedStatusTypeDisplay: function() {
            return $("#" + RemovedStatusTypeDisplay).val();
        },
        GetMergeLabel: function() {
            return $("#" + MergeLabel).val();
        },
        GetSelectedPatientId: function() {
            var result = null;
            var patientPanels = $("#" + MergePatientDivId).find("." + MergePatientPanelCss);
            var patientId = null;
            var allNewDataIsEmpty = true;
            for (var i = 0; i < patientPanels.length; i++) {
                //preload the values if moving backward
                var obj = patientPanels[i];
                var position = $(obj).attr(ApprovalPositionAttribute);
                var newValue = $("#" + GetDataRowId(position, "NewDataPoint")).val();
                if (newValue != null && newValue.length > 0) {
                    patientId = $(obj).attr("patientId");
                    allNewDataIsEmpty = false;
                }
            }

            result = !allNewDataIsEmpty && patientId != null ? patientId : null;

            return result;
        },
        SetSelectedPatientFields: function(newPatientId) {
            var patientPanels = $("#" + MergePatientDivId).find("." + MergePatientPanelCss);
            var statusHiddens = $("." + MergeStatusCss);

            for (var j = 0; j < statusHiddens.length; j++) {
                var obj = statusHiddens[j];
                var approvalPosition = $(obj).attr(ApprovalPositionAttribute);
                var patientId = $(obj).attr(PatientIdAttribute);

                //set the patientstatus stuff
                $("#" + GetDataRowId(approvalPosition, "NewDataPoint")).val(patientId == newPatientId
                    ? null
                    : YPrime.MergePatient.GetRemovedStatusTypeId());
                $("#" + GetDataRowId(approvalPosition, "NewDisplayValue")).val(patientId == newPatientId
                    ? null
                    : YPrime.MergePatient.GetRemovedStatusTypeDisplay());
            }

            //set all the hiddens to merge to the correct patientid
            //var mergeHiddens = $('.' + MergePatientVisitCss).toArray();
            //mergeHiddens = mergeHiddens.concat($('.' + MergeDiaryCss).toArray());
            //vso #28109 - only move the diary entries - jo 13Mar2018
            var mergeHiddens = $("." + MergeDiaryCss).toArray();

            for (var i = 0; i < mergeHiddens.length; i++) {
                var hiddenPosition = $(mergeHiddens[i]).attr(PositionAttribute);
                var oldDataPoint = $("#" + GetDataRowId(hiddenPosition, "OldDataPoint")).val();
                $("#" + GetDataRowId(hiddenPosition, "NewDataPoint"))
                    .val(oldDataPoint == newPatientId ? null : newPatientId);
                $("#" + GetDataRowId(hiddenPosition, "NewDisplayValue"))
                    .val(oldDataPoint == newPatientId ? null : YPrime.MergePatient.GetMergeLabel());
            }

            //load the values
            for (var k = 0; k < patientPanels.length; k++) {
                var obj = patientPanels[k];
                var approvalposition = $(obj).attr(ApprovalPositionAttribute);
                var patientId = $(obj).attr(PatientIdAttribute);

                if (patientId != newPatientId) {
                    $(obj).removeClass(SelectedClass);
                } else {
                    $(obj).addClass(SelectedClass);
                }
            }

            CheckValidCorrection();
        },
        LoadMergedPatients: function(patientId, selectedPatId) {
            var data = {
                PatientId: patientId,
                CorrectionId: CorrectionId
            };

            function success(data) {
                var patientPanels = $("#" + MergePatientDivId).find("." + MergePatientPanelCss);

                for (var i = 0; i < patientPanels.length; i++) {
                    //preload the values if moving backward
                    var obj = patientPanels[i];
                    $(obj).bind("click", YPrime.MergePatient.SelectMergePatientHandler);
                }

                var selectedPatientId =
                    selectedPatId == null ? YPrime.MergePatient.GetSelectedPatientId() : selectedPatId;

                if (selectedPatientId != null && selectedPatientId.length > 0) {
                    YPrime.MergePatient.SetSelectedPatientFields(selectedPatientId);
                }

                LoadDirectives();
            }

            function fail() {}

            //do this to insure the spinner shows
            setTimeout(function() {
                    ajaxLoad(GetDuplicatePatientsViewUrl, data, success, fail, MergePatientDivId);
                },
                1);
        },
        SelectMergePatientHandler: function() {
            var patientId = $(this).attr(PatientIdAttribute);
            YPrime.MergePatient.SetSelectedPatientFields(patientId);
            CheckValidCorrection();
        },
        //vso #28029 - allow validation of control - jo - 13Mar2018
        HasSelectedPatient: function() {
            return YPrime.MergePatient.GetSelectedPatientId() != null;
        },
        ValidateFormType: function() {
            var changedValues = $("div.merge-patient.selected").length > 0;

            return changedValues;
        }
    };
}());