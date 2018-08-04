using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Proxy
{
  public class ActionProxy
  {
    public ActionProxy() {}
    public int ActionID { get; set; }
    public int? ActionTypeID { get; set; }
    //public SystemActionType SystemActionTypeID { get; set; }
    public string Name { get; set; }
    public int? TimeSpent { get; set; }
    public DateTime? DateStarted { get; set; }
    public bool IsVisibleOnPortal { get; set; }
    public bool IsKnowledgeBase { get; set; }
    public string ImportID { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
    public int? CreatorID { get; set; }
    public int? ModifierID { get; set; }
    public int TicketID { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public string SalesForceID { get; set; }          
    public DateTime? DateModifiedBySalesForceSync { get; set; }
    public bool Pinned { get; set; }
    public int? ImportFileID { get; set; }
          
  }
  
}
