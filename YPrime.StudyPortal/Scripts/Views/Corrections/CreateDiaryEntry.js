
var loadQuestionnaireListArguments = {};

function LoadQuestionnaireList(getQuestionnairesUrl,
    questionnaireId,
    questionnaireName,
    GetPaperDiaryEntryViewUrl,
    QuestionnaireResponseDivId,
    correctionId) {

    loadQuestionnaireListArguments.questionnaireId = questionnaireId;
    loadQuestionnaireListArguments.GetPaperDiaryEntryViewUrl = GetPaperDiaryEntryViewUrl;
    loadQuestionnaireListArguments.QuestionnaireResponseDivId = QuestionnaireResponseDivId;
    loadQuestionnaireListArguments.correctionId = correctionId;
    loadQuestionnaireListArguments.questionnaireName = questionnaireName;

    function init() {

        function success(data) {
            var $dropdown = $("#questionnaireDropdown");
            $dropdown.append($("<option />"));

            $.each(data.JsonData,
                function(key, value) {
                    $dropdown.append($("<option />").val(key).text(value));
                });

            if (loadQuestionnaireListArguments.questionnaireId) {
                $dropdown.val(loadQuestionnaireListArguments.questionnaireId);
                loadQuestionsFromQuestionnaire(loadQuestionnaireListArguments.questionnaireId, loadQuestionnaireListArguments.questionnaireName, loadQuestionnaireListArguments.GetPaperDiaryEntryViewUrl, loadQuestionnaireListArguments.QuestionnaireResponseDivId, loadQuestionnaireListArguments.correctionId, true);
            }

            RefreshSelect($dropdown);
        }

        var callParams = {
            patientId: GetPatientId()
        };

        ajaxCall(getQuestionnairesUrl, callParams, success, {}, "GET", {});
    }

    $(document).ready(init);
}

function loadQuestionsFromQuestionnaire(selectedQuestionnaireId, selectedQuestionnaireName, GetPaperDiaryEntryViewUrl, QuestionnaireResponseDivId, correctionId, isLoadedByInitialLoad) {

    CheckValidCorrection();

    if (isLoadedByInitialLoad) {
        $("#questionnaireDropdownPrevious").val(selectedQuestionnaireId);
    }
    if ($("#questionnaireDropdownPrevious").val() != selectedQuestionnaireId) {
        CleanCorrection();
    }

    if (!isLoadedByInitialLoad) {
        ClearValidations();
    }
    
    if (selectedQuestionnaireId == null || selectedQuestionnaireId == '' || selectedQuestionnaireId == undefined) {
        selectedQuestionnaireId = $('#questionnaireDropdown').val();
    }

    if (selectedQuestionnaireId != null && selectedQuestionnaireId != "") {
        var data = {
            QuestionnaireId: selectedQuestionnaireId,
            CorrectionId: correctionId,
            PatientId: GetPatientId()
        };
        $("#questionnaireDropdownPrevious").val(selectedQuestionnaireId);
        function success(data) {

            function bindResponse(obj) {
                var position = $(obj).attr("position");
                var loadedQuestionnaireId = $("#" + GetDataRowId(0, "NewDataPoint")).val();
                if (selectedQuestionnaireId == loadedQuestionnaireId) {
                    LoadChangeControlInitial(obj, position);
                }

                BindDataChangeControl(obj, position);

                if ($(obj).is("select")) {
                    RefreshSelect(obj);
                }
            }

            function bindRemoveItem(obj) {

                $(obj).bind("change", YPrime.ChangeQuestionnaireResponses.RemoveItemHandler);

                var position = $(obj).attr("position");
                var removeItem = $("#" + GetDataRowId(position, "RemoveItem")).val() == "true";

                if (removeItem) {
                    $(obj).prop("checked", true);
                    $(obj).trigger("change");
                }
            }

            function updateQuestionnaireDataPoint(questionnaireId, questionnaireName) {
                $("#" + GetDataRowId(0, "NewDataPoint")).val(questionnaireId);
                $("#" + GetDataRowId(0, "NewDisplayValue")).val(questionnaireName);
            }

            $("#" + QuestionnaireResponseDivId).find(".response").each(
                function() { bindResponse(this); }
            );
            $("#" + QuestionnaireResponseDivId).find(".correction-remove").each(
                function() { bindRemoveItem(this); }
            );

            $("#Correction_QuestionnaireId").val(selectedQuestionnaireId);

            updateQuestionnaireDataPoint(selectedQuestionnaireId, $("#questionnaireDropdown option:selected").text());
            LoadDirectives();
        }

        function fail() {}

        setTimeout(function() {
                ajaxLoad(GetPaperDiaryEntryViewUrl, data, success, fail, QuestionnaireResponseDivId);
            },
            1);
    } else {
        $("#" + QuestionnaireResponseDivId).html("");
    }
}

var CreateDiaryEntryValidateFormType = function () {
    var questionnaireDropdown = $("#questionnaireDropdown");
    var questionnaireId = questionnaireDropdown.val();

    if (questionnaireId) {
        return true;
    }

    return false;
};