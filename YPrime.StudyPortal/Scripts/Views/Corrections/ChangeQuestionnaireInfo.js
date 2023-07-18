/*********************************
 * ChangeQuestionnaireInfo.js
 * -------------------------------
 * Date:    28Jun2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Change Questionnaire Correction view
 * Directly controls the Views/Correction/ChangeQuestionnaireInfo.cshtml page
 ********************************/
var YPrime = YPrime || {};

YPrime.ChangeQuestionnaireInfo = (function() {
    "use strict";
    var GetDiaryEntriesUrl;
    var GetDiaryEntryUrl;
    var ChangeQuestionnairePatientId = null;
    var DiaryEntryIdInput = "DiaryEntryId";
    var DiarySelectId = "RowId";
    var DiaryDateLabel = "DiaryDateLabel";
    var VisitNameLabel = "VisitNameLabel";
    var VisitSelectCss = "VisitList";
    var DiaryDateEntryField = "DiaryDateDCF";
    var QuestionnaireFieldSetId = "QuestionnaireInformationFieldset";
    var QuestionnaireDate = "DiaryDateDCF";
    var VisitName = "NewDataPoint";
    var LoadInitialValues = false;

    return {
        InitChangeQuestionnaireInfo: function(getDiaryEntriesUrl, getDiaryEntryUrl, patientId, diaryEntryId) {
            function init() {
                GetDiaryEntriesUrl = getDiaryEntriesUrl;
                GetDiaryEntryUrl = getDiaryEntryUrl;
                ChangeQuestionnairePatientId = patientId;

                YPrime.ChangeQuestionnaireInfo.SetChangeQuestionnairePageDisplay();

                BindDataChangeControl(DiaryDateEntryField, 0, "DiaryDate");
                BindDataChangeControl($("." + VisitSelectCss), 1, "VisitId");

                LoadInitialValues = typeof diaryEntryId !== "undefined" && diaryEntryId !== null;

                YPrime.ChangeQuestionnaireInfo.LoadDiaryEntries(diaryEntryId);
            }

            $(document).ready(init);
        },

        ValidateFormType: function() {
            var rowId = $("#RowId").val();
            var dateVal = $("#" + QuestionnaireDate).val();
            var visitVal = $("#" + VisitName).val();

            var hasQuestionnaireInfoSelection = rowId !== undefined && rowId !== null && rowId !== "";
            var hasDateChange = dateVal !== undefined && dateVal !== null && dateVal !== "";
            var hasVisitChange = visitVal !== undefined && visitVal !== null && visitVal != "";

            var result = hasQuestionnaireInfoSelection && (hasDateChange || hasVisitChange);
            return result;
        },

        LoadDiaryEntries: function(diaryEntryId) {
            var data = { PatientId: ChangeQuestionnairePatientId };

            function success(data) {
                var diaries = data.JsonData.DiaryEntries;
                var visits = data.JsonData.Visits;
                var items = diaries.reduce(function(total, current) {
                        total[current.Id] = current.QuestionnaireDisplayName +
                            " " +
                            current.DiaryEntryDateDisplay +
                            " " +
                            (current.VisitName || "");
                        return total;
                    },
                    {});
                LoadSelectItems(DiarySelectId, items);

                var visitItems = visits.reduce(function(total, current) {
                        total[current.Id] = current.Name;
                        return total;
                    },
                    {});
                LoadSelectItems($("." + VisitSelectCss), visitItems);
                LoadDirectives();

                YPrime.ChangeQuestionnaireInfo.SetCurrentDiaryEntryId(diaryEntryId);
                $("#" + DiarySelectId).val(diaryEntryId);
                RefreshSelect($("#" + DiarySelectId));
                
                YPrime.ChangeQuestionnaireInfo.LoadDiaryEntry();
            }

            ajaxPost(GetDiaryEntriesUrl, data, success);
        },

        CurrentDiaryEntryId: function() {
            return $("#" + DiarySelectId).val();
        },

        SetCurrentDiaryEntryId: function(id) {
            $("#" + DiarySelectId).val(id);
        },

        LoadDiaryEntry: function() {
            var diaryEntryId = YPrime.ChangeQuestionnaireInfo.CurrentDiaryEntryId(); //$('#' + DiarySelectId).val();
            //retain the diary entry id to match up later for display - jo 28Aug2017
            $("#" + DiaryEntryIdInput).val(diaryEntryId);
            if (diaryEntryId !== null && diaryEntryId !== "") {
                var data = { DiaryEntryId: diaryEntryId };

                function success(data) {
                    var diaryEntry = data.JsonData;

                    if (!LoadInitialValues) {
                        YPrime.ChangeQuestionnaireInfo.SetChangeQuestionnairePageDisplay();
                    } else {
                        YPrime.ChangeQuestionnaireInfo.LoadChangeValues();
                    }

                    $("#" + DiaryDateLabel).html(diaryEntry.DiaryEntryDateDisplay);
                    $("#" + VisitNameLabel).html(diaryEntry.VisitName);

                    $("#" + GetDataRowId(0, "OldDataPoint")).val(diaryEntry.DiaryEntryDateDisplay);
                    $("#" + GetDataRowId(1, "OldDataPoint")).val(diaryEntry.VisitId);
                    $("#" + GetDataRowId(0, "OldDisplayValue")).val(diaryEntry.DiaryEntryDateDisplay);
                    $("#" + GetDataRowId(1, "OldDisplayValue")).val(diaryEntry.VisitName);
                    $("#" + GetDataRowId(0, "RowId")).val(diaryEntry.Id);
                    $("#" + GetDataRowId(1, "RowId")).val(diaryEntry.Id);
                }

                ajaxPost(GetDiaryEntryUrl, data, success);
            }
        },

        SetChangeQuestionnairePageDisplay: function() {
            YPrime.ChangeQuestionnaireInfo.CheckQuestionnaireFieldsetDisplay();

            $("#" + DiaryDateLabel).html("");
            $("." + VisitSelectCss).val("");

            $("." + GetDataRowId(0, "NewDataPoint")).val("");
            $("." + GetDataRowId(1, "NewDataPoint")).val("");
        },

        LoadChangeValues: function() {
            showSpinner(true);
            YPrime.ChangeQuestionnaireInfo.CheckQuestionnaireFieldsetDisplay();

            //load up values from back button
            var diaryDate = $("#" + GetDataRowId(0, "NewDataPoint")).val();
            var visitId = $("#" + GetDataRowId(1, "NewDataPoint")).val();
            $("#" + DiaryDateEntryField).val(diaryDate);
            $("." + VisitSelectCss).val(visitId);
            RefreshSelect($("." + VisitSelectCss));
            CheckValidCorrection();

            showSpinner(false);
            LoadInitialValues = false;
        },

        CheckQuestionnaireFieldsetDisplay: function() {
            var hasDiaryEntry = YPrime.ChangeQuestionnaireInfo.CurrentDiaryEntryId() != null;
            var fieldset = $("#" + QuestionnaireFieldSetId);

            if (hasDiaryEntry) {
                $(fieldset).show();
            } else {
                $(fieldset).hide();
            }
        }
    };
}());