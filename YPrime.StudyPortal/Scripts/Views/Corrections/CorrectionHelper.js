/*********************************
 * CorrectionHelper.js
 * -------------------------------
 * Date:    29Jun2017
 * Author:  J Osifchin
 * Desc:    Javascript to help functionality for data changes
 ********************************/
var unchangedCss = "unchanged";
var placeholderCss = "placeholder";
var placeholderText = "Please provide a response";

function GetDataRowId(position, field) {
    return "CorrectionApprovalDatas_" + position + "__" + field;
}

function LoadChangeControlInitial(control, position) {
    control = typeof control == "object" ? $(control) : $("#" + control);
    var initialValue = $("#" + GetDataRowId(position, "NewDataPoint")).val();

    if (initialValue != null && initialValue.length > 0) {
        //set the control itself
        SetControlValue(control, initialValue, position);
    }

    if (ControlHasValue(control)) {
        $(control).closest("tr").removeClass(unchangedCss);
    } else {
        $(control).closest("tr").addClass(unchangedCss);
    }
}

function BindDataChangeControl(control, position) {
    control = typeof control == "object" ? $(control) : $("#" + control);

    function changeEventHandler() {
        SetCorrectionApprovalData($(this), position);
        CheckValidCorrection(); //check if display the next
    }

    $(control).bind("change", changeEventHandler);

    var classList = $(control)[0].className;

    // Text input should check for validity on keystroke
    if (classList && classList.includes("text-input")) {
        $(control).bind('keyup', changeEventHandler);
    }

    //Set placeholder text for controls involved in DCFs
    $(control).attr('placeholder', placeholderText);
}

function SetCorrectionApprovalData(control, position) {
    var val = GetControlValue(control);
    var displayValue = GetControlText(control);

    $("#" + GetDataRowId(position, "NewDataPoint")).val(val);
    $("#" + GetDataRowId(position, "NewDisplayValue")).val(displayValue == placeholderText ? "" : displayValue);

    var newCss = ControlHasValue(control) ? "" : unchangedCss;
    $(control).closest("tr").removeClass(unchangedCss).addClass(newCss);

    SetSelectCss(control, displayValue)
}

function SetSelectCss(control, displayValue)
{
    if ($(control).prop('tagName') == "SELECT") {
        if (displayValue == placeholderText) {
            $(control).parent().parent().children(".selected").addClass(placeholderCss); 
        }
        else {
            $(control).parent().parent().children(".selected").removeClass(placeholderCss);
        }
    }
}

function SetControlCss(obj) {
    var val = GetControlValue(obj); // $(control).val();
    if (val == null || val.length == 0) {
        $(obj).closest("tr").addClass(unchangedCss);
    } else {
        $(obj).closest("tr").removeClass(unchangedCss);
    }
}

function ControlHasValue(obj) {
    var val = GetControlValue(obj, true);
    return val?.length > 0;
}

function GetControlValue(obj, allValues) {
    var inputType = $(obj)[0].tagName;

    result = $(obj).val();
    switch (inputType) {
        case "INPUT":
            var type = $(obj).attr('type');
            switch (type) {
                case "checkbox":
                    if (allValues) {
                        result = [];
                        $(obj).closest('td').find('input:checkbox').each(function () {
                            if ($(this)[0].checked) {
                                result.push($(obj).attr('value'));
                            }
                        });

                if (result.length == 0) {
                    result = "";
                }
            } else {
                if ($(obj)[0].checked) {
                    result = $(obj).attr("value");
                } else {
                    result = "";
                }
            }

            break;
        }
        break;
    }

    return result;
}

function GetControlText(obj) {
    var inputType = $(obj)[0].tagName;
    var result = $(obj).text();
    //This is stubbed out in case we need to handle alternate control types generically
    //INPUT, SELECT, ...
    switch (inputType) {
        case "INPUT":
            result = $(obj).val();
            var type = $(obj).attr('type');
            switch (type) {
                case "radio":
                    result = $(obj).parent().text().trim();
                    break;
                case "checkbox":
                    result = $(obj)[0].checked ? $(obj).parent().find('label').text() : '';

            break;
        }
        break;
    case "SELECT":
        result = $(obj).find("option:selected").text();
        break;
    }

    return result;
}

function SetControlValue(obj, val, position) {
    var inputType = $(obj)[0].tagName;
    //This is stubbed out in case we need to handle alternate control types generically
    //INPUT, SELECT, ...
    switch (inputType) {
    case "INPUT":
        var len = $(obj).length;
        for (var i = 0; i < len; i++) {
            var input = $($(obj)[i]);
            var type = input.attr("type");
            switch (type) {
            case "radio":
                input.parent().removeClass("active");
                $(input).removeAttr("checked");

                if (input.val() == val) {
                    input.parent().addClass('active');
                    $(input).attr('checked');
                    if (input.length > 0) {
                        SetCorrectionApprovalData($(input[0]), position);
                    }
                }
                break;
            case "checkbox":
                if (input.val() == val) {
                    input[0].checked = true;
                    SetCorrectionApprovalData(input, position);
                }
                break;
                default:
                    val = val.replace('&amp;', '&');
                    input.val(val);
                    SetCorrectionApprovalData(input, position);
                    break;
                }
            }

        break;
    case "SELECT":
        $(obj).val(val);;
        RefreshSelect(obj);
        SetCorrectionApprovalData($(obj), position);
        break;
    }

}