// ===============================================================================
if (typeof ($) === 'undefined') {

    $ = parent.$;

    jQuery = parent.jQuery;

}

var PlusAddress = {};

// ===============================================================================

PlusAddress.Form_OnLoad = function () {

    Xrm.Page.getControl("address1_composite_compositionLinkControl_address1_country").setVisible(false);

};

// ===============================================================================




// ===============================================================================
PlusAddress.Country_OnChange = function () {


    var lookupObject = Xrm.Page.getAttribute("tsp_country");
    var _CountryName;

    if (lookupObject != null) {
        var lookUpObjectValue = lookupObject.getValue();
        if ((lookUpObjectValue != null)) {

            var _CountryName = lookUpObjectValue[0].name;
            var lookupid = lookUpObjectValue[0].id;
            //set to address1 field

            Common_SetAttributeValue("address1_country", _CountryName);


            //    window.parent.document.getElementById("address1_composite").click()
            //  window.parent.document.getElementsByClassName("ui-widget-overlay-flyout")[0].click()

        }
    }

}

// ===============================================================================

PlusAddress.ValidateNumber = function (context) {
    try {
        var fieldName = context.getEventSource().getName();
        if (!fieldName) return;

        var phoneNumber = Common_GetAttributeValue(fieldName);
        if (phoneNumber) {
            var tendigit = /^[0-9]{10}$/;
            if (phoneNumber.match(tendigit)) {
                var formatedNumber = "(" + phoneNumber.substr(0, 3) + ") " + phoneNumber.substr(3, 3) + "-" + phoneNumber.substr(6, 4);
                Common_SetAttributeValue(fieldName, formatedNumber);
                Common_ClearControlNotification(fieldName);
            }
            else {
                var phoneno = /^\([0-9]{3}\)\s[0-9]{3}-[0-9]{4}$/;
                if (!phoneNumber.match(phoneno)) {
                    Common_SetControlNotification(fieldName, "Invalid format. Please use xxxxxxxxxx Or (xxx) xxx-xxxx.");
                }
                else {
                    Common_ClearControlNotification(fieldName);
                }
            }
        }
    }
    catch (err) {
        return;
    }
};

// ===============================================================================

PlusAddress.ValidatePostalZip = function () {
    try {
        var postalZipControl = "address1_composite_compositionLinkControl_address1_postalcode";
        var postalZip = Common_GetControlValue(postalZipControl);
        Common_ClearControlNotification(postalZipControl);
        if (!postalZip) return;

        var validationRegEx;
        var errMsg;
        var country = Common_GetAttributeValue("address1_country");
        switch (country) {
            case "Canada":
                validationRegEx = /^[A-Z]{1}[0-9]{1}[A-Z]{1}\s[0-9]{1}[A-Z]{1}[0-9]{1}$/;
                errMsg = "Invalid postal code format. Please use H0H 0H0."
                break;
            case "United States":
                validationRegEx = /^[0-9]{5}$/;
                errMsg = "Invalid zip code format. Please use 00000."
                break;
        }

        if (!postalZip.match(validationRegEx)) {
            Common_SetControlNotification(postalZipControl, errMsg);
        }
    }
    catch (err) {
        return;
    }
};

// ===============================================================================