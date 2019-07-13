
/// <reference path="../../Utility/GlobalHelper.js" />

var AddressFormScript = (function (window, document) {
    /****************************************************************************************
    VARIABLES
    ****************************************************************************************/

    /****************************************************************************************
    PRIVATE FUNCTIONS
    ****************************************************************************************/



    return {


        /****************************************************************************************
        FIELD EVENTS
        ****************************************************************************************/
        SetCountryCode: function (executionContext) {

            var formContext = executionContext.getFormContext();

            var countryName = executionContext.getEventSource().getValue();
            if (countryName != null && typeof (countryName) != undefined) {
                

                Xrm.WebApi.online.retrieveMultipleRecords("tsp_country", "?$select=tsp_code&$filter=tsp_name eq '" + countryName + "'&$top=1").then(
                  function success(result) {
                      for (var i = 0; i < result.entities.length; i++) {
                          var tsp_code = result.entities[i]["tsp_code"];
                      }
                      gHelper.SetValue(formContext, "tsp_countrycode", tsp_code)
                  },
                  function (error) {
                      console.log(error.message);
                  }
                );
            }
        },

        /****************************************************************************************
        PAGE EVENTS
        ****************************************************************************************/

        OnLoad: function (executionContext) {
            formContext = executionContext.getFormContext();

            formContext.getAttribute("country").addOnChange(AddressFormScript.SetCountryCode);

            formContext.getAttribute("tsp_countrycode").setSubmitMode("always");

        },

        OnSave: function (executionContext) {

            var formContext = executionContext.getFormContext();
            

        }
    };

})(window, document);




