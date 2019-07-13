/// <reference path="../../Utility/GlobalHelper.js" />

var AccountSpireScript = (function (window, document) {
    /****************************************************************************************
    VARIABLES
    ****************************************************************************************/

    /****************************************************************************************
    PRIVATE FUNCTIONS
    ****************************************************************************************/


    /****************************************************************************************
    PUBLIC METHODS
    ****************************************************************************************/

    return {


        /****************************************************************************************
        FIELD EVENTS
        ****************************************************************************************/

        FormatPhoneNumber: function (executionContext) {
            gHelper.setNotificationWithPhoneNumberFormat(executionContext);
        },
        SetCountryCode: function (executionContext) {

            var formContext = executionContext.getFormContext();

            var countryName = executionContext.getEventSource().getValue();
            if (countryName != null && typeof (countryName) != undefined) {


                Xrm.WebApi.online.retrieveMultipleRecords("tsp_country", "?$select=tsp_code&$filter=tsp_name eq '" + countryName + "'&$top=1").then(
                  function success(result) {
                      for (var i = 0; i < result.entities.length; i++) {
                          var tsp_code = result.entities[i]["tsp_code"];
                      }
                      gHelper.SetValue(formContext, "address1_country", tsp_code)
                  },
                  function (error) {
                      console.log(error.message);
                  }
                );
            }
        },


        //SetCountryCode: function (executionContext) {

        //    var formContext = executionContext.getFormContext();

        //    var countryLookup = executionContext.getEventSource().getValue();
        //    if (countryLookup != null && typeof (countryLookup) != undefined) {
        //        countryid = countryLookup[0].id;

        //        Xrm.WebApi.retrieveRecord("tsp_country", countryid, "?$select=tsp_code").then(
        //          function success(result) {
        //              gHelper.SetValue(formContext, "address1_country", result.tsp_code)
        //          },
        //          function (error) {
        //              console.log(error.message);
        //          }
        //        );
        //    }


        //},

        /****************************************************************************************
        PAGE EVENTS
        ****************************************************************************************/

        OnLoad: function (executionContext) {
            formContext = executionContext.getFormContext();
            var formType = gHelper.GetFormType(formContext);

            //
            gHelper.SetControlVisibility(formContext, "address1_country", false);
            ////add events
            formContext.getAttribute("tsp_countrycode").addOnChange(AccountSpireScript.SetCountryCode);
            //formContext.getAttribute("telephone1").addOnChange(AccountSpireScript.FormatPhoneNumber);
            //formContext.getAttribute("fax").addOnChange(AccountSpireScript.FormatPhoneNumber);


            //formContext.getAttribute("address1_country").setSubmitMode("always");
        },

        OnSave: function (executionContext) {
            var formContext = executionContext.getFormContext();


            //validate postal code  
            //if (formContext.getAttribute("address1_postalcode").getIsDirty()) {
                //if (!gHelper.postalCodeValidation(formContext,"address1_postalcode", "address1_country", true, false)) {
                //    executionContext.getEventArgs().preventDefault();
                //    return false;
                //}
            //}
        }
    };

})(window, document);

