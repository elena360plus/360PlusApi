//tsp_common.js [20181219]
// ===============================================================================

var RequirementLevel_None = "none";
var RequirementLevel_Required = "required";
var RequirementLevel_Recommended = "recommended";

var TabDisplayState_Expanded = "expanded";
var TabDisplayState_Collapsed = "collapsed";

var HeaderPosition_1 = ".ms-crm-HeaderTileElement.ms-crm-HeaderTileElementPosition0";
var HeaderPosition_2 = ".ms-crm-HeaderTileElement.ms-crm-HeaderTileElementPosition1";
var HeaderPosition_3 = ".ms-crm-HeaderTileElement.ms-crm-HeaderTileElementPosition2";
var HeaderPosition_4 = ".ms-crm-HeaderTileElement.ms-crm-HeaderTileElementPosition3";

var FormType_Undefined = 0;
var FormType_Create = 1;
var FormType_Update = 2;
var FormType_ReadOnly = 3;
var FormType_Disabled = 4;
var FormType_QuickCreate = 5;
var FormType_BulkEdit = 6;
var FormType_ReadOptimized = 11;

var Action_Application_CreateReview = 175660000;
var Action_Application_CreateProcessingTask = 175660001;
var Action_Application_CreateProcessingRequirement = 175660002;
var Action_Application_CreateApproval = 175660003;

var PersonalInformatAlertMessage = "Personal information entered in this record must have been collected in compliance with FIPPA and for the purpose(s) authorized by the statute and identified in the associated notice of collection.  Furthermore, the personal information may only be used for the purpose(s) authorized by the statute and identified in the associated notice of collection.";
// ===============================================================================

function Common_Load() { }

// ===============================================================================

function Common_GetAttributeValue(attributeName) {
    var attribute = Xrm.Page.getAttribute(attributeName);
    if (attribute != null) {
        return attribute.getValue();
    }
}

// ===============================================================================

function Common_SetAttributeValue(attributeName, attributeValue) {
    var attribute = Xrm.Page.getAttribute(attributeName);
    if (attribute != null) {
        attribute.setValue(attributeValue);
    }
}

// ===============================================================================

function Common_GetControlValue(contorlName) {
    var control = Xrm.Page.getControl(contorlName);
    if (control != null) {
        return control.getAttribute().getValue();
    }
}

// ===============================================================================

function Common_SetLookupAttributeValue(attributeName, attributeValue) {
    if (attributeValue != null) {
        var lookupAttribute = new Array();
        lookupAttribute[0] = new Object();
        lookupAttribute[0].id = attributeValue.Id;
        lookupAttribute[0].name = attributeValue.Name;
        lookupAttribute[0].entityType = attributeValue.LogicalName;

        Common_SetAttributeValue(attributeName, lookupAttribute);
    }
}

// ===============================================================================

function Common_EnableAllControls() {
    var controls = Xrm.Page.ui.controls.get();
    for (var i in controls) {
        var control = controls[i];
        try { control.setDisabled(false); } catch (err) { }
    }
}

// ===============================================================================

function Common_DisableAllControls() {
    var controls = Xrm.Page.ui.controls.get();
    for (var i in controls) {
        var control = controls[i];
        try { control.setDisabled(true); } catch (err) { }
    }
}

// ===============================================================================

function Common_EnableControl(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setDisabled(false);
    }
}

// ===============================================================================

function Common_DisableControl(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setDisabled(true);
    }
}

// ===============================================================================

function Common_HideTab(tabName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab != null) {
        tab.setVisible(false);
    }
}

// ===============================================================================

function Common_ShowTab(tabName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab != null) {
        tab.setVisible(true);
    }
}

// ===============================================================================

function Common_HideSection(tabName, sectionName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab == null) return;
    var section = tab.sections.get(sectionName);
    if (section == null) return;
    if (section != null) {
        section.setVisible(false);
    }
}

// ===============================================================================

function Common_ShowSection(tabName, sectionName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab == null) return;
    var section = tab.sections.get(sectionName);
    if (section == null) return;
    if (section != null) {
        section.setVisible(true);
    }
}

// ===============================================================================

function Common_HideControl(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setVisible(false);
    }
}

// ===============================================================================

function Common_ShowControl(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setVisible(true);
    }
}

// ===============================================================================

