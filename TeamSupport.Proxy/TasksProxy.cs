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
    [KnownType(typeof(TaskProxy))]
    [Table(Name = "Tasks")]
    public class TaskProxy
    {
        public TaskProxy() { }
        [DataMember, Column]
        public int TaskID { get; set; }
        [DataMember, Column]
        public int OrganizationID { get; set; }
        [DataMember, Column]
        public string Name { get; set; }
        [DataMember, Column]
        public string Description { get; set; }
        [DataMember, Column]
        public DateTime? DueDate { get; set; }
        [DataMember, Column]
        public int? UserID { get; set; }
        [DataMember, Column]
        public bool IsComplete { get; set; }
        [DataMember, Column]
        public DateTime? DateCompleted { get; set; }
        [DataMember, Column]
        public string CompletionComment { get; set; }
        [DataMember, Column]
        public int? ParentID { get; set; }
        [DataMember, Column]
        public int CreatorID { get; set; }
        [DataMember, Column]
        public DateTime DateCreated { get; set; }
        [DataMember, Column]
        public int ModifierID { get; set; }
        [DataMember, Column]
        public DateTime DateModified { get; set; }
        [DataMember, Column]
        public int? ReminderID { get; set; }
        [DataMember, Column]
        public bool IsDismissed { get; set; }
        [DataMember, Column]
        public DateTime? ReminderDueDate { get; set; }
        [DataMember, Column]
        public bool NeedsIndexing { get; set; }

    }

}
