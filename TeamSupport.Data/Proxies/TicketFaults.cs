using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace TeamSupport.Data {

    [DataContract(Namespace="http://teamsupport.com/")]
    [KnownType(typeof(TicketsViewItemProxy))]
    public class TicketFaultsProxy {

        public TicketFaultsItem() {}
        [DataMember] public string Status { get; set; }

    }

    public partial class TicketFaultsItem : BaseItem {

        public TicketFaultsItemProxy GetProxy() {
            TicketFaultsItemProxy result = new TicketFaultsItemProxy();
            result.Status = this.Status;
            return result;
        }

    }
}
