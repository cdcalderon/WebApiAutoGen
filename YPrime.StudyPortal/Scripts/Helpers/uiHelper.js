/*************************
 * uiHelper.js
 * J Osifchin
 * 21Jun2016
 * methods to aid in getting, setting or rendering html ui controls
 ***************************/

/***********************
 * Panels
 ***********************/
const enterKeyCode = 13;
var panelOrderMainObjectKey = "RetainedPanelOrder";
var disabledPanelClass = "disabled-panel";
var breadCrumbWrapperClass = "breadcrumb";

/*************************************
 * Generic methods
 * **********************************/
function getUniqueId() {
    return (new Date().getTime()).toString(36);
}

function getCurrentUrl() {
    return window.location.href.replace(/#/, "");
}

function hasValue(val) {
    return typeof val != "undefined" && val != null && val.length > 0;
}

function isNullOrWhitespace(val) {
    if (typeof val === 'undefined' || val == null) 
    {
        return true;
    }
    return val.replace(/\s/g, '').length < 1;
}

function CleanText(text) {
    return $("<div>" + text + "</div>").text();
}

//VSO #11189 - Question Builder and Portal: Set Max value to 6 but able to enter more then 6 
//chrome does not support maxlength on number inputs
function initNumericMaxLength() {
    $(':input[type="number"]').each(function() {
        $(this).bind("keyup", function() { limitKeyStrokes(this) });
    });
}

function limitKeyStrokes(obj) {
    if ($(obj).attr("maxlength")) {
        var len = $(obj).attr("maxlength") * 1;
        if (obj.value.length > len) {
            obj.value = obj.value.substr(0, len);
        }
    }
}

function positiveInt() {
    $(":input[type=number][positiveInt]").each(function() {
        var inputField = this;
        $(this).on("keydown",
            function(e) {
                if ((e.which >= 96 && e.which <= 105) //numberpad
                        ||
                        (e.which >= 48 && e.which <= 57) //0-9
                        ||
                        e.which == 8 //backspace
                        ||
                        e.which == 46 //del
                        ||
                        (e.which >= 35 && e.which <= 39) //arrows and home/end
                        ||
                        e.which == 9 //tab
                ) {
                    return true;
                }
                return false;
            });
    });
}

/*************************************
 * IsDirty Methods
 * **********************************/
function getIsDirty() {
    if (typeof window.IsDirty == "undefined") {
        setIsDirty(false);
    }
    return window.IsDirty;
}

function setIsDirty(val) {
    window.IsDirty = val;
}

function initIsDirty() {
    //add the before unload event to the window
    window.onbeforeunload = function() {
        if (getIsDirty()) {
            return "You have unsaved changes!";
        }
    };
}

/***********************************
 * Active Tab Methods 
 ***********************************/
var activeTabVariable = "ActiveTab";
var lastUrlBaseVariable = "LastUrlBase";

function getActiveTab() {
    var result = localStorage.getItem(activeTabVariable);
    return result;
}

function setActiveTab(value) {
    var url = window.location;
    localStorage.setItem(activeTabVariable, value);
    setLastUrl(url);
}

function getLastUrl() {
    return localStorage.getItem(lastUrlBaseVariable);
}

function setLastUrl(value) {
    localStorage.setItem(lastUrlBaseVariable, value);
}

function checkActiveTabs() {
    if (getLastUrl() != window.location) {
        setActiveTab(null);
    }
}


/*************************************
 * Last Message Cookie Methods
 * **********************************/
var messageIdCookieValue = "LastMessageId";

function getLastMessageGuid() {
    return getCookie(messageIdCookieValue);
}

function setLastMessageGuid(value) {
    setCookie(messageIdCookieValue, value, 1);
}

/*************************************
 * Cookie Methods
 * **********************************/
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


/*************************************
 * Panels
 * **********************************/
function initDraggablePanel(containerId) {
    var panelList = $("#" + containerId);

    panelList.sortable({
        // Only make the .panel-heading child elements support dragging.
        // Omit this to make then entire <li>...</li> draggable.
        handle: ".panel-heading",
        create: function(event, ui) {
            //load up the order items
            var obj = getSortedPanels(containerId);
            var panelList = [];
            for (var par in obj[containerId]) {
                panelList.push({ key: par, value: obj[containerId][par] });
            }

            if (panelList.length > 0) {
                panelList.sort(function(a, b) {
                    return a.value == b.value ? 0 : (a.value < b.value ? -1 : 1);
                });

                for (var i = 0; i < panelList.length; i++) {
                    $(this).append($("#" + panelList[i].key));
                }
            }
        },
        update: function() {
            var order = $(this).sortable("serialize");
            var retainPanelOrder = {};

            $(this).children().each(function(index, elem) {
                //not sure if it is safe to use the index passed in
                var panelId = $(elem).attr("id");
                var newIndex = $(elem).index();

                if (typeof panelId != undefined && panelId != null && panelId != "") {
                    retainPanelOrder[panelId] = newIndex;
                }
            });

            setSortedPanels(containerId, retainPanelOrder);
        }
    });
}

function getSortedPanels(containerId) {
    var obj = {};
    if (typeof $.cookie(panelOrderMainObjectKey) != "undefined") {
        obj = eval("(" + $.cookie(panelOrderMainObjectKey) + ")");
    }
    if (typeof obj[containerId] == "undefined" || obj[containerId] == null) {
        obj[containerId] = {};
    }
    return obj;
}

function setSortedPanels(containerId, value) {
    ;
    var obj = getSortedPanels(containerId);
    obj[containerId] = value;
    $.cookie(panelOrderMainObjectKey, JSON.stringify(obj));
}


/*************************************
 * Bootstrap Modals
 * **********************************/
function showBootStrapMessage(title, message, buttons, onhiddenEvent) {
    var options = {
        title: title,
        message: message,
        groupButtons: true
    };

    if (onhiddenEvent && onhiddenEvent instanceof Function) {
        options.onhidden = onhiddenEvent;
    }

    if (typeof buttons != "undefined" && buttons != null) {
        options.buttons = buttons;
        options.closeByBackdrop = false;

        BootstrapDialog.show(options);
    } else {
        //this is a standart alert
        BootstrapDialog.alert(options);
    }
}

function showBootStrapConfirmation(title, message, okCallback) {
    var htmlOk = "btn btn-primary";
    var htmlCancel = "btn btn-secondary";
    var okButton = createBootstrapButtonObject("Ok", htmlOk, okCallback);
    var cancelButton = createBootstrapButtonObject("Cancel", htmlCancel);

    showBootStrapMessage(title, message, [okButton, cancelButton]);
}

function createBootstrapButtonObject(text, css, callback, keepDialogOpen) {
    return {
        label: text,
        cssClass: css != null ? css : "btn-primary",
        action: function(dialog) {
            if (typeof callback == "function") {
                callback(dialog);
            }
            if (!keepDialogOpen) {
                dialog.close();
            }
        }
    };
}

/*************************************
 * Spinner Methods
 * **********************************/
function initFormPostSpinner() {
    function attachSubmit() {
        
        $(this).submit(function(ev) {
            //skip any ajax posts
            if (typeof $(this).attr("data-ajax") == "undefined" && !isElectronicSignatureForm(this)) {
                ev.preventDefault();
                //show the spinner
                showSpinner(true);
                this.submit();
            }
        });
    }

    window.addEventListener("beforeunload",
        function(event) {
            if (typeof event.srcElement.activeElement != "undefined" &&
                typeof event.srcElement.activeElement.nextSiblingElement != "undefined" &&
                !event.srcElement.activeElement.classList.contains("excel-export") &&
                !event.srcElement.activeElement.nextSiblingElement.contains("a.data-export") ||
                typeof event.srcElement.activeElement == "undefined" ||
                typeof event.srcElement.activeElement.nextElementSibling == "undefined") {
                showSpinner(true);
            }
        });

    $("form").each(attachSubmit);
}

function showSpinner(show, delay) {
    var loader = $("#loadingSpinner");
    if (show) {
        loader.fadeIn(delay);
    } else {
        loader.fadeOut(delay);
    }
}

function AddPaddingToValue(Text) {
    //var PaddingCharacters = "@Model.PaddingCharacters";

    var EnteredText = $(Text).val();
    if (EnteredText.length == 2) {
        var newText = [EnteredText.slice(0, 1), "-", EnteredText.slice(1)].join("");
        $(Text).val(newText);
    }
}

/*************************************
 * Form Methods
 * **********************************/
function getFormData(obj) {
    var unindexed_array = obj.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array,
        function(n, i) {
            indexed_array[n["name"]] = n["value"];
        });

    return indexed_array;
}

