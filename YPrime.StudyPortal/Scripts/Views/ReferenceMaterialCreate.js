/*********************************
 * ReferenceMaterialCreate.js
 * -------------------------------
 * Date:    08Nov2016
 * Author:  BS
 * Desc:    Javascript to help functionality on the create Reference Material partial view
 * Directly controls the Views/ReferenceMaterial/_Create.cshtml page
 ********************************/
var _deleteUrl;

//vso #28093 - add file not working - jo 19Mar2018
function getReferenceMaterialErrorsDiv() {
    var div = $("#createReferenceMaterial").find(".main-reference-errors");
    return div;
}

function initReferenceMaterialCreate(createUrl, deleteUrl) {
    _deleteUrl = deleteUrl;

    //Bind delete
    bindDeleteButton();

    //Set the file & upload
    $("#btnSubmit").click(function() {
        //Clear errors
        getReferenceMaterialErrorsDiv().hide();
        getReferenceMaterialErrorsDiv().empty();
        //this will be gone once it empties ???
        getReferenceMaterialErrorsDiv().find("ul").empty();

        //Set the file
        var formData = new FormData($("form")[0]);

        if ($("#referenceMaterialName").val() == "") {
            addReferenceMaterialError("Please enter a file name.");
        }

        if ($("#fileUpload")[0].files.length == 0) {
            addReferenceMaterialError("Please select a file to upload");
        }

        if (getReferenceMaterialErrorsDiv()[0].innerHTML.length > 0) {
            getReferenceMaterialErrorsDiv().show();
            return;
        }

        formData.append("file", $("#fileUpload")[0].files[0]);
        showSpinner(true);

        //Upload
        var ajaxOptions = {
            cache: false,
            contentType: false,
            processData: false
        };

        ajaxPost(
            createUrl,
            formData,
            createReferenceMaterialResponseHandler,
            null,
            ajaxOptions
        );
    });

    //Bind file picker
    $("#fileUpload").change(function() {
        var fileName = $(this).val();
        if (fileName) {

            fileName = fileName.split("\\").pop();

            $("#fileName").text(fileName);
            $("#fileUploadText").text("Change file");
        }
    });
}

function bindDeleteButton() {
    $(".delete").off("click").on("click",
        function(e) {
            e.preventDefault();
            var id = this.id;
            var name = $(this).attr("data-name");
            showBootStrapConfirmation("Confirm Delete",
                "Are you sure you want to delete the ReferenceMaterial '" + name + "'?",
                function() {
                    ajaxPost(_deleteUrl,
                        { referenceMaterialId: id },
                        function(data, status, xhr) { deleteReferenceMaterialSuccess(data, status, xhr) });
                });
        });
}

function deleteReferenceMaterialSuccess(data, status, xhr) {
    ShowResponseMessage({ MessageTitle: "Info", Message: data.responseText });
    updateReferenceMaterialGrid();
}

function createReferenceMaterialResponseHandler(data, status, xhr) {
    showSpinner(false);

    if (data.success) {
        createReferenceMaterialSuccess(data, status, xhr);
    } else {
        createReferenceMaterialFailure(data, status, xhr);
    }
}

function createReferenceMaterialSuccess(data, status, xhr) {
    clearForm();
    ShowResponseMessage({ MessageTitle: "Info", Message: data.responseText });
    updateReferenceMaterialGrid();
    $("input[type=text]:enabled, textarea, .datepicker").val(null);
    $("select option:first-child").attr("selected", "selected");
}

function createReferenceMaterialFailure(data, status, xhr) {
    var errors = data.responseText.split(",");
    errors.forEach(function(error) {
        addReferenceMaterialError(error);
    });

    getReferenceMaterialErrorsDiv().show();
}

function addReferenceMaterialError(error) {
    if (getReferenceMaterialErrorsDiv()[0].innerHTML.length > 0) {
        getReferenceMaterialErrorsDiv()[0].innerHTML += "<br>";
    }
    getReferenceMaterialErrorsDiv()[0].innerHTML += error;
}

function clearForm() {
    //Clear errors
    getReferenceMaterialErrorsDiv().hide();
    getReferenceMaterialErrorsDiv().find("ul").empty();

    //Reset file upload 
    $("#fileName").text("");
    $("#fileUploadText").text("Select a file");
}