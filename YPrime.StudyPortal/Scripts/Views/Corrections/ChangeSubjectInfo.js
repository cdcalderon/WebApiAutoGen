/*********************************
 * ChangeSubjectInfo.js
 * -------------------------------
 * Date:    29Jun2017
 * Author:  C Patel
 * Desc:    Javascript to help functionality on the Change Subject Info Correction view
 * Directly controls the Views/Correction/ChangeSubjectInfo.cshtml page
 ********************************/

var YPrime = YPrime || {};

YPrime.ChangeSubjectInfo = (function() {
    "use strict";

    var Settings;
    var ChangeSubjectInfoPatientId;
    var ChangeSubjectCorrectionId;
    var ChangeCorrectionApprovalData;

    return {
        init: function (settings, patientId, correctionId, correctionApprovalData) {
            Settings = settings;
            ChangeSubjectInfoPatientId = patientId;
            ChangeSubjectCorrectionId = correctionId;
            ChangeCorrectionApprovalData = JSON.parse(correctionApprovalData);

            ValidateFormType = YPrime.ChangeSubjectInfo.patientAttributesHaveBeenChanged;
            this.loadPatientAttributes();
        },
        loadPatientAttributes: function() {
            var data = {
                PatientId: ChangeSubjectInfoPatientId,
                CorrectionId: ChangeSubjectCorrectionId,
                CorrectionApprovalData: ChangeCorrectionApprovalData
            };
            ajaxLoad(Settings.GetPatientAttributesUrl, data, this.success, null, Settings.FieldSetSelector);
        },
        patientAttributesHaveBeenChanged: function() {
            var newDataPointCSS = "new-data-point";
            var fields = $("." + newDataPointCSS);
            var result = false;
            for (var i = 0; i < fields.length; i++) {
                if ($(fields[i]).val() != null && $(fields[i]).val().length > 0) {
                    result = true;
                    break;
                }
            }
            return result;
        },
        success: function() {
            //rename the controls for the datepicker requests - jo 21Sep2017
            for (var i = 0; i < 100; i++) {
                var obj = $("#Question_Container_" + i + " .response");

                if (obj.length > 0) {
                    if (obj[0].tagName != "SELECT")
                    {
                        for (var j = 0; j < obj.length; j++) {
                            var id = $(obj).prop("id");
                            var name = $(obj).prop("name");
                            $(obj).prop("id", id + "_" + i);
                            $(obj).prop("name", name + "_" + i);
                        }
                    }
                    BindDataChangeControl($("#Question_Container_" + i + " .response"), i);
                }
            }
        },
        ValidateFormType: function() {
            var changedValues = false;

            $("#subjectInfoFields .response").each(function() {

                switch ($(this).attr("type")) {
                case "radio":
                    changedValues = changedValues || $(this).is(":checked");
                    break;
                default:
                    changedValues = changedValues || $(this).val() !== "";
                    break;
                }
            });

            return changedValues;
        }
    };
}());