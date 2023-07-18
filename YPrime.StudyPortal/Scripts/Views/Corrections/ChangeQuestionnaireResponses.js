/*********************************
 * ChangeQuestionnaireResponses.js
 * -------------------------------
 * Date:    30Aug2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Change Questionnaire Answers view
 * Directly controls the Views/Correction/ChangeQuestionnaireInfo.cshtml page
 ********************************/
var YPrime = YPrime || {};

YPrime.ChangeQuestionnaireResponses = (function() {
    "use strict";
    var GetDiaryEntriesUrl;
    var GetDiaryEntryViewUrl;
    var ChangeQuestionnairePatientId = null;
    var DiaryEntryIdInput = "DiaryEntryId";
    var DiarySelectId = "RowId";
    var DiaryDateLabel = "DiaryDateLabel";
    var VisitNameLabel = "VisitNameLabel";
    var VisitSelectCss = "VisitList";
    var DiaryDateEntryField = "DiaryDateDCF";
    var QuestionnaireFieldSetId = "QuestionnaireInformationFieldset";
    var LoadInitialValues = false;
    var CorrectionTableId = "DataCorrectionTable";
    var QuestionnaireResponseDivId = "QuestionnaireResponseDiv";
    var CorrectionRemoveCss = "correction-remove";
    var CorrectionAnswerCss = "correction-answer";
    var CorrectionId;
    var RowIdEmpty = '00000000-0000-0000-0000-000000000000';

    return {
        InitChangeQuestionnaireResponses: function(getDiaryEntriesUrl,
            getDiaryEntryViewUrl,
            patientId,
            diaryEntryId,
            correctionId) {
            function init() {
                GetDiaryEntriesUrl = getDiaryEntriesUrl;
                GetDiaryEntryViewUrl = getDiaryEntryViewUrl;
                ChangeQuestionnairePatientId = patientId;
                CorrectionId = correctionId;

                YPrime.ChangeQuestionnaireResponses.SetChangeQuestionnairePageDisplay();

                LoadInitialValues = typeof diaryEntryId != "undefined" && diaryEntryId != null;

                YPrime.ChangeQuestionnaireResponses.LoadDiaryEntries(diaryEntryId);
            }

            $(document).ready(init);
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

                YPrime.ChangeQuestionnaireResponses.SetCurrentDiaryEntryId(diaryEntryId);
                $("#" + DiarySelectId).val(diaryEntryId);
                RefreshSelect($("#" + DiarySelectId));

                YPrime.ChangeQuestionnaireResponses.LoadDiaryEntry(true);
            }

            ajaxPost(GetDiaryEntriesUrl, data, success);
        },

        CurrentDiaryEntryId: function() {
            return $("#" + DiarySelectId).val();
        },

        SetCurrentDiaryEntryId: function(id) {
            $("#" + DiarySelectId).val(id);
        },

        LoadDiaryEntry: function (removeToggleEnabled) {
            var diaryEntryId =
                YPrime.ChangeQuestionnaireResponses.CurrentDiaryEntryId(); //$('#' + DiarySelectId).val();
            
            //retain the diary entry id to match up later for display - jo 28Aug2017
            $("#" + DiaryEntryIdInput).val(diaryEntryId);

            if ((diaryEntryId != null && diaryEntryId != "")
                && ($("#questionnaireDropdownPrevious").val() == null
                || $("#questionnaireDropdownPrevious").val() == ""))
            {
                $("#questionnaireDropdownPrevious").val(diaryEntryId);
            }

            if ($("#questionnaireDropdownPrevious").val() != diaryEntryId) {
                CleanCorrection();
            }

            if (diaryEntryId != null && diaryEntryId != "") {
                var data = {
                    DiaryEntryId: diaryEntryId,
                    CorrectionId: CorrectionId
                };

                $("#questionnaireDropdownPrevious").val(diaryEntryId);
                function success(data) {
                    //bind up the hidden fields
                    function bindResponse(obj) {
                        var position = $(obj).attr("position");
                        LoadChangeControlInitial(obj, position);
                        BindDataChangeControl(obj, position);
                    }

                    function bindRemoveItem(obj, toggleEnabled) {
                        $(obj).bind("change", YPrime.ChangeQuestionnaireResponses.RemoveItemHandler);

                        //set the initial remove item value
                        var position = $(obj).attr("position");
                        var removeItem = $("#" + GetDataRowId(position, "RemoveItem")).val() == "true";
                        var position = $(obj).attr('position');
                        var removeItem = false;

                        if ($(obj).data('multi-select') == 'True') {
                            $(obj).closest('tr').find('.' + position + '_multi-select').each(
                                function () {
                                    if (!removeItem) {
                                        if ($('#' + GetDataRowId($(this).attr('position'), "RowId")).val() !== RowIdEmpty) {
                                            removeItem = $('#' + GetDataRowId($(this).attr('position'), "RemoveItem")).val() == 'true';
                                        }
                                    }
                                });
                        }

                        else {
                            removeItem = $('#' + GetDataRowId(position, "RemoveItem")).val() == 'true';
                        }

                        if (removeItem) {
                            $(obj).prop("checked", toggleEnabled);
                            $(obj).trigger("change");
                        }
                    }

                    $("#" + QuestionnaireResponseDivId).find(".response").each(
                        function() { bindResponse(this); }
                    );

                    $("#" + QuestionnaireResponseDivId).find("." + CorrectionRemoveCss).each(
                        function() { bindRemoveItem(this, removeToggleEnabled); }
                    );

                    LoadDirectives();
                    CheckValidCorrection();
                }

                function fail() {}

                //do this to insure the spinner shows
                setTimeout(function() {
                        ajaxLoad(GetDiaryEntryViewUrl, data, success, fail, QuestionnaireResponseDivId);
                    },
                    1);

            } else {

                //clear the div
                $("#" + QuestionnaireResponseDivId).html("");
            }
        },
        RemoveItemHandler: function() {
            var position = $(this).attr("position");
            var checked = $(this)[0].checked;
            var label = $(this).closest("tr").find("." + CorrectionAnswerCss);
            var disabledCss = "disabled-panel";

            if (checked) {
                //set up display
                $(label).addClass("strike-through");
                $(this).closest("tr").removeClass(unchangedCss);
                $(this).closest("tr").find(".question-response").each(
                    function() {
                        $(this).addClass(disabledCss);
                    });
            } else {
                $(label).removeClass("strike-through");
                $(this).closest("tr").addClass(unchangedCss);
                $(this).closest("tr").find(".question-response").each(
                    function() {
                        $(this).removeClass(disabledCss);
                    });
            }

            //set the removeItem value

            var $removeToggle = $(this).closest('tr').find('.' + CorrectionRemoveCss);

            if ($removeToggle.data('multi-select') == 'True') {
                $(this).closest('tr').find('.' + position + '_multi-select').each(
                    function () {
                        if ($('#' + GetDataRowId($(this).attr('position'), "RowId")).val() !== RowIdEmpty) {
                            $('#' + GetDataRowId($(this).attr('position'), "RemoveItem")).val(checked);
                        }
                    });
            }

            else {
                $('#' + GetDataRowId(position, "RemoveItem")).val(checked);
            }

            CheckValidCorrection();
        },
        SetChangeQuestionnairePageDisplay: function() {
            YPrime.ChangeQuestionnaireResponses.CheckQuestionnaireFieldsetDisplay();

            //$('#' + DiaryDateLabel).html("");
            //$('.' + VisitSelectCss).val("");

            //$('.' + GetDataRowId(0, "NewDataPoint")).val("");
            //$('.' + GetDataRowId(1, "NewDataPoint")).val("");
        },

        LoadChangeValues: function() {
            showSpinner(true);
            YPrime.ChangeQuestionnaireResponses.CheckQuestionnaireFieldsetDisplay();

            //load up values from back button
            var diaryDate = $("#" + GetDataRowId(0, "NewDataPoint")).val();
            var visitId = $("#" + GetDataRowId(1, "NewDataPoint")).val();
            $("#" + DiaryDateEntryField).val(diaryDate);
            $("." + VisitSelectCss).val(visitId);
            RefreshSelect($("." + VisitSelectCss));

            showSpinner(false);
            LoadInitialValues = false;
        },

        CheckQuestionnaireFieldsetDisplay: function() {
            var hasDiaryEntry = YPrime.ChangeQuestionnaireResponses.CurrentDiaryEntryId() != null;
            var fieldset = $("#" + QuestionnaireFieldSetId);

            if (hasDiaryEntry) {
                $(fieldset).show();
            } else {
                $(fieldset).hide();
            }
        },
        ValidateFormType: function() {
            var rowId = $("#RowId").val();

            var isQuestionnaireSelected = rowId !== null && rowId !== undefined && rowId !== "";

            var questionCount = $(`#${CorrectionTableId} tr`).length - 1; //-1 for header row
            var unchangedQuestionCount = $(`#${CorrectionTableId} tr.unchanged`).length;

            return isQuestionnaireSelected && questionCount != unchangedQuestionCount;
        }
    };
}());