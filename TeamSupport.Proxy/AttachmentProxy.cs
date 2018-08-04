using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Proxy
{
  public class AttachmentProxy
  {
    public AttachmentProxy() {}
    public int AttachmentID { get; set; }
    public int OrganizationID { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public int CreatorID { get; set; }
    public int ModifierID { get; set; }
    public Data.ReferenceType RefType { get; set; }
    public int RefID { get; set; }
    public string CreatorName { get; set; }          
    public bool SentToJira { get; set; }
    public int? ProductFamilyID { get; set; }
    public string ProductFamily { get; set; }
    public bool SentToTFS { get; set; }
    public bool SentToSnow { get; set; }
    public int? FilePathID { get; set; }
  }
}
