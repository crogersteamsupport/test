using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TeamSupport.Data
{
  [DataContract(Namespace = "http://teamsupport.com/")]
  [KnownType(typeof(ActionLogProxy))]
  public class SearchResultsProxy
  {
    [DataMember] public int Count { get; set; }
    [DataMember] public SearchResultsItemProxy[] Items { get; set; }

  }

  [DataContract]
  public class SearchResultsItemProxy
  {
    public SearchResultsItemProxy()
    {
      //PageIndex = pageIndex;
      //PageSize = pageSize;
      //PageCount = (int)(count / pageSize);
      //if (count % pageSize > 0) PageCount++;
      //Count = count;
      //Tickets = tickets;
      //Filter = filter;
    }
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public int TypeID { get; set; }
    [DataMember]
    public string DisplayName { get; set; }
    [DataMember]
    public int Number { get; set; }
    [DataMember]
    public int ScorePercent { get; set; }
    [DataMember]
    public string Status { get; set; }
    [DataMember]
    public string Severity { get; set; }
    [DataMember]
    public string Creator { get; set; }
    [DataMember]
    public string Modifier { get; set; }
    [DataMember]
    public string DateModified { get; set; }
    [DataMember]
    public int CustomerID { get; set; }
    [DataMember]
    public int ProductID { get; set; }
    [DataMember]
    public int? RefType { get; set; }
    [DataMember]
    public int? AttachmentID { get; set; }
    [DataMember]
    public int MessageParent { get; set; }

  }

}
