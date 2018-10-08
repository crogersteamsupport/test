using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(ActionProxy))]
    public class ActionProxy
    {
        public ActionProxy() { }
    [DataMember] public int ActionID { get; set; }
    [DataMember] public int? ActionTypeID { get; set; }
    [DataMember] public SystemActionType SystemActionTypeID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public int? TimeSpent { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string Description { get; set; }
        [DataMember] public string DisplayName { get; set; }
    [DataMember] public string SalesForceID { get; set; }          
    [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
    [DataMember] public bool Pinned { get; set; }
    [DataMember] public int? ImportFileID { get; set; }

        //public ActionProxy(DataRow row)
        //{
        //    ActionID = (int)row["ActionID"];
        //    ActionTypeID = (int?)row["ActionTypeID"];
        //    SystemActionTypeID = (SystemActionType)row["SystemActionTypeID"];
        //    Name = (string)row["Name"];
        //    TimeSpent = (int?)row["TimeSpent"];
        //    DateStarted = (DateTime)row["DateStarted"];
        //    IsVisibleOnPortal = (bool)row["IsVisibleOnPortal"];
        //    IsKnowledgeBase = (bool)row["IsKnowledgeBase"];
        //    ImportID = row["ImportID"] != DBNull.Value ? row["ImportID"].ToString().Trim().ToLower() : "";
        //    DateCreated = (DateTime?)row["DateCreated"];
        //    DateModified = (DateTime?)row["DateModified"];
        //    CreatorID = (int?)row["CreatorID"];
        //    ModifierID = (int?)row["ModifierID"];
        //    TicketID = (int)row["TicketID"];
        //    Description = (string)row["Description"];
        //    //DisplayName;
        //    SalesForceID = row["SalesForceID"] != DBNull.Value ? row["SalesForceID"].ToString().Trim().ToLower() : "";
        //    DateModifiedBySalesForceSync = row["DateModifiedBySalesForceSync"] != DBNull.Value ? (DateTime?)row["DateModifiedBySalesForceSync"] : null;
        //    Pinned = (bool)row["Pinned"];
        //    ImportFileID = row["ImportFileID"] != DBNull.Value ? (int?)row["ImportFileID"] : null; ;
        //}

        //string ToSql<T>(T value) { return value == null ? "NULL" : value.ToString(); }
        //string ToSql(int? value) { return value.HasValue ? value.Value.ToString() : "NULL"; }
        //string ToSql(DateTime? value) { return value.HasValue ? ToSql(value.Value) : "NULL"; }
        //string ToSql(DateTime value) { return "'" + value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'"; }
        //string ToSql(bool value) { return value ? "1" : "0"; }
        //string ToSql(string value) { return $"'{value}'"; }

        //public string InsertCommandText()
        //{
        //    string command = "INSERT INTO Actions(ActionTypeID, SystemActionTypeID, Name, TimeSpent, DateStarted, IsVisibleOnPortal, IsKnowledgeBase, ImportID, DateCreated, DateModified, CreatorID, ModifierID, TicketID, Description, SalesForceID, DateModifiedBySalesForceSync, Pinned, ImportFileID)" +
        //    $"VALUES({ToSql(ActionTypeID)}, '{(int)SystemActionTypeID}', {ToSql(Name)}, {ToSql(TimeSpent)}, {ToSql(DateStarted)}, {ToSql(IsVisibleOnPortal)}, {ToSql(IsKnowledgeBase)}, {ToSql(ImportID)}, {ToSql(DateCreated)}, {ToSql(DateModified)}, {ToSql(CreatorID)}, {ToSql(ModifierID)}, {ToSql(TicketID)}, {ToSql(Description)}, {ToSql(SalesForceID)}, {ToSql(DateModifiedBySalesForceSync)}, {ToSql(Pinned)}, {ToSql(ImportFileID)})" +
        //    "SELECT SCOPE_IDENTITY()";
        //    return command;
        //}
    }

}
