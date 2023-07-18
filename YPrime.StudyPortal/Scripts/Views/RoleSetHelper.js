/*********************************
 * RoleSetHelper.js
 * -------------------------------
 * Date:    19Sep2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality on the set role permission page
 * Directly controls the Views/Role/SetPermission.cshtml page
 ********************************/
var UpdateUrl;
var RoleIdAttribute = "role-id";
var SystemActionIdAttribute = "system-action-id";
var SubscriptionIdAttribute = "subscription-id";
var ReportIdAttribute = "report-id";
var AnalyticsIdAttribute = "analytics-id";
function InitRoleSetHelper(updateUrl) {
    UpdateUrl = updateUrl;
}

function SetRolePermission(obj) {
    var roleId = $(obj).attr(RoleIdAttribute);
    var actionId = $(obj).attr(SystemActionIdAttribute);

    var data = {
        RoleId: roleId,
        SystemActionId: actionId,
        Add: $(obj)[0].checked
    };

    function success() {
    }

    ajaxPost(UpdateUrl, data, success);
}

function SetRoleSubscription(obj) {
    var roleId = $(obj).attr(RoleIdAttribute);
    var actionId = $(obj).attr(SubscriptionIdAttribute);

    var data = {
        RoleId: roleId,
        SubscriptionId: actionId,
        Add: $(obj)[0].checked
    };

    function success() {
    }

    ajaxPost(UpdateUrl, data, success);
}

function SetRoleReport(obj) {
    var roleId = $(obj).attr(RoleIdAttribute);
    var actionId = $(obj).attr(ReportIdAttribute);

    var data = {
        RoleId: roleId,
        ReportId: actionId,
        Add: $(obj)[0].checked
    };

    function success() {
    }

    ajaxPost(UpdateUrl, data, success);
}

function SetRoleAnalytics(obj) {
    var roleId = $(obj).attr(RoleIdAttribute);
    var actionId = $(obj).attr(AnalyticsIdAttribute);

    var data = {
        RoleId: roleId,
        ReportId: actionId,
        Add: $(obj)[0].checked
    };

    function success() {
    }

    ajaxPost(UpdateUrl, data, success);
}