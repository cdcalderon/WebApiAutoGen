/*********************************
 * ResupplySettings.js
 * -------------------------------
 * Date:    10Jan2017
 * Author:  Joel Marcinik
 * Desc:    Javascript to help functionality on the resupply settings page
 * Directly controls the Views/ResupplySettings/Index.cshtml page
 ********************************/
$(function() {

});

function initGrids() {
    var gridName = "ResupplySettingsGrid";
    var pagingUrl = "ResupplySettings/ResupplySettingsGridPager";
    initAjaxGrid(gridName, pagingUrl);
}

function initControls() {
    $("input").change(function() {
        $("#isDirty").val(true);
    });
}