var gHelper = (function (window, document) {
    // global variables
    var FORMTYPE_CREATE = 1;
    var FORMTYPE_EDIT = 2;
    var FORMTYPE_READONLY = 3;
    var FORMTYPE_DISABLED = 4;

    //executes callback method based on user role.
    //if user has role executes callback(true) adn so on
    function isInRole(globalContext, roleName, callback) {
        var userRolesIds = globalContext.userSettings.securityRoles;

        var filterTemplate = "roleid eq ";
        var filter = "";
        for (var i = 0; i < userRolesIds.length; i++)
            filter = filter + ((userRolesIds.length > 1 && i < (userRolesIds.length - 1))
                ? (filterTemplate + userRolesIds[i] + " or ") : (filterTemplate + userRolesIds[i]));

        var url = "$select=name";

        if (filter != null) {
            url += "&$filter=" + filter;
        }
        url += "&$orderby=name";

        Xrm.WebApi.retrieveMultipleRecords("role", url)
            .then(function (data) {
                if (data[roleName]) callback(true);
                else callback(false);
            })
            .fail(function (error) {
                var message = error.message;
                console.log("Error: " + message);
            });
    }


    function getAllRolesNames(globalContext, callback) {
        var userRolesIds = globalContext.userSettings.securityRoles;

        var filterTemplate = "roleid eq ";
        var filter = "";
        for (var i = 0; i < userRolesIds.length; i++)
            filter = filter + ((userRolesIds.length > 1 && i < (userRolesIds.length - 1))
                ? (filterTemplate + userRolesIds[i] + " or ") : (filterTemplate + userRolesIds[i]));

        var url = "$select=name";

        if (filter != null) {
            url += "&$filter=" + filter;
        }
        url += "&$orderby=name";

        Xrm.WebApi.retrieveMultipleRecords("role", url)
            .then(function (data) {
                if (!data || !data.entities) callback(null);

                var result = new Array();
                for (var i = 0; i < data.entities.length; i++) {
                    result[i] = data.entities[i].name;
                }

                callback(result);
            })
            .fail(function (error) {
                var message = error.message;
                console.log("Error: " + message);
            });
    }


    function hasRole(rolesNames, roleName) {
        return rolesNames.indexOf(roleName) > -1;
    }


    function webApiOutputParse(collection, propertiesArray) {
        var prop = [];

        collection.forEach(function (row, i) {
            propertiesArray.forEach(function (p) {
                var f = p + "@OData.Community.Display.V1.FormattedValue";
                prop.push((row[f] ? row[f] : row[p])); // Get formatted value if one exists for this property.  
            });
        });

        return prop;
    }


    /****************************************************************************************
    Service methods
    ****************************************************************************************/

    function getGlobalContexNoUtility() {
        var context;
        // GetGlobalContext defined by including reference to   
        // ClientGlobalContext.js.aspx in the HTML page.  
        if (typeof GetGlobalContext != "undefined")
        { context = GetGlobalContext(); }
        else
        {
            if (typeof Xrm != "undefined") {
                // Xrm.Page.context defined within the Xrm.Page object model for form scripts.  
                context = Xrm.Utility ? gHelper.getGlobalContext() : Xrm.Page.context;
            }
            else { throw new Error("Context is not available."); }
        }
        return context;
    }

    function getGlobalContext() {
        return Xrm.Utility.getGlobalContext();
    }

    function getFormContext(executionContext) {
        return executionContext.getFormContext();
    }

    function GetFormType(formContext) {
        if (formContext != null)
            return formContext.ui.getFormType();
        else {
            console.log("formContext is null");
            return null;
        }
    }

    function GetFormName(formContext) {
        var fs = formContext.ui.formSelector;
        if (fs != null) {
            return fs.getCurrentItem().getLabel();
        }
        else { return null; }
    }

    function SwitchFormByName(formContext, formName) {
        if (formContext.ui.formSelector.getCurrentItem().getLabel() != formName) {
            var items = formContext.ui.formSelector.items.get();
            for (var i in items) {
                var item = items[i];
                var itemId = item.getId();
                var itemLabel = item.getLabel();
                if (itemLabel == formName) {
                    SwitchFormById(formContext, itemId);
                }
            }
        }
    }

    function SwitchFormById(formContext, formId) {
        formContext.ui.formSelector.items.get(formId).navigate();
    }

    /****************************************************************************************
    VALUES
    ****************************************************************************************/

    function GetValue(formContext, attr) {
        var result;
        var val = formContext.getAttribute(attr).getValue();
        if (val == null) {
            result = "";
        }
        else {
            if (val.toString().length > 0) {
                result = val;
            }
            else {
                result = "";
            }
        }
        return result;
    }

    function GetOptionsetText(formContext, attr) {
        var val = formContext.getAttribute(attr).getText();
        return val ? val : "";
    }

    function GetLookupAttrId(formContext, attr) {
        var lu = formContext.getAttribute(attr);
        if (lu != null) {
            if (lu.getValue() != null && lu.getValue().length > 0) {
                var luValue = lu.getValue();
                if (luValue != null) {
                    return luValue[0].id;
                }
            }
            else {
                return null;
            }
        }
        return null;
    }

    function GetLookupName(formContext, attr) {
        var lu = formContext.getAttribute(attr);
        if (lu != null) {
            if (lu.getValue() != null && lu.getValue().length > 0) {
                var luValue = lu.getValue();
                if (luValue != null) {
                    return luValue[0].name;
                }
            }
            else {
                return "";
            }
        }
        return "";
    }

    function GetLookupEntityType(formContext, attr) {
        var lu = formContext.getAttribute(attr);
        if (lu != null) {
            var luValue = lu.getValue();
            if (luValue != null) {
                return luValue[0].entityType;
            }
        }
        return null;
    }

    function SetValue(formContext, attr, val) {
        formContext.getAttribute(attr).setValue(val);
        formContext.getAttribute(attr).setSubmitMode("always");
    }

    function SetLookup(formContext, attr, entitytype, id, name) {
        var setLookupValue = new Array();
        setLookupValue[0] = new Object();
        setLookupValue[0].id = id;
        setLookupValue[0].entityType = entitytype;
        setLookupValue[0].name = name;
        formContext.getAttribute(attr).setValue(setLookupValue);
    }

    function SetOptionsetByText(formContext, attr, text) {
        var options = formContext.getAttribute(attr).getOptions();
        for (var i = 0; i < options.length; i++) {
            if (options[i].text == text) {
                formContext.getAttribute(attr).setValue(options[i].value);
            }
        }
    }

    function SetRequiredLevel(formContext, attr, required) {
        if (required) {
            formContext.getAttribute(attr).setRequiredLevel("required");
        }
        else {
            formContext.getAttribute(attr).setRequiredLevel("none");
        }
    }

    function isAttributeRequired(formContext, attr) {
        return formContext.getAttribute(attr).getRequiredLevel() == 'required';
    }

    function ResetField(formContext, attr) {
        var attribute = formContext.getAttribute(attr);
        var attributeType = formContext.getAttribute(attr).getAttributeType();

        switch (attributeType) {
            case "boolean":
                attribute.setValue(false);
                break;

            case "datetime":
                attribute.setValue(null);
                break;

            case "decimal":
                attribute.setValue(0);
                break;

            case "money":
                break;

            case "double":
                attribute.setValue(0);
                break;

            case "integer":
                attribute.setValue(0);
                break;

            case "lookup":
                attribute.setValue([]);
                break;

            case "memo":
                attribute.setValue("");
                break;

            case "string":
                attribute.setValue("");
                break;

            case "optionset":
                attribute.setValue(0);
                break;
        }
    }

    /****************************************************************************************
    VISIBILITY
    ****************************************************************************************/

    function SetControlVisibility(formContext, controlname, visible) {
        if (formContext.getControl(controlname)) {
            formContext.getControl(controlname).setVisible(visible);
        }
    }

    function SetSectionVisibility(formContext, tabname, sectionname, visible) {
        var tab = formContext.ui.tabs.get(tabname);
        var section = tab.sections.get(sectionname);
        section.setVisible(visible);
    }

    function SetTabVisibility(formContext, tabname, visible) {
        formContext.ui.tabs.get(tabname).setVisible(visible);
    }

    function SetTabDisplayState(formContext, tabname, expand) {
        var tab = formContext.ui.tabs.get(tabname);
        if (expand == true) {
            tab.setDisplayState("expanded");
        }
        else {
            tab.setDisplayState("collapsed");
        }
    }

    function SetControlsSectionVisibility(ctrl, visible) {
        if (ctrl != null) { ctrl.getParent().setVisible(visible); }
    }

    /****************************************************************************************
    MISCELLANEOUS
    ****************************************************************************************/

    function SetDisabled(formContext, attr, disabled) {
        formContext.getControl(attr).setDisabled(disabled);
    }

    function GetCurrentUserId() {
        return getGlobalContext().userSettings.userId;
    }

    function GetCurrentUserName() {
        return getGlobalContext().userSettings.userName;
    }

    function RemoveOptionSetOption(formContext, attr, optionSetValue) {
        var current = formContext.getControl(attr);
        if (current)
            formContext.getControl(attr).removeOption(optionSetValue);
    }

    function GetCurrentRecordId(formContext) {
        return formContext.data.entity.getId();
    }

    function GetCurrentRecordName(formContext) {
        return formContext.data.entity.getPrimaryAttributeValue();
    }

    function setNotificationWithPhoneNumberFormat(executionContext) {
        var phoneNumber = executionContext.getEventSource();
        if (typeof (phoneNumber) != "undefined") {
            var formatedNumber = FormatPhoneNumber2(phoneNumber.getValue());
            var formContext = executionContext.getFormContext();
            var attributeName = phoneNumber.getName();
            if (formatedNumber.length >= 10)  // fix for auto adjust
                phoneNumber.setValue(formatedNumber);
            if (!checkPhoneNumber(formatedNumber)) {
                formContext.getControl(attributeName).setNotification("Phone field requires 10 digit number", attributeName + "_ID");
            }
            else formContext.getControl(attributeName).clearNotification(attributeName + "_ID");
        }
    }


    function FormatPhoneNumber(executionContext) {
        var phoneNumber = executionContext.getEventSource();
        if (typeof (phoneNumber) != "undefined") {
            phoneNumber.setValue(FormatPhoneNumber2(phoneNumber.getValue()));
            var formContext = executionContext.getFormContext();
            var attributeName = phoneNumber.getName();

            if (!IsPhoneNumberValid(formContext, attributeName)) {
                formContext.getControl(attributeName).setNotification("Phone field requires 10 digit number", attributeName + "_ID");
            }
            else formContext.getControl(attributeName).clearNotification(attributeName + "_ID");
        }
    }

    function FormatPhoneNumberByAttr(formContext, attr) {
        var phoneNumber = GetValue(formContext, attr);
        if (!checkPhoneNumber(phoneNumber)) {
            SetValue(formContext, attr, FormatPhoneNumber2(phoneNumber));
        }
    }

    function FormatPhoneNumber2(val) {
        if (val != null && val != "") {
            var sTmp = val.replace(/[^0-9]/g, "");
            if (sTmp.length == 10) {
                return sTmp.substr(0, 3) + "-" + sTmp.substr(3, 3) + "-" + sTmp.substr(6, 4);
            }
            else {
                return val;
            }
        }
        return null;
    }

    function IsPhoneNumberValid(formContext, attr) {
        var phone = GetValue(formContext, attr);
        return checkPhoneNumber(phone);
    }

    function checkPhoneNumber(phone) {
        if (phone != null && phone != "") {
            //a) Validate all characters are numeric or "-"
            var regex = /^[0123456789-]/g;
            if (!regex.test(phone)) {
                return false;
            }
            //b) validate the numbers are 10
            if (phone.replace(/[^0-9]/g, "").length != 10) {
                return false;
            }
            //c) Ensure proper phone format xxx-xxx-xxxx is entered. if user enters phone in another format can it be allowed and reformatted before submission. 
            //don't really need to bug the user and ask him to enter in the format above, as long as we can reformat it before saving to ensure data consistency.
            if (phone.indexOf("-") !== -1) {
                var regex2 = /^\(?([0-9]{3})\)?[-]?([0-9]{3})[-]?([0-9]{4})$/;
                if (!phone.match(regex2)) {
                    return false;
                }
            }
        }
        return true;
    }

    function canadienPostalCodeValidation(postalCode) {
        var caRegex = new RegExp(/^[ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ]( )?\d[ABCEGHJKLMNPRSTVWXYZ]\d$/i);
        if (caRegex.test(postalCode)) return true;
        else return false;
    }

    function usPostalCodeValidation(postalCode) {
        var caRegex = new RegExp(/^(?!0{5})(\d{5})(?!-?0{4})(|-\d{4})?$/);
        if (caRegex.test(postalCode)) return true;
        else return false;
    }





    function postalCodeValidation(formContext, postalCodeAttr, countryAttr, showAlert, showFieldNotification) {
        var postalCode = GetValue(formContext, postalCodeAttr);
        var country = GetValue(formContext, countryAttr);

        if (!postalCode || postalCode == undefined || postalCode == '') {
            //release notification
            formContext.getControl(postalCodeAttr).clearNotification(postalCodeAttr + "_ID");
            return true;
        }


        //validate postal code
        if (country.match("^CAN") || country.match("^can") || country.match("^Can")) {
            if (!canadienPostalCodeValidation(postalCode)) {
                if (showAlert) Xrm.Navigation.openAlertDialog(gHelper.alertDialogText("The provided Postal Code is not a valid Canadian Postal Code. Postal code format requires 6 symbols with space in the middle: XXX XXX"));
                //sets field level notification
                if (showFieldNotification) formContext.getControl(postalCodeAttr).setNotification("The provided Postal Code is not a valid Canadian Postal Code", postalCodeAttr + "_ID");
                return false;
            }
                //format Canadian postal code if there is no space
            else {
                if (postalCode.length != 7 || postalCode.indexOf(' ') != 3) {
                    var pcFormated = postalCode.substr(0, 3) + " " + postalCode.substr(3, 3);
                    SetValue(formContext, postalCodeAttr, pcFormated);
                }
            }
        }

        if (country.match("^US") || country.match("^us")) {
            var pcFormated = postalCode;
            //format US postal code if there is no space
            if (postalCode.length == 9 && postalCode.indexOf('-') == -1) {
                pcFormated = postalCode.substr(0, 5) + "-" + postalCode.substr(5, 4);
                SetValue(formContext, postalCodeAttr, pcFormated);
            }

            if (!usPostalCodeValidation(pcFormated)) {
                if (showAlert) Xrm.Navigation.openAlertDialog(gHelper.alertDialogText("The provided Zip Code is not a valid USA Zip Code. Zip code format requires 5 digits (xxxxx) or 9 digits with dash (xxxxx-xxxx)"));
                //set field notification
                if (showFieldNotification) formContext.getControl(postalCodeAttr).setNotification("The provided Zip Code is not a valid USA Zip Code", postalCodeAttr + "_ID");
                return false;
            }
        }


        //release notification
        formContext.getControl(postalCodeAttr).clearNotification(postalCodeAttr + "_ID");
        return true;
    }

    function PhoneValidation(formContext, phone, warning_message) {
        gHelper.FormatPhoneNumberByAttr(formContext, phone);
        if (!gHelper.IsPhoneNumberValid(formContext, phone)) {
            Xrm.Navigation.openAlertDialog(gHelper.alertDialogText(warning_message));
            return false;
        }
        return true;
    }


    function alertDialogText(messeageText, confirmaionText) {
        var mt = (messeageText != null && messeageText != undefined && messeageText.length > 0) ? messeageText : "A message from form";
        var cbl = (confirmaionText != null && confirmaionText != undefined && confirmaionText.length > 0) ? confirmaionText : "OK";
        return { confirmButtonLabel: cbl, text: mt };
    }

    //accepts two integers each must be > 100
    //if empty =>default h: 150, w: 350
    function alertDialogWindow(windowHeigth, windowWidth) {
        try {
            var wh = parseInt(windowHeigth) > 100 ? windowHeigth : 150;
            var ww = parseInt(windowWidth) > 100 ? windowWidth : 350;
            return { height: wh, width: ww };

        } catch (e) {
            return { height: 150, width: 350 };
        }
    }

    //Public  properties and methods
    return {

        FORMTYPE_CREATE: FORMTYPE_CREATE,
        FORMTYPE_EDIT: FORMTYPE_EDIT,
        FORMTYPE_READONLY: FORMTYPE_READONLY,
        FORMTYPE_DISABLED: FORMTYPE_DISABLED,

        alertDialogText: alertDialogText,
        alertDialogWindow: alertDialogWindow,

        IsPhoneNumberValid: IsPhoneNumberValid,
        getGlobalContexNoUtility: getGlobalContexNoUtility,
        getGlobalContext: getGlobalContext,
        GetFormName: GetFormName,
        GetFormType: GetFormType,
        GetLookupAttrId: GetLookupAttrId,
        GetLookupName: GetLookupName,
        GetOptionsetText: GetOptionsetText,
        GetValue: GetValue,
        getAllRolesNames: getAllRolesNames,
        FormatPhoneNumber: FormatPhoneNumber,
        setNotificationWithPhoneNumberFormat: setNotificationWithPhoneNumberFormat,
        FormatPhoneNumberByAttr: FormatPhoneNumberByAttr,
        ResetField: ResetField,
        SetTabVisibility: SetTabVisibility,
        SetValue: SetValue,
        SetDisabled: SetDisabled,
        SetSectionVisibility: SetSectionVisibility,
        SetControlVisibility: SetControlVisibility,
        isAttributeRequired: isAttributeRequired,
        SetRequiredLevel: SetRequiredLevel,
        hasRole: hasRole,
        SwitchFormByName: SwitchFormByName,
        RemoveOptionSetOption: RemoveOptionSetOption,
        SetLookup: SetLookup,
        webApiOutputParse: webApiOutputParse,
        GetCurrentUserId: GetCurrentUserId,
        GetCurrentUserName: GetCurrentUserName,
        PhoneValidation: PhoneValidation,
        postalCodeValidation: postalCodeValidation
    };

})(window, document);