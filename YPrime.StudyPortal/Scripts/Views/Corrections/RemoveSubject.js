/*********************************
 * ChangeQuestionnaireInfo.js
 * -------------------------------
 * Date:    23Aug2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Remove Subject Correction view
 * Directly controls the Views/Correction/RemoveSubject.cshtml page
 ********************************/
var YPrime = YPrime || {};

YPrime.RemoveSubject = (function() {
    "use strict";
    var GetPatientUrl;
    var LoadInitialValues = false;
    var PatientId;

    return {
        InitRemoveSubject: function(getPatientUrl, patientId) {
            function init() {
                PatientId = patientId;
                GetPatientUrl = getPatientUrl;
                YPrime.RemoveSubject.SetPatientDisplay();
            }

            $(document).ready(init);
        },

        SetPatientDisplay: function() {
            var data = { PatientId: PatientId };

            function success(data) {
                var patient = data.JsonData;
                var patientStatusTypeId = patient.PatientStatusTypeId;
                var patientStatusType = patient.PatientStatus;

                $("#" + GetDataRowId(0, "OldDataPoint")).val(patientStatusTypeId);
                $("#" + GetDataRowId(0, "OldDisplayValue")).val(patientStatusType);
            }

            ajaxPost(GetPatientUrl, data, success);
        },

        ValidateFormType: function() {
            return true;
        }
    };
}());