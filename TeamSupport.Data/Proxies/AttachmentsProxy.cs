using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(AttachmentProxy))]
  [Table(Name = "Attachments")] // linq to sql
  public class AttachmentProxy
  {
    public AttachmentProxy() {}
    [Column, DataMember] public int AttachmentID { get; set; }
    [Column, DataMember] public int OrganizationID { get; set; }
    [Column, DataMember] public string FileName { get; set; }
    [Column, DataMember] public string FileType { get; set; }
    [Column, DataMember] public long FileSize { get; set; }
    [Column, DataMember] public string Path { get; set; }
    [Column, DataMember] public string Description { get; set; }
    [Column, DataMember] public DateTime DateCreated { get; set; }
    [Column, DataMember] public DateTime DateModified { get; set; }
    [Column, DataMember] public int CreatorID { get; set; }
    [Column, DataMember] public int ModifierID { get; set; }
    [Column, DataMember] public ReferenceType RefType { get; set; }
    [Column, DataMember] public int RefID { get; set; }
    [Column, DataMember] public string CreatorName { get; set; }          
    [Column, DataMember] public bool SentToJira { get; set; }
    [Column, DataMember] public int? ProductFamilyID { get; set; }
    [Column, DataMember] public string ProductFamily { get; set; }
    [Column, DataMember] public bool SentToTFS { get; set; }
    [Column, DataMember] public bool SentToSnow { get; set; }
    [Column, DataMember] public int? FilePathID { get; set; }
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