/*************************************
 * Control Methods
 * **********************************/
function enableControls(div, enabled) {
    div = $(div); // make sure it is an object
    if (enabled) {
        $(div).removeClass(disabledPanelClass);
        $(div).removeAttr("disabled");
    } else {
        $(div).addClass(disabledPanelClass);
        $(div).prop("disabled", true);
    }
    var inputType = $(div)[0].tagName;

    switch (inputType) {
    case "SELECT":
        RefreshSelect($(div));
        break;
    }
}

/*************************************
 * Select Methods
 * **********************************/
function LoadSelectItems(select, items, skipBlank, placeholderText = "") {
    //NOTE: items is a dictionary, the ddl value is the key and the ddl text is the dictionary value 
    var dropdown = typeof select == "object" ? $(select) : $("#" + select);

    $(dropdown).empty();
    if (!skipBlank) {
        $("<option />",
            {
                val: null,
                text: placeholderText
            }).appendTo(dropdown);
    }

    for (key in items) {
        $("<option />",
            {
                val: key,
                text: items[key]
            }).appendTo(dropdown);
    }

    RefreshSelect($(dropdown));
}

function RefreshSelect(obj) {

    //if (typeof $(obj).easyDropDown() != 'undefined' && typeof $(obj).easyDropDown.destroy != 'undefined') {
    //}
    //NOTE: hate to do the check this way, but couldn't get the function check above to work correctly - jo 30May2017
    try {
        $(obj).easyDropDown("destroy");
    } catch (error) {
    }

    CreateSelect(obj);
}

