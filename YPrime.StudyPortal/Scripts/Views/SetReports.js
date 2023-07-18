///*********************************
// * SetReports.js
// * -------------------------------
// * Date:    27Jun2017
// * Author:  C Patel
// * Desc:    Javascript to help functionality on the report assignment to a role
// * Directly controls the SetReportStudyRoles.cshtml page
// ********************************/

//var hiddenAddIds = "AddItemGuids";
//var hiddenRemoveIds = "RemoveItemGuids";
//var userContainerId = "role-reports-panel";
//var visitContainerId = "reports-panel";
//var lblTotalCount = "NumberAssociated";

//$(document).ready(function () { init(); });

//function init() {

//    $("#reports-panel, #role-reports-panel").sortable({
//        connectWith: ".connectedSortable",
//        receive: selectionDropped
//    }).disableSelection();

//    $(".fa-times-circle-o, .fa-plus-circle").on('click', function () {
//        var panel = $(this).parent().parent().closest('div');
//        selectionClicked(panel);
//    });

//    checkForChanges();
//    setTotalCount();
//}

//function selectionClicked(ui) {
//    var guid = ui.attr('data-guid');
//    var saved = ui.attr('data-saved') == 'true';
//    var addItemToUser = ui.parent().closest('div').attr("id") == "reports-panel";
//    var currentRoleId = $('#id').val();

//    setAddValues();
//    if (addItemToUser) {
//        //add the item to user
//        addReportToRole(currentRoleId, guid, ui);
//        $('#role-reports-panel').prepend(ui);
//        $('#reports-panel', ui).remove();
//    } else {
//        //check if it is a save object
//        if (saved) {
//            removeReportFromRole(currentRoleId, guid, ui);
//            $('#reports-panel').prepend(ui);
//            $('#role-reports-panel', ui).remove();
//        }
//    }
//}

////this is the event when the item is added to a new panel!
//function selectionDropped(event, ui) {
//    var guid = $(ui.item).attr('data-guid');
//    var saved = $(ui.item).attr('data-saved') == 'true';
//    var addItemToUser = $(ui.sender).attr("id") == "reports-panel";
//    var currentRoleId = $('#id').val();
//    //call this no matter what
//    setAddValues();
//    if (addItemToUser) {
//        //add the item to user
//        addReportToRole(currentRoleId, guid, ui.item);
//    } else {
//        //check if it is a save object
//        if (saved) {
//            removeReportFromRole(currentRoleId, guid, ui.item);
//        }
//    }

//}

////NOTE: urls are created in the cshtml view!
//function addReportToRole(roleId, guid, obj) {
//    function successCallback(data) {
//        if (data.Success) {
//            $(obj).attr('data-saved', true);
//            setItemDisplayColor(obj, true);
//        } else {
//            failCallback();
//        }
//    }
//    function failCallback() {
//        showBootStrapMessage("Error", "An error occurred adding the report to the role.");
//        $(obj).attr('data-saved', false);
//        $('#reports-panel').prepend(obj);
//        $('#role-reports-panel').remove(obj);
//    }

//    var data = {
//        roleId: roleId,
//        reportId: guid
//    };

//    ajaxPost(
//        addReportToRoleUrl,
//        data,
//        successCallback,
//        failCallback,
//        { NoSpinner: true });
//}

//function removeReportFromRole(roleId, guid, obj) {
//    function successCallback(data) {
//        if (data.Success) {
//            $(obj).attr('data-saved', false);
//            setItemDisplayColor(obj, false);
//        } else {
//            failCallback();
//        }
//    }
//    function failCallback() {
//        showBootStrapMessage("Error", "An error occurred removing the permission from the role.");
//        $(obj).attr('data-saved', true);
//        $('#role-reports-panel').prepend(obj);
//        $('#reports-panel').remove(obj);
//    }

//    var data = {
//        roleId: roleId,
//        reportId: guid
//    };

//    ajaxPost(
//        removeReportFromRoleUrl,
//        data,
//        successCallback,
//        failCallback,
//        { NoSpinner: true });
//}

//function setItemDisplayColor(obj, addingItem) {
//    var gray = '#f5f5f5';
//    var black = '#000000';

//    function resetPanel() {
//        $(obj).addClass('panel-default');
//        $(obj).removeClass(addingItem ? 'panel-success' : 'panel-danger');
//        $(obj).find('.panel-heading').attr('style', '');
//    }

//    $(obj).removeClass('panel-default');
//    $(obj).addClass(addingItem ? 'panel-success' : 'panel-danger');
//    $(obj).find('>:first-child').find('#addRemoveIcon').removeClass(addingItem ? 'fa-plus-circle grid-true-icon' : 'fa-times-circle-o grid-false-icon');
//    $(obj).find('>:first-child').find('#addRemoveIcon').addClass(addingItem ? 'fa-times-circle-o grid-false-icon' : 'fa-plus-circle grid-true-icon');
//    $(obj).find('.panel-heading')
//        .animate({
//                backgroundColor: gray,
//                color: black
//            },
//            2000,
//            resetPanel
//        );

//    setTotalCount();
//}

//function setTotalCount() {
//    var count = $('#' + userContainerId).find('[data-saved]').length;
//    $('#' + lblTotalCount).html(count);
//}

///*************************
// * This functionality may be deprecated it is used to hold multiple values to be sent at once
// ************************/
//function setAddValues() {
//    var add = [];
//    $('#' + visitContainerId).children().each(function () {
//        if ($(this).attr('data-saved') == 'false') {
//            add.push($(this).attr('data-guid'));
//        }
//    });

//}

//function checkForChanges() {
//    pageHasChanges ? $('#' + btnSaveChangesId).show() : $('#' + btnSaveChangesId).hide();
//}

//function setItemDisplay(obj, addItemToUser) {
//    var saved = $(obj).attr('data-saved') == 'true';
//    var icon = $(obj).find('i.fa');

//    //remove all css
//    $(obj).removeClass('panel-default');
//    $(obj).removeClass('panel-danger');
//    $(obj).removeClass('panel-success');

//    $(icon).removeClass('fa-save');
//    $(icon).removeClass('fa-remove');
//    $(icon).removeClass('fa-plus');

//    //will be deleted
//    if (saved && !addItemToUser) {
//        $(icon).addClass('fa-remove');
//        $(obj).addClass('panel-danger');
//    }
//    //undo delete
//    if (saved && addItemToUser) {
//        $(icon).addClass('fa-save');
//        $(obj).addClass('panel-default');
//    }
//    //add to the user
//    if (!saved && addItemToUser) {
//        $(icon).addClass('fa-plus');
//        $(obj).addClass('panel-success');
//    }
//    //return to pending visits to assign
//    if (!saved && !addItemToUser) {
//        $(obj).addClass('panel-default');
//    }
//}