function Common_SetRequiredLevel(controlName, requirementLevel) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setRequiredLevel(requirementLevel);
    }
}

// ===============================================================================

function Common_ClearControlNotification(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).clearNotification();
    }
}

// ===============================================================================

function Common_SetControlNotification(controlName, message) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setNotification(message);
    }
}

// ===============================================================================

function Common_RefreshSubGrid(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        field.refresh();
    }
}

// ===============================================================================

function SetHeaderFieldWidth(postionName, width) {
    // NOTE: This is unsupported JavaScript in CRM 2013.
    $(document).ready(function () {
        if ($(postionName).length) $(postionName).width(width);
    });
    $(window).resize(function () {
        if ($(postionName).length) $(postionName).width(width);
    });
}

// ===============================================================================

function Common_GetFormType() {
    return Xrm.Page.ui.getFormType();
}

// ===============================================================================

function Common_SaveForm() {
    Xrm.Page.data.entity.save();
}

// ===============================================================================

function Common_SaveAndCloseForm() {
    Xrm.Page.data.entity.save("saveandclose");
}

// ===============================================================================

function Common_RetrieveCrmRecord(query, callBackFunction) {

    var oDataSelect = Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/" + query;

    var retrieveReq = new XMLHttpRequest();
    retrieveReq.open("GET", oDataSelect, false);
    retrieveReq.setRequestHeader("Accept", "application/json");
    retrieveReq.setRequestHeader("Content-Type", "application/json;charset=utf-8");
    retrieveReq.onreadystatechange = function () {
        if (retrieveReq.readyState == 4) {
            if (retrieveReq.status == 200) {
                var retrievedRecord = JSON.parse(retrieveReq.responseText).d.results[0];
                callBackFunction(retrievedRecord);
            }
        }
    };
    retrieveReq.send();
}

// ===============================================================================

function Common_GetDateFromJson(jsonDate) {

    try {
        if (jsonDate == undefined) {
            return "";
        }
        var utcTime = parseInt(jsonDate.substr(6));

        var date = new Date(utcTime);
        var minutesOffset = date.getTimezoneOffset();

        var dateWithOffset = new Date(date.getTime() + minutesOffset * 60000);
    }
    catch (exception) {
        alert(exception);
    }

    return dateWithOffset;
}

// ===============================================================================

function Common_ExpandTab(tabName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab != null) {
        tab.setDisplayState(TabDisplayState_Expanded);
    }
}

// ===============================================================================

function Common_CollapseTab(tabName) {
    var tab = Xrm.Page.ui.tabs.get(tabName);
    if (tab != null) {
        tab.setDisplayState(TabDisplayState_Collapsed);
    }
}

// ===============================================================================

function Common_GetCurrencyCode(key) {
    var value = "";
    var query = "TransactionCurrencySet?$filter=TransactionCurrencyId eq guid'" + key + "'";
    Common_RetrieveCrmRecord(query, function (currency) {
        value = currency.ISOCurrencyCode;
    });

    return value;
}

function Common_GetConfigValue(key) {
    var value = "";
    var query = "tsp_configurationSet?$filter=tsp_Key eq '" + key + "'";
    Common_RetrieveCrmRecord(query, function (configuration) {
        value = configuration.tsp_Value;
    });

    return value;
}

function Common_GetTerritoryName(key) {
    var value = "";
    var query = "TerritorySet?$filter=TerritoryId eq guid'" + key + "'";
    Common_RetrieveCrmRecord(query, function (territory) {
        value = territory.Name;
    });

    return value;
}

function Common_GetWarehouseCode(key) {
    var value = "";
    var query = "tsp_warehouseSet?$filter=tsp_warehouseid eq guid'" + key + "'";
    Common_RetrieveCrmRecord(query, function (warehouse) {
        value = warehouse.tsp_name;
    });

    return value;
}

// ===============================================================================

function Common_RefreshWebResource(webResourceName) {
    var webResource = Xrm.Page.ui.controls.get(webResourceName);
    if (webResource != null) {
        webResource.setSrc(webResource.getSrc());
    }
}

// ===============================================================================

function Common_RefreshSubGrid(subGridName) {
    var subGrid = Xrm.Page.ui.controls.get(subGridName);
    if (subGrid != null) {
        subGrid.refresh();
    }
}

// ===============================================================================

