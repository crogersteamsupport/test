/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="~/Default.aspx" />

(function () {

  function TsCache() {
    this._users = null;
    this._groups = null;
    this._products = null;
    this._ticketTypes = null;
    this._ticketStatuses = null;
    this._ticketNextStatuses = null;
    this._ticketSeverities = null;
    this._actionTypes = null;
  }

  TsCache.prototype =
  {
    constructor: TsCache,
    _init: function (callback) {
      this.checkAll();
      callback();
    },
    checkAll: function () {
      this.getUsers();
      this.getGroups();
      this.getProducts();
      this.getTicketNextStatuses();
      this.getTicketSeverities();
      this.getTicketStatuses();
      this.getTicketTypes();
      this.getActionTypes();
    },
    getUsers: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.Users, function (checksum) {
        if (!self._users || !self._users.CheckSum || checksum != self._users.CheckSum) {
          Ts.Services.Users.GetUsers(function (result) {
            self._users = result;
            self._users.CheckSum = checksum;
          });
        }
      });
      return self._users;
    },
    getGroups: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.Groups, function (checksum) {
        if (!self._groups || !self._groups.CheckSum || checksum != self._groups.CheckSum) {
          Ts.Services.Users.GetGroups(function (result) {
            self._groups = result;
            self._groups.CheckSum = checksum;
          });
        }
      });
      return self._groups;
    },
    getTicketTypes: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketTypes, function (checksum) {
        if (!self._ticketTypes || !self._ticketTypes.CheckSum || checksum != self._ticketTypes.CheckSum) {
          Ts.Services.Tickets.GetTicketTypes(function (result) {
            self._ticketTypes = result;
            self._ticketTypes.CheckSum = checksum;
          });
        }
      });
      return self._ticketTypes;
    },
    getTicketStatuses: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketStatuses, function (checksum) {
        if (!self._ticketStatuses || !self._ticketStatuses.CheckSum || checksum != self._ticketStatuses.CheckSum) {
          Ts.Services.Tickets.GetTicketStatuses(function (result) {
            self._ticketStatuses = result;
            self._ticketStatuses.CheckSum = checksum;
          });
        }
      });
      return self._ticketStatuses;
    },
    getTicketNextStatuses: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketNextStatuses, function (checksum) {
        if (!self._ticketNextStatuses || !self._ticketNextStatuses.CheckSum || checksum != self._ticketNextStatuses.CheckSum) {
          Ts.Services.Tickets.GetNextTicketStatuses(function (result) {
            self._ticketNextStatuses = result;
            self._ticketNextStatuses.CheckSum = checksum;
          });
        }
      });
      return self._ticketNextStatuses;
    },

    getNextStatuses: function (ticketStatusID) {
      var next = this.getTicketNextStatuses();
      var statuses = this.getTicketStatuses();
      var result = [];
      result.push(this.getTicketStatus(ticketStatusID));

      for (var i = 0; i < next.length; i++) {
        if (next[i].CurrentStatusID == ticketStatusID) {
          var status = this.getTicketStatus(next[i].NextStatusID);
          if (status !== null) result.push(status);
        }
      }
      if (result.length < 2) return this.getTicketStatuses();
      return result;
    },
    getTicketStatus: function (ticketStatusID) {
      var statuses = this.getTicketStatuses();
      for (var i = 0; i < statuses.length; i++) {
        if (statuses[i].TicketStatusID == ticketStatusID) {
          return statuses[i];
        }
      }
      return null;
    },
    getTicketSeverities: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketSeverities, function (checksum) {
        if (!self._ticketSeverities || !self._ticketSeverities.CheckSum || checksum != self._ticketSeverities.CheckSum) {
          Ts.Services.Tickets.GetTicketSeverities(function (result) {
            self._ticketSeverities = result;
            self._ticketSeverities.CheckSum = checksum;
          });
        }
      });
      return self._ticketSeverities;
    },
    getProducts: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ProductVersions, function (checksum) {
        if (!self._products || !self._products.CheckSum || checksum != self._products.CheckSum) {
          Ts.Services.Products.GetProducts(function (result) {
            self._products = result;
            self._products.CheckSum = checksum;
          });
        }
      });
      return self._products;
    },
    getProduct: function (productID) {
      var products = this.getProducts();
      for (var i = 0; i < products.length; i++) {
        if (products[i].ProductID == productID) {
          return products[i];
        }
      }
      return null;
    },
    getActionTypes: function () {
      var self = this;
      Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ActionTypes, function (checksum) {
        if (!self._actionTypes || !self._actionTypes.CheckSum || checksum != self._actionTypes.CheckSum) {
          Ts.Services.Tickets.GetActionTypes(function (result) {
            self._actionTypes = result;
            self._actionTypes.CheckSum = checksum;
          });
        }
      });
      return self._actionTypes;
    }

  };

  Ts.Cache = new TsCache();
  Ts._addInit(Ts.Cache);

})();