function CreateSelect(obj) {
    if ($(obj).attr("multiple") == undefined) {
        $(obj).easyDropDown({
            wrapperClass: "metro",
            cutOff: 6,
            onChange: function(selected) {
                if (selected && selected.title && selected.title.length > 0) {
                    if ($(this).parent() &&
                        $(this).parent().parent() &&
                        $(this).parent().parent().parent() &&
                        $(this).parent().parent().parent().hasClass("dcf-multiple-choice-response")) {
                        var htmlStripRegex = /(<([^>]+)>)/ig;
                        var spaceStripRegex = /&nbsp;/g;
                        var result = selected.title.replace(htmlStripRegex, " ").replace(spaceStripRegex, " ");

                        $(this).parent().parent().children(".selected").text(result);
                    }
                }
            }
        });
    } else {
        $(obj).select2({
            width: "100%"
        });
    }

    if ($(obj).hasClass("form-control") && $(obj).hasClass("response"))
    {
        var displayValue = GetControlText(obj);
        SetSelectCss(obj, displayValue)
    }
}

function SelectClickHandler(obj) {
    //fix the width of the dropdown'

    //var offset = 10;
    //var mainWidth = parseInt($(obj).width());
    //var ulWidth = parseInt($(obj).find('ul').width()) + offset;
    //var div = $(obj).find('div');

    //if (div.length > 0 && ulWidth > 0) {
    //    $(div).width(ulWidth > mainWidth ? ulWidth : mainWidth);
    //    if (ulWidth < mainWidth - offset) {
    //        alert(ulWidth + ' ' + mainWidth);
    //        $(obj).find('ul').css('width', 'auto');
    //    }
    //}
}

