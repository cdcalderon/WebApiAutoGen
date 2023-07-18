/*********************************
 * SiteImport.js
 * -------------------------------
 * Date:    23Jun2017
 * Author:  BS
 * Desc:    Javascript to help functionality on the import pages
 ********************************/
function importHelper(templateUrl, validateUrl, importUrl) {
    var self = this;
    this.templateUrl = templateUrl;
    this.validateUrl = validateUrl;
    this.importUrl = importUrl;
    this.importedObjects = [];

    this.init = function () {
        self.bindImport();
        self.bindFileChange();
        self.bindDelimiterChange();
        self.bindExtensionChange();
        self.bindValidateImport();
        self.bindTemplateDownload();
    };

    this.bindImport = function () {
        $('#import').click(function () {
            //Import em
            ajaxPost(self.importUrl, JSON.stringify(self.importedObjects), function (data) {
                var errors = data.Errors;
                if (errors.length > 0) {
                    var errorMessage = "<ul>";
                    for (var i = 0; i < errors.length; i++) {
                        errorMessage += "<li>" + errors[i] + "</li>";
                    }
                    errorMessage += "</ul>";
                    ShowResponseMessage({ MessageTitle: "Error", Message: errorMessage })
                }
                else {
                    ShowResponseMessage({ MessageTitle: "Info", Message: data.SuccessMessage });
                    self.resetForm();
                }
            }, self.apiFailure, { contentType: 'application/json; charset=utf-8', dataType: 'json' });
        });
    }

    this.bindFileChange = function () {
        $('#fileImport').on('change', function (e) {
            var fileName = $(this).val()

            fileName = (this.files[0].name);

            $('#fileImportInfo').val(fileName)
            var $btn = $('#validateImport');
            if (fileName) {
                $btn.prop('disabled', false);
                $btn.removeClass("disabled");
            }
            else {
                $btn.prop('disabled', true);
                $btn.addClass("disabled");
            }
            e.preventDefault();
        });
    }

    this.bindDelimiterChange = function () {
        $('#importFileDelimiter').on('change', function (e) {
            self.setTemplateUrl();
            e.preventDefault();
        });
    }

    this.bindExtensionChange = function () {
        $('#importFileExtension').on('change', function (e) {
            self.setTemplateUrl();
            e.preventDefault();
        });
    }

    this.bindValidateImport = function () {
        $("#validateImport").click(function (e) {
            e.preventDefault();

            //Set formData & validate
            ajaxPost(self.validateUrl, self.getFormData(), function (data) {
                $("#importValidationSummary").html(data.html);
                $("#loadingSpinner").hide();
                self.toggleImportButton();
                self.importedObjects = JSON.parse(data.importedObjects);
            }, self.apiFailure, { contentType: false, dataType: 'json', processData: false });
        });
    }

    this.bindTemplateDownload = function () {
        self.setTemplateUrl();
        $("#downloadImportTemplate").click(function (e) {
            window.location = self.templateUrl;
        });
    }

    this.setTemplateUrl = function () {
        var params = $.param({ delimiter: $('#importFileDelimiter').val(), extension: $('#importFileExtension').val() });
        var updatedHref = self.templateUrl.split('?')[0] + '?' + params;
        self.templateUrl = updatedHref;
    };

    this.toggleImportButton = function () {
        var $btn = $('#import');
        if ($(".alert.alert-danger").length == 0) {
            $btn.prop('disabled', false);
            $btn.removeClass("disabled");
        }
        else {
            $btn.prop('disabled', true);
            $btn.addClass("disabled");
        }
    }

    this.getFormData = function () {
        var formData = new FormData($('form')[0]);
        formData.append('file', $('#fileImport')[0].files[0]);
        formData.append('Delimiter', $('#importFileDelimiter').val());
        formData.append('Extension', $('#importFileExtension').val());
        return formData;
    }

    this.resetForm = function () {
        var $btn = $('#import');
        $btn.prop('disabled', true);
        $btn.addClass("disabled");
    }

    this.apiFailure = function (error) {
        ShowResponseMessage({ MessageTitle: "Error", Message: error })
    }
}