function Common_FocusControl(controlName) {
    var field = Xrm.Page.ui.controls.get(controlName);
    if (field != null) {
        Xrm.Page.getControl(controlName).setFocus();
    }
}

// ===============================================================================

function Common_GetParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

// ===============================================================================

function Common_CheckUserRole(roleName) {
    var currentUserRoles = Xrm.Page.context.getUserRoles();
    for (var i = 0; i < currentUserRoles.length; i++) {
        var userRoleId = currentUserRoles[i];
        var userRoleName = Common_GetRoleName(userRoleId);
        if (userRoleName == roleName) {
            return true;
        }
    }
    return false;
}

function Common_GetRoleName(userRoleId) {
    var odataSelect = Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/RoleSet?$top=1&$filter=RoleId eq guid'" + userRoleId + "'&$select=Name";
    //alert(odataSelect);
    var roleName = null;
    $.ajax({
        type: "GET",
        async: false,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: odataSelect,
        beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
        success: function (data, textStatus, XmlHttpRequest) {
            var result = data.d;
            if (!!result) {
                roleName = result.results[0].Name;
            }
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            //alert('OData Select Failed: ' + odataSelect);
        }
    });
    return roleName;
}

// ===============================================================================
function Common_SetPHUAccountbyUser(contactId, fieldName) {
    var oDataSelect = Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ContactSet?$filter=ContactId eq guid'" + contactId + "'";

    var retrieveReq = new XMLHttpRequest();
    retrieveReq.open("GET", oDataSelect, false);
    retrieveReq.setRequestHeader("Accept", "application/json");
    retrieveReq.setRequestHeader("Content-Type", "application/json;charset=utf-8");
    retrieveReq.onreadystatechange = function () {
        if (retrieveReq.readyState == 4) {
            if (retrieveReq.status == 200) {
                var retrieved = JSON.parse(retrieveReq.responseText).d;
                //  var setCustomer = retrieveReq.parentcustomerid;
                //set the value of the incident company to the field “parentcustomerid”
                //   Xrm.Page.getAttribute('parentcustomerid').setValue(setCustomer);
                var account = new Array();
                account[0] = new Object();
                account[0].id = retrieved.results[0].ParentCustomerId.Id;
                account[0].name = retrieved.results[0].ParentCustomerId.Name;
                account[0].entityType = retrieved.results[0].ParentCustomerId.LogicalName;
                //var LocCode = retrieved.results[0].acc_locationcode;

                Common_SetAttributeValue(fieldName, account);
                //Common_SetAttributeValue("acc_phulocationcode", LocCode);


            }
        }

    };
    retrieveReq.send();

}

function Common_SetPHULocationCodebyUser(contactId, fieldName) {

    var oDataSelect = Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ContactSet?$filter=ContactId eq guid'" + contactId + "'";

    var retrieveReq = new XMLHttpRequest();
    retrieveReq.open("GET", oDataSelect, false);
    retrieveReq.setRequestHeader("Accept", "application/json");
    retrieveReq.setRequestHeader("Content-Type", "application/json;charset=utf-8");
    retrieveReq.onreadystatechange = function () {
        if (retrieveReq.readyState == 4) {
            if (retrieveReq.status == 200) {
                var retrieved = JSON.parse(retrieveReq.responseText).d;
                var LocCode = retrieved.results[0].acc_LocationCode;

                // Common_SetAttributeValue("acc_account", account);
                Common_SetAttributeValue(fieldName, LocCode);


            }
        }

    };

    retrieveReq.send();


}

function Common_SetPHULocationCodebyAccount(accountId, fieldName) {

    var oDataSelect = Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/AccountSet?$filter=AccountId eq guid'" + accountId + "'";

    var retrieveReq = new XMLHttpRequest();
    retrieveReq.open("GET", oDataSelect, false);
    retrieveReq.setRequestHeader("Accept", "application/json");
    retrieveReq.setRequestHeader("Content-Type", "application/json;charset=utf-8");
    retrieveReq.onreadystatechange = function () {
        if (retrieveReq.readyState == 4) {
            if (retrieveReq.status == 200) {
                var retrieved = JSON.parse(retrieveReq.responseText).d;
                var LocCode = retrieved.results[0].accountnumber;

                // Common_SetAttributeValue("acc_account", account);
                Common_SetAttributeValue(fieldName, LocCode);


            }
        }

    };

    retrieveReq.send();

}
