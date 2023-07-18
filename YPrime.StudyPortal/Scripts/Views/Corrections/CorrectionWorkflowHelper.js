/*********************************
 * CorrectionWorkflowHelper.js
 * -------------------------------
 * Date:    19Jul2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the Correction Workflow page
 ********************************/
var YPrime = YPrime || {};

YPrime.CorrectionWorkflowHelper = (function() {
    "use strict";

    var DiscussionCommentId = "discussionComment";
    var CorrectionActionId = "correctionActionId";
    var SubmitCorrectionId = "SubmitCorrection";

    return {
        GetCorrectionAction: function() {
            return $("#" + CorrectionActionId + ":hidden").val() ||
                $("input[name=" + CorrectionActionId + "]:checked").val();
        },
        GetDiscussionComment: function() {
            return $("#" + DiscussionCommentId).val();
        },
        InitCorrectionWorkflow: function() {
            function init() {
                YPrime.CorrectionWorkflowHelper.ValidateCorrectionWorkflow();
                $("#" + DiscussionCommentId).bind("keyup", YPrime.CorrectionWorkflowHelper.ValidateCorrectionWorkflow);
                $("input[name=" + CorrectionActionId + "]")
                    .bind("change", YPrime.CorrectionWorkflowHelper.ValidateCorrectionWorkflow);
            }

            $(document).ready(init);
        },
        ValidateCorrectionWorkflow: function() {
            var submitButton = $("#" + SubmitCorrectionId);
            enableControls(submitButton, false);

            if (!isNullOrWhitespace(YPrime.CorrectionWorkflowHelper.GetDiscussionComment()) &&
                hasValue(YPrime.CorrectionWorkflowHelper.GetCorrectionAction())) {
                enableControls(submitButton, true);
            }
        }

    };
}());