//***************************************************************************
//this loads up any css configured attributes
//if the css property is on the object, the functionality will be added
//1. oneclick - this will make the button disabled on first click
//2. checkisdirty - this will bind events to force the page to record is dirty changes
//      ex.         <input type="text" class="checkisdirty" />
//3. datepicker - adds the datepicker object to the element
//4. multiselect-control - multiselect dropdown http://davidstutz.github.io/bootstrap-multiselect/
//***************************************************************************/
var calendarCss = "calendar-click";

var directiveConfiguration = {
    'oneclick': function() {
        $(this).bind(
            "click",
            function() {
                if (typeof event.preventDefault != "undefined") {
                    event.preventDefault();
                }

                var obj = this;
                var refreshTimeout = 6000;

                //check for anchor link
                var tagName = this.tagName;
                if (tagName != "A") {
                    var form = $("form");

                    $(form).submit();
                    $(obj).prop("disabled", true);
                }
            });
    },
    'checkisdirty': function() {
        $(this).bind(
            "change",
            function() {
                setIsDirty(true);
            });
    },
    'datepicker': function () {

        if (isDCF(this))
        {
            this.setAttribute("type", "search");
        }

        $(this).on("keyup keydown keypress",
            function(e) {
                e.preventDefault();
            });

        var minDate = $(this).attr("min");
        var maxDate = $(this).attr("max");
        var today = $(this).attr("today");

        var defaultDateFormat = "DD-MMMM-YYYY";
        var nonConfigurableDefaultDateFormat = "DD-MMM-YYYY";
        var dcfQuestionnaireDefaultDateFormat = "DD/MMMM/YYYY";

        var dateFormatElement = $(this).siblings(".date-format");
        var dateFormat = null;

        if (dateFormatElement && dateFormatElement.val()) {
            dateFormat = dateFormatElement.val()
        }

        if (!dateFormat && isDCFNonconfigurableDate(this)) {
            dateFormat = nonConfigurableDefaultDateFormat;
        } else if (!dateFormat && isDCFQuestionnaireDate(this)) {
            dateFormat = dcfQuestionnaireDefaultDateFormat;
        } else if (!dateFormat) {
            dateFormat = defaultDateFormat;
        }

        var datetimeConfig = {
            format: dateFormat,
            keepInvalid: true,
            useCurrent: false,
            collapse: false
        };

        if (minDate) {
            datetimeConfig.minDate = minDate;
        } 

        if (maxDate) {
            datetimeConfig.maxDate = maxDate;
        }

        $(this).datetimepicker(datetimeConfig);

        if (today)
        {
            $(this).on("dp.show",
                function () {
                    const todayClassName = "today";
                    $('.day').each(function () {
                        if (this.getAttribute("data-day") == today) {
                            $(this).addClass(todayClassName);
                        }
                        else {
                            $(this).removeClass(todayClassName);
                        }
                    });
                });
        }
        
        $(this).on("dp.change",
            function() {
                $(this).change();
            });

        $(this).prop("autocomplete", "off");
        //vso #22340 - Val : IRT/eCOA : Calendar picker should pop up when we click on the calendar icon  jo 21Aug2017
        var sideButton = $(this).next();
        if (sideButton.length > -1 && $(sideButton).hasClass(calendarCss)) {
            var input = this;

            function clickHandler() {
                $(input).focus();
            }

            $(sideButton).bind("click", clickHandler);
        }
    },
    'dobdatepicker': function() {
        var min = new Date($(this).attr("min"));
        var max = new Date($(this).attr("max"));

        $(this).datepicker({
            changeMonth: true,
            changeYear: true,
            minDate: min,
            maxDate: max,
            yearRange: min.getFullYear() + ":" + max.getFullYear()
        });
        $(this).prop("autocomplete", "off");
        var sideButton = $(this).next();
        if (sideButton.length > -1 && $(sideButton).hasClass(calendarCss)) {
            var input = this;

            function clickHandler() {
                $(input).focus();
            }

            $(sideButton).bind("click", clickHandler);
        }
    },

    'multiselect-control': function() {
        $(this).multiselect();
        //vso #18955 -  Individual Drug Assignment - CSS issue with button not being visable properly jo 25May2017
        $(this).parent().find(".multiselect").each(function() {
            $(this).removeClass("btn").removeClass("btn-default");
        });
    },
    'select': function() {
        CreateSelect(this);
    },
    'tooltip': function() {
        $(this).tooltip();
    },
    'pop-over': function () {
        $(this).popover({ sanitize: false });
    },
    'disabled-panel': function() {
        $(this).bind("click",
            function() {
                return false;
            });
    },
    'electronic-signature': function() {
        //this will prevent a form post until the electronic signature is entered
        $(this).attr("type", "button");
        var form = $(this).closest("form");
        $(form).addClass(electronicSignatureClass);
        $(form).on("submit", ElectronicSignatureInit);
    },
    'non-numeric-text-control': function() {
        $(this).on("input",
            function() {
                var regex = new RegExp("\\d+");

                var parentElement = $(this).parent();

                if (!parentElement) {
                    return;
                }

                const errorClassName = "has-error";

                var controlValue = $(this).val();

                if (regex.test(controlValue)) {
                    parentElement.addClass(errorClassName);
                } else {
                    parentElement.removeClass(errorClassName);
                }
            });
    },
    'multiselectcheckbox': function () {
        $(this).on("change",
            function (e) {
                var eventTriggeredByUser = e.originalEvent !== undefined;
                var questionId = this.getAttribute("questionid");

                // Change events are triggered on the adjacent checkboxes so the CorrectionData grids can be updated, 
                // but we only want this logic to respond to user input
                if (eventTriggeredByUser) {
                    if (this.getAttribute("clearResponses") == "True") {
                        $("[clearResponses^='False'][questionid='" + questionId + "']").each(function () {
                            // if it is checked, uncheck it
                            if ($(this).is(":checked")) {
                                $(this).prop('checked', false);
                                $(this).trigger('change');
                            }
                        });
                    }
                    else {
                        // there should only be one, but this is a safe guard to make sure all of them get unchecked.
                        $("[clearResponses^='True'][questionid='" + questionId + "']").each(function () {
                            $(this).prop('checked', false);
                            $(this).trigger('change');
                        });
                    }
                }
            }
        );
    },

    'numeric-value-control': function () {
        $(this).on("keydown",
            function (e) {
                // if the min is not below 0, prevent negatives
                var minValue = $(this).attr("min");
                var allowNegatives = minValue && parseFloat(minValue) < 0;

                // prevent scenario where html numeric fields allow exponential characters or negative symbols in weird spots
                var isDash = e.key && e.key == '-';

                var isExponentialChar = e.key && (
                    e.key == '+' || 
                    e.key == 'e' || 
                    e.key == 'E'
                );

                // known edge case - val() will return an empty string if a html numeric input is invalid - it can get into
                //     this state here if two dashes are entered at the beginning of the input
                var value = $(this).val();

                // allow dash if value is empty, otherwise prevent & always prevent exp chars
                if (isExponentialChar || (isDash && !allowNegatives) || (isDash && value.length > 0)) {
                    e.preventDefault();
                    return false;
                }

                return true;
            });

        $(this).on("keyup focusout",
            function() {
                validateNumericInput(this, false);
            });

        $(this).on("change",
            function () {
                validateNumericInput(this, true);
            });
    }
};

