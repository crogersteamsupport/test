using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(AttachmentProxy))]
  [Table(Name = "Attachments")]
  public class AttachmentProxy
  {
    public AttachmentProxy() {}
    [DataMember, Column] public int AttachmentID { get; set; }
    [DataMember, Column] public int OrganizationID { get; set; }
    [DataMember, Column] public string FileName { get; set; }
    [DataMember, Column] public string FileType { get; set; }
    [DataMember, Column] public long FileSize { get; set; }
    [DataMember, Column] public string Path { get; set; }
    [DataMember, Column] public string Description { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public DateTime DateModified { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public int ModifierID { get; set; }
    [DataMember, Column] public ReferenceType RefType { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public string CreatorName { get; set; }          
    [DataMember, Column] public bool SentToJira { get; set; }
    [DataMember, Column] public int? ProductFamilyID { get; set; }
    [DataMember, Column] public string ProductFamily { get; set; }
    [DataMember, Column] public bool SentToTFS { get; set; }
    [DataMember, Column] public bool SentToSnow { get; set; }
    [DataMember, Column] public int? FilePathID { get; set; }
          
  }
  
  public partial class Attachment : BaseItem
  {
    public AttachmentProxy GetProxy()
    {
      AttachmentProxy result = new AttachmentProxy();
      result.FilePathID = this.FilePathID;
      result.SentToSnow = this.SentToSnow;
      result.SentToTFS = this.SentToTFS;
      result.ProductFamilyID = this.ProductFamilyID;
      result.SentToJira = this.SentToJira;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.Path = this.Path;
      result.FileSize = this.FileSize;
      result.FileType = this.FileType;
      result.FileName = this.FileName;
      result.OrganizationID = this.OrganizationID;
      result.AttachmentID = this.AttachmentID;
      result.CreatorName = this.CreatorName;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
