var privateServices;
var g_PrivateServices;

function pageLoad() {
  var oldInvoke = Sys.Net.WebServiceProxy.invoke;
  Sys.Net.WebServiceProxy.invoke = function (servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout) {
    if (!params) params = {};
    params.SessionID = Ts.System.getSessionID();

    return oldInvoke(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout);
  }


  g_PrivateServices = privateServices = new TeamSupport.Services.PrivateServices();
  g_PrivateServices.set_defaultSucceededCallback(function (result) { });
  g_PrivateServices.set_defaultFailedCallback(function (error, userContext, methodName) {
    //if (error !== null) { alert("An error occurred: " + error.get_message()) };
  });
}


if (typeof (Sys) !== "undefined") Sys.Application.notifyScriptLoaded();