function validateNumericInput(input, checkDecimal) {

    if (checkDecimal && $(input).attr("class").includes("decimalSpinner-control")) {
        var decimalSpots = $(input).attr("mantissa")
        var parseFloatValue = parseFloat($(input).val()).toFixed(decimalSpots);
        parseFloatValue = isNaN(parseFloatValue) ? null : parseFloatValue;
        $(input).val(parseFloatValue);
        var position = $(input).attr("position");
        $("#CorrectionApprovalDatas_" + position + "__NewDisplayValue")[0].value = parseFloatValue;
        $("#CorrectionApprovalDatas_" + position + "__NewDataPoint")[0].value = parseFloatValue;
    }

    var maximumValue = $(input).attr("max");
    var minimumValue = $(input).attr("min");
    var mantissa = $(input).attr("mantissa");

    var parentElement = $(input).parent();

    if (!parentElement) {
        return;
    }

    var isValid = true;
    const errorClassName = "has-error";

    var isSubjectNumberEditor = isDCFSubjectNumber(input);

    if (isSubjectNumberEditor) {
        isValid = validateSubjectNumberDCFAttribute(input, minimumValue, maximumValue);
    } else {
        isValid = validateNumericValueAttribute(input, minimumValue, maximumValue, mantissa);
    }

    if (isValid) {
        parentElement.removeClass(errorClassName);
    } else {
        parentElement.addClass(errorClassName);
    }
}

