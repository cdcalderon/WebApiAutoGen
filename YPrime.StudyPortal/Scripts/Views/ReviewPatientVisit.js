/*********************************
 * ReviewPatientVisit.js
 * -------------------------------
 * Date:    20Jul2016
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the review visit page
 * Directly controls the ReviewPatientVisit.cshtml page
 ********************************/

function reviewVisit(guid, approved) {

    var data = {
        PatientVisitId: guid,
        Approved: approved
    };

    ajaxPost(
        reviewVisitUrl,
        data
    );
}