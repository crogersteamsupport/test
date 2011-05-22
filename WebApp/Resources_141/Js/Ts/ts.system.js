var Ts = {};
Ts.Pages = {};
Ts.Ui = {};
Ts.Services = {};

(function () {

  Ts._inits = [];
  Ts._addInit = function (fn) {  Ts._inits.push(fn); };
  Ts._init = function (callback) {
    function callFn(){
      var fn = Ts._inits.shift();
      if (fn) { 
        fn(callFn); 
      }
      else {
        callback();
      }
    }
    callFn();
  };

  function initServices (callback)
  {

    var oldInvoke = Sys.Net.WebServiceProxy.invoke;
    Sys.Net.WebServiceProxy.invoke = function(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout)
    {
      if (!params) params = {};
      params.SessionID = Ts.System.getSessionID();

      return oldInvoke(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout);
    }
    


    function defaultSucceededCallback(result) { }
    function defaultFailedCallback(error, userContext, methodName) { 
      //if (error) { alert("An error occurred: " + error.get_message()); } 
    }

    Ts.Services.System = new TSWebServices.TSSystem();
    Ts.Services.System.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.System.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Settings = new TSWebServices.SettingService();
    Ts.Services.Settings.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Settings.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Tickets = new TSWebServices.TicketService();
    Ts.Services.Tickets.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Tickets.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Automation = new TSWebServices.AutomationService();
    Ts.Services.Automation.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Automation.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Users = new TSWebServices.UserService();
    Ts.Services.Users.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Users.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Organizations = new TSWebServices.OrganizationService();
    Ts.Services.Organizations.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Organizations.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Products = new TSWebServices.ProductService();
    Ts.Services.Products.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Products.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.CustomFields = new TSWebServices.CustomFieldsService();
    Ts.Services.CustomFields.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.CustomFields.set_defaultFailedCallback(defaultFailedCallback);
    callback();
  }

  Ts._addInit(initServices);



  function TsSystem() {
    this.User = null;
    this.Organization = null;
    this.Culture = null;
    this.ChatUserSettings = null;
    this.TicketTypes = null;

    var self = this;

    this._init = function (callback) {
      Ts.Services.Tickets.GetTicketTypes(function (result) {
        self.TicketTypes = result;
        self.refreshUser(callback);
      });
    }
  }

  TsSystem.prototype =
  {
    constructor: TsSystem,
    refreshUser: function (callback) {
      var self = this;
      Ts.Services.System.GetCurrentUser(function (result) {
        self.User = result;
        Ts.Services.System.GetCurrentOrganization(function (result) {
          self.Organization = result;
          Ts.Services.System.GetCulture(function(result){
            self.Culture = result;
            Ts.Services.System.GetCurrentUserChatSettings(function (result) {
              self.ChatUserSettings = result;
              if (callback) { callback(self.User); }
            });
          });
        });
      });
    },

    openSupport: function () {
      window.open("http://www.teamsupport.com/customer_portal_login.php?OrganizationID=1078&UserName=" + this.User.Email + "&Password=57EE1F58-5C8B-4B47-B629-BE7C702A2022", "TSPortal");
    },

    signOut: function (callback) {
      Ts.Services.System.SignOut(function () { window.location = window.location; });
    },


    getSessionID: function () { return $('#fieldSID').val(); }
  };

  Ts.System = new TsSystem();
  Ts._addInit(Ts.System._init);

})();

function pageLoad() {
  Ts._init(function () { if (window.teamSupportLoad) {window.teamSupportLoad(); }});
}
