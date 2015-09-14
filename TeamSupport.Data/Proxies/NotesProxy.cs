using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;


namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(NoteProxy))]
  public class NoteProxy
  {
    public NoteProxy() {}
    [DataMember] public int NoteID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember]
    public bool IsAlert { get; set; }
  }
  
  public partial class Note : BaseItem
  {
    public NoteProxy GetProxy()
    {
      NoteProxy result = new NoteProxy();
      var sanitizer = new HtmlSanitizer();
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = sanitizer.Sanitize(this.Description);
      result.Title = sanitizer.Sanitize(this.Title);
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.NoteID = this.NoteID;
      result.CreatorName = this.CreatorName;
      result.IsAlert = this.IsAlert;
            

       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