function isDCFNonconfigurableDate(control) {
    var isAttribute = $(control).parent() &&
        $(control).parent().hasClass("dcf-nonconfigurable-date");

    return isAttribute;
}

function isDCFQuestionnaireDate(control) {
    return $(control).hasClass("dcf-questionnaire-date") ||
        $(control).parent(".dcf-questionnaire-date")
}

function isDCF(control) {
    return control.className.includes("dcf") ||
        (control.parentNode && control.parentNode.className.includes("dcf"))
}

function isDCFSubjectNumber(control) {
    var isSubjectNumberEditor = $(control).parent() &&
        $(control).parent().parent() &&
        $(control).parent().parent().hasClass("SubjectNumberUpdate");

    return isSubjectNumberEditor;
}

function validateSubjectNumberDCFAttribute(control, minimumValue, maximumValue) {
    var isValid = true;

    var controlValue = $(control).val();

    var regex = new RegExp("^[0-9]*$");

    if (!regex.test(controlValue)) {
        isValid = false;
    } else if (controlValue !== "" && (maximumValue !== null || minimumValue !== null)) {
        var parsedValue = parseFloat(controlValue);

        if ((maximumValue !== null && (parsedValue > maximumValue || controlValue.length !== maximumValue.length)) 
            || (minimumValue !== null && parsedValue < minimumValue)) {
            isValid = false;
        }
    }

    return isValid;
}

function validateNumericValueAttribute(control, minimumValue, maximumValue, mantissa) {
    var isValid = true;  
    var regex;

    if (mantissa && mantissa > 0) {
        regex = new RegExp("^-?(\\d*(\\.\\d{" + mantissa + "})?)$");
    } else {
        regex = new RegExp("^\\d*$");
    }

    var controlValue = $(control).val();

    var questionType = control.getAttribute("questionType")
    if (questionType === undefined) {
        questionType = "";
    }

    if (maximumValue === undefined) {
        maximumValue = null;
    }

    if (minimumValue === undefined) {
        minimumValue = null;
    }   

    if (!regex.test(controlValue))
    {
        isValid = false;
    } else if (maximumValue !== null || minimumValue !== null) {
        var parsedValue = parseFloat(controlValue); 

        if ((maximumValue !== null && parsedValue > maximumValue) ||
            (minimumValue !== null && parsedValue < minimumValue))
        {
            isValid = false;
        }        
    }

    return isValid;
}

function LoadDirectives() {
    for (var directive in directiveConfiguration) {
        $("." + directive).each(directiveConfiguration[directive]);
    }

    initIsDirty();
    initNumericMaxLength();
    positiveInt();

    // make sure if any number input that is not valid (in the given range) remains red outlined until corrected.
    $(':input[type=number]').each(function () {
        validateNumericInput(this, true);
    });

    //prevent enter key from displaying the select options again
    $('select').on('keydown', function (event) {
        if (event.keyCode == enterKeyCode) {
            event.preventDefault();
            return false;
        }
    });

    //note only update the visible select dropdowns
    //vso #18961 - for wide text, use the .standard css class and use native dropdown - jo 30May2017
    //the other fix was too slow
    $("select:visible").not(".standard").not('[name*="_length"]').each(function() {
        CreateSelect(this);
    });

    initFormPostSpinner();

    if (typeof String.prototype.endsWith !== "function") {
        String.prototype.endsWith = function(suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }

    checkActiveTabs();
}