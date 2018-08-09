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
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(AttachmentProxy))]
    public class AttachmentProxy
    {
        public AttachmentProxy() { }
        [DataMember] public int AttachmentID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public string FileName { get; set; }
        [DataMember] public string FileType { get; set; }
        [DataMember] public long FileSize { get; set; }
        [DataMember] public string Path { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember] public bool SentToJira { get; set; }
        [DataMember] public int? ProductFamilyID { get; set; }
        [DataMember] public string ProductFamily { get; set; }
        [DataMember] public bool SentToTFS { get; set; }
        [DataMember] public bool SentToSnow { get; set; }
        [DataMember] public int? FilePathID { get; set; }

    }

}
