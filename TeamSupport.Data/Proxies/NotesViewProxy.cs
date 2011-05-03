using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(NotesViewItemProxy))]
  public class NotesViewItemProxy
  {
    public NotesViewItemProxy() {}
    [DataMember] public int NoteID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
          
  }
  
  public partial class NotesViewItem : BaseItem
  {
    public NotesViewItemProxy GetProxy()
    {
      NotesViewItemProxy result = new NotesViewItemProxy();
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.Title = this.Title;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.NoteID = this.NoteID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
       
       
      return result;
    }	
  }
}
