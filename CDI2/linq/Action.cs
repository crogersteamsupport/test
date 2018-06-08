using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[Actions] 
    /// Actions on a Ticket
    /// </summary>
    [Table(Name = "Actions")]
    class Action
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionID;
        [Column(Storage = "_actionID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionID { get { return _actionID; } }

        [Column]
        public int TicketID;
        [Column]
        public string Description;
#pragma warning restore CS0649
    }

    /*[Table(Name = "ActionSentiments")]
    class ActionSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionSentimentID;
        [Column(Storage = "_actionSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionSentimentID { get { return _actionSentimentID; } }

        [Column]
        public int ActionID;
#pragma warning restore CS0649
    }*/

}
