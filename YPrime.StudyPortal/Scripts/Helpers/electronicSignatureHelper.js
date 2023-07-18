var electronicSignatureClass = "electronic-signature";
var electronicSignatureTitle = "ElectronicSignatureTitle";
var submitButton = "SubmitCorrection";

function ElectronicSignatureInit(event) {
    var form = event.target;
    event.preventDefault();

    //display the electronic signature screen
    var correctionAction = YPrime.CorrectionWorkflowHelper.GetCorrectionAction();
    var url = GLOBAL_ElectronicSignatureUrl + "?correctionAction=" + correctionAction + "&siteId=" + GetValidationSiteId(); //note: this is retained on the main layout page

    var options = { "dataType": "html" };

    function success(data) {
        var html = "btn ";

        function okCallback(dialog) {
            $(form).removeClass(electronicSignatureClass);

            //show the spinner
            showSpinner(true);
            setTimeout(function() {
                    $(form).submit();
                },
                1);
        }

        function cancelClicked(dialog) {

            /* Canceled so enable the Submit button. */
            enableControls("#" + submitButton, true);
        }

        var okButton = createBootstrapButtonObject("Approve", html + "btn-primary", okCallback, true);
        var cancelButton = createBootstrapButtonObject("Decline", html + "btn-secondary", cancelClicked, false);

        showBootStrapMessage(getElectronicSignatureTitle(), data, [okButton, cancelButton], cancelClicked);
    }

    if (isElectronicSignatureForm(form)) {
        ajaxGet(url, success, null, options);
    }
}

function isElectronicSignatureForm(obj) {
    return $(obj).hasClass(electronicSignatureClass);
}

function getElectronicSignatureTitle() {
    return $("#" + electronicSignatureTitle).val